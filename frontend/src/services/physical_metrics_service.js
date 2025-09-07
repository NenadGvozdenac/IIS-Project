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

// Field mapping between frontend (camelCase) and backend (camelCase from JSON)
const fieldMapping = {
  // Frontend -> Backend
  id: 'idPhysicalMetrics',
  verticalJump: 'verticalJump', 
  fatPercentage: 'fatPercentage',
  benchPressWeight: 'benchPressWeight',
  squatWeight: 'squatWeight',
  sprintSpeed: 'sprintSpeed',  // This is now decimal in backend
  weight: 'weight',
  height: 'height',
  wingspan: 'wingspan',
  dateOfMeasurement: 'dateOfMeasurement',
  playerId: 'idPlayer',  // This is the key fix - backend uses 'idPlayer' not 'IdPlayer'
  playerName: 'playerName'
};

// Reverse mapping for response transformation
const reverseFieldMapping = Object.fromEntries(
  Object.entries(fieldMapping).map(([key, value]) => [value, key])
);

function transformToBackend(frontendData) {
  const backendData = {};
  for (const [frontKey, backKey] of Object.entries(fieldMapping)) {
    if (frontendData.hasOwnProperty(frontKey)) {
      backendData[backKey] = frontendData[frontKey];
    }
  }
  return backendData;
}

function transformFromBackend(backendData) {
  console.log('transformFromBackend input:', backendData);
  
  // Since the backend JSON response already uses camelCase, we can mostly use it directly
  // Just need to map idPlayer to playerId for consistency
  const frontendData = {
    ...backendData,
    playerId: backendData.idPlayer,
    id: backendData.idPhysicalMetrics
  };
  
  console.log('transformFromBackend output:', frontendData);
  return frontendData;
}

// Validate and ensure all required fields are present with proper types
function validateMetricsData(data) {
  const requiredFields = [
    'verticalJump', 'fatPercentage', 'benchPressWeight', 'squatWeight', 
    'sprintSpeed', 'weight', 'height', 'wingspan', 'dateOfMeasurement', 'playerId'
  ];
  
  const validated = {};
  
  for (const field of requiredFields) {
    if (!(field in data) || data[field] === null || data[field] === undefined) {
      // Set default values for missing fields
      switch (field) {
        case 'verticalJump':
        case 'benchPressWeight':
        case 'squatWeight':
          validated[field] = 0;
          break;
        case 'fatPercentage':
          validated[field] = 10;
          break;
        case 'sprintSpeed':
          validated[field] = 10.0;
          break;
        case 'weight':
          validated[field] = 70;
          break;
        case 'height':
        case 'wingspan':
          validated[field] = 180;
          break;
        case 'dateOfMeasurement':
          validated[field] = new Date().toISOString().split('T')[0];
          break;
        case 'playerId':
          throw new Error('playerId is required and cannot be null');
        default:
          throw new Error(`Unknown required field: ${field}`);
      }
    } else {
      validated[field] = data[field];
    }
  }
  
  return validated;
}

// Get all physical metrics
export const getAllPhysicalMetrics = async () => {
  try {
    const response = await fetch(`${API_URL}/PhysicalMetrics`, {
      method: 'GET',
      headers: createHeaders()
    });

    const data = await handleApiResponse(response);
    console.log('Raw data from backend:', data);
    
    // Transform backend response to frontend format
    if (Array.isArray(data)) {
      const transformed = data.map(transformFromBackend);
      console.log('Transformed data:', transformed);
      return transformed;
    }
    
    const transformed = data ? [transformFromBackend(data)] : [];
    console.log('Transformed single item:', transformed);
    return transformed;
  } catch (error) {
    console.error('Error fetching physical metrics:', error);
    throw error;
  }
};

// Create physical metrics for a player
export const createPhysicalMetrics = async (playerId, metricsData) => {
  try {
    // Create a fixed request that matches the backend Command exactly
    const requestBody = {
      VerticalJump: parseInt(metricsData.verticalJump) || 0,
      FatPercentage: parseInt(metricsData.fatPercentage) || 10,
      BenchPressWeight: parseInt(metricsData.benchPressWeight) || 0,
      SquatWeight: parseInt(metricsData.squatWeight) || 0,
      SprintSpeed: parseFloat(metricsData.sprintSpeed) || 10.0,
      Weight: parseInt(metricsData.weight) || 70,
      Height: parseInt(metricsData.height) || 180,
      Wingspan: parseInt(metricsData.wingspan) || 180,
      DateOfMeasurement: new Date().toISOString().split('T')[0], // YYYY-MM-DD format
      IdPlayer: parseInt(playerId)
    };
    
    console.log('Sending direct request body (matching Command exactly):', requestBody);
    console.log('Sending request to:', `${API_URL}/PhysicalMetrics`);
    console.log('Request body JSON:', JSON.stringify(requestBody, null, 2));

    const response = await fetch(`${API_URL}/PhysicalMetrics`, {
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
      } catch (parseError) {
        console.error('Could not parse error as JSON');
      }
      
      throw new Error(`HTTP error! status: ${response.status}, body: ${errorText}`);
    }

    const result = await handleApiResponse(response);
    return result;
  } catch (error) {
    console.error('Error creating physical metrics:', error);
    throw error;
  }
};

// Get physical metrics for a specific player (filter from all metrics)
export const getPhysicalMetricsByPlayerId = async (playerId) => {
  try {
    const allMetrics = await getAllPhysicalMetrics();
    console.log('All metrics from API:', allMetrics);
    console.log('Looking for playerId:', parseInt(playerId));
    
    const filtered = allMetrics.filter(metric => {
      console.log('Metric playerId:', metric.playerId, 'Type:', typeof metric.playerId);
      return metric.playerId === parseInt(playerId);
    });
    
    console.log('Filtered metrics for player:', filtered);
    return filtered;
  } catch (error) {
    console.error('Error fetching physical metrics for player:', error);
    throw error;
  }
};

// Note: Update and Delete operations are not available in the current backend API
// They would need to be implemented in the PhysicalMetricsController first

export { transformToBackend, transformFromBackend, validateMetricsData };
