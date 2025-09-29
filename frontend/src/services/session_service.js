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

// Get all sessions
export const getAllSessions = async () => {
  try {
    const response = await fetch(`${API_URL}/Sessions`, {
      method: 'GET',
      headers: createHeaders()
    });
    
    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error fetching sessions:', error);
    throw error;
  }
};

// Get session by ID
export const getSessionById = async (sessionId) => {
  try {
    const response = await fetch(`${API_URL}/Sessions/${sessionId}`, {
      method: 'GET',
      headers: createHeaders()
    });
    
    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error fetching session:', error);
    throw error;
  }
};

// Get sessions by player ID
export const getSessionsByPlayerId = async (playerId) => {
  try {
    // Since the backend doesn't support filtering by player ID,
    // we'll get all sessions and filter them locally
    const allSessions = await getAllSessions();
    return allSessions.filter(session => session.idPlayer === playerId);
  } catch (error) {
    console.error('Error fetching sessions by player ID:', error);
    throw error;
  }
};

// Create a new session
export const createSession = async (sessionData) => {
  try {
    const response = await fetch(`${API_URL}/Sessions`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(sessionData)
    });
    
    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error creating session:', error);
    throw error;
  }
};

// Update an existing session
export const updateSession = async (sessionId, sessionData) => {
  try {
    const response = await fetch(`${API_URL}/Sessions/${sessionId}`, {
      method: 'PUT',
      headers: createHeaders(),
      body: JSON.stringify(sessionData)
    });
    
    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error updating session:', error);
    throw error;
  }
};

// Get all session types
export const getSessionTypes = async () => {
  try {
    const response = await fetch(`${API_URL}/SessionTypes`, {
      method: 'GET',
      headers: createHeaders()
    });
    
    const result = await handleApiResponse(response);
    // The response has sessionTypes array inside
    return result.sessionTypes || [];
  } catch (error) {
    console.error('Error fetching session types:', error);
    throw error;
  }
};

// Get all session statuses
export const getSessionStatuses = async () => {
  try {
    const response = await fetch(`${API_URL}/SessionStatuses`, {
      method: 'GET',
      headers: createHeaders()
    });
    
    const result = await handleApiResponse(response);
    // The response has sessionStatuses array inside
    return result.sessionStatuses || [];
  } catch (error) {
    console.error('Error fetching session statuses:', error);
    throw error;
  }
};

// Get all metrics
export const getAllMetrics = async () => {
  try {
    const response = await fetch(`${API_URL}/Metrics`, {
      method: 'GET',
      headers: createHeaders()
    });
    
    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error fetching metrics:', error);
    throw error;
  }
};

// Get all session metrics
export const getAllSessionMetrics = async () => {
  try {
    const response = await fetch(`${API_URL}/SessionMetrics`, {
      method: 'GET',
      headers: createHeaders()
    });
    
    const result = await handleApiResponse(response);
    return result.sessionMetrics || [];
  } catch (error) {
    console.error('Error fetching session metrics:', error);
    throw error;
  }
};

// Get session metrics by session ID
export const getSessionMetricsBySessionId = async (sessionId) => {
  try {
    const allSessionMetrics = await getAllSessionMetrics();
    return allSessionMetrics.filter(metric => metric.idSession === sessionId);
  } catch (error) {
    console.error('Error fetching session metrics by session ID:', error);
    throw error;
  }
};

// Create session metric
export const createSessionMetric = async (sessionMetricData) => {
  try {
    const response = await fetch(`${API_URL}/SessionMetrics`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(sessionMetricData)
    });
    
    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error creating session metric:', error);
    throw error;
  }
};

// Update session metric
export const updateSessionMetric = async (sessionId, metricId, sessionMetricData) => {
  try {
    const response = await fetch(`${API_URL}/SessionMetrics/${sessionId}/${metricId}`, {
      method: 'PUT',
      headers: createHeaders(),
      body: JSON.stringify(sessionMetricData)
    });
    
    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error updating session metric:', error);
    throw error;
  }
};
