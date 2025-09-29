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

// Create a new player
export const createPlayer = async (playerData) => {
  try {
    // Map frontend field names to backend field names
    const backendPlayerData = {
      name: playerData.firstName,
      surname: playerData.lastName,
      birthday: playerData.dateOfBirth,
      weight: playerData.weight,
      height: playerData.height,
      idNationality: playerData.idNationality,
      idPosition: playerData.idPosition
    };

    const response = await fetch(`${API_URL}/players`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(backendPlayerData)
    });

    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error creating player:', error);
    throw error;
  }
};

// Get player by ID
export const getPlayerById = async (playerId) => {
  try {
    const response = await fetch(`${API_URL}/players/${playerId}`, {
      method: 'GET',
      headers: createHeaders()
    });

    const data = await handleApiResponse(response);
    
    // Map backend field names to frontend field names
    return {
      id: data.idPlayer,
      firstName: data.name,
      lastName: data.surname,
      dateOfBirth: data.birthday,
      weight: data.weight,
      height: data.height,
      nationality: data.nationalityName,
      position: data.positionName,
      idNationality: data.idNationality,
      idPosition: data.idPosition
    };
  } catch (error) {
    console.error('Error fetching player:', error);
    throw error;
  }
};

// Update player
export const updatePlayer = async (playerId, playerData) => {
  try {
    // Map frontend field names to backend field names
    const backendPlayerData = {
      name: playerData.firstName,
      surname: playerData.lastName,
      birthday: playerData.dateOfBirth,
      weight: playerData.weight,
      height: playerData.height,
      idNationality: playerData.idNationality,
      idPosition: playerData.idPosition
    };

    const response = await fetch(`${API_URL}/players/${playerId}`, {
      method: 'PUT',
      headers: createHeaders(),
      body: JSON.stringify(backendPlayerData)
    });

    return await handleApiResponse(response);
  } catch (error) {
    console.error('Error updating player:', error);
    throw error;
  }
};

// Get all players
export const getAllPlayers = async () => {
  try {
    const response = await fetch(`${API_URL}/players`, {
      method: 'GET',
      headers: createHeaders()
    });

    const data = await handleApiResponse(response);
    
    // Map backend field names to frontend field names for each player
    return data.map(player => ({
      id: player.idPlayer,
      idPlayer: player.idPlayer, // Keep original for compatibility
      firstName: player.name,
      lastName: player.surname,
      name: player.name, // Keep original for compatibility
      surname: player.surname, // Keep original for compatibility
      dateOfBirth: player.birthday,
      weight: player.weight,
      height: player.height,
      nationality: player.nationalityName,
      position: player.positionName,
      idNationality: player.idNationality,
      idPosition: player.idPosition,
      physicalMetrics: player.latestPhysicalMetric ? {
        wingspan: player.latestPhysicalMetric.wingspan,
        verticalJump: player.latestPhysicalMetric.verticalJump,
        fatPercentage: player.latestPhysicalMetric.fatPercentage,
        benchPressWeight: player.latestPhysicalMetric.benchPressWeight,
        squatWeight: player.latestPhysicalMetric.squatWeight,
        sprintSpeed: player.latestPhysicalMetric.sprintSpeed
      } : null
    }));
  } catch (error) {
    console.error('Error fetching players:', error);
    throw error;
  }
};

// Delete player
export const deletePlayer = async (playerId) => {
  try {
    const response = await fetch(`${API_URL}/players/${playerId}`, {
      method: 'DELETE',
      headers: createHeaders()
    });

    await handleApiResponse(response);
    return true;
  } catch (error) {
    console.error('Error deleting player:', error);
    throw error;
  }
};

// Generate PDF scouting report
export const generateScoutingReport = async (filters = {}) => {
  try {
    // Send data in the format expected by GenerateScoutingReportCommand
    const requestData = {
      seasonId: filters.seasonId, // Can be null for general summaries across all seasons
      filters: {
        position: filters.position || null,
        nationality: filters.nationality || null,
        playerName: filters.playerName || null
      }
    };

    console.log('Generating scouting report with data:', requestData); // Debug log

    const response = await fetch(`${API_URL}/scoutingreports/generate-report`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify(requestData)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    // Return the PDF blob
    const blob = await response.blob();
    return blob;
  } catch (error) {
    console.error('Error generating scouting report:', error);
    throw error;
  }
};