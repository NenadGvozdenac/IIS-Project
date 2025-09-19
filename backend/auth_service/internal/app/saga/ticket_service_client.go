package saga

import (
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"
	"time"
)

// HTTPClient interface for making HTTP requests (allows for mocking in tests)
type HTTPClient interface {
	Do(req *http.Request) (*http.Response, error)
}

// TicketServiceClient handles communication with the ticket service
type TicketServiceClient struct {
	baseURL    string
	httpClient HTTPClient
	timeout    time.Duration
}

// NewTicketServiceClient creates a new ticket service client
func NewTicketServiceClient(baseURL string) *TicketServiceClient {
	return &TicketServiceClient{
		baseURL: baseURL,
		httpClient: &http.Client{
			Timeout: 30 * time.Second,
		},
		timeout: 30 * time.Second,
	}
}

// CreateCustomer creates a customer in the ticket service
func (c *TicketServiceClient) CreateCustomer(request CustomerCreationRequest) (*CustomerCreationResponse, error) {
	// Prepare the request payload
	payload, err := json.Marshal(request)
	if err != nil {
		return nil, fmt.Errorf("failed to marshal request: %w", err)
	}

	// Create HTTP request
	url := fmt.Sprintf("%s/api/saga/customers", c.baseURL)
	req, err := http.NewRequest("POST", url, bytes.NewBuffer(payload))
	if err != nil {
		return nil, fmt.Errorf("failed to create request: %w", err)
	}

	// Set headers
	req.Header.Set("Content-Type", "application/json")
	req.Header.Set("Accept", "application/json")

	// Make the request
	resp, err := c.httpClient.Do(req)
	if err != nil {
		return nil, fmt.Errorf("failed to make request: %w", err)
	}
	defer resp.Body.Close()

	// Check status code
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusCreated {
		return nil, fmt.Errorf("ticket service returned status %d", resp.StatusCode)
	}

	// Parse response
	var response CustomerCreationResponse
	if err := json.NewDecoder(resp.Body).Decode(&response); err != nil {
		return nil, fmt.Errorf("failed to decode response: %w", err)
	}

	return &response, nil
}

// DeleteCustomer deletes a customer from the ticket service (for compensation)
func (c *TicketServiceClient) DeleteCustomer(customerID int) error {
	// Create HTTP request
	url := fmt.Sprintf("%s/api/saga/customers/%d", c.baseURL, customerID)
	req, err := http.NewRequest("DELETE", url, nil)
	if err != nil {
		return fmt.Errorf("failed to create request: %w", err)
	}

	// Set headers
	req.Header.Set("Accept", "application/json")

	// Make the request
	resp, err := c.httpClient.Do(req)
	if err != nil {
		return fmt.Errorf("failed to make request: %w", err)
	}
	defer resp.Body.Close()

	// Check status code
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusNoContent {
		return fmt.Errorf("ticket service returned status %d", resp.StatusCode)
	}

	return nil
}
