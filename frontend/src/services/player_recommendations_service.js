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

// Get player recommendations
export const getPlayerRecommendations = async (metricWeights) => {
  try {
    const requestBody = {
      metricWeights: metricWeights.map(mw => ({
        metricId: mw.metricId,
        weight: mw.weight
      }))
    };

    const response = await fetch(`${API_URL}/Players/recommendations`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(requestBody)
    });

    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error fetching player recommendations:', error);
    throw error;
  }
};

// Get all seasons (we'll need this for the season selection)
export const getAllSeasons = async () => {
  try {
    const response = await fetch(`${API_URL}/Seasons`, {
      method: 'GET',
      headers: createHeaders()
    });

    const data = await handleApiResponse(response);
    return Array.isArray(data) ? data : [data];
  } catch (error) {
    console.error('Error fetching seasons:', error);
    throw error;
  }
};