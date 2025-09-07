<template>
  <div class="scout-dashboard">
    <ScoutNavbar />
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
            <button @click="clearFilters" class="btn btn-secondary">Clear Filters</button>
          </div>
        </div>

        <!-- Players Table -->
        <div class="table-container">
          <table class="players-table">
            <thead>
              <tr>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Date of Birth</th>
                <th>Position</th>
                <th>Nationality</th>
                <th>Height (cm)</th>
                <th>Wingspan (cm)</th>
                <th>Vertical Jump (cm)</th>
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
                <td>{{ player.wingspan }}</td>
                <td>{{ player.verticalJump }}</td>
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
import ScoutNavbar from '../../components/scout/ScoutNavbar.vue'

const router = useRouter()

const players = ref([
  {
    id: 1,
    firstName: 'LeBron',
    lastName: 'James',
    dateOfBirth: '1984-12-30',
    position: 'Small Forward',
    nationality: 'USA',
    height: 206,
    wingspan: 214,
    verticalJump: 97
  },
  {
    id: 2,
    firstName: 'Stephen',
    lastName: 'Curry',
    dateOfBirth: '1988-03-14',
    position: 'Point Guard',
    nationality: 'USA',
    height: 191,
    wingspan: 193,
    verticalJump: 91
  },
  {
    id: 3,
    firstName: 'Giannis',
    lastName: 'Antetokounmpo',
    dateOfBirth: '1994-12-06',
    position: 'Power Forward',
    nationality: 'Greece',
    height: 211,
    wingspan: 224,
    verticalJump: 97
  },
  {
    id: 4,
    firstName: 'Kevin',
    lastName: 'Durant',
    dateOfBirth: '1988-09-29',
    position: 'Small Forward',
    nationality: 'USA',
    height: 208,
    wingspan: 228,
    verticalJump: 86
  },
  {
    id: 5,
    firstName: 'Luka',
    lastName: 'Dončić',
    dateOfBirth: '1999-02-28',
    position: 'Point Guard',
    nationality: 'Slovenia',
    height: 201,
    wingspan: 218,
    verticalJump: 71
  },
  {
    id: 6,
    firstName: 'Joel',
    lastName: 'Embiid',
    dateOfBirth: '1994-03-16',
    position: 'Center',
    nationality: 'Cameroon',
    height: 213,
    wingspan: 226,
    verticalJump: 81
  },
  {
    id: 7,
    firstName: 'Jayson',
    lastName: 'Tatum',
    dateOfBirth: '1998-03-03',
    position: 'Small Forward',
    nationality: 'USA',
    height: 203,
    wingspan: 209,
    verticalJump: 94
  },
  {
    id: 8,
    firstName: 'Nikola',
    lastName: 'Jokić',
    dateOfBirth: '1995-02-19',
    position: 'Center',
    nationality: 'Serbia',
    height: 211,
    wingspan: 215,
    verticalJump: 61
  },
  {
    id: 9,
    firstName: 'Damian',
    lastName: 'Lillard',
    dateOfBirth: '1990-07-15',
    position: 'Point Guard',
    nationality: 'USA',
    height: 188,
    wingspan: 201,
    verticalJump: 94
  },
  {
    id: 10,
    firstName: 'Anthony',
    lastName: 'Davis',
    dateOfBirth: '1993-03-11',
    position: 'Power Forward',
    nationality: 'USA',
    height: 208,
    wingspan: 228,
    verticalJump: 89
  }
])

const filters = ref({
  name: '',
  position: '',
  nationality: ''
})

const filteredPlayers = computed(() => {
  return players.value.filter(player => {
    const nameMatch = !filters.value.name || 
      `${player.firstName} ${player.lastName}`.toLowerCase().includes(filters.value.name.toLowerCase())
    
    const positionMatch = !filters.value.position || 
      player.position === filters.value.position
    
    const nationalityMatch = !filters.value.nationality || 
      player.nationality.toLowerCase().includes(filters.value.nationality.toLowerCase())
    
    return nameMatch && positionMatch && nationalityMatch
  })
})

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString()
}

const clearFilters = () => {
  filters.value = {
    name: '',
    position: '',
    nationality: ''
  }
}

const viewPlayer = (player) => {
  router.push(`/scout/player/${player.id}`)
}

const editPlayer = (player) => {
  router.push(`/scout/player/${player.id}/edit`)
}

onMounted(() => {
  // Any initialization logic
})
</script>

<style scoped>
.scout-dashboard {
  min-height: 100vh;
  background-color: var(--color-surface);
}

.main-content {
  margin-left: 0;
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

.btn {
  padding: var(--spacing-xs) var(--spacing-sm);
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-block;
  text-align: center;
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
