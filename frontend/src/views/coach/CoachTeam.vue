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
      <!-- Header Button -->
      <div class="team-header">
        <button @click="goBack" class="btn-back">
          ← Back to Matches
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

const teamId = 1 // Our team ID is always 1 for coach

// Loading states
const loading = ref(true)
const loadingStats = ref(false)
const loadingPlayers = ref(false)
const error = ref(null)

// Data
const team = ref(null)
const teamStats = ref(null)
const players = ref([])
const matches = ref([])

const goBack = () => {
  router.push('/coach/matches')
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

// Utility functions
const formatFoundedYear = (foundedDate) => {
  if (!foundedDate) return null
  return new Date(foundedDate).getFullYear()
}

const handleMatchAction = (match) => {
  console.log('Navigating to previous match detail for finished match:', match)
  router.push(`/analyst/matches/${match.idMatch}/previous`)
}

const goToPlayerDetail = (playerId) => {
  console.log('Navigating to player detail:', playerId, 'from team:', teamId)
  router.push(`/coach/player/${playerId}`)
}

// Helper function to get player name from the players list
const getPlayerNameById = (playerId) => {
  const player = players.value.find(p => p.playerId === playerId || p.idPlayer === playerId)
  if (player) {
    return `${player.playerName} ${player.playerSurname}`
  }
  return `Player ${playerId}`
}

onMounted(() => {
  fetchTeamData()
  fetchTeamStatistics()
  fetchPlayers()
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

.btn-back {
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

.btn-back:hover {
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
}

@media (max-width: 480px) {
  .team-header {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
  }

  .btn-back {
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
}
</style>
