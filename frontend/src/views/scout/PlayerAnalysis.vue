<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <!-- Header -->
        <div class="page-header">
          <div class="header-content">
            <h1>Player Analysis</h1>
            <p v-if="player">{{ player.firstName }} {{ player.lastName }}</p>
          </div>
          <div class="header-actions">
            <button @click="goBack" class="btn btn-secondary">
              Back to Profile
            </button>
          </div>
        </div>

        <div v-if="loading" class="loading">
          Loading player analysis...
        </div>

        <div v-else-if="error" class="error-message">
          <div class="error-content">
            <h3>Error Loading Analysis</h3>
            <p>{{ error }}</p>
            <button @click="loadData" class="btn btn-primary">
              Try Again
            </button>
          </div>
        </div>

        <div v-else-if="player" class="analysis-layout">
          <!-- Player Basic Info Section -->
          <div class="info-section">
            <div class="section-card">
              <h2>Player Information</h2>
              <div class="player-info-grid">
                <div class="info-item">
                  <label>Full Name</label>
                  <span>{{ player.firstName }} {{ player.lastName }}</span>
                </div>
                <div class="info-item">
                  <label>Position</label>
                  <span>{{ player.position }}</span>
                </div>
                <div class="info-item">
                  <label>Age</label>
                  <span>{{ calculateAge(player.dateOfBirth) }} years</span>
                </div>
                <div class="info-item">
                  <label>Height</label>
                  <span>{{ player.height }} cm</span>
                </div>
                <div class="info-item">
                  <label>Weight</label>
                  <span>{{ player.weight }} kg</span>
                </div>
                <div class="info-item">
                  <label>Nationality</label>
                  <span>{{ player.nationality }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Main Content Layout -->
          <div class="content-layout">
            <!-- Sessions Table Section -->
            <div class="sessions-section">
              <div class="section-card">
                <div class="section-header">
                  <h2>Scouting Sessions</h2>
                  <div class="filters">
                    <select v-model="sessionFilters.status" @change="loadSessions">
                      <option value="">All Statuses</option>
                      <option value="Pending">Pending</option>
                      <option value="Ongoing">Ongoing</option>
                      <option value="Finished">Finished</option>
                      <option value="Canceled">Canceled</option>
                    </select>
                    <input 
                      type="date" 
                      v-model="sessionFilters.dateFrom" 
                      @change="loadSessions"
                      placeholder="From Date"
                    >
                    <input 
                      type="date" 
                      v-model="sessionFilters.dateTo" 
                      @change="loadSessions"
                      placeholder="To Date"
                    >
                  </div>
                </div>
                
                <div v-if="sessionsLoading" class="table-loading">
                  Loading sessions...
                </div>
                
                <div v-else-if="sessions.length === 0" class="no-data">
                  No sessions found for the selected filters.
                </div>
                
                <div v-else class="table-container">
                  <table class="sessions-table">
                    <thead>
                      <tr>
                        <th>Date</th>
                        <th>Type</th>
                        <th>Status</th>
                        <th>Scout</th>
                        <th>Action</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="session in sessions" :key="session.sessionId" class="session-row">
                        <td>{{ formatDate(session.startTime) }}</td>
                        <td>
                          <span :class="`type-badge type-${session.sessionType ? session.sessionType.toLowerCase().replace(' ', '-') : 'unknown'}`">
                            {{ session.sessionType || 'Unknown' }}
                          </span>
                        </td>
                        <td>
                          <span :class="`status-badge status-${session.sessionStatus ? session.sessionStatus.toLowerCase() : 'unknown'}`">
                            {{ session.sessionStatus || 'Unknown' }}
                          </span>
                        </td>
                        <td>{{ session.scoutName }} {{ session.scoutSurname }}</td>
                        <td>
                          <button 
                            @click="viewSession(session.sessionId)" 
                            class="btn btn-sm btn-view"
                            :disabled="session.sessionStatus === 'Canceled'"
                          >
                            View
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- Metrics Averages Section -->
            <div class="metrics-section">
              <div class="section-card">
                <div class="section-header">
                  <h2>Season Performance</h2>
                  <div class="filters">
                    <select v-model="selectedSeason" @change="loadMetrics">
                      <option v-for="season in seasons" :key="season.id" :value="season.id">
                        {{ season.name }}
                      </option>
                    </select>
                    <select v-model="selectedSessionType" @change="loadMetrics">
                      <option value="">All Types</option>
                      <option value="Training">Training</option>
                      <option value="Game">Game</option>
                      <option value="Playoff Game">Playoff Game</option>
                    </select>
                  </div>
                </div>
                
                <div v-if="metricsLoading" class="metrics-loading">
                  Loading metrics...
                </div>
                
                <div v-else-if="metrics.length === 0" class="no-data">
                  No metrics data available for the selected filters.
                </div>
                
                <div v-else class="metrics-grid">
                  <div v-for="metric in metrics" :key="metric.metricId" class="metric-card">
                    <div class="metric-header">
                      <h3>{{ metric.metricName }}</h3>
                      <span class="metric-weight">Weight: {{ metric.metricWeight }}</span>
                    </div>
                    <div class="metric-value">
                      <span class="value">{{ metric.averageValue }}</span>
                      <span class="sessions">from {{ metric.sessionCount }} session(s)</span>
                    </div>
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
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getPlayerById } from '../../services/player_service.js'
import { 
  getPlayerSeasonMetricAverages, 
  getPlayerSessions, 
  getSeasons, 
  getSessionTypes 
} from '../../services/player_analysis_service.js'

const route = useRoute()
const router = useRouter()

// Reactive data
const player = ref(null)
const sessions = ref([])
const metrics = ref([])
const seasons = ref([])
const sessionTypes = ref([])

const loading = ref(true)
const sessionsLoading = ref(false)
const metricsLoading = ref(false)
const error = ref(null)

const playerId = route.params.id
const selectedSeason = ref(1) // Default to first season
const selectedSessionType = ref('')

// Session filters
const sessionFilters = ref({
  status: '',
  dateFrom: '',
  dateTo: ''
})

// Computed properties
const calculateAge = (dateOfBirth) => {
  if (!dateOfBirth) return 'N/A'
  const today = new Date()
  const birth = new Date(dateOfBirth)
  let age = today.getFullYear() - birth.getFullYear()
  const monthDiff = today.getMonth() - birth.getMonth()
  
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
    age--
  }
  
  return age
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

// Methods
const loadData = async () => {
  try {
    loading.value = true
    error.value = null
    
    // Load basic data in parallel
    const [playerData, seasonsData] = await Promise.all([
      getPlayerById(playerId),
      getSeasons()
    ])
    
    player.value = playerData
    seasons.value = seasonsData
    
    if (seasonsData.length > 0) {
      selectedSeason.value = seasonsData[0].id
    }
    
    // Load sessions and metrics
    await Promise.all([
      loadSessions(),
      loadMetrics()
    ])
    
  } catch (err) {
    console.error('Error loading player analysis:', err)
    error.value = 'Failed to load player analysis data. Please try again.'
  } finally {
    loading.value = false
  }
}

const loadSessions = async () => {
  try {
    sessionsLoading.value = true
    
    const filters = {
      seasonId: selectedSeason.value,
      ...sessionFilters.value
    }
    
    // Remove empty filter values
    Object.keys(filters).forEach(key => {
      if (!filters[key]) delete filters[key]
    })
    
    const sessionsData = await getPlayerSessions(playerId, filters)
    sessions.value = sessionsData
    
  } catch (err) {
    console.error('Error loading sessions:', err)
    // Don't show error for sessions, just show empty state
  } finally {
    sessionsLoading.value = false
  }
}

const loadMetrics = async () => {
  try {
    metricsLoading.value = true
    
    const metricsData = await getPlayerSeasonMetricAverages(
      playerId, 
      selectedSeason.value, 
      selectedSessionType.value || null
    )
    metrics.value = metricsData
    
  } catch (err) {
    console.error('Error loading metrics:', err)
    // Don't show error for metrics, just show empty state
  } finally {
    metricsLoading.value = false
  }
}

const viewSession = (sessionId) => {
  router.push(`/scout/sessions/${sessionId}/edit`)
}

const goBack = () => {
  router.push(`/scout/player/${playerId}`)
}

// Lifecycle
onMounted(() => {
  loadData()
})
</script>

<style scoped>
.scout-page {
  min-height: 100vh;
  background-color: var(--color-surface);
}

.main-content {
  padding: var(--spacing-xl);
}

.container {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
  padding-bottom: var(--spacing-lg);
  border-bottom: 2px solid var(--color-border);
}

.header-content h1 {
  font-size: 2.5rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-xs);
}

.header-content p {
  color: var(--color-text-light);
  font-size: 1.25rem;
  font-weight: 500;
}

.loading {
  text-align: center;
  padding: var(--spacing-xl);
  color: var(--color-text-light);
  font-size: 1.125rem;
}

.error-message {
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: var(--border-radius);
  padding: var(--spacing-xl);
  margin: var(--spacing-xl) 0;
}

.error-content {
  text-align: center;
}

.error-content h3 {
  color: #dc2626;
  margin-bottom: var(--spacing-md);
}

.error-content p {
  color: #7f1d1d;
  margin-bottom: var(--spacing-lg);
}

.analysis-layout {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.info-section {
  width: 100%;
}

.content-layout {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: var(--spacing-xl);
  align-items: start;
}

.section-card {
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  padding: var(--spacing-xl);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
  flex-wrap: wrap;
  gap: var(--spacing-md);
}

.section-header h2 {
  color: var(--color-text);
  font-size: 1.5rem;
  margin: 0;
}

.filters {
  display: flex;
  gap: var(--spacing-sm);
  flex-wrap: wrap;
}

.filters select,
.filters input {
  padding: var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius);
  font-size: 0.875rem;
}

.player-info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.info-item label {
  font-weight: 600;
  color: var(--color-text-light);
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.info-item span {
  color: var(--color-text);
  font-weight: 500;
}

.table-loading,
.metrics-loading,
.no-data {
  text-align: center;
  padding: var(--spacing-xl);
  color: var(--color-text-light);
}

.table-container {
  overflow-x: auto;
}

.sessions-table {
  width: 100%;
  border-collapse: collapse;
}

.sessions-table th {
  background: var(--color-surface);
  color: var(--color-text);
  font-weight: 600;
  text-align: left;
  padding: var(--spacing-md);
  border-bottom: 2px solid var(--color-border);
}

.sessions-table td {
  padding: var(--spacing-md);
  border-bottom: 1px solid var(--color-border);
  color: var(--color-text);
}

.session-row:hover {
  background-color: var(--color-surface);
}

.type-badge,
.status-badge {
  display: inline-block;
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.type-training {
  background-color: #dbeafe;
  color: #1e40af;
}

.type-game {
  background-color: #dcfce7;
  color: #166534;
}

.type-playoff-game {
  background-color: #fef3c7;
  color: #92400e;
}

.status-pending {
  background-color: #fbbf24;
  color: #78350f;
}

.status-ongoing {
  background-color: #34d399;
  color: #064e3b;
}

.status-finished {
  background-color: #60a5fa;
  color: #1e3a8a;
}

.status-canceled {
  background-color: #f87171;
  color: #7f1d1d;
}

.metrics-grid {
  display: grid;
  gap: var(--spacing-md);
  max-height: 600px;
  overflow-y: auto;
}

.metric-card {
  background: var(--color-surface);
  border-radius: var(--border-radius);
  padding: var(--spacing-md);
  border-left: 4px solid var(--color-primary);
}

.metric-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-sm);
}

.metric-header h3 {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--color-text);
  margin: 0;
}

.metric-weight {
  font-size: 0.75rem;
  color: var(--color-text-light);
  background: white;
  padding: var(--spacing-xs);
  border-radius: var(--border-radius);
}

.metric-value {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.metric-value .value {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-primary);
}

.metric-value .sessions {
  font-size: 0.75rem;
  color: var(--color-text-light);
}

.btn {
  padding: var(--spacing-sm) var(--spacing-md);
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.btn-sm {
  padding: var(--spacing-xs) var(--spacing-sm);
  font-size: 0.75rem;
}

.btn-primary {
  background-color: var(--color-primary);
  color: white;
}

.btn-primary:hover {
  background-color: var(--color-primary-dark);
}

.btn-secondary {
  background-color: var(--color-secondary);
  color: white;
}

.btn-secondary:hover {
  background-color: var(--color-secondary-dark);
}

.btn-view {
  background-color: #10b981;
  color: white;
}

.btn-view:hover {
  background-color: #059669;
}

.btn-view:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

@media (max-width: 1024px) {
  .content-layout {
    grid-template-columns: 1fr;
  }
  
  .player-info-grid {
    grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  }
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: var(--spacing-md);
    text-align: center;
  }
  
  .section-header {
    flex-direction: column;
    align-items: stretch;
  }
  
  .filters {
    flex-direction: column;
  }
  
  .player-info-grid {
    grid-template-columns: 1fr;
  }
  
  .container {
    max-width: 100%;
    padding: 0 var(--spacing-md);
  }
}

:root {
  --color-primary: #3b82f6;
  --color-primary-dark: #2563eb;
  --color-secondary: #6b7280;
  --color-secondary-dark: #4b5563;
  --color-surface: #f8fafc;
  --color-text: #1f2937;
  --color-text-light: #6b7280;
  --color-border: #e5e7eb;
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --border-radius: 0.375rem;
  --spacing-xs: 0.25rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 0.75rem;
  --spacing-lg: 1rem;
  --spacing-xl: 1.5rem;
}
</style>
