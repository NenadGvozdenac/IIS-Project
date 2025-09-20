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
            <div class="stats-row">
              <span class="stat-label">Playing style:</span>
              <span class="stat-value">{{ teamStats.playingStyle || 'N/A' }}</span>
            </div>
            <div class="stats-row">
              <span class="stat-label">Head-to-head:</span>
              <span class="stat-value">{{ teamStats.headToHead || 'N/A' }}</span>
            </div>
            
            <!-- Performance Table -->
            <div class="performance-table">
              <div class="table-header">
                <div class="header-cell"></div>
                <div class="header-cell">2P</div>
                <div class="header-cell">3P</div>
                <div class="header-cell">FT</div>
                <div class="header-cell">REB O/D</div>
                <div class="header-cell">AST</div>
                <div class="header-cell">STL</div>
                <div class="header-cell">BLK</div>
                <div class="header-cell">PTS</div>
              </div>
              
              <div class="table-row">
                <div class="cell row-label">{{ team.name }}</div>
                <div class="cell">
                  <div class="stat-fraction">{{ teamStats.performance.twoPoint.made }}/{{ teamStats.performance.twoPoint.attempted }}</div>
                  <div class="stat-percentage">{{ teamStats.performance.twoPoint.percentage }}%</div>
                </div>
                <div class="cell">
                  <div class="stat-fraction">{{ teamStats.performance.threePoint.made }}/{{ teamStats.performance.threePoint.attempted }}</div>
                  <div class="stat-percentage">{{ teamStats.performance.threePoint.percentage }}%</div>
                </div>
                <div class="cell">
                  <div class="stat-fraction">{{ teamStats.performance.freeThrow.made }}/{{ teamStats.performance.freeThrow.attempted }}</div>
                  <div class="stat-percentage">{{ teamStats.performance.freeThrow.percentage }}%</div>
                </div>
                <div class="cell">
                  <div class="stat-fraction">{{ teamStats.performance.rebounds.offensive }}/{{ teamStats.performance.rebounds.defensive }}</div>
                  <div class="stat-total">{{ teamStats.performance.rebounds.total }}</div>
                </div>
                <div class="cell">{{ teamStats.performance.general.assistsAvg }}</div>
                <div class="cell">{{ teamStats.performance.general.stealsAvg }}</div>
                <div class="cell">{{ teamStats.performance.general.blocksAvg }}</div>
                <div class="cell">{{ teamStats.performance.general.pointsAvg }}</div>
              </div>
              
              <div class="table-row totals-row">
                <div class="cell row-label">TOT</div>
                <div class="cell">{{ teamStats.performance.general.totalGames }}</div>
                <div class="cell">{{ teamStats.performance.general.wins }}</div>
                <div class="cell">{{ teamStats.performance.general.losses }}</div>
                <div class="cell">{{ teamStats.performance.general.winPercentage }}%</div>
                <div class="cell">{{ teamStats.performance.general.totalGames }}</div>
                <div class="cell">{{ Math.round(teamStats.performance.general.pointsAvg * teamStats.performance.general.totalGames) }}</div>
                <div class="cell"></div>
                <div class="cell"></div>
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
          <div v-for="player in players" :key="player.idPlayer" class="player-card">
            <h3>{{ player.playerName }} {{ player.playerSurname }} (#{{ player.jerseyNumber }})</h3>
            <p><strong>Position:</strong> {{ player.positionName }}</p>
            <p>Age: {{ player.age || 'N/A' }}</p>
            <p>Height: {{ player.height ? player.height + 'cm' : 'N/A' }}</p>
            <p>Weight: {{ player.weight ? player.weight + 'kg' : 'N/A' }}</p>
            <p>Status: {{ player.status || 'N/A' }}</p>
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
                {{ parseInt(teamId) === 1 ? `KK Partizan vs ${getOpponentName(match)}` : 
                                            `${getOpponentName(match)} vs KK Partizan` }}
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
          </div>
        </div>
        <div v-else class="no-matches">
          <p>No match history available.</p>
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
.loading-stats {
  text-align: center;
  color: #666;
  font-style: italic;
  padding: 2rem;
}

.stats-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.stats-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
}

.stat-label {
  font-weight: 500;
  color: #666;
}

.stat-value {
  font-weight: 600;
  color: #333;
}

/* Performance Table */
.performance-table {
  margin-top: 1rem;
  border: 1px solid #e9ecef;
  border-radius: 6px;
  overflow: hidden;
}

.table-header, .table-row {
  display: grid;
  grid-template-columns: 1fr repeat(8, 0.8fr);
  border-bottom: 1px solid #e9ecef;
}

.table-row:last-child {
  border-bottom: none;
}

.header-cell, .cell {
  padding: 0.5rem 0.25rem;
  text-align: center;
  font-size: 0.85rem;
  border-right: 1px solid #e9ecef;
}

.header-cell:last-child, .cell:last-child {
  border-right: none;
}

.header-cell {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #333;
}

.row-label {
  font-weight: 600;
  text-align: left !important;
  padding-left: 0.75rem;
}

.stat-fraction {
  font-size: 0.8rem;
  color: #333;
}

.stat-percentage {
  font-size: 0.75rem;
  color: #666;
}

.stat-total {
  font-size: 0.8rem;
  font-weight: 600;
  color: #333;
}

.totals-row {
  background-color: #f8f9fa;
  font-weight: 600;
}

/* Strengths and Weaknesses */
.strengths-weaknesses {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-top: 1rem;
}

.sw-label {
  font-weight: 600;
  color: #333;
  margin-bottom: 0.5rem;
}

.sw-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.sw-list li {
  padding: 0.25rem 0;
  font-size: 0.9rem;
  color: #666;
}

.sw-list li:before {
  content: "• ";
  color: #007bff;
  font-weight: bold;
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
}

.player-card h3 {
  font-size: 0.9rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #333;
}

.player-card p {
  font-size: 0.8rem;
  color: #666;
  margin: 0.2rem 0;
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

/* Responsive Design */
@media (max-width: 768px) {
  .team-detail {
    padding: 1rem;
  }

  .team-profile-grid {
    grid-template-columns: 1fr;
  }

  .table-header, .table-row {
    grid-template-columns: 1.2fr repeat(9, 0.6fr);
  }

  .header-cell, .cell {
    padding: 0.4rem 0.15rem;
    font-size: 0.75rem;
  }

  .players-grid, .matches-grid {
    grid-template-columns: 1fr;
  }

  .strengths-weaknesses {
    grid-template-columns: 1fr;
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

  .table-header, .table-row {
    min-width: 600px;
  }
}
</style>
