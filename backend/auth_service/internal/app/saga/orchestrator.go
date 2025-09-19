package saga

import (
	"auth_service/internal/domain/models"
	"fmt"
)

// SagaOrchestrator manages the execution of saga transactions
type SagaOrchestrator struct {
	ticketServiceClient *TicketServiceClient
	steps               []SagaStep
	status              SagaStatus
	logger              *SagaLogger
	retryConfig         RetryConfig
}

// NewSagaOrchestrator creates a new saga orchestrator
func NewSagaOrchestrator(ticketServiceClient *TicketServiceClient) *SagaOrchestrator {
	return &SagaOrchestrator{
		ticketServiceClient: ticketServiceClient,
		steps:               make([]SagaStep, 0),
		status:              SagaStatusPending,
		logger:              NewSagaLogger(),
		retryConfig:         DefaultRetryConfig(),
	}
}

// UserRegistrationSagaResult contains the result of user registration saga
type UserRegistrationSagaResult struct {
	User       *models.User
	CustomerID int
	Success    bool
	Error      error
}

// ExecuteUserRegistrationSaga executes the complete user registration saga
func (s *SagaOrchestrator) ExecuteUserRegistrationSaga(
	user *models.User,
	createUserFunc func() (*models.User, error),
	deleteUserFunc func(uint) error,
) *UserRegistrationSagaResult {
	result := &UserRegistrationSagaResult{
		Success: false,
	}

	// Clear previous steps and initialize
	s.steps = make([]SagaStep, 0)
	s.status = SagaStatusInProgress
	s.logger.LogEvent(SagaEventStarted, "", fmt.Sprintf("Starting user registration saga for email: %s", user.Email), nil)

	var createdUser *models.User
	var customerID int

	// Step 1: Create user in auth database
	userCreationStep := SagaStep{
		Name: "CreateUser",
		Execute: func() error {
			return RetryWithBackoff(func() error {
				var err error
				createdUser, err = createUserFunc()
				if err != nil {
					return fmt.Errorf("failed to create user: %w", err)
				}
				result.User = createdUser
				return nil
			}, s.retryConfig, "CreateUser")
		},
		Compensate: func() error {
			if createdUser != nil {
				return RetryWithBackoff(func() error {
					if err := deleteUserFunc(createdUser.ID); err != nil {
						return fmt.Errorf("failed to compensate user creation: %w", err)
					}
					return nil
				}, s.retryConfig, "CompensateUserCreation")
			}
			return nil
		},
	}

	// Step 2: Create customer in ticket service
	customerCreationStep := SagaStep{
		Name: "CreateCustomer",
		Execute: func() error {
			if createdUser == nil {
				return fmt.Errorf("user not created, cannot create customer")
			}

			// Only create customer for "customer" user type
			if createdUser.UserType != "customer" {
				s.logger.LogEvent(SagaEventStepSuccess, "CreateCustomer", fmt.Sprintf("Skipping customer creation for user type: %s", createdUser.UserType), nil)
				return nil
			}

			return RetryWithBackoff(func() error {
				request := CustomerCreationRequest{
					Email:   createdUser.Email,
					Name:    createdUser.Name,
					Surname: createdUser.Surname,
					Phone:   createdUser.Phone,
					Type:    createdUser.UserType,
				}

				response, err := s.ticketServiceClient.CreateCustomer(request)
				if err != nil {
					return fmt.Errorf("failed to create customer in ticket service: %w", err)
				}

				if !response.Success {
					return fmt.Errorf("ticket service failed to create customer: %s", response.Message)
				}

				customerID = response.Data.ID
				result.CustomerID = customerID
				return nil
			}, s.retryConfig, "CreateCustomer")
		},
		Compensate: func() error {
			if customerID > 0 {
				return RetryWithBackoff(func() error {
					if err := s.ticketServiceClient.DeleteCustomer(customerID); err != nil {
						return fmt.Errorf("failed to compensate customer creation: %w", err)
					}
					return nil
				}, s.retryConfig, "CompensateCustomerCreation")
			}
			return nil
		},
	}

	// Add steps to the saga
	s.steps = append(s.steps, userCreationStep, customerCreationStep)

	// Execute all steps
	for i := range s.steps {
		s.steps[i].Executed = false
		s.steps[i].Compensated = false

		s.logger.LogEvent(SagaEventStepStarted, s.steps[i].Name, fmt.Sprintf("Executing step: %s", s.steps[i].Name), nil)

		if err := s.steps[i].Execute(); err != nil {
			s.logger.LogEvent(SagaEventStepFailed, s.steps[i].Name, fmt.Sprintf("Step %s failed", s.steps[i].Name), err)
			result.Error = err
			s.status = SagaStatusFailed

			// Compensate all executed steps in reverse order
			s.logger.LogEvent(SagaEventCompensationStarted, "", "Starting compensation process", nil)
			if err := s.compensate(); err != nil {
				s.logger.LogEvent(SagaEventCompensationFailed, "", "Compensation failed", err)
				result.Error = fmt.Errorf("saga failed and compensation failed: %w", err)
				s.status = SagaStatusFailed
			} else {
				s.logger.LogEvent(SagaEventCompensationSuccess, "", "Compensation completed successfully", nil)
				s.status = SagaStatusCompensated
			}

			s.logger.LogEvent(SagaEventFailed, "", "User registration saga failed", result.Error)
			return result
		}

		s.steps[i].Executed = true
		s.logger.LogEvent(SagaEventStepSuccess, s.steps[i].Name, fmt.Sprintf("Step %s completed successfully", s.steps[i].Name), nil)
	}

	// All steps completed successfully
	s.status = SagaStatusCompleted
	result.Success = true
	s.logger.LogEvent(SagaEventCompleted, "", "User registration saga completed successfully", nil)
	return result
}

// compensate executes compensation logic for all executed steps in reverse order
func (s *SagaOrchestrator) compensate() error {
	s.logger.LogEvent(SagaEventCompensationStarted, "", fmt.Sprintf("Starting compensation for %d steps", len(s.steps)), nil)

	// Execute compensation in reverse order
	for i := len(s.steps) - 1; i >= 0; i-- {
		if s.steps[i].Executed && !s.steps[i].Compensated {
			s.logger.LogEvent(SagaEventCompensationStarted, s.steps[i].Name, fmt.Sprintf("Compensating step: %s", s.steps[i].Name), nil)
			if err := s.steps[i].Compensate(); err != nil {
				s.logger.LogEvent(SagaEventCompensationFailed, s.steps[i].Name, fmt.Sprintf("Compensation failed for step %s", s.steps[i].Name), err)
				return fmt.Errorf("compensation failed for step %s: %w", s.steps[i].Name, err)
			}
			s.steps[i].Compensated = true
			s.logger.LogEvent(SagaEventCompensationSuccess, s.steps[i].Name, fmt.Sprintf("Step %s compensated successfully", s.steps[i].Name), nil)
		}
	}

	s.logger.LogEvent(SagaEventCompensationSuccess, "", "Compensation completed successfully", nil)
	return nil
}

// GetStatus returns the current status of the saga
func (s *SagaOrchestrator) GetStatus() SagaStatus {
	return s.status
}

// GetEvents returns all saga events for debugging and monitoring
func (s *SagaOrchestrator) GetEvents() []SagaEvent {
	return s.logger.GetEvents()
}
