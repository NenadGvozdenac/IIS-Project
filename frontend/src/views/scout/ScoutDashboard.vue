<template>
  <div class="scout-dashboard">
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Player Profiles</h1>
          <p>Manage and analyze player profiles</p>
        </div>

        <!-- Filters Section -->
        <div class="filters-section">
          <div class="filters-row">
            <div class="filter-group">
              <label for="nameFilter">Name</label>
              <input 
                id="nameFilter"
                v-model="filters.name" 
                type="text" 
                placeholder="Search by name..."
                class="filter-input"
              />
            </div>
            <div class="filter-group">
              <label for="positionFilter">Position</label>
              <select id="positionFilter" v-model="filters.position" class="filter-select">
                <option value="">All Positions</option>
                <option value="Point Guard">Point Guard</option>
                <option value="Shooting Guard">Shooting Guard</option>
                <option value="Small Forward">Small Forward</option>
                <option value="Power Forward">Power Forward</option>
                <option value="Center">Center</option>
              </select>
            </div>
            <div class="filter-group">
              <label for="nationalityFilter">Nationality</label>
              <input 
                id="nationalityFilter"
                v-model="filters.nationality" 
                type="text" 
                placeholder="Search by nationality..."
                class="filter-input"
              />
            </div>
            <div class="action-buttons">
              <button @click="clearFilters" class="btn btn-secondary">Clear Filters</button>
              <button @click="generateScoutingReportPDF" class="btn btn-primary" :disabled="generatingReport">
                <svg v-if="generatingReport" class="btn-spinner" viewBox="0 0 24 24">
                  <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none" opacity="0.25"/>
                  <path d="M4,12a8,8 0 0,1 8,-8" stroke="currentColor" stroke-width="4" fill="none" stroke-linecap="round"/>
                </svg>
                <svg v-else class="btn-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                </svg>
                {{ generatingReport ? 'Generating...' : 'Generate PDF Report' }}
              </button>
            </div>
          </div>
        </div>

        <!-- Players Table -->
        <div class="table-container">
          <div v-if="loading" class="loading">Loading players...</div>
          <table v-else class="players-table">
            <thead>
              <tr>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Date of Birth</th>
                <th>Position</th>
                <th>Nationality</th>
                <th>Height (cm)</th>
                <th>Weight (kg)</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="player in filteredPlayers" :key="player.id">
                <td>{{ player.firstName }}</td>
                <td>{{ player.lastName }}</td>
                <td>{{ formatDate(player.dateOfBirth) }}</td>
                <td>{{ player.position }}</td>
                <td>{{ player.nationality }}</td>
                <td>{{ player.height }}</td>
                <td>{{ player.weight }}</td>
                <td>
                  <button @click="viewPlayer(player)" class="btn btn-sm btn-primary">View</button>
                  <button @click="editPlayer(player)" class="btn btn-sm btn-secondary">Edit</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getAllPlayers, generateScoutingReport } from '../../services/player_service.js'

const router = useRouter()

const players = ref([])
const loading = ref(true)
const generatingReport = ref(false)

// Sample data for demonstration - replace with API call
const samplePlayers = [
  {
    id: 1,
    firstName: 'LeBron',
    lastName: 'James',
    dateOfBirth: '1984-12-30',
    position: 'Small Forward',
    nationality: 'USA',
    height: 206,
    weight: 113
  },
  {
    id: 2,
    firstName: 'Stephen',
    lastName: 'Curry',
    dateOfBirth: '1988-03-14',
    position: 'Point Guard',
    nationality: 'USA',
    height: 191,
    weight: 84
  },
  {
    id: 3,
    firstName: 'Giannis',
    lastName: 'Antetokounmpo',
    dateOfBirth: '1994-12-06',
    position: 'Power Forward',
    nationality: 'Greece',
    height: 211,
    weight: 110
  }
]

const filters = ref({
  name: '',
  position: '',
  nationality: ''
})

onMounted(async () => {
  await loadPlayers()
})

const loadPlayers = async () => {
  try {
    loading.value = true
    // Try to load from API, fallback to sample data
    try {
      const apiPlayers = await getAllPlayers()
      players.value = Array.isArray(apiPlayers) ? apiPlayers : []
    } catch (error) {
      console.warn('Using sample data:', error)
      players.value = samplePlayers
    }
  } catch (error) {
    console.error('Error loading players:', error)
    players.value = samplePlayers
  } finally {
    loading.value = false
  }
}

const filteredPlayers = computed(() => {
  if (!Array.isArray(players.value)) {
    return []
  }
  
  return players.value.filter(player => {
    const nameMatch = !filters.value.name || 
      (player.firstName + ' ' + player.lastName).toLowerCase().includes(filters.value.name.toLowerCase())
    const positionMatch = !filters.value.position || player.position === filters.value.position
    const nationalityMatch = !filters.value.nationality || 
      (player.nationality && player.nationality.toLowerCase().includes(filters.value.nationality.toLowerCase()))
    
    return nameMatch && positionMatch && nationalityMatch
  })
})

const clearFilters = () => {
  filters.value = {
    name: '',
    position: '',
    nationality: ''
  }
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString()
}

const viewPlayer = (player) => {
  router.push(`/scout/player/${player.id}`)
}

const editPlayer = (player) => {
  router.push(`/scout/player/${player.id}/edit`)
}

// Generate PDF scouting report
const generateScoutingReportPDF = async () => {
  try {
    generatingReport.value = true
    
    // Call the scouting service through the player service
    const blob = await generateScoutingReport({
      seasonId: null, // NULL means all seasons - general summary
      position: filters.value.position || null,
      nationality: filters.value.nationality || null,
      playerName: filters.value.name || null
    })
    
    // Create a download link
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    
    // Generate filename with current date
    const now = new Date()
    const dateStr = now.toISOString().split('T')[0]
    link.download = `scouting-report-${dateStr}.pdf`
    
    // Trigger download
    document.body.appendChild(link)
    link.click()
    
    // Cleanup
    window.URL.revokeObjectURL(url)
    document.body.removeChild(link)
    
    // Show success message
    alert('Scouting report generated successfully!')
    
  } catch (error) {
    console.error('Error generating scouting report:', error)
    alert('Failed to generate scouting report. Please try again.')
  } finally {
    generatingReport.value = false
  }
}
</script>

<style scoped>
.scout-dashboard {
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
  color: var(--color-text-light);
  background: white;
  border-radius: var(--border-radius);
}

.filters-section {
  background: white;
  padding: var(--spacing-lg);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  margin-bottom: var(--spacing-xl);
}

.filters-row {
  display: flex;
  gap: var(--spacing-lg);
  align-items: end;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  flex-direction: column;
  min-width: 200px;
}

.filter-group label {
  font-weight: 500;
  margin-bottom: var(--spacing-xs);
  color: var(--color-text);
}

.filter-input,
.filter-select {
  padding: var(--spacing-sm);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius);
  font-size: 1rem;
}

.filter-input:focus,
.filter-select:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.1);
}

.action-buttons {
  display: flex;
  gap: var(--spacing-md);
  align-items: end;
  margin-left: auto;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-xs);
  padding: var(--spacing-sm) var(--spacing-md);
  border: none;
  border-radius: var(--border-radius);
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  text-decoration: none;
  white-space: nowrap;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-icon,
.btn-spinner {
  width: 16px;
  height: 16px;
  flex-shrink: 0;
}

.btn-spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.btn-primary {
  background-color: var(--color-primary) !important;
  color: white !important;
  border: 1px solid var(--color-primary) !important;
}

.btn-primary:hover:not(:disabled) {
  background-color: var(--color-primary-dark) !important;
  border-color: var(--color-primary-dark) !important;
  color: white !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(59, 130, 246, 0.3);
}

.btn-secondary {
  background-color: var(--color-secondary) !important;
  color: white !important;
  border: 1px solid var(--color-secondary) !important;
}

.btn-secondary:hover:not(:disabled) {
  background-color: var(--color-secondary-dark) !important;
  border-color: var(--color-secondary-dark) !important;
  color: white !important;
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(107, 114, 128, 0.3);
}

.table-container {
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  overflow: hidden;
  max-height: calc(100vh - 300px);
  overflow-y: auto;
}

.players-table {
  width: 100%;
  border-collapse: collapse;
}

.players-table th {
  background-color: var(--color-primary);
  color: white;
  padding: var(--spacing-md);
  text-align: left;
  font-weight: 600;
  position: sticky;
  top: 0;
  z-index: 10;
}

.players-table td {
  padding: var(--spacing-md);
  border-bottom: 1px solid var(--color-border);
}

.players-table tbody tr:hover {
  background-color: var(--color-surface);
}

.btn-sm {
  padding: var(--spacing-xs);
  font-size: 0.75rem;
  margin-right: var(--spacing-xs);
}

/* CSS Variables */
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
  --spacing-2xl: 2rem;
}
</style>
