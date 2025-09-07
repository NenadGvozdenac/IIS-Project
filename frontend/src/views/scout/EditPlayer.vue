<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Edit Player Profile</h1>
          <p>Update player information and metrics</p>
        </div>
        
        <div v-if="loading" class="loading">
          Loading player information...
        </div>
        
        <div v-else-if="error" class="error">
          {{ error }}
        </div>
        
        <div v-else class="form-container">
          <form @submit.prevent="updatePlayer" class="player-form">
            <div class="form-columns">
              <!-- Left Column - Basic Info -->
              <div class="form-column">
                <h3>Basic Information</h3>
                <div class="form-group">
                  <label for="firstName">First Name</label>
                  <input id="firstName" v-model="player.firstName" type="text" required />
                </div>
                <div class="form-group">
                  <label for="lastName">Last Name</label>
                  <input id="lastName" v-model="player.lastName" type="text" required />
                </div>
                <div class="form-group">
                  <label for="dateOfBirth">Date of Birth</label>
                  <input id="dateOfBirth" v-model="player.dateOfBirth" type="date" required />
                </div>
                <div class="form-group">
                  <label for="position">Position</label>
                  <select id="position" v-model="player.idPosition" required>
                    <option value="">Select Position</option>
                    <option v-for="position in positions" :key="position.id" :value="position.id">
                      {{ position.name }}
                    </option>
                  </select>
                </div>
                <div class="form-group">
                  <label for="nationality">Nationality</label>
                  <select id="nationality" v-model="player.idNationality" required>
                    <option value="">Select Nationality</option>
                    <option v-for="nationality in nationalities" :key="nationality.id" :value="nationality.id">
                      {{ nationality.state }}
                    </option>
                  </select>
                </div>
                <div class="form-group">
                  <label for="height">Height (cm)</label>
                  <input id="height" v-model="player.height" type="number" min="150" max="250" required />
                </div>
                <div class="form-group">
                  <label for="weight">Weight (kg)</label>
                  <input id="weight" v-model="player.weight" type="number" min="50" max="200" required />
                </div>
              </div>

              <!-- Right Column - New Physical Metrics -->
              <div class="form-column">
                <h3>Add New Physical Metrics</h3>
                <div class="form-group">
                  <label for="wingspan">Wingspan (cm)</label>
                  <input id="wingspan" v-model="newMetrics.wingspan" type="number" min="150" max="300" />
                </div>
                <div class="form-group">
                  <label for="verticalJump">Vertical Jump (cm)</label>
                  <input id="verticalJump" v-model="newMetrics.verticalJump" type="number" min="30" max="150" />
                </div>
                <div class="form-group">
                  <label for="speed">Sprint Speed (seconds)</label>
                  <input id="speed" v-model="newMetrics.speed" type="number" step="0.1" min="5" max="15" />
                </div>
                <div class="form-group">
                  <label for="fatPercentage">Body Fat Percentage (%)</label>
                  <input id="fatPercentage" v-model="newMetrics.fatPercentage" type="number" step="0.1" min="5" max="30" />
                </div>
                <div class="form-group">
                  <label for="benchPressWeight">Bench Press Weight (kg)</label>
                  <input id="benchPressWeight" v-model="newMetrics.benchPressWeight" type="number" min="20" max="300" />
                </div>
                <div class="form-group">
                  <label for="squatWeight">Squat Weight (kg)</label>
                  <input id="squatWeight" v-model="newMetrics.squatWeight" type="number" min="20" max="400" />
                </div>
                
                <!-- Existing Physical Metrics -->
                <div v-if="existingMetrics.length > 0" class="existing-metrics">
                  <h4>Previous Metrics</h4>
                  <div v-for="metric in existingMetrics" :key="metric.id" class="metric-entry">
                    <div class="metric-date">
                      <small>{{ formatDate(metric.dateOfMeasurement) }}</small>
                    </div>
                    <div class="metric-values">
                      <span v-if="metric.wingspan">Wingspan: {{ metric.wingspan }}cm</span>
                      <span v-if="metric.verticalJump">Jump: {{ metric.verticalJump }}cm</span>
                      <span v-if="metric.sprintSpeed">Speed: {{ metric.sprintSpeed }}s</span>
                      <span v-if="metric.fatPercentage">Body Fat: {{ metric.fatPercentage }}%</span>
                      <span v-if="metric.benchPressWeight">Bench: {{ metric.benchPressWeight }}kg</span>
                      <span v-if="metric.squatWeight">Squat: {{ metric.squatWeight }}kg</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            
            <div class="form-actions">
              <button type="button" @click="goBack" class="btn btn-secondary">Cancel</button>
              <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
                {{ isSubmitting ? 'Updating...' : 'Update Player' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getPlayerById, updatePlayer as updatePlayerAPI } from '../../services/player_service.js'
import { getPhysicalMetricsByPlayerId, createPhysicalMetrics } from '../../services/physical_metrics_service.js'
import { getAllPositions } from '../../services/position_service.js'
import { getAllNationalities } from '../../services/nationality_service.js'

const route = useRoute()
const router = useRouter()

const player = ref({
  firstName: '',
  lastName: '',
  dateOfBirth: '',
  idPosition: '',
  idNationality: '',
  height: null,
  weight: null
})

const newMetrics = ref({
  wingspan: null,
  verticalJump: null,
  speed: null,
  fatPercentage: null,
  benchPressWeight: null,
  squatWeight: null
})

const positions = ref([])
const nationalities = ref([])
const existingMetrics = ref([])
const loading = ref(true)
const error = ref(null)
const isSubmitting = ref(false)

const playerId = route.params.id

onMounted(async () => {
  await loadPlayerData()
})

const loadPlayerData = async () => {
  try {
    loading.value = true
    error.value = null
    
    // Load dropdown data
    const [positionsData, nationalitiesData] = await Promise.all([
      getAllPositions(),
      getAllNationalities()
    ])
    
    positions.value = positionsData
    nationalities.value = nationalitiesData
    
    // Load player basic info
    const playerData = await getPlayerById(playerId)
    player.value = {
      firstName: playerData.firstName || playerData.name, // Handle both field names
      lastName: playerData.lastName || playerData.surname, // Handle both field names
      dateOfBirth: playerData.dateOfBirth ? playerData.dateOfBirth.split('T')[0] : '',
      idPosition: playerData.idPosition,
      idNationality: playerData.idNationality,
      height: playerData.height,
      weight: playerData.weight
    }
    
    // Load existing physical metrics
    try {
      const metricsData = await getPhysicalMetricsByPlayerId(playerId)
      existingMetrics.value = Array.isArray(metricsData) ? metricsData : [metricsData]
    } catch (metricsError) {
      console.warn('No physical metrics found:', metricsError)
      existingMetrics.value = []
    }
    
  } catch (err) {
    console.error('Error loading player data:', err)
    error.value = 'Failed to load player information. Please try again.'
  } finally {
    loading.value = false
  }
}

const updatePlayer = async () => {
  isSubmitting.value = true
  try {
    // Prepare player data with correct field mapping for backend
    const playerUpdateData = {
      name: player.value.firstName,
      surname: player.value.lastName,
      birthday: player.value.dateOfBirth,
      idPosition: parseInt(player.value.idPosition),
      idNationality: parseInt(player.value.idNationality),
      height: parseInt(player.value.height),
      weight: parseInt(player.value.weight)
    }
    
    // Update player basic info
    await updatePlayerAPI(playerId, playerUpdateData)
    
    // Add new physical metrics if any values are provided
    const hasNewMetrics = Object.values(newMetrics.value).some(value => value !== null && value !== '')
    
    if (hasNewMetrics) {
      // Prepare metrics data with all required fields for the backend
      const metricsToAdd = {
        wingspan: newMetrics.value.wingspan ? parseInt(newMetrics.value.wingspan) : null,
        verticalJump: newMetrics.value.verticalJump ? parseInt(newMetrics.value.verticalJump) : null,
        sprintSpeed: newMetrics.value.speed ? parseFloat(newMetrics.value.speed) : null, // Map speed to sprintSpeed
        fatPercentage: newMetrics.value.fatPercentage ? parseInt(newMetrics.value.fatPercentage) : null,
        benchPressWeight: newMetrics.value.benchPressWeight ? parseInt(newMetrics.value.benchPressWeight) : null,
        squatWeight: newMetrics.value.squatWeight ? parseInt(newMetrics.value.squatWeight) : null,
        // Include basic measurements from player data
        weight: player.value.weight ? parseInt(player.value.weight) : null,
        height: player.value.height ? parseInt(player.value.height) : null
      }
      
      console.log('Adding new physical metrics:', metricsToAdd);
      await createPhysicalMetrics(playerId, metricsToAdd)
    }
    
    alert('Player profile updated successfully!')
    router.push(`/scout/player/${playerId}`)
  } catch (error) {
    console.error('Error updating player:', error)
    alert('Failed to update player profile. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

const goBack = () => {
  router.push(`/scout/player/${playerId}`)
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
  font-size: 1.125rem;
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

.form-container {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
}

.form-columns {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-xl);
  margin-bottom: var(--spacing-xl);
}

.form-column h3 {
  color: var(--color-text);
  margin-bottom: var(--spacing-lg);
  padding-bottom: var(--spacing-sm);
  border-bottom: 2px solid var(--color-border);
}

.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: var(--spacing-lg);
}

.form-group label {
  font-weight: 500;
  margin-bottom: var(--spacing-sm);
  color: var(--color-text);
}

.form-group input,
.form-group select {
  padding: var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius);
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.1);
}

.existing-metrics {
  margin-top: var(--spacing-xl);
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

.existing-metrics h4 {
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
  font-size: 1rem;
}

.metric-entry {
  background: var(--color-surface);
  padding: var(--spacing-sm);
  border-radius: var(--border-radius);
  margin-bottom: var(--spacing-sm);
}

.metric-date {
  margin-bottom: var(--spacing-xs);
}

.metric-date small {
  color: var(--color-text-light);
  font-style: italic;
  font-size: 0.75rem;
}

.metric-values {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.metric-values span {
  background: white;
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius);
  font-size: 0.875rem;
  color: var(--color-text-light);
  border: 1px solid var(--color-border);
}

.form-actions {
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
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background-color: var(--color-primary);
  color: white;
}

.btn-primary:hover:not(:disabled) {
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
  .form-columns {
    grid-template-columns: 1fr;
    gap: var(--spacing-lg);
  }
  
  .container {
    max-width: 100%;
    padding: 0 var(--spacing-md);
  }
  
  .form-actions {
    flex-direction: column;
  }
  
  .metric-values {
    flex-direction: column;
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
