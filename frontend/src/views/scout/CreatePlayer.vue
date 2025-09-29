<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Create Player Profile</h1>
          <p>Add a new player to the database</p>
        </div>
        <div v-if="loading" class="loading">
          Loading form data...
        </div>
        <div v-else class="form-container">
          <form @submit.prevent="createPlayer" class="player-form">
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
                      {{ nationality.name }}
                    </option>
                  </select>
                </div>
              </div>

              <!-- Right Column - Physical Metrics -->
              <div class="form-column">
                <h3>Physical Metrics</h3>
                <div class="form-group">
                  <label for="height">Height (cm)</label>
                  <input id="height" v-model="physicalMetrics.height" type="number" min="150" max="250" required />
                </div>
                <div class="form-group">
                  <label for="weight">Weight (kg)</label>
                  <input id="weight" v-model="physicalMetrics.weight" type="number" min="50" max="200" required />
                </div>
                <div class="form-group">
                  <label for="wingspan">Wingspan (cm)</label>
                  <input id="wingspan" v-model="physicalMetrics.wingspan" type="number" min="150" max="300" />
                </div>
                <div class="form-group">
                  <label for="verticalJump">Vertical Jump (cm)</label>
                  <input id="verticalJump" v-model="physicalMetrics.verticalJump" type="number" min="30" max="150" />
                </div>
                <div class="form-group">
                  <label for="speed">Sprint Speed (m/s)</label>
                  <input id="speed" v-model="physicalMetrics.speed" type="number" step="0.1" min="5" max="15" />
                </div>
                <div class="form-group">
                  <label for="fatPercentage">Body Fat (%)</label>
                  <input id="fatPercentage" v-model="physicalMetrics.fatPercentage" type="number" step="0.1" min="5" max="30" />
                </div>
                <div class="form-group">
                  <label for="benchPressWeight">Bench Press (kg)</label>
                  <input id="benchPressWeight" v-model="physicalMetrics.benchPressWeight" type="number" min="20" max="300" />
                </div>
                <div class="form-group">
                  <label for="squatWeight">Squat Weight (kg)</label>
                  <input id="squatWeight" v-model="physicalMetrics.squatWeight" type="number" min="20" max="400" />
                </div>
              </div>
            </div>
            
            <div class="form-actions">
              <button type="button" @click="resetForm" class="btn btn-secondary">Reset</button>
              <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
                {{ isSubmitting ? 'Creating...' : 'Create Player' }}
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
import { useRouter } from 'vue-router'
import { createPlayer as createPlayerAPI } from '../../services/player_service.js'
import { createPhysicalMetrics } from '../../services/physical_metrics_service.js'
import { getAllPositions } from '../../services/position_service.js'
import { getAllNationalities } from '../../services/nationality_service.js'

const router = useRouter()
const isSubmitting = ref(false)

const player = ref({
  firstName: '',
  lastName: '',
  dateOfBirth: '',
  idPosition: '',
  idNationality: ''
})

const physicalMetrics = ref({
  height: null,
  weight: null,
  wingspan: null,
  verticalJump: null,
  speed: null,
  fatPercentage: null,
  benchPressWeight: null,
  squatWeight: null
})

const positions = ref([])
const nationalities = ref([])
const loading = ref(true)

onMounted(async () => {
  await loadDropdownData()
})

const loadDropdownData = async () => {
  try {
    loading.value = true
    
    // Load positions and nationalities for dropdowns
    const [positionsData, nationalitiesData] = await Promise.all([
      getAllPositions(),
      getAllNationalities()
    ])
    
    positions.value = positionsData
    nationalities.value = nationalitiesData
  } catch (error) {
    console.error('Error loading dropdown data:', error)
    alert('Failed to load form data. Please refresh the page.')
  } finally {
    loading.value = false
  }
}

const createPlayer = async () => {
  isSubmitting.value = true
  try {
    // First create the player with basic info and physical metrics for height/weight
    const playerData = {
      firstName: player.value.firstName,
      lastName: player.value.lastName,
      dateOfBirth: player.value.dateOfBirth,
      height: physicalMetrics.value.height,
      weight: physicalMetrics.value.weight,
      idNationality: parseInt(player.value.idNationality),
      idPosition: parseInt(player.value.idPosition)
    }
    
    const createdPlayer = await createPlayerAPI(playerData)
    
    // Always create physical metrics with ALL required fields
    // Since the database requires all fields to be NOT NULL, we always create the record
    const metricsData = {
      // Basic measurements (always required from the player form)
      weight: physicalMetrics.value.weight ? parseInt(physicalMetrics.value.weight) : 70,
      height: physicalMetrics.value.height ? parseInt(physicalMetrics.value.height) : 180,
      
      // Additional physical metrics (use form values or sensible defaults)
      wingspan: physicalMetrics.value.wingspan ? parseInt(physicalMetrics.value.wingspan) : (physicalMetrics.value.height ? parseInt(physicalMetrics.value.height) : 180),
      verticalJump: physicalMetrics.value.verticalJump ? parseInt(physicalMetrics.value.verticalJump) : 0,
      sprintSpeed: physicalMetrics.value.speed ? parseFloat(physicalMetrics.value.speed) : 10.0, // Convert to decimal
      fatPercentage: physicalMetrics.value.fatPercentage ? parseInt(physicalMetrics.value.fatPercentage) : 10,
      benchPressWeight: physicalMetrics.value.benchPressWeight ? parseInt(physicalMetrics.value.benchPressWeight) : 0,
      squatWeight: physicalMetrics.value.squatWeight ? parseInt(physicalMetrics.value.squatWeight) : 0
    }
    
    console.log('Creating physical metrics with data:', metricsData);
    await createPhysicalMetrics(createdPlayer.idPlayer, metricsData)
    
    alert('Player profile created successfully!')
    router.push('/scout')
  } catch (error) {
    console.error('Error creating player:', error)
    alert('Failed to create player profile. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = () => {
  player.value = {
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    idPosition: '',
    idNationality: ''
  }
  
  physicalMetrics.value = {
    height: null,
    weight: null,
    wingspan: null,
    verticalJump: null,
    speed: null,
    fatPercentage: null,
    benchPressWeight: null,
    squatWeight: null
  }
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

.loading {
  text-align: center;
  padding: var(--spacing-xl);
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  color: var(--color-text-light);
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
  --spacing-sm: 0.5rem;
  --spacing-md: 0.75rem;
  --spacing-lg: 1rem;
  --spacing-xl: 1.5rem;
}
</style>
