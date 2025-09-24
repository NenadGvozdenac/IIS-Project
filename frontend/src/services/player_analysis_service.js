import { SCOUTINGS_URL } from './const_service.js';

const API_BASE_URL = SCOUTINGS_URL

export async function getPlayerSeasonMetricAverages(playerId, seasonId, sessionTypeFilter = null) {
  try {
    let url = `${API_BASE_URL}/players/${playerId}/season/${seasonId}/metrics/averages`
    
    if (sessionTypeFilter) {
      url += `?sessionType=${sessionTypeFilter}`
    }
    
    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    if (!response.ok) {
      throw new Error(`Failed to fetch player metric averages: ${response.status}`)
    }
    
    const data = await response.json()
    return data.value || data // Handle both wrapped and unwrapped responses
  } catch (error) {
    console.error('Error fetching player season metric averages:', error)
    throw error
  }
}

export async function getPlayerSessions(playerId, filters = {}) {
  try {
    const queryParams = new URLSearchParams()
    
    if (filters.seasonId) queryParams.append('seasonId', filters.seasonId)
    if (filters.status) queryParams.append('status', filters.status)
    if (filters.dateFrom) queryParams.append('dateFrom', filters.dateFrom)
    if (filters.dateTo) queryParams.append('dateTo', filters.dateTo)
    
    const url = `${API_BASE_URL}/players/${playerId}/sessions${queryParams.toString() ? '?' + queryParams.toString() : ''}`
    
    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    if (!response.ok) {
      throw new Error(`Failed to fetch player sessions: ${response.status}`)
    }
    
    const data = await response.json()
    return data.value || data // Handle both wrapped and unwrapped responses
  } catch (error) {
    console.error('Error fetching player sessions:', error)
    throw error
  }
}

export async function getSeasons() {
  try {
    const response = await fetch(`${API_BASE_URL}/seasons`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    if (!response.ok) {
      throw new Error(`Failed to fetch seasons: ${response.status}`)
    }
    
    const data = await response.json()
    const result = data.value || data
    // Handle nested seasons structure
    return result.seasons || result
  } catch (error) {
    console.error('Error fetching seasons:', error)
    throw error
  }
}

export async function getSessionTypes() {
  try {
    const response = await fetch(`${API_BASE_URL}/session-types`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    if (!response.ok) {
      throw new Error(`Failed to fetch session types: ${response.status}`)
    }
    
    const data = await response.json()
    return data.value || data // Handle both wrapped and unwrapped responses
  } catch (error) {
    console.error('Error fetching session types:', error)
    throw error
  }
}