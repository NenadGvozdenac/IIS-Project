<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <!-- Page Header -->
        <div class="page-header">
          <h1>Sessions</h1>
          <p>Scouting sessions and reports</p>
          <button 
            class="btn btn-primary create-session-btn"
            @click="openCreateSessionModal"
          >
            Create Session
          </button>
        </div>

        <!-- Sessions Table -->
        <div class="content-card">
          <!-- Debug Info -->
          <div v-if="sessions.length === 0" class="debug-info">
            <p>Sessions: {{ sessions.length }}</p>
            <p>Session Types: {{ sessionTypes.length }}</p>
            <p>Session Statuses: {{ sessionStatuses.length }}</p>
          </div>

          <!-- Filters -->
          <div class="filters-section">
            <div class="filter-group">
              <label>Type:</label>
              <select v-model="filters.type" @change="applyFilters">
                <option value="">All Types</option>
                <option 
                  v-for="type in sessionTypes" 
                  :key="type.id" 
                  :value="type.id"
                >
                  {{ type.name }}
                </option>
              </select>
            </div>

            <div class="filter-group">
              <label>Status:</label>
              <select v-model="filters.status" @change="applyFilters">
                <option value="">All Statuses</option>
                <option 
                  v-for="status in sessionStatuses" 
                  :key="status.id" 
                  :value="status.id"
                >
                  {{ status.name }}
                </option>
              </select>
            </div>

            <div class="filter-group">
              <label>Player Name:</label>
              <input 
                type="text" 
                v-model="filters.playerName" 
                @input="applyFilters"
                placeholder="Search by player name..."
              />
            </div>

            <div class="filter-group">
              <label>Start Date:</label>
              <input 
                type="date" 
                v-model="filters.startDate" 
                @change="applyFilters"
              />
            </div>

            <div class="filter-group">
              <label>End Date:</label>
              <input 
                type="date" 
                v-model="filters.endDate" 
                @change="applyFilters"
              />
            </div>

            <button class="btn btn-secondary" @click="clearFilters">
              Clear Filters
            </button>
          </div>

          <!-- Sessions Table -->
          <div class="table-container">
            <table class="sessions-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Player</th>
                  <th>Type</th>
                  <th>Status</th>
                  <th>Start Date</th>
                  <th>End Date</th>
                  <th>Scout</th>
                  <th>Note</th>
                </tr>
              </thead>
              <tbody>
                <tr 
                  v-for="session in filteredSessions" 
                  :key="session.idSession"
                  @click="editSession(session)"
                  class="clickable-row"
                >
                  <td>{{ session.idSession }}</td>
                  <td>{{ session.playerName }}</td>
                  <td>{{ session.sessionTypeName }}</td>
                  <td>
                    <span 
                      class="status-badge" 
                      :class="getStatusClass(session.sessionStatusName)"
                    >
                      {{ session.sessionStatusName }}
                    </span>
                  </td>
                  <td>{{ formatDate(session.startTime) }}</td>
                  <td>{{ formatDate(session.endTime) }}</td>
                  <td>{{ session.userName }}</td>
                  <td class="note-cell">{{ session.note }}</td>
                </tr>
              </tbody>
            </table>
            
            <div v-if="filteredSessions.length === 0" class="no-data">
              No sessions found
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Session Modal -->
    <div v-if="showCreateModal" class="modal-overlay" @click="closeCreateSessionModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>Create New Session</h2>
          <button class="close-btn" @click="closeCreateSessionModal">&times;</button>
        </div>
        
        <div class="modal-body">
          <div class="create-session-layout">
            <!-- Left side - Player selection and sessions -->
            <div class="left-section">
              <!-- Player Selection -->
              <div class="player-selection">
                <label>Select Player:</label>
                <div class="player-search-container">
                  <input 
                    type="text" 
                    v-model="playerSearchTerm" 
                    @input="searchPlayers"
                    placeholder="Search players by name..."
                    class="player-search-input"
                  />
                </div>

                <!-- Players Table -->
                <div class="players-table-container">
                  <table class="players-table">
                    <thead>
                      <tr>
                        <th>Name</th>
                        <th>Surname</th>
                        <th>Date of Birth</th>
                        <th>Action</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr 
                        v-for="player in paginatedPlayers" 
                        :key="player.id"
                        :class="{ 'selected-player': selectedPlayer?.id === player.id }"
                      >
                        <td>{{ player.firstName }}</td>
                        <td>{{ player.lastName }}</td>
                        <td>{{ formatDate(player.dateOfBirth) }}</td>
                        <td>
                          <button 
                            class="btn btn-sm btn-primary"
                            @click="selectPlayer(player)"
                            :disabled="selectedPlayer?.id === player.id"
                          >
                            {{ selectedPlayer?.id === player.id ? 'Selected' : 'Select' }}
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>

                  <!-- Pagination -->
                  <div class="pagination-container">
                    <div class="pagination-info">
                      Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, filteredPlayersForTable.length) }} of {{ filteredPlayersForTable.length }} players
                    </div>
                    <div class="pagination-controls">
                      <button 
                        class="btn btn-sm btn-secondary"
                        @click="previousPage"
                        :disabled="currentPage === 1"
                      >
                        Previous
                      </button>
                      <span class="page-info">Page {{ currentPage }} of {{ totalPages }}</span>
                      <button 
                        class="btn btn-sm btn-secondary"
                        @click="nextPage"
                        :disabled="currentPage === totalPages"
                      >
                        Next
                      </button>
                    </div>
                  </div>

                  <div v-if="filteredPlayersForTable.length === 0" class="no-players">
                    No players found
                  </div>
                </div>

                <!-- Selected Player Info -->
                <div v-if="selectedPlayer" class="selected-player-info">
                  <h4>Selected Player:</h4>
                  <div class="player-info-card">
                    <p><strong>{{ selectedPlayer.name }} {{ selectedPlayer.surname }}</strong></p>
                    <p>Position: {{ selectedPlayer.positionName || 'N/A' }}</p>
                    <p>Nationality: {{ selectedPlayer.nationalityName || 'N/A' }}</p>
                  </div>
                </div>
              </div>

              <!-- Player Sessions Table -->
              <div v-if="selectedPlayer" class="player-sessions">
                <h3>Sessions for {{ selectedPlayer.firstName }} {{ selectedPlayer.lastName }}</h3>
                <div class="table-container">
                  <table class="player-sessions-table">
                    <thead>
                      <tr>
                        <th>Date</th>
                        <th>Type</th>
                        <th>Status</th>
                        <th>Note</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="session in playerSessions" :key="session.idSession">
                        <td>{{ formatDate(session.startTime) }}</td>
                        <td>{{ session.sessionTypeName }}</td>
                        <td>
                          <span 
                            class="status-badge small" 
                            :class="getStatusClass(session.sessionStatusName)"
                          >
                            {{ session.sessionStatusName }}
                          </span>
                        </td>
                        <td class="note-cell">{{ session.note }}</td>
                      </tr>
                    </tbody>
                  </table>
                  
                  <div v-if="playerSessions.length === 0" class="no-data">
                    No sessions found for this player
                  </div>
                </div>
              </div>
            </div>

            <!-- Right side - Create session form -->
            <div class="right-section">
              <form @submit.prevent="createSession" class="session-form">
                <div class="form-group">
                  <label>Start Date:</label>
                  <input 
                    type="date" 
                    v-model="newSession.startTime" 
                    required 
                  />
                </div>

                <div class="form-group">
                  <label>End Date:</label>
                  <input 
                    type="date" 
                    v-model="newSession.endTime" 
                  />
                </div>

                <div class="form-group">
                  <label>Type:</label>
                  <select v-model="newSession.idSessionType" required>
                    <option value="">Select Type</option>
                    <option 
                      v-for="type in sessionTypes" 
                      :key="type.id" 
                      :value="type.id"
                    >
                      {{ type.name }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>Status:</label>
                  <select v-model="newSession.idSessionStatus" required>
                    <option value="">Select Status</option>
                    <option 
                      v-for="status in sessionStatuses" 
                      :key="status.id" 
                      :value="status.id"
                    >
                      {{ status.name }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>Note:</label>
                  <textarea 
                    v-model="newSession.note" 
                    rows="4"
                    placeholder="Session notes..."
                  ></textarea>
                </div>

                <div class="form-actions">
                  <button 
                    type="button" 
                    class="btn btn-secondary" 
                    @click="closeCreateSessionModal"
                  >
                    Cancel
                  </button>
                  <button 
                    type="submit" 
                    class="btn btn-primary"
                    :disabled="!selectedPlayer || isCreating"
                  >
                    {{ isCreating ? 'Creating...' : 'Create Session' }}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { getAllSessions, getSessionsByPlayerId, createSession as createSessionAPI, getSessionTypes, getSessionStatuses } from '../../services/session_service.js'
import { getAllPlayers } from '../../services/player_service.js'

// Router
const router = useRouter()

// Reactive data
const sessions = ref([])
const sessionTypes = ref([])
const sessionStatuses = ref([])
const players = ref([])
const filteredPlayers = ref([])
const playerSessions = ref([])
const showPlayerDropdown = ref(false)

// Pagination
const currentPage = ref(1)
const pageSize = ref(5)
const playerSearchTerm = ref('')

// Filters
const filters = ref({
  type: '',
  status: '',
  playerName: '',
  startDate: '',
  endDate: ''
})

// Modal and form data
const showCreateModal = ref(false)
const selectedPlayer = ref(null)
const isCreating = ref(false)

const newSession = ref({
  startTime: '',
  endTime: '',
  idSessionType: '',
  idSessionStatus: '',
  note: ''
})

// Computed
const filteredSessions = computed(() => {
  let filtered = [...sessions.value]

  if (filters.value.type) {
    filtered = filtered.filter(session => session.idSessionType === parseInt(filters.value.type))
  }

  if (filters.value.status) {
    filtered = filtered.filter(session => session.idSessionStatus === parseInt(filters.value.status))
  }

  if (filters.value.playerName) {
    filtered = filtered.filter(session => 
      session.playerName && session.playerName.toLowerCase().includes(filters.value.playerName.toLowerCase())
    )
  }

  if (filters.value.startDate) {
    filtered = filtered.filter(session => {
      if (!session.startTime) return false
      const sessionDate = new Date(session.startTime)
      const filterDate = new Date(filters.value.startDate)
      return sessionDate >= filterDate
    })
  }

  if (filters.value.endDate) {
    filtered = filtered.filter(session => {
      if (!session.endTime) return true // Include sessions without end date
      const sessionDate = new Date(session.endTime)
      const filterDate = new Date(filters.value.endDate)
      return sessionDate <= filterDate
    })
  }

  return filtered
})

// Computed properties for player pagination
const filteredPlayersForTable = computed(() => {
  if (!playerSearchTerm.value.trim()) {
    return players.value
  }
  
  return players.value.filter(player => 
    `${player.firstName} ${player.lastName}`.toLowerCase().includes(playerSearchTerm.value.toLowerCase())
  )
})

const totalPages = computed(() => {
  return Math.ceil(filteredPlayersForTable.value.length / pageSize.value)
})

const paginatedPlayers = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredPlayersForTable.value.slice(start, end)
})

// Methods
const loadSessions = async () => {
  try {
    console.log('Loading sessions...')
    const data = await getAllSessions()
    console.log('Sessions loaded:', data)
    sessions.value = data || []
  } catch (error) {
    console.error('Error loading sessions:', error)
  }
}

const loadSessionTypes = async () => {
  try {
    console.log('Loading session types...')
    const data = await getSessionTypes()
    console.log('Session types loaded:', data)
    sessionTypes.value = data || []
  } catch (error) {
    console.error('Error loading session types:', error)
  }
}

const loadSessionStatuses = async () => {
  try {
    console.log('Loading session statuses...')
    const data = await getSessionStatuses()
    console.log('Session statuses loaded:', data)
    sessionStatuses.value = data || []
  } catch (error) {
    console.error('Error loading session statuses:', error)
  }
}

const loadPlayers = async () => {
  try {
    const data = await getAllPlayers()
    console.log('Loaded players:', data) // Debug log
    players.value = data || []
  } catch (error) {
    console.error('Error loading players:', error)
  }
}

const loadPlayerSessions = async (playerId) => {
  try {
    const data = await getSessionsByPlayerId(playerId)
    playerSessions.value = data || []
  } catch (error) {
    console.error('Error loading player sessions:', error)
    playerSessions.value = []
  }
}

const searchPlayers = () => {
  // Reset to first page when searching
  currentPage.value = 1
}

const selectPlayer = async (player) => {
  selectedPlayer.value = player
  await loadPlayerSessions(player.idPlayer)
}

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const hidePlayerDropdown = () => {
  setTimeout(() => {
    showPlayerDropdown.value = false
  }, 200) // Small delay to allow click events to fire
}

const editSession = (session) => {
  // Navigate to session edit page with session ID
  router.push(`/scout/sessions/${session.idSession}/edit`)
}

const openCreateSessionModal = () => {
  showCreateModal.value = true
  resetForm()
}

const closeCreateSessionModal = () => {
  showCreateModal.value = false
  resetForm()
  selectedPlayer.value = null
  playerSearchTerm.value = ''
  currentPage.value = 1
}

const resetForm = () => {
  newSession.value = {
    startTime: '',
    endTime: '',
    idSessionType: '',
    idSessionStatus: '',
    note: ''
  }
}

const createSession = async () => {
  if (!selectedPlayer.value) {
    alert('Please select a player')
    return
  }

  isCreating.value = true
  
  try {
    // Get current user ID from localStorage or auth service
    const currentUserId = localStorage.getItem('userId') || 1 // Default to 1 for now
    
    const sessionData = {
      startTime: newSession.value.startTime,
      endTime: newSession.value.endTime || null,
      idSessionType: parseInt(newSession.value.idSessionType),
      idSessionStatus: parseInt(newSession.value.idSessionStatus),
      idUser: parseInt(currentUserId),
      idPlayer: selectedPlayer.value.idPlayer,
      note: newSession.value.note || ''
    }

    await createSessionAPI(sessionData)
    
    // Reload sessions and close modal
    await loadSessions()
    if (selectedPlayer.value) {
      await loadPlayerSessions(selectedPlayer.value.idPlayer)
    }
    
    closeCreateSessionModal()
    alert('Session created successfully!')
    
  } catch (error) {
    console.error('Error creating session:', error)
    alert('Error creating session: ' + error.message)
  } finally {
    isCreating.value = false
  }
}

const applyFilters = () => {
  // Filters are applied automatically through computed property
}

const clearFilters = () => {
  filters.value = {
    type: '',
    status: '',
    playerName: '',
    startDate: '',
    endDate: ''
  }
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  try {
    return new Date(dateString).toLocaleDateString()
  } catch {
    return '-'
  }
}

const getStatusClass = (status) => {
  if (!status) return ''
  const lowerStatus = status.toLowerCase()
  if (lowerStatus.includes('completed') || lowerStatus.includes('finished')) {
    return 'status-completed'
  } else if (lowerStatus.includes('active') || lowerStatus.includes('ongoing')) {
    return 'status-active'
  } else if (lowerStatus.includes('cancelled') || lowerStatus.includes('canceled')) {
    return 'status-cancelled'
  } else if (lowerStatus.includes('planned') || lowerStatus.includes('scheduled')) {
    return 'status-planned'
  }
  return 'status-default'
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadSessions(),
    loadSessionTypes(),
    loadSessionStatuses(),
    loadPlayers()
  ])
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
  margin-bottom: var(--spacing-xl);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
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

.create-session-btn {
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
  font-weight: 600;
}

.content-card {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
}

.debug-info {
  background-color: #fef3c7;
  border: 1px solid #f59e0b;
  padding: 1rem;
  border-radius: 0.375rem;
  margin-bottom: 1rem;
}

/* Filters */
.filters-section {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  margin-bottom: 2rem;
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: var(--border-radius);
}

.filter-group {
  display: flex;
  flex-direction: column;
  min-width: 150px;
}

.filter-group label {
  font-weight: 600;
  margin-bottom: 0.25rem;
  color: var(--color-text);
}

.filter-group input,
.filter-group select {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 0.9rem;
}

.filter-group input:focus,
.filter-group select:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

/* Table */
.table-container {
  overflow-x: auto;
}

.sessions-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

.sessions-table th,
.sessions-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.sessions-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: var(--color-text);
}

/* Clickable rows */
.clickable-row {
  cursor: pointer;
  transition: background-color 0.2s;
}

.clickable-row:hover {
  background-color: #f9fafb;
}

.note-cell {
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-badge.small {
  padding: 0.125rem 0.375rem;
  font-size: 0.7rem;
}

.status-completed {
  background-color: #d4edda;
  color: #155724;
}

.status-active {
  background-color: #cce5ff;
  color: #004085;
}

.status-cancelled {
  background-color: #f8d7da;
  color: #721c24;
}

.status-planned {
  background-color: #fff3cd;
  color: #856404;
}

.status-default {
  background-color: #e9ecef;
  color: #495057;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: var(--border-radius);
  width: 90%;
  max-width: 1200px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #eee;
}

.modal-header h2 {
  margin: 0;
  color: var(--color-text);
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #999;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  color: #333;
}

.modal-body {
  padding: 1.5rem;
}

/* Create Session Layout */
.create-session-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  min-height: 500px;
}

.left-section,
.right-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

/* Player Selection */
.player-selection {
  margin-bottom: 2rem;
}

.player-selection label {
  display: block;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: var(--color-text);
}

.player-search-container {
  margin-bottom: 1rem;
}

.player-search-input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.375rem;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.player-search-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

/* Players Table */
.players-table-container {
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  overflow: hidden;
  background: white;
  margin-bottom: 1rem;
}

.players-table {
  width: 100%;
  border-collapse: collapse;
}

.players-table th,
.players-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #f3f4f6;
}

.players-table th {
  background-color: #f9fafb;
  font-weight: 600;
  color: var(--color-text);
  border-bottom: 1px solid #e5e7eb;
}

.players-table tr.selected-player {
  background-color: #eff6ff;
}

.players-table tr:hover:not(.selected-player) {
  background-color: #f9fafb;
}

/* Pagination */
.pagination-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: #f9fafb;
  border-top: 1px solid #e5e7eb;
}

.pagination-info {
  font-size: 0.875rem;
  color: var(--color-text-light);
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.page-info {
  font-size: 0.875rem;
  color: var(--color-text);
}

/* Selected Player Info */
.selected-player-info {
  margin-top: 1rem;
  padding: 1rem;
  background-color: #f0f9ff;
  border: 1px solid #0ea5e9;
  border-radius: 0.375rem;
}

.selected-player-info h4 {
  margin: 0 0 0.5rem 0;
  color: var(--color-text);
}

.player-info-card {
  background: white;
  padding: 0.75rem;
  border-radius: 0.375rem;
  border: 1px solid #e0f2fe;
}

.player-info-card p {
  margin: 0.25rem 0;
  color: var(--color-text);
}

.no-players {
  padding: 2rem;
  text-align: center;
  color: var(--color-text-light);
  font-style: italic;
}

/* Button sizes */
.btn-sm {
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
  color: var(--color-text);
}

/* Player Selection Combobox */
.combobox-container {
  position: relative;
  width: 100%;
}

.player-combobox {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.375rem;
  font-size: 1rem;
  background: white;
  transition: border-color 0.2s;
}

.player-combobox:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.player-search {
  position: relative;
}

.player-search input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.player-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  max-height: 200px;
  overflow-y: auto;
  z-index: 1000;
}

.player-option {
  padding: 0.75rem;
  cursor: pointer;
  border-bottom: 1px solid #f3f4f6;
  transition: background-color 0.2s;
}

.player-option:hover,
.player-option.active {
  background-color: #f3f4f6;
}

.no-results {
  padding: 0.75rem;
  color: #6b7280;
  font-style: italic;
}

/* Player Sessions */
.player-sessions h3 {
  margin: 0 0 1rem 0;
  color: var(--color-text);
}

.player-sessions-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.player-sessions-table th,
.player-sessions-table td {
  padding: 0.5rem;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.player-sessions-table th {
  background-color: #f8f9fa;
  font-weight: 600;
}

/* Session Form */
.session-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-weight: 600;
  margin-bottom: 0.25rem;
  color: var(--color-text);
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: auto;
  padding-top: 1rem;
}

/* Buttons */
.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  text-decoration: none;
  display: inline-block;
  transition: all 0.2s;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background-color: #545b62;
}

.no-data {
  text-align: center;
  padding: 2rem;
  color: var(--color-text-light);
  font-style: italic;
}

/* Responsive */
@media (max-width: 768px) {
  .create-session-layout {
    grid-template-columns: 1fr;
  }
  
  .filters-section {
    flex-direction: column;
  }
  
  .filter-group {
    min-width: auto;
  }
  
  .page-header {
    flex-direction: column;
    align-items: flex-start;
  }
}

:root {
  --color-surface: #f8fafc;
  --color-text: #1f2937;
  --color-text-light: #6b7280;
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --border-radius: 0.375rem;
  --spacing-sm: 0.5rem;
  --spacing-xl: 1.5rem;
}
</style>
