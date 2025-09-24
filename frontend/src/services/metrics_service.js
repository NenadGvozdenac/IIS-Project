import { SCOUTINGS_URL } from './const_service.js';

const API_URL = SCOUTINGS_URL;

// Get auth token
const getAuthToken = () => {
  return localStorage.getItem('token');
};

// Create request headers
const createHeaders = () => {
  const token = getAuthToken();
  return {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` })
  };
};

// Helper function to handle API response
const handleApiResponse = async (response) => {
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  const result = await response.json();
  
  // Check if the response follows the Result<T> pattern
  if (result && typeof result === 'object' && 'isSuccess' in result) {
    if (!result.isSuccess) {
      throw new Error(result.error || 'API request failed');
    }
    return result.value; // Extract the actual data from Result<T>.Value
  }
  
  return result; // Return as-is if not wrapped in Result<T>
};

// Get all metrics
export const getAllMetrics = async () => {
  try {
    const response = await fetch(`${API_URL}/Metrics`, {
      method: 'GET',
      headers: createHeaders()
    });

    const data = await handleApiResponse(response);
    return Array.isArray(data) ? data : [data];
  } catch (error) {
    console.error('Error fetching metrics:', error);
    throw error;
  }
};

// Create a new metric
export const createMetric = async (metricData) => {
  try {
    const requestBody = {
      Name: metricData.name,
      IsPermanent: metricData.isPermanent ? 1 : 0,
      MetricWeight: parseInt(metricData.metricWeight),
      IdUser: parseInt(metricData.idUser),
      IdMetricType: parseInt(metricData.idMetricType)
    };

    console.log('Creating metric with data:', requestBody);
    console.log('Request URL:', `${API_URL}/Metrics`);

    const response = await fetch(`${API_URL}/Metrics`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(requestBody)
    });

    console.log('Response status:', response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('API Error Response:', errorText);
      
      // Try to parse error as JSON for better debugging
      try {
        const errorJson = JSON.parse(errorText);
        console.error('Parsed error:', errorJson);
        throw new Error(`HTTP error! status: ${response.status}, error: ${JSON.stringify(errorJson)}`);
      } catch (parseError) {
        console.error('Could not parse error as JSON');
        throw new Error(`HTTP error! status: ${response.status}, body: ${errorText}`);
      }
    }

    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error creating metric:', error);
    throw error;
  }
};

// Get all metric types
export const getAllMetricTypes = async () => {
  try {
    const response = await fetch(`${API_URL}/MetricTypes`, {
      method: 'GET',
      headers: createHeaders()
    });

    const data = await handleApiResponse(response);
    return Array.isArray(data) ? data : [data];
  } catch (error) {
    console.error('Error fetching metric types:', error);
    throw error;
  }
};

// Update an existing metric
export const updateMetric = async (metricId, metricData) => {
  try {
    const requestBody = {
      Name: metricData.name,
      IsPermanent: metricData.isPermanent ? 1 : 0,
      MetricWeight: parseInt(metricData.metricWeight),
      IdUser: parseInt(metricData.idUser),
      IdMetricType: parseInt(metricData.idMetricType)
    };

    console.log('Updating metric with data:', requestBody);
    console.log('Request URL:', `${API_URL}/Metrics/${metricId}`);

    const response = await fetch(`${API_URL}/Metrics/${metricId}`, {
      method: 'PUT',
      headers: createHeaders(),
      body: JSON.stringify(requestBody)
    });

    console.log('Response status:', response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('API Error Response:', errorText);
      
      // Try to parse error as JSON for better debugging
      try {
        const errorJson = JSON.parse(errorText);
        console.error('Parsed error:', errorJson);
        throw new Error(`HTTP error! status: ${response.status}, error: ${JSON.stringify(errorJson)}`);
      } catch (parseError) {
        console.error('Could not parse error as JSON');
        throw new Error(`HTTP error! status: ${response.status}, body: ${errorText}`);
      }
    }

    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error updating metric:', error);
    throw error;
  }
};

// Create a new metric type
export const createMetricType = async (typeData) => {
  try {
    const requestBody = {
      Type: typeData.type
    };

    console.log('Creating metric type with data:', requestBody);
    console.log('Request URL:', `${API_URL}/MetricTypes`);

    const response = await fetch(`${API_URL}/MetricTypes`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(requestBody)
    });

    console.log('Response status:', response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('API Error Response:', errorText);
      
      // Try to parse error as JSON for better debugging
      try {
        const errorJson = JSON.parse(errorText);
        console.error('Parsed error:', errorJson);
        throw new Error(`HTTP error! status: ${response.status}, error: ${JSON.stringify(errorJson)}`);
      } catch (parseError) {
        console.error('Could not parse error as JSON');
        throw new Error(`HTTP error! status: ${response.status}, body: ${errorText}`);
      }
    }

    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error creating metric type:', error);
    throw error;
  }
};
