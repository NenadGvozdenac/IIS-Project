<template>
  <div class="team-detail">
    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <p>Loading team details...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error">
      <p>Error: {{ error }}</p>
      <button @click="fetchTeamData" class="btn-primary">Retry</button>
    </div>

    <!-- Team Content -->
    <div v-else-if="team" class="team-content">
      <!-- Header with Edit Button -->
      <div class="team-header">
        <button @click="goBack" class="btn-back">
          ← Back to Data Analysis
        </button>
        <button @click="goToEdit" class="btn-edit">
          Edit
        </button>
      </div>

      <!-- Team Profile Title -->
      <div class="profile-title">
        <h1>{{ team.name }} profile</h1>
      </div>

      <!-- Main Content Grid -->
      <div class="team-profile-grid">
        <!-- Basic Information Section -->
        <div class="info-section basic-info">
          <h2>Basic information</h2>
          <div class="info-content">
            <div class="info-item">
              <span class="label">Coach:</span>
              <span class="value">{{ team.coach || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Location:</span>
              <span class="value">{{ `${team.city}, ${team.state}` || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Founded:</span>
              <span class="value">{{ formatFoundedYear(team.foundedDate) || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Hall:</span>
              <span class="value">{{ team.hall || 'N/A' }}</span>
            </div>
          </div>
        </div>

        <!-- Statistics Section -->
        <div class="info-section statistics">
          <h2>Statistics</h2>
          <div v-if="loadingStats" class="loading-stats">
            Loading statistics...
          </div>
          <div v-else-if="teamStats" class="stats-content">
            <!-- Horizontal Performance Table -->
            <div class="performance-table">
              <!-- Header with playing style and head-to-head -->
              <div class="table-header">
                <div class="header-cell team-name">Playing style: {{teamStats.playingStyle}}</div>
                <div class="header-cell head-to-head">Head-to-head: {{teamStats.headToHead}}</div>
              </div>

              <!-- Stats columns header -->
              <div class="stats-header">
                <div class="stats-section shooting-section">
                  <div class="stat-group">
                    <div class="stat-header">2P</div>
                    <div class="stat-subheaders">
                      <span>2PTA</span>
                      <span>2PTM</span>
                    </div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-header">3P</div>
                    <div class="stat-subheaders">
                      <span>3PTA</span>
                      <span>3PTM</span>
                    </div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-header">FT</div>
                    <div class="stat-subheaders">
                      <span>FTA</span>
                      <span>FTM</span>
                    </div>
                  </div>
                </div>
                <div class="stats-section other-section">
                  <div class="stat-group">
                    <div class="stat-header">REB O/D</div>
                    <div class="stat-subheaders">
                      <span>AVG/TOT</span>
                    </div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-header">AST</div>
                    <div class="stat-subheaders">
                      <span>AVG/TOT</span>
                    </div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-header">STL</div>
                    <div class="stat-subheaders">
                      <span>AVG/TOT</span>
                    </div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-header">BLK</div>
                    <div class="stat-subheaders">
                      <span>AVG/TOT</span>
                    </div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-header">PTS</div>
                    <div class="stat-subheaders">
                      <span>AVG/TOT</span>
                    </div>
                  </div>
                </div>
              </div>
              
              <!-- Data rows -->
              <div class="data-row">
                <div class="stats-section shooting-section">
                  <div class="stat-group">
                    <div class="stat-values">
                      <span>{{ teamStats.performance?.twoPoint?.attempted/teamStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ teamStats.performance?.twoPoint?.made || 0.0 }}</span>
                    </div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-values">
                      <span>{{ teamStats.performance?.threePoint?.attempted/teamStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ teamStats.performance?.threePoint?.made/teamStats.performance?.general?.totalGames || 0.0 }}</span>
                    </div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-values">
                      <span>{{ teamStats.performance?.freeThrow?.attempted/teamStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ teamStats.performance?.freeThrow?.made/teamStats.performance?.general?.totalGames || 0.0 }}</span>
                    </div>
                  </div>
                </div>
                <div class="stats-section other-section">
                  <div class="stat-group">
                    <div class="stat-values">
                      <span>{{ teamStats.performance?.rebounds?.offensive/teamStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ teamStats.performance?.rebounds?.defensive/teamStats.performance?.general?.totalGames || 0.0 }}</span>
                    </div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single">{{ teamStats.performance?.general?.assistsAvg || 0.0 }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single">{{ teamStats.performance?.general?.stealsAvg || 0.0 }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single">{{ teamStats.performance?.general?.blocksAvg || 0.0 }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single total-points">{{ teamStats.performance?.general?.pointsAvg || 0.0 }}</div>
                  </div>
                </div>
              </div>
              
              <!-- Percentage/Total row -->
              <div class="data-row percentage-row">
                <div class="stats-section shooting-section">
                  <div class="stat-group">
                    <div class="stat-percentage">{{ teamStats.performance?.twoPoint?.percentage || 0.0 }}%</div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-percentage">{{ teamStats.performance?.threePoint?.percentage || 0.0 }}%</div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-percentage">{{ teamStats.performance?.freeThrow?.percentage || 0.0 }}%</div>
                  </div>
                </div>
                <div class="stats-section other-section">
                  <div class="stat-group">
                    <div class="stat-total">{{ ((teamStats.performance?.rebounds?.offensive || 0.0) + (teamStats.performance?.rebounds?.defensive || 0.0)) || 0}}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total">{{ Math.round((teamStats.performance?.general?.assistsAvg || 0.0) * (teamStats.performance?.general?.totalGames || 0)) }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total">{{ Math.round((teamStats.performance?.general?.stealsAvg || 0.0) * (teamStats.performance?.general?.totalGames || 0)) }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total">{{ Math.round((teamStats.performance?.general?.blocksAvg || 0.0) * (teamStats.performance?.general?.totalGames || 0)) }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total total-points">{{ Math.round((teamStats.performance?.general?.pointsAvg || 0.0) * (teamStats.performance?.general?.totalGames || 0)) }}</div>
                  </div>
                </div>
              </div>
            </div>
            <!-- Key Strengths and Weaknesses -->
            <div class="strengths-weaknesses">
              <div class="strengths">
                <div class="sw-label">Key Strengths:</div>
                <ul class="sw-list">
                  <li v-for="strength in teamStats.keyStrengths" :key="strength">{{ strength }}</li>
                </ul>
              </div>
              <div class="weaknesses">
                <div class="sw-label">Key Weaknesses:</div>
                <ul class="sw-list">
                  <li v-for="weakness in teamStats.keyWeaknesses" :key="weakness">{{ weakness }}</li>
                </ul>
              </div>  
            </div>
          </div>
        </div>
      </div>

      <!-- Players Section -->
      <div class="players-section">
        <h2>Players</h2>
        <div v-if="loadingPlayers" class="loading-players">
          Loading players...
        </div>
        <div v-else-if="players.length > 0" class="players-grid">
          <div 
            v-for="player in players" 
            :key="player.idPlayer" 
            class="player-card"
            @click="goToPlayerDetail(player.playerId)"
          >
            <h3>{{ player.playerName }} {{ player.playerSurname }} (#{{ player.jerseyNumber }})</h3>
            <p><strong>Position:</strong> {{ player.positionName }}</p>
            <p>Age: {{ player.age || 'N/A' }}</p>
            <p>Height: {{ player.height ? player.height + 'cm' : 'N/A' }}</p>
            <p>Weight: {{ player.weight ? player.weight + 'kg' : 'N/A' }}</p>
            <p>Status: {{ player.status || 'N/A' }}</p>
            <div class="player-card-overlay"></div>
          </div>
        </div>
        <div v-else class="no-players">
          <p>No players found for this team.</p>
        </div>
      </div>

      <!-- Match History Section -->
      <div class="match-history-section">
        <h2>Match history</h2>
        <div v-if="loadingMatches" class="loading-matches">
          Loading match history...
        </div>
        <div v-else-if="matches.length > 0" class="matches-grid">
          <div v-for="match in matches" :key="match.idMatch" class="match-card" @click="handleMatchAction(match)">
            <div class="match-result-indicator" :class="getMatchResultClass(match)">
              {{ getMatchResult(match) }}
            </div>
            <div class="match-info">
              <div class="match-name">
                {{ parseInt(teamId) === 1 ? `vs ${getOpponentName(match)}` : `vs KK Partizan` }}
              </div>
              <div class="match-details">
                <div class="match-date">{{ formatMatchDate(match.scheduledAt) }}</div>
                <div class="match-location">{{ match.state }}, {{ match.city }} - {{ match.hall }}</div>
                <div class="match-competition" v-if="match.competitionName">{{ match.competitionName }}</div>
              </div>
            </div>
            <div class="match-score">
              {{ getOurScore(match) }}-{{ getOpponentScore(match) }}
            </div>
            <div class="player-card-overlay"></div>
          </div>
        </div>
        <div v-else class="no-matches">
          <p>No match history available.</p>
        </div>
      </div>

      <!-- Season Statistics Section -->
      <div class="season-statistics-section">
        <h2>Season Player Statistics</h2>
        
        <!-- Date Range Controls -->
        <div class="season-controls">
          <div class="date-inputs">
            <div class="date-input-group">
              <label for="season-start">Start Date:</label>
              <input 
                id="season-start"
                type="date" 
                v-model="seasonStartDate" 
                class="date-input"
              />
            </div>
            <div class="date-input-group">
              <label for="season-end">End Date:</label>
              <input 
                id="season-end"
                type="date" 
                v-model="seasonEndDate" 
                class="date-input"
              />
            </div>
          </div>
          <button 
            @click="fetchSeasonAverages" 
            :disabled="loadingSeasonAverages"
            class="btn-fetch-season"
          >
            {{ loadingSeasonAverages ? 'Loading...' : 'Fetch Season Statistics' }}
          </button>
        </div>

        <!-- Loading State -->
        <div v-if="loadingSeasonAverages" class="loading-season">
          Loading season statistics...
        </div>

        <!-- Error State -->
        <div v-else-if="seasonAveragesError" class="error-season">
          <p>Error: {{ seasonAveragesError }}</p>
          <button @click="fetchSeasonAverages" class="btn-primary">Retry</button>
        </div>

        <!-- Season Statistics Table -->
        <div v-else-if="seasonAverages.length > 0" class="season-table-container">
          <table class="season-stats-table">
            <thead>
              <tr>
                <th class="player-col">Player</th>
                <th class="matches-col">Matches</th>
                <th class="avg-col">Avg Points</th>
                <th class="avg-col">Avg Assists</th>
                <th class="avg-col">Avg Fouls</th>
                <th class="total-col">Total Points</th>
                <th class="total-col">Total Assists</th>
                <th class="total-col">Total Fouls</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="player in seasonAverages" :key="player.playerId" class="season-player-row">
                <td class="player-name-cell">
                  <div class="player-info">
                    <span class="player-name">{{ getPlayerNameById(parseInt(player.playerId)) || `Player ${player.playerId}` }}</span>
                  </div>
                </td>
                <td class="matches-cell">{{ player.matchesPlayed || 0 }}</td>
                <td class="avg-cell">{{ (player.avgPoints || 0).toFixed(2) }}</td>
                <td class="avg-cell">{{ (player.avgAssists || 0).toFixed(2) }}</td>
                <td class="avg-cell">{{ (player.avgFouls || 0).toFixed(2) }}</td>
                <td class="total-cell">{{ Math.round(player.totalPoints || 0) }}</td>
                <td class="total-cell">{{ Math.round(player.totalAssists || 0) }}</td>
                <td class="total-cell">{{ Math.round(player.totalFouls || 0) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- No Data State -->
        <div v-else class="no-season-data">
          <p>No season statistics available for the selected date range.</p>
          <p>Try adjusting the date range or ensure there are matches in this period.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'

const route = useRoute()
const router = useRouter()

const teamId = route.params.id

// Loading states
const loading = ref(true)
const loadingStats = ref(false)
const loadingPlayers = ref(false)
const loadingMatches = ref(false)
const error = ref(null)

// Data
const team = ref(null)
const teamStats = ref(null)
const players = ref([])
const matches = ref([])

// Season averages data
const seasonStartDate = ref(new Date(new Date().getFullYear(), 0, 1).toISOString().substr(0, 10))
const seasonEndDate = ref(new Date().toISOString().substr(0, 10))
const loadingSeasonAverages = ref(false)
const seasonAveragesError = ref(null)
const seasonAverages = ref([])

const goBack = () => {
  router.push('/analyst')
}

const goToEdit = () => {
  router.push(`/analyst/team/${teamId}/edit`)
}

// Fetch team basic information
const fetchTeamData = async () => {
  loading.value = true
  error.value = null

  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/team/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })

    team.value = response.data.value || response.data
    console.log('Fetched team:', team.value)
  } catch (err) {
    console.error('Error fetching team:', err)
    error.value = err.message || 'Failed to fetch team data'
  } finally {
    loading.value = false
  }
}

// Fetch team statistics
const fetchTeamStatistics = async () => {
  loadingStats.value = true

  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/chronologicalevent/team-statistics/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })

    teamStats.value = response.data.value || response.data
    console.log('Fetched team statistics:', teamStats.value)
  } catch (err) {
    console.error('Error fetching team statistics:', err)
  } finally {
    loadingStats.value = false
  }
}

// Fetch team players
const fetchPlayers = async () => {
  loadingPlayers.value = true

  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/teammember/team/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    
    players.value = response.data.value?.teamPlayers || []
    console.log('Fetched players:', players.value)
  } catch (err) {
    console.error('Error fetching players:', err)
  } finally {
    loadingPlayers.value = false
  }
}

// Fetch team matches
const fetchMatches = async () => {
  loadingMatches.value = true

  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/match/finished/team/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    
    // Use the filtered finished matches from the new endpoint
    matches.value = response.data.value?.matches || []
    
    console.log('Fetched finished matches:', matches.value)
  } catch (err) {
    console.error('Error fetching finished matches:', err)
  } finally {
    loadingMatches.value = false
  }
}

// Utility functions
const formatFoundedYear = (foundedDate) => {
  if (!foundedDate) return null
  return new Date(foundedDate).getFullYear()
}

const formatMatchDate = (dateString) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-GB', {
    day: '2-digit',
    month: '2-digit', 
    year: 'numeric'
  })
}

const getOpponentName = (match) => {
  // Use the team name from the backend response
  return match.teamName || `Team ${match.idTeam}`
}

const getMatchResult = (match) => {
  const isOurTeam = parseInt(teamId) === 1
  let ourScore, opponentScore
  
  if (isOurTeam) {
    // For our team, we use OurPoints vs OpponentPoints
    ourScore = match.ourPoints || 0
    opponentScore = match.opponentPoints || 0
  } else {
    // For opponent teams, we flip the perspective
    ourScore = match.opponentPoints || 0
    opponentScore = match.ourPoints || 0
  }
  
  if (ourScore > opponentScore) return 'W'
  if (ourScore < opponentScore) return 'L'
  return 'D'
}

const getMatchResultClass = (match) => {
  const result = getMatchResult(match)
  return {
    'win': result === 'W',
    'loss': result === 'L',
    'draw': result === 'D'
  }
}

const getOurScore = (match) => {
  const isOurTeam = parseInt(teamId) === 1
  if (isOurTeam) {
    return match.ourPoints || 0
  } else {
    return match.opponentPoints || 0
  }
}

const getOpponentScore = (match) => {
  const isOurTeam = parseInt(teamId) === 1
  if (isOurTeam) {
    return match.opponentPoints || 0
  } else {
    return match.ourPoints || 0
  }
}

const handleMatchAction = (match) => {
  console.log('Navigating to previous match detail for finished match:', match)
  router.push(`/analyst/matches/${match.idMatch}/previous`)
}

const goToPlayerDetail = (playerId) => {
  console.log('Navigating to player detail:', playerId, 'from team:', teamId)
  router.push(`/analyst/player/${playerId}/${teamId}`)
}

// Helper function to get player name from the players list
const getPlayerNameById = (playerId) => {
  const player = players.value.find(p => p.playerId === playerId || p.idPlayer === playerId)
  if (player) {
    return `${player.playerName} ${player.playerSurname}`
  }
  return `Player ${playerId}`
}

// Fetch season player averages
const fetchSeasonAverages = async () => {
  loadingSeasonAverages.value = true
  seasonAveragesError.value = null
  seasonAverages.value = []

  try {
    const jwt = localStorage.getItem('token')
    //console.log('Fetching season averages for team:', teamId, 'from', seasonStartDate.value, 'to', seasonEndDate.value)
    // Convert date strings to ISO format with time
    const startDateISO = new Date(seasonStartDate.value + 'T00:00:00Z').toISOString()
    const endDateISO = new Date(seasonEndDate.value + 'T23:59:59Z').toISOString()
    //console.log('Using date range:', startDateISO, 'to', endDateISO)
    
    // Build URL with proper encoding
    const url = `${MATCHES_URL}/ChronologicalEventInflux/season-averages`
    const params = {
      startDate: startDateISO,
      endDate: endDateISO,
      teamId: teamId,
      minMatches: 1
    }
    
    const response = await axios.get(url, {
      params: params,
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })

    // Handle CreateResponse wrapper - try to access .value first, fallback to direct data
    const data = response.data.value || response.data
    const rawPlayerAverages = data?.playerAverages || data || []
    
    // Map and enrich the data with proper field names and player names
    seasonAverages.value = rawPlayerAverages.map(player => ({
      playerId: player.playerId,
      playerName: getPlayerNameById(player.playerId),
      matchesPlayed: player.matchesPlayed || 0,
      avgPoints: player.avgPoints || 0,
      avgAssists: player.avgAssists || 0,
      avgRebounds: (player.avgRebounds || 0), // Calculate total rebounds if separate off/def exist
      avgFouls: player.avgFouls || 0,
      avgEfficiency: player.avgEfficiency || 0,
      totalPoints: player.totalPoints || 0,
      // Add other available fields from the response
      pointsStdDev: player.pointsStdDev || 0,
      totalAssists: player.totalAssists || 0,
      totalFouls: player.totalFouls || 0
    }))
    
    console.log('Processed season averages:', seasonAverages.value)
  } catch (err) {
    console.error('Error fetching season averages:', err)
    seasonAveragesError.value = err.message || 'Failed to fetch season averages'
  } finally {
    loadingSeasonAverages.value = false
  }
}

onMounted(() => {
  fetchTeamData()
  fetchTeamStatistics()
  fetchPlayers()
  fetchMatches()
})
</script>

<style scoped>
.team-detail {
  min-height: 100vh;
  background-color: #f8f9fa;
  padding: 2rem;
}

/* Loading and Error States */
.loading, .error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  text-align: center;
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  max-width: 600px;
  margin: 2rem auto;
}

.loading p, .error p {
  font-size: 1.1rem;
  color: #666;
  margin-bottom: 1rem;
}

.btn-primary {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-primary:hover {
  background-color: #0056b3;
}

/* Header */
.team-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
}

.btn-back, .btn-edit {
  background: none;
  border: 2px solid #333;
  color: #333;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-back:hover, .btn-edit:hover {
  background-color: #333;
  color: white;
}

/* Profile Title */
.profile-title {
  text-align: center;
  margin-bottom: 2rem;
}

.profile-title h1 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin: 0;
}

/* Main Grid Layout */
.team-profile-grid {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto 3rem auto;
}

/* Info Sections */
.info-section {
  background: white;
  border: 2px solid #333;
  border-radius: 8px;
  padding: 1.5rem;
}

.info-section h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #333;
  border-bottom: 1px solid #e9ecef;
  padding-bottom: 0.5rem;
}

/* Basic Information */
.basic-info .info-content {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.info-item .label {
  font-weight: 500;
  color: #666;
}

.info-item .value {
  font-weight: 600;
  color: #333;
}

/* Statistics Section */
.statistics {
  background: white;
  border: 2px solid #333;
  border-radius: 8px;
  padding: 1.5rem;
}

.statistics h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #333;
  border-bottom: 1px solid #e9ecef;
  padding-bottom: 0.5rem;
}

.loading-stats {
  text-align: center;
  color: #666;
  font-style: italic;
  padding: 2rem;
}

.stats-content {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.basic-stats {
  display: flex;
  gap: 32px;
  flex-wrap: wrap;
}

.stats-row {
  display: flex;
  gap: 8px;
  align-items: center;
}

.stat-label {
  font-weight: 600;
  color: #374151;
  min-width: 120px;
}

.stat-value {
  color: #1f2937;
}

/* Performance Table */
.performance-table {
  margin-top: 20px;
  border: 1px solid #333;
  border-radius: 8px;
  overflow: hidden;
  background: white;
}

.table-header {
  display: grid;
  grid-template-columns: 1fr 1fr;
  background: #f8fafc;
  border-bottom: 1px solid #333;
}

.header-cell {
  padding: 12px 16px;
  font-size: 14px;
  font-weight: 600;
  color: #374151;
  border-right: 1px solid #e5e7eb;
}

.header-cell:last-child {
  border-right: none;
}

.head-to-head {
  text-align: right;
}

.stats-header {
  display: grid;
  grid-template-columns: 3fr 6fr;
  border-bottom: 1px solid #333;
  background: #f8fafc;
}

.stats-section {
  display: flex;
  border-right: 1px solid #333;
}

.stats-section:last-child {
  border-right: none;
}

.shooting-section {
  background: #f0f9ff;
}

.other-section {
  background: #f8fafc;
}

.stat-group {
  flex: 1;
  border-right: 1px solid #e5e7eb;
  text-align: center;
}

.stat-group:last-child {
  border-right: none;
}

.stat-group-single {
  flex: 1;
  border-right: 1px solid #e5e7eb;
  text-align: center;
}

.stat-group-single:last-child {
  border-right: none;
}

.stat-header {
  padding: 8px 4px;
  font-weight: 600;
  font-size: 14px;
  color: #374151;
  border-bottom: 1px solid #e5e7eb;
}

.stat-subheaders {
  display: flex;
  font-size: 12px;
  font-weight: 500;
  color: #6b7280;
}

.stat-subheaders span {
  flex: 1;
  padding: 6px 2px;
  border-right: 1px solid #e5e7eb;
}

.stat-subheaders span:last-child {
  border-right: none;
}

.data-row {
  display: grid;
  grid-template-columns: 3fr 6fr;
  border-bottom: 1px solid #e5e7eb;
}

.data-row:last-child {
  border-bottom: none;
}

.percentage-row {
  background: #f3f4f6;
  font-weight: 600;
}

.stat-values {
  display: flex;
  font-size: 14px;
}

.stat-values span {
  flex: 1;
  padding: 12px 4px;
  border-right: 1px solid #e5e7eb;
  text-align: center;
}

.stat-values span:last-child {
  border-right: none;
}

.stat-value-single {
  padding: 12px 8px;
  text-align: center;
  font-size: 14px;
}

.stat-percentage {
  padding: 12px 8px;
  text-align: center;
  font-size: 14px;
  color: #059669;
  font-weight: 600;
}

.stat-total {
  padding: 12px 8px;
  text-align: center;
  font-size: 14px;
  font-weight: 600;
  color: #1f2937;
}

.sw-label {
  font-weight: 600;
  color: #374151;
  margin-bottom: 12px;
  font-size: 16px;
}

.sw-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.sw-list li {
  padding: 8px 12px;
  background: #f3f4f6;
  margin-bottom: 8px;
  border-radius: 6px;
  color: #4b5563;
}

.strengths .sw-list li {
  background: #ecfdf5;
  color: #065f46;
  border-left: 4px solid #10b981;
}

.weaknesses .sw-list li {
  background: #fef2f2;
  color: #991b1b;
  border-left: 4px solid #ef4444;
}

.strengths-weaknesses {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin-top: 1rem;
}

.total-points {
  font-weight: 700;
  color: #d97706;
}

/* Players Section */
.players-section {
  background: white;
  border: 2px solid #333;
  border-radius: 8px;
  padding: 1.5rem;
  max-width: 1200px;
  margin: 0 auto 3rem auto;
}

.players-section h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #333;
  border-bottom: 1px solid #e9ecef;
  padding-bottom: 0.5rem;
}

.loading-players, .no-players {
  text-align: center;
  color: #666;
  font-style: italic;
  padding: 2rem;
}

.players-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.player-card {
  background-color: #f8f9fa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1rem;
  text-align: left;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  transform: translateY(0);
}

.player-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
}
.match-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
}


.player-card h3 {
  font-size: 0.9rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #333;
  transition: color 0.3s ease;
}

.player-card:hover h3 {
  color: #007bff;
}
.match-card:hover h3 {
  color: #007bff;
}

.player-card p {
  font-size: 0.8rem;
  color: #666;
  margin: 0.2rem 0;
  transition: color 0.3s ease;
}

.player-card:hover p {
  color: #444;
}

.player-card-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 123, 255, 0.9);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
  font-weight: 600;
  font-size: 0.9rem;
}

.player-card:hover .player-card-overlay {
  opacity: 0.2;
}
.match-card:hover .player-card-overlay {
  opacity: 0.2;
}

/* Match History Section */
.match-history-section {
  background: white;
  border: 2px solid #333;
  border-radius: 8px;
  padding: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
}

.match-history-section h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #333;
  border-bottom: 1px solid #e9ecef;
  padding-bottom: 0.5rem;
}

.loading-matches, .no-matches {
  text-align: center;
  color: #666;
  font-style: italic;
  padding: 2rem;
}

.matches-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.match-card {
  background-color: #f8f9fa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1rem;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  transform: translateY(0);
  display: flex;
  align-items: center;
  gap: 1rem;
  cursor: pointer;
}

.match-result-indicator {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 0.8rem;
  color: white;
}

.match-result-indicator.win {
  background-color: #28a745;
}

.match-result-indicator.loss {
  background-color: #dc3545;
}

.match-result-indicator.draw {
  background-color: #6c757d;
}

.match-info {
  flex: 1;
}

.match-name {
  font-weight: 600;
  color: #333;
  margin-bottom: 0.25rem;
}

.match-details {
  font-size: 0.8rem;
  color: #666;
}

.match-competition {
  font-weight: 500;
  color: #007bff;
  margin-top: 0.25rem;
}

.match-score {
  font-weight: 600;
  color: #333;
  font-size: 0.9rem;
}

/* Season Statistics Section */
.season-statistics-section {
  background: white;
  border: 2px solid #333;
  border-radius: 8px;
  padding: 1.5rem;
  max-width: 1200px;
  margin: 3rem auto 0 auto;
}

.season-statistics-section h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #333;
  border-bottom: 1px solid #e9ecef;
  padding-bottom: 0.5rem;
}

.season-controls {
  display: flex;
  align-items: end;
  gap: 2rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.date-inputs {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.date-input-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.date-input-group label {
  font-weight: 600;
  color: #333;
  font-size: 0.9rem;
}

.date-input {
  padding: 0.5rem;
  border: 2px solid #ddd;
  border-radius: 4px;
  font-size: 0.9rem;
  min-width: 140px;
}

.date-input:focus {
  outline: none;
  border-color: #007bff;
}

.btn-fetch-season {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 0.6rem 1.5rem;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s ease;
  white-space: nowrap;
}

.btn-fetch-season:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn-fetch-season:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.loading-season, .error-season, .no-season-data {
  text-align: center;
  padding: 2rem;
  color: #666;
  font-style: italic;
}

.error-season {
  color: #dc3545;
}

.season-table-container {
  overflow-x: auto;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  margin-top: 1rem;
}

.season-stats-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  font-size: 14px;
  min-width: 800px;
}

.season-stats-table th {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
  color: white;
  font-weight: 600;
  text-align: center;
  padding: 12px 8px;
  border-bottom: 2px solid #004494;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.player-col {
  width: 200px;
  text-align: left !important;
}

.matches-col {
  width: 80px;
}

.avg-col {
  width: 90px;
}

.efficiency-col {
  width: 90px;
}

.total-col {
  width: 100px;
}

.season-stats-table td {
  padding: 10px 8px;
  vertical-align: middle;
  border-bottom: 1px solid rgba(0,0,0,0.06);
  text-align: center;
}

.season-player-row:nth-child(even) {
  background-color: #fbfbfb;
}

.season-player-row:hover {
  background-color: #f1f5f9;
}

.player-name-cell {
  text-align: left;
}

.player-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.player-name {
  font-weight: 600;
  color: #333;
}

.matches-cell {
  font-weight: 600;
  color: #007bff;
}

.avg-cell {
  font-weight: 500;
  color: #333;
}

.efficiency-cell {
  font-weight: 700;
  font-size: 15px;
}

.efficiency-cell.positive-eff {
  color: #28a745;
}

.efficiency-cell.negative-eff {
  color: #dc3545;
}

.total-cell {
  font-weight: 700;
  color: #e67e22;
  font-size: 15px;
}

/* Responsive Design */
@media (max-width: 768px) {
  .team-detail {
    padding: 1rem;
  }

  .team-profile-grid {
    grid-template-columns: 1fr;
  }

  .stats-header {
    grid-template-columns: 2fr 4fr;
  }

  .data-row {
    grid-template-columns: 2fr 4fr;
  }

  .stat-header {
    font-size: 12px;
    padding: 6px 2px;
  }

  .stat-subheaders span {
    font-size: 10px;
    padding: 4px 1px;
  }

  .stat-values span {
    font-size: 12px;
    padding: 8px 2px;
  }

  .stat-value-single, .stat-percentage, .stat-total {
    font-size: 12px;
    padding: 8px 4px;
  }

  .players-grid, .matches-grid {
    grid-template-columns: 1fr;
  }

  .strengths-weaknesses {
    grid-template-columns: 1fr;
  }

  .season-controls {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }

  .date-inputs {
    justify-content: space-between;
  }

  .season-stats-table {
    font-size: 12px;
  }

  .season-stats-table th,
  .season-stats-table td {
    padding: 8px 4px;
  }
}

@media (max-width: 480px) {
  .team-header {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
  }

  .btn-back, .btn-edit {
    text-align: center;
  }

  .performance-table {
    overflow-x: auto;
  }

  .stats-header, .data-row {
    min-width: 500px;
  }

  .stat-header {
    font-size: 10px;
    padding: 4px 1px;
  }

  .stat-subheaders span {
    font-size: 8px;
    padding: 2px;
  }

  .stat-values span, .stat-value-single, .stat-percentage, .stat-total {
    font-size: 10px;
    padding: 6px 2px;
  }

  .season-controls {
    gap: 0.5rem;
  }

  .date-inputs {
    flex-direction: column;
    gap: 0.5rem;
  }

  .season-stats-table {
    font-size: 11px;
    min-width: 600px;
  }

  .season-stats-table th,
  .season-stats-table td {
    padding: 6px 3px;
  }
}
</style>
