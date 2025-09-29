<template>
  <div class="player-detail">
    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <p>Loading player information...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error">
      <p>{{ error }}</p>
      <button @click="goBack" class="btn-primary">Go Back</button>
    </div>

    <!-- Player Detail Content -->
    <div v-else-if="playerInfo && playerStats" class="player-content">
      <!-- Header -->
      <div class="player-header">
        <button @click="goBack" class="btn-back">← Back to Teams</button>
        <div class="header-spacer"></div>
      </div>

      <!-- Player Name -->
      <div class="player-name">
        <h1>{{ playerInfo.playerName }} {{ playerInfo.playerSurname }}</h1>
      </div>

      <!-- Main Content Grid -->
      <div class="player-profile-grid">
        <!-- Basic Information Section -->
        <div class="info-section basic-info">
          <h2>Basic information</h2>
          <div class="info-content">
            <div class="info-item">
              <span class="label">Number:</span>
              <span class="value">#{{ playerInfo.jerseyNumber || '0' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Nationality:</span>
              <span class="value">{{ playerInfo.playerNationality || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Position:</span>
              <span class="value">{{ playerInfo.playerPosition || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Birthday:</span>
              <span class="value">{{ playerInfo.playerBirthday || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Height:</span>
              <span class="value">{{ playerInfo.playerHeight ? playerInfo.playerHeight + 'cm' : 'N/A' }}</span>
            </div>
            <div class="info-item">
              <span class="label">Weight:</span>
              <span class="value">{{ playerInfo.playerWeight ? playerInfo.playerWeight + 'kg' : 'N/A' }}</span>
            </div>
          </div>
        </div>

        <!-- Statistics Section -->
        <div class="info-section statistics">
          <h2>Statistics</h2>
          <div v-if="loadingStats" class="loading-stats">
            Loading statistics...
          </div>
          <div v-else-if="playerStats" class="stats-content">
            <!-- Horizontal Performance Table -->
            <div class="performance-table">
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
                      <span>{{ playerStats.performance?.twoPoint?.attempted/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ playerStats.performance?.twoPoint?.made/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                    </div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-values">
                      <span>{{ playerStats.performance?.threePoint?.attempted/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ playerStats.performance?.threePoint?.made/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                    </div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-values">
                      <span>{{ playerStats.performance?.freeThrow?.attempted/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ playerStats.performance?.freeThrow?.made/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                    </div>
                  </div>
                </div>
                <div class="stats-section other-section">
                  <div class="stat-group">
                    <div class="stat-values">
                      <span>{{ playerStats.performance?.rebounds?.offensive/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                      <span>{{ playerStats.performance?.rebounds?.defensive/playerStats.performance?.general?.totalGames || 0.0 }}</span>
                    </div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single">{{ playerStats.performance?.general?.assistsAvg || 0.0 }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single">{{ playerStats.performance?.general?.stealsAvg || 0.0 }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single">{{ playerStats.performance?.general?.blocksAvg || 0.0 }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-value-single total-points">{{ playerStats.performance?.general?.pointsAvg || 0.0 }}</div>
                  </div>
                </div>
              </div>
              
              <!-- Percentage/Total row -->
              <div class="data-row percentage-row">
                <div class="stats-section shooting-section">
                  <div class="stat-group">
                    <div class="stat-percentage">{{ playerStats.performance?.twoPoint?.percentage || 0.0 }}%</div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-percentage">{{ playerStats.performance?.threePoint?.percentage || 0.0 }}%</div>
                  </div>
                  <div class="stat-group">
                    <div class="stat-percentage">{{ playerStats.performance?.freeThrow?.percentage || 0.0 }}%</div>
                  </div>
                </div>
                <div class="stats-section other-section">
                  <div class="stat-group">
                    <div class="stat-total">{{ ((playerStats.performance?.rebounds?.offensive || 0.0) + (playerStats.performance?.rebounds?.defensive || 0.0))}}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total">{{ Math.round((playerStats.performance?.general?.assistsAvg || 0.0) * (playerStats.performance?.general?.totalGames || 0)) }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total">{{ Math.round((playerStats.performance?.general?.stealsAvg || 0.0) * (playerStats.performance?.general?.totalGames || 0.0)) }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total">{{ Math.round((playerStats.performance?.general?.blocksAvg || 0.0) * (playerStats.performance?.general?.totalGames || 0.0)) }}</div>
                  </div>
                  <div class="stat-group-single">
                    <div class="stat-total total-points">{{ Math.round((playerStats.performance?.general?.pointsAvg || 0.0) * (playerStats.performance?.general?.totalGames || 0.0)) }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
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

const playerId = route.params.playerId
const teamId = route.params.teamId

// State
const loading = ref(true)
const loadingStats = ref(false)
const error = ref(null)
const playerInfo = ref(null)
const playerStats = ref(null)

const goBack = () => {
  router.push(`/analyst/team/${teamId}`)
}

// Fetch player basic information
const fetchPlayerInfo = async () => {
  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/teammember/${playerId}/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    //console.log('fetchPlayer: ', response.data.value)

    playerInfo.value = response.data.value.teamMember || response.data
    console.log('Fetched player info:', playerInfo.value)
  } catch (err) {
    console.error('Error fetching player info:', err)
    error.value = err.response?.data?.message || err.message || 'Failed to fetch player information'
  }
}

// Fetch player statistics
const fetchPlayerStatistics = async () => {
  loadingStats.value = true
  
  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/chronologicalevent/player-statistics/${playerId}/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })

    playerStats.value = response.data.value || response.data
    console.log('Fetched player statistics:', playerStats.value)
  } catch (err) {
    console.error('Error fetching player statistics:', err)
  } finally {
    loadingStats.value = false
  }
}

const fetchAllData = async () => {
  loading.value = true
  error.value = null
  console.log('playerid: ', playerId)
  console.log('teamId: ', teamId)

  await Promise.all([
    fetchPlayerInfo(),
    fetchPlayerStatistics()
  ])

  loading.value = false
}

onMounted(() => {
  fetchAllData()
})
</script>

<style scoped>
.player-detail {
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
.player-header {
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

.header-spacer {
  width: 100px; /* Balance the layout */
}

/* Player Name */
.player-name {
  text-align: center;
  margin-bottom: 2rem;
}

.player-name h1 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin: 0;
}

/* Main Grid Layout */
.player-profile-grid {
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

/* Performance Table */
.performance-table {
  margin-top: 20px;
  border: 1px solid #333;
  border-radius: 8px;
  overflow: hidden;
  background: white;
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

.total-points {
  font-weight: 700;
  color: #d97706;
}

/* Responsive Design */
@media (max-width: 768px) {
  .player-detail {
    padding: 1rem;
  }

  .player-profile-grid {
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
}

@media (max-width: 480px) {
  .player-header {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
  }

  .header-spacer {
    display: none;
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