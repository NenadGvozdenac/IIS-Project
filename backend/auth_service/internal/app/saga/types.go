package saga

// SagaStep represents a single step in the saga
type SagaStep struct {
	Name        string
	Execute     func() error
	Compensate  func() error
	Executed    bool
	Compensated bool
}

// CustomerCreationRequest represents the request to create a customer in ticket service
type CustomerCreationRequest struct {
	Email   string `json:"email"`
	Name    string `json:"name"`
	Surname string `json:"surname"`
	Phone   string `json:"phone"`
	Type    string `json:"type"`
}

// CustomerCreationResponse represents the response from ticket service
type CustomerCreationResponse struct {
	Success bool   `json:"success"`
	Message string `json:"message"`
	Data    struct {
		ID        int    `json:"id"`
		ElementID string `json:"elementId"`
		Email     string `json:"email"`
		Name      string `json:"name"`
		Surname   string `json:"surname"`
		Phone     string `json:"phone"`
		Type      string `json:"type"`
	} `json:"data"`
}

// SagaStatus represents the status of a saga execution
type SagaStatus string

const (
	SagaStatusPending     SagaStatus = "PENDING"
	SagaStatusInProgress  SagaStatus = "IN_PROGRESS"
	SagaStatusCompleted   SagaStatus = "COMPLETED"
	SagaStatusFailed      SagaStatus = "FAILED"
	SagaStatusCompensated SagaStatus = "COMPENSATED"
)
