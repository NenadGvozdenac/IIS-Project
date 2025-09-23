<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Player Profile</h1>
          <p>{{ player?.firstName }} {{ player?.lastName }}</p>
        </div>
        
        <div v-if="loading" class="loading">
          Loading player information...
        </div>
        
        <div v-else-if="error" class="error">
          {{ error }}
        </div>
        
        <div v-else-if="player" class="player-info">
          <div class="info-columns">
            <!-- Left Column - Basic Info -->
            <div class="info-column">
              <h3>Basic Information</h3>
              <div class="info-group">
                <label>Full Name</label>
                <span>{{ player.firstName }} {{ player.lastName }}</span>
              </div>
              <div class="info-group">
                <label>Date of Birth</label>
                <span>{{ formatDate(player.dateOfBirth) }}</span>
              </div>
              <div class="info-group">
                <label>Age</label>
                <span>{{ calculateAge(player.dateOfBirth) }} years</span>
              </div>
              <div class="info-group">
                <label>Position</label>
                <span>{{ player.position }}</span>
              </div>
              <div class="info-group">
                <label>Nationality</label>
                <span>{{ player.nationality }}</span>
              </div>
              <div class="info-group">
                <label>Height</label>
                <span>{{ player.height }} cm</span>
              </div>
              <div class="info-group">
                <label>Weight</label>
                <span>{{ player.weight }} kg</span>
              </div>
            </div>

            <!-- Right Column - Physical Metrics -->
            <div class="info-column">
              <h3>Physical Metrics</h3>
              <div v-if="mostRecentMetrics" class="metrics-entry">
                <div class="metrics-date">
                  <small>Last updated: {{ formatDate(mostRecentMetrics.dateOfMeasurement) }}</small>
                </div>
                <div class="info-group" v-if="mostRecentMetrics.wingspan && mostRecentMetrics.wingspan > 0">
                  <label>Wingspan</label>
                  <span>{{ mostRecentMetrics.wingspan }} cm</span>
                </div>
                <div class="info-group" v-if="mostRecentMetrics.verticalJump && mostRecentMetrics.verticalJump > 0">
                  <label>Vertical Jump</label>
                  <span>{{ mostRecentMetrics.verticalJump }} cm</span>
                </div>
                <div class="info-group" v-if="mostRecentMetrics.sprintSpeed && mostRecentMetrics.sprintSpeed > 0">
                  <label>Sprint Speed</label>
                  <span>{{ mostRecentMetrics.sprintSpeed }} s</span>
                </div>
                <div class="info-group" v-if="mostRecentMetrics.fatPercentage && mostRecentMetrics.fatPercentage > 0">
                  <label>Body Fat</label>
                  <span>{{ mostRecentMetrics.fatPercentage }}%</span>
                </div>
                <div class="info-group" v-if="mostRecentMetrics.benchPressWeight && mostRecentMetrics.benchPressWeight > 0">
                  <label>Bench Press</label>
                  <span>{{ mostRecentMetrics.benchPressWeight }} kg</span>
                </div>
                <div class="info-group" v-if="mostRecentMetrics.squatWeight && mostRecentMetrics.squatWeight > 0">
                  <label>Squat Weight</label>
                  <span>{{ mostRecentMetrics.squatWeight }} kg</span>
                </div>
                <!-- Show message if no meaningful metrics are available -->
                <div v-if="!hasValidMetrics(mostRecentMetrics)" class="no-valid-metrics">
                  <p>No detailed physical measurements recorded yet.</p>
                  <p><small>Only basic measurements (height/weight) are available.</small></p>
                </div>
              </div>
              <div v-else class="no-metrics">
                <p>No physical metrics recorded yet.</p>
                <p><small>Basic measurements (height/weight) are shown in the left column.</small></p>
              </div>
            </div>
          </div>
          
          <div class="actions">
            <button @click="editPlayer" class="btn btn-primary">
              Edit Player Profile
            </button>
            <button @click="goBack" class="btn btn-secondary">
              Back to Dashboard
            </button>
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
import { getPhysicalMetricsByPlayerId } from '../../services/physical_metrics_service.js'

const route = useRoute()
const router = useRouter()

const player = ref(null)
const physicalMetrics = ref([])
const loading = ref(true)
const error = ref(null)

const playerId = route.params.id

// Computed property to get the most recent metrics
const mostRecentMetrics = computed(() => {
  console.log('Computing most recent metrics from:', physicalMetrics.value)
  
  if (!physicalMetrics.value || physicalMetrics.value.length === 0) {
    console.log('No physical metrics available')
    return null
  }
  
  // Sort by date of measurement and get the most recent
  const sorted = [...physicalMetrics.value].sort((a, b) => {
    const dateA = new Date(a.dateOfMeasurement || a.createdAt)
    const dateB = new Date(b.dateOfMeasurement || b.createdAt)
    return dateB - dateA // Most recent first
  })
  
  console.log('Most recent metrics:', sorted[0])
  return sorted[0]
})

onMounted(async () => {
  await loadPlayerData()
})

const loadPlayerData = async () => {
  try {
    loading.value = true
    error.value = null
    
    // Load player basic info
    const playerData = await getPlayerById(playerId)
    player.value = playerData
    console.log('Loaded player data:', playerData)
    
    // Load physical metrics
    try {
      const metricsData = await getPhysicalMetricsByPlayerId(playerId)
      console.log('Raw metrics data received:', metricsData)
      physicalMetrics.value = Array.isArray(metricsData) ? metricsData : (metricsData ? [metricsData] : [])
      console.log('Processed physical metrics:', physicalMetrics.value)
    } catch (metricsError) {
      console.warn('No physical metrics found:', metricsError)
      physicalMetrics.value = []
    }
    
  } catch (err) {
    console.error('Error loading player data:', err)
    error.value = 'Failed to load player information. Please try again.'
  } finally {
    loading.value = false
  }
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

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

const hasValidMetrics = (metrics) => {
  if (!metrics) return false
  
  // Check if any meaningful metric (other than basic height/weight) exists and is > 0
  const validFields = ['wingspan', 'verticalJump', 'sprintSpeed', 'fatPercentage', 'benchPressWeight', 'squatWeight']
  return validFields.some(field => metrics[field] && metrics[field] > 0)
}

const editPlayer = () => {
  router.push(`/scout/player/${playerId}/edit`)
}

const goBack = () => {
  router.push('/scout')
}
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
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: var(--spacing-xl);
}

.page-header h1 {
  font-size: 2.5rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.page-header p {
  color: var(--color-text-light);
  font-size: 1.25rem;
  font-weight: 500;
}

.loading, .error {
  text-align: center;
  padding: var(--spacing-xl);
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
}

.error {
  color: #dc2626;
}

.player-info {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
}

.info-columns {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-xl);
  margin-bottom: var(--spacing-xl);
}

.info-column h3 {
  color: var(--color-text);
  margin-bottom: var(--spacing-lg);
  padding-bottom: var(--spacing-sm);
  border-bottom: 2px solid var(--color-border);
}

.info-group {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md) 0;
  border-bottom: 1px solid var(--color-border);
}

.info-group:last-child {
  border-bottom: none;
}

.info-group label {
  font-weight: 500;
  color: var(--color-text);
}

.info-group span {
  color: var(--color-text-light);
  font-weight: 400;
}

.metrics-entry {
  background: var(--color-surface);
  padding: var(--spacing-md);
  border-radius: var(--border-radius);
  margin-bottom: var(--spacing-md);
}

.metrics-entry:last-child {
  margin-bottom: 0;
}

.metrics-date {
  margin-bottom: var(--spacing-sm);
}

.metrics-date small {
  color: var(--color-text-light);
  font-style: italic;
}

.no-metrics, .no-valid-metrics {
  text-align: center;
  padding: var(--spacing-xl);
  color: var(--color-text-light);
}

.no-valid-metrics {
  background: #f9fafb;
  border-radius: var(--border-radius);
  border: 1px dashed var(--color-border);
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: var(--spacing-md);
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

.btn {
  padding: var(--spacing-md) var(--spacing-xl);
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
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

.btn-small {
  padding: var(--spacing-sm) var(--spacing-md);
  font-size: 0.875rem;
  margin-top: var(--spacing-md);
}

@media (max-width: 768px) {
  .info-columns {
    grid-template-columns: 1fr;
    gap: var(--spacing-lg);
  }
  
  .container {
    max-width: 100%;
    padding: 0 var(--spacing-md);
  }
  
  .actions {
    flex-direction: column;
  }
  
  .info-group {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--spacing-xs);
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
