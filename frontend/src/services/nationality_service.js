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

// Get all nationalities
export const getAllNationalities = async () => {
  try {
    const response = await fetch(`${API_URL}/nationalities`, {
      method: 'GET',
      headers: createHeaders()
    });

    const data = await handleApiResponse(response);
    
    // Map backend field names to frontend field names
    return data.map(nationality => ({
      id: nationality.idNationality,
      name: nationality.state
    }));
  } catch (error) {
    console.error('Error fetching nationalities:', error);
    throw error;
  }
};

// Create a new nationality
export const createNationality = async (nationalityData) => {
  try {
    const response = await fetch(`${API_URL}/nationalities`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(nationalityData)
    });

    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error creating nationality:', error);
    throw error;
  }
};
