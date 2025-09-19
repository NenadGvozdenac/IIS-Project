package saga

import (
	"fmt"
	"log"
	"time"
)

// RetryConfig defines retry configuration for SAGA operations
type RetryConfig struct {
	MaxRetries int
	BaseDelay  time.Duration
	MaxDelay   time.Duration
}

// DefaultRetryConfig returns a default retry configuration
func DefaultRetryConfig() RetryConfig {
	return RetryConfig{
		MaxRetries: 3,
		BaseDelay:  1 * time.Second,
		MaxDelay:   10 * time.Second,
	}
}

// RetryWithBackoff executes a function with exponential backoff retry logic
func RetryWithBackoff(operation func() error, config RetryConfig, operationName string) error {
	var lastErr error

	for attempt := 0; attempt <= config.MaxRetries; attempt++ {
		if attempt > 0 {
			// Calculate delay with exponential backoff
			delay := time.Duration(attempt) * config.BaseDelay
			if delay > config.MaxDelay {
				delay = config.MaxDelay
			}

			log.Printf("SAGA: Retrying %s (attempt %d/%d) after %v", operationName, attempt, config.MaxRetries, delay)
			time.Sleep(delay)
		}

		err := operation()
		if err == nil {
			if attempt > 0 {
				log.Printf("SAGA: %s succeeded on attempt %d", operationName, attempt+1)
			}
			return nil
		}

		lastErr = err
		log.Printf("SAGA: %s failed on attempt %d: %v", operationName, attempt+1, err)
	}

	return fmt.Errorf("operation %s failed after %d attempts: %w", operationName, config.MaxRetries+1, lastErr)
}

// SagaEventType represents different types of saga events
type SagaEventType string

const (
	SagaEventStarted             SagaEventType = "SAGA_STARTED"
	SagaEventStepStarted         SagaEventType = "STEP_STARTED"
	SagaEventStepSuccess         SagaEventType = "STEP_SUCCESS"
	SagaEventStepFailed          SagaEventType = "STEP_FAILED"
	SagaEventCompensationStarted SagaEventType = "COMPENSATION_STARTED"
	SagaEventCompensationSuccess SagaEventType = "COMPENSATION_SUCCESS"
	SagaEventCompensationFailed  SagaEventType = "COMPENSATION_FAILED"
	SagaEventCompleted           SagaEventType = "SAGA_COMPLETED"
	SagaEventFailed              SagaEventType = "SAGA_FAILED"
)

// SagaEvent represents an event in the saga execution
type SagaEvent struct {
	Type      SagaEventType `json:"type"`
	StepName  string        `json:"step_name,omitempty"`
	Message   string        `json:"message"`
	Timestamp time.Time     `json:"timestamp"`
	Error     string        `json:"error,omitempty"`
}

// SagaLogger handles logging of saga events
type SagaLogger struct {
	events []SagaEvent
}

// NewSagaLogger creates a new saga logger
func NewSagaLogger() *SagaLogger {
	return &SagaLogger{
		events: make([]SagaEvent, 0),
	}
}

// LogEvent logs a saga event
func (l *SagaLogger) LogEvent(eventType SagaEventType, stepName, message string, err error) {
	event := SagaEvent{
		Type:      eventType,
		StepName:  stepName,
		Message:   message,
		Timestamp: time.Now(),
	}

	if err != nil {
		event.Error = err.Error()
	}

	l.events = append(l.events, event)

	// Also log to standard logger
	if err != nil {
		log.Printf("SAGA Event [%s] %s: %s - Error: %v", eventType, stepName, message, err)
	} else {
		log.Printf("SAGA Event [%s] %s: %s", eventType, stepName, message)
	}
}

// GetEvents returns all logged events
func (l *SagaLogger) GetEvents() []SagaEvent {
	return l.events
}
