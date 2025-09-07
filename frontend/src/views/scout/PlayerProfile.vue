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
              <div v-if="physicalMetrics.length > 0">
                <div v-for="metric in physicalMetrics" :key="metric.id" class="metrics-entry">
                  <div class="metrics-date">
                    <small>Recorded: {{ formatDate(metric.createdAt) }}</small>
                  </div>
                  <div class="info-group" v-if="metric.wingspan">
                    <label>Wingspan</label>
                    <span>{{ metric.wingspan }} cm</span>
                  </div>
                  <div class="info-group" v-if="metric.verticalJump">
                    <label>Vertical Jump</label>
                    <span>{{ metric.verticalJump }} cm</span>
                  </div>
                  <div class="info-group" v-if="metric.speed">
                    <label>Speed</label>
                    <span>{{ metric.speed }} m/s</span>
                  </div>
                  <div class="info-group" v-if="metric.agility">
                    <label>Agility Score</label>
                    <span>{{ metric.agility }}/10</span>
                  </div>
                </div>
              </div>
              <div v-else class="no-metrics">
                <p>No physical metrics recorded yet.</p>
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
import { ref, onMounted } from 'vue'
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
    
    // Load physical metrics
    try {
      const metricsData = await getPhysicalMetricsByPlayerId(playerId)
      physicalMetrics.value = Array.isArray(metricsData) ? metricsData : [metricsData]
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

.no-metrics {
  text-align: center;
  padding: var(--spacing-xl);
  color: var(--color-text-light);
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
