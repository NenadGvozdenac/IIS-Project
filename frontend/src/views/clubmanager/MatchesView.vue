<template>
  <div class="matches-page">
    <header class="matches-header">
      <div class="container">
        <div class="header-content">
          
          <nav class="nav-menu" v-if="isTeamManager">
            <router-link to="/club-manager/matches" class="nav-link active">Matches</router-link>
            <router-link to="/club-manager/travelinfo" class="nav-link">Travel Info</router-link>
            <div class="nav-dropdown">
                <span class="nav-link dropdown-toggle">Requests</span>
                <div class="dropdown-menu">
                    <router-link to="/team-manager/transportation-requests-active" class="dropdown-item">Transportation - Active</router-link>
                    <router-link to="/team-manager/transportation-requests-archive" class="dropdown-item">Transportation - Archive</router-link>
                    <router-link to="/team-manager/accommodation-requests-active" class="dropdown-item">Accommodation - Active</router-link>
                    <router-link to="/team-manager/accommodation-requests-archive" class="dropdown-item">Accommodation - Archive</router-link>
                </div>
            </div>
          </nav>
        </div>
      </div>
    </header>

    <div class="container">
      <div class="calendar-header">
        <div class="calendar-controls">
          <button @click="previousMonth" class="btn btn-icon">
            <span>‹</span> Previous
          </button>
          <h2 class="current-month">{{ currentMonthYear }}</h2>
          <button @click="nextMonth" class="btn btn-icon">
            Next <span>›</span>
          </button>
        </div>
        
        
        <div class="calendar-actions">
          <!-- Club manager nema opciju dodavanja meča -->
        </div>
      </div>

      <div class="calendar-container">
        <div class="calendar-grid">
          <div class="calendar-header-row">
            <div class="calendar-day-header">S</div>
            <div class="calendar-day-header">M</div>
            <div class="calendar-day-header">T</div>
            <div class="calendar-day-header">W</div>
            <div class="calendar-day-header">T</div>
            <div class="calendar-day-header">F</div>
            <div class="calendar-day-header">S</div>
          </div>
          
          <div 
            v-for="day in calendarDays" 
            :key="day.date" 
            class="calendar-day"
            :class="{
              'other-month': !day.isCurrentMonth,
              'today': day.isToday
            }"
          >
            <div class="day-number">{{ day.number }}</div>
            
            <div class="day-matches">
              <div 
                v-for="match in day.matches" 
                :key="match.idMatch"
                class="match-item"
                :class="getMatchClass(match)"
                @click="showMatchMenu(match, $event)"
              >
                <div class="match-time">{{ formatMatchTime(match.scheduledAt) }}</div>
                <div class="match-name">{{ match.name }}</div>
                
                <div class="match-indicators">
                  <span v-if="match.transportationRequired" class="indicator transport" title="Transport required">🚌</span>
                  <span v-if="match.accommodationRequired" class="indicator accommodation" title="Accommodation required">🏨</span>
                  <!--<span v-if="!match.travelCreated" class="indicator no-travel" title="Travel not created">●</span>-->
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="legend">
        <h3>Legend:</h3>
        <div class="legend-items">
          <div class="legend-item">
            <span class="legend-color next-match"></span>
            <span>Next match</span>
          </div>
          <div class="legend-item">
            <span class="legend-color future-matches"></span>
            <span>Future matches</span>
          </div>
          <div class="legend-item">
            <span class="legend-color matches-played"></span>
            <span>Matches played</span>
          </div>
          <div class="legend-item">
            <span class="legend-color home-matches"></span>
            <span>Home matches</span>
          </div>
          <div class="legend-item">
            <span class="legend-color away-matches"></span>
            <span>Away matches</span>
          </div>
          <div class="legend-item">
            <span class="legend-icon">🚌</span>
            <span>Transport required</span>
          </div>
          <div class="legend-item">
            <span class="legend-icon">🏨</span>
            <span>Accommodation required</span>
          </div>
          <div class="legend-item">
            <span class="legend-color no-travel-dot"></span>
            <span>Travel not created</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Match Context Menu -->
    <div 
      v-if="selectedMatch && showMenu" 
      class="match-menu"
      :style="{ top: menuPosition.y + 'px', left: menuPosition.x + 'px' }"
      @click.stop
    >
      <!-- Club manager vidi samo detalje meča -->
      <div class="menu-item" @click="viewMatchDetails">Details</div>
    </div>

    <!-- Create/Edit Match Modal -->
    <div v-if="showCreateModal || showEditModal" class="modal-overlay" @click="closeMatchModal">
  <!-- Club manager nema modal za kreiranje/editovanje meča -->
    </div>

    <div v-if="showDeleteModal" class="modal-overlay" @click="closeDeleteModal">
  <!-- Club manager nema modal za brisanje meča -->
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { AuthService } from '../../services/auth_service.js'
import axios from 'axios'

const router = useRouter()

// Get user ID from token using AuthService
const getUserId = () => {
  const token = localStorage.getItem('token')
  if (!token) return null
  
  const userData = AuthService.decode(token)
  return userData?.userID || null
}

const currentDate = ref(new Date())

const showTranspoOptions = (match) => {
  return match.transportationRequired && !isMatchPlayed(match)
}

const showAccommodOptions = (match) => {
  return match.accommodationRequired && !isMatchPlayed(match)
}

const pad = (n) => n.toString().padStart(2, '0')
const getMinDateTime = () => {
  const now = new Date()
  const yyyy = now.getFullYear()
  const mm = pad(now.getMonth() + 1)
  const dd = pad(now.getDate())
  const hh = pad(now.getHours())
  const min = pad(now.getMinutes())
  return `${yyyy}-${mm}-${dd}T${hh}:${min}`
}
const minDateTime = ref(getMinDateTime())

setInterval(() => {
  minDateTime.value = getMinDateTime()
}, 60000)
const matches = ref([])
const selectedMatch = ref(null)
const showMenu = ref(false)
const menuPosition = ref({ x: 0, y: 0 })
const showCreateModal = ref(false)
const isTeamManager = ref(true) 
const userName = ref('Team Manager') 
const loading = ref(false)

const competitions = ref([])
const seasons = ref([])
const teams = ref([])

const newMatch = ref({
  name: '',
  scheduledAt: '',
  type: '',
  state: '',
  city: '',
  hall: '',
  isInOurHall: false,
  transportationRequired: false,
  accommodationRequired: false,
  competitionId: 0,
  seasonId: 0,
  teamId: 0,
  userId: getUserId() 
})

const currentMonthYear = computed(() => {
  return currentDate.value.toLocaleDateString('en-US', { 
    month: 'long', 
    year: 'numeric' 
  })
})

const calendarDays = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const startDate = new Date(firstDay)
  startDate.setDate(startDate.getDate() - firstDay.getDay())
  
  const days = []
  const today = new Date()
  
  for (let i = 0; i < 42; i++) {
    const date = new Date(startDate)
    date.setDate(startDate.getDate() + i)
    
    const dayMatches = matches.value.filter(match => {
      const matchDate = new Date(match.scheduledAt)
      return matchDate.toDateString() === date.toDateString()
    })
    
    days.push({
      date: date.toISOString(),
      number: date.getDate(),
      isCurrentMonth: date.getMonth() === month,
      isToday: date.toDateString() === today.toDateString(),
      matches: dayMatches
    })
  }
  
  return days
})

// Match status and menu logic functions
const isMoreThan24HoursAway = (match) => {
  const now = new Date()
  const matchDate = new Date(match.scheduledAt)
  const hoursUntilMatch = (matchDate - now) / (1000 * 60 * 60)
  return hoursUntilMatch > 24
}

const getMatchStatus = (match) => {
  const now = new Date()
  const matchDate = new Date(match.scheduledAt)
  
  if (matchDate < now) {
    return 'played'
  }
  
  const futureMatches = matches.value.filter(m => new Date(m.scheduledAt) > now)
  const nextMatch = futureMatches.sort((a, b) => new Date(a.scheduledAt) - new Date(b.scheduledAt))[0]
  
  if (nextMatch && nextMatch.idMatch === match.idMatch) {
    return 'next'
  }
  
  return 'future'
}

const canEditMatch = (match) => {
  const status = getMatchStatus(match)
  return (status === 'future' || status === 'next') && 
         isMoreThan24HoursAway(match) && 
         isTeamManager.value
}

const canDeleteMatch = (match) => {
  const status = getMatchStatus(match)
  return (status === 'future' || status === 'next') && 
         isMoreThan24HoursAway(match) && 
         isTeamManager.value
}

const isMatchPlayed = (match) => {
  const status = getMatchStatus(match)
  return status === 'played'
}


const fetchMatches = async () => {
  try {
    loading.value = true
    const response = await axios.get('https://localhost:5007/api/matches', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    console.log('Fetched matches:', response.data.value.matches)
    matches.value = Array.isArray(response.data.value.matches)
      ? response.data.value.matches
      : []
    for (const match of matches.value) {
      await checkTravelStatus(match)
    }
  } catch (error) {
    console.error('Error fetching matches:', error)
  } finally {
    loading.value = false
  }
}

const fetchCompetitions = async () => {
  try {
    const response = await axios.get('https://localhost:5007/api/competitions', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    competitions.value = response.data.value || []
  } catch (error) {
    console.error('Error fetching competitions:', error)
  }
}

const fetchSeasons = async () => {
  try {
    const response = await axios.get('https://localhost:5007/api/seasons', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    seasons.value = response.data.value || []
  } catch (error) {
    console.error('Error fetching seasons:', error)
  }
}

const fetchTeams = async () => {
  try {
    const response = await axios.get('https://localhost:5007/api/teams', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    // Preskoči prvi tim (moj tim)
    const allTeams = response.data.value || []
    teams.value = allTeams.length > 1 ? allTeams.slice(1) : []
  } catch (error) {
    console.error('Error fetching teams:', error)
  }
}

const checkTravelStatus = async (match) => {
  try {
    await axios.get(`/api/travels/by-match/${match.idMatch}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    match.travelCreated = true
  } catch (error) {
    match.travelCreated = false
  }
}

const createMatch = async () => {
  const userId = getUserId()
  if (!userId) {
    alert('User not authenticated')
    return
  }
  // Validacija: ne dozvoli vreme pre trenutnog ako je danasnji datum
  const selected = new Date(editMatchData.value.scheduledAt)
  const now = new Date()
  if (
    selected.getFullYear() === now.getFullYear() &&
    selected.getMonth() === now.getMonth() &&
    selected.getDate() === now.getDate() &&
    (selected.getHours() < now.getHours() || (selected.getHours() === now.getHours() && selected.getMinutes() < now.getMinutes()))
  ) {
    alert('Vreme mora biti veće od trenutnog!')
    return
  }
  // Update userId in newMatch before sending
  editMatchData.value.userId = userId
  editMatchData.value.scheduledAt = selected.toISOString()
  try {
    loading.value = true
    console.log('Creating match with data:', editMatchData.value)
    const response = await axios.post(`https://localhost:5007/api/matches/${userId}`, editMatchData.value, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    showCreateModal.value = false
    resetNewMatch()
    await fetchMatches()
  } catch (error) {
    console.error('Error creating match:', error)
    alert(error.response?.data?.error || 'Failed to create match')
  } finally {
    loading.value = false
  }
}

const deleteMatch = () => {
  showDeleteModal.value = true;
}

const previousMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() - 1, 1)
}

const nextMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 1)
}

const showMatchMenu = (match, event) => {
  event.preventDefault()
  event.stopPropagation()
  
  selectedMatch.value = match
  
  // Get the match element's position
  const rect = event.currentTarget.getBoundingClientRect()
  const menuHeight = 200 // estimated menu height
  const menuWidth = 200
  
  let x = rect.left
  let y = rect.bottom + 5 // Position below the match item
  
  // Check if menu would go below viewport
  if (y + menuHeight > window.innerHeight) {
    y = rect.top - menuHeight - 5 // Position above the match item
  }
  
  // Check if menu would go beyond right edge
  if (x + menuWidth > window.innerWidth) {
    x = window.innerWidth - menuWidth - 10
  }
  
  // Ensure menu doesn't go beyond left edge
  if (x < 10) {
    x = 10
  }
  
  menuPosition.value = { x, y }
  showMenu.value = true
  
  console.log('Match menu opened for:', match.name, {
    status: getMatchStatus(match),
    canEdit: canEditMatch(match),
    canDelete: canDeleteMatch(match),
    isPlayed: isMatchPlayed(match),
    position: { x, y },
    elementRect: rect
  })
}

const hideMenu = () => {
  showMenu.value = false
  selectedMatch.value = null
}

const getMatchClass = (match) => {
  const matchDate = new Date(match.scheduledAt)
  const now = new Date()
  const classes = []

  // Stripe for home/away
  if (match.type === 'home') {
    classes.push('home-stripe')
  } else if (match.type === 'away') {
    classes.push('away-stripe')
  }

  // Color for match status
  if (matchDate < now) {
    classes.push('played-match') // gray
  } else {
    // Find next match
    const futureMatches = matches.value.filter(m => new Date(m.scheduledAt) > now)
    const nextMatch = futureMatches.sort((a, b) => new Date(a.scheduledAt) - new Date(b.scheduledAt))[0]
    if (nextMatch && nextMatch.idMatch === match.idMatch) {
      classes.push('next-match') // green
    } else {
      classes.push('future-match') // blue
    }
  }
  return classes.join(' ')
}

const formatMatchTime = (dateTime) => {
  return new Date(dateTime).toLocaleTimeString('en-US', { 
    hour: '2-digit', 
    minute: '2-digit',
    hour12: false 
  })
}

const viewMatchDetails = () => {
  if (selectedMatch.value && selectedMatch.value.idMatch) {
    router.push({
      name: 'MatchDetailsClubManager',
      params: { id: selectedMatch.value.idMatch }
    })
  }
  hideMenu()
}

const showEditModal = ref(false)
const showDeleteModal = ref(false)
const editMatchData = ref({
  name: '',
  scheduledAt: '',
  type: '',
  state: '',
  city: '',
  hall: '',
  isInOurHall: false,
  transportationRequired: false,
  accommodationRequired: false,
  competitionId: 0,
  seasonId: 0,
  teamId: 0,
  userId: getUserId()
})

const closeMatchModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
}

const closeDeleteModal = () => {
  showDeleteModal.value = false
}

const editMatch = async () => {
  await Promise.all([
    fetchCompetitions(),
    fetchSeasons(),
    fetchTeams()
  ])
  if (selectedMatch.value) {
    Object.assign(editMatchData.value, selectedMatch.value)
    // Convert scheduledAt to datetime-local format
    if (editMatchData.value.scheduledAt) {
      const d = new Date(editMatchData.value.scheduledAt)
      editMatchData.value.scheduledAt = d.toISOString().slice(0,16)
    }
    showEditModal.value = true
    showCreateModal.value = false
  }
  hideMenu()
}

const updateMatch = async () => {
  const userId = getUserId()
  if (!userId) {
    alert('User not authenticated')
    return
  }
  // Validacija: ne dozvoli vreme pre trenutnog ako je danasnji datum
  const selected = new Date(editMatchData.value.scheduledAt)
  const now = new Date()
  if (
    selected.getFullYear() === now.getFullYear() &&
    selected.getMonth() === now.getMonth() &&
    selected.getDate() === now.getDate() &&
    (selected.getHours() < now.getHours() || (selected.getHours() === now.getHours() && selected.getMinutes() < now.getMinutes()))
  ) {
    alert('Vreme mora biti veće od trenutnog!')
    return
  }
  // Prepare data
  const payload = { ...editMatchData.value, userId }
  payload.scheduledAt = selected.toISOString()
  try {
    loading.value = true
    await axios.put(`https://localhost:5007/api/matches/${editMatchData.value.idMatch}/${userId}`, payload, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    closeMatchModal()
    await fetchMatches()
  } catch (error) {
    console.error('Error updating match:', error)
    alert(error.response?.data?.error || 'Failed to update match')
  } finally {
    loading.value = false
  }
}


const confirmDeleteMatch = async () => {
    console.log('Confirm delete for match:', selectedMatch.value)
  if (!selectedMatch.value) return
  const userId = getUserId()
  if (!userId) {
    alert('User not authenticated')
    return
  }
  try {
    await axios.delete(`https://localhost:5007/api/matches/${selectedMatch.value.idMatch}/${userId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    closeDeleteModal()
    await fetchMatches()
  } catch (error) {
    console.error('Error deleting match:', error)
    alert(error.response?.data?.error || 'Failed to delete match')
  }
  hideMenu()
}

const generateReport = () => {
  console.log('Generate report for match:', selectedMatch.value)
  hideMenu()
}


const resetNewMatch = () => {
  editMatchData.value = {
    name: '',
    scheduledAt: '',
    type: '',
    state: '',
    city: '',
    hall: '',
    isInOurHall: false,
    transportationRequired: false,
    accommodationRequired: false,
    competitionId: 0,
    seasonId: 0,
    teamId: 0,
    userId: getUserId() || 1
  }
}

const openCreateModal = async () => {
  await Promise.all([
    fetchCompetitions(),
    fetchSeasons(),
    fetchTeams()
  ])
  Object.assign(editMatchData.value, {
    name: '',
    scheduledAt: '',
    type: '',
    state: '',
    city: '',
    hall: '',
    isInOurHall: false,
    transportationRequired: false,
    accommodationRequired: false,
    competitionId: '',
    seasonId: '',
    teamId: '',
    userId: getUserId()
  })
  showCreateModal.value = true
  showEditModal.value = false
}

const onTypeChange = () => {
  // Reset checkboxes when changing type
  if (editMatchData.value.type === 'home') {
    editMatchData.value.transportationRequired = false
    editMatchData.value.accommodationRequired = false
  } else if (editMatchData.value.type === 'away') {
    editMatchData.value.isInOurHall = false
  }
}

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}

const handleClickOutside = (event) => {
  if (showMenu.value && !event.target.closest('.match-menu')) {
    hideMenu()
  }
}

onMounted(() => {
  // Check if user is authenticated
  const token = localStorage.getItem('token')
  const userId = getUserId()
  
  if (!token || !userId) {
    router.push('/login')
    return
  }
  
  fetchMatches()
  fetchCompetitions()
  fetchSeasons() 
  fetchTeams()
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.matches-header {
  background: white;
  border-bottom: 1px solid #e5e7eb;
  padding: 1rem 0;
}

.header-content {
  display: flex;
  justify-content: center;
  align-items: center;
}

.logo h2 {
  color: var(--color-primary);
  margin: 0;
}

.nav-menu {
  display: flex;
  gap: 2rem;
}

.nav-link {
  text-decoration: none;
  color: #6b7280;
  font-weight: 500;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  transition: all 0.2s;
}

.nav-link:hover,
.nav-link.active {
  color: var(--color-primary);
  background-color: #f3f4f6;
}

/* Dropdown styles */
.nav-dropdown {
  position: relative;
  padding: 0.5rem;
}

.dropdown-toggle {
  cursor: pointer;
}

.dropdown-toggle::after {
  content: ' ▼';
  font-size: 12px;
}

.nav-dropdown:hover .dropdown-toggle::after {
  content: ' ▲';
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  min-width: 200px;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
  z-index: 1000;
}

.nav-dropdown:hover .dropdown-menu {
  opacity: 1;
  visibility: visible;
}

.dropdown-item {
  display: block;
  padding: 10px 15px;
  text-decoration: none;
  color: #333;
  transition: background-color 0.3s;
}

.dropdown-item:last-child {
  border-bottom: none;
}

.dropdown-item:hover {
  background-color: #f0f0f0;
}

.user-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-name {
  font-weight: 500;
  color: #374151;
}

/* Calendar */
.calendar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 2rem 0;
  gap: 2rem;
}

.calendar-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.current-month {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: #374151;
  min-width: 200px;
  text-align: center;
}

.info-box {
  background: #fef3c7;
  border: 1px solid #f59e0b;
  border-radius: 0.5rem;
  padding: 1rem;
  max-width: 300px;
  font-size: 0.875rem;
  color: #92400e;
}

.calendar-container {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
}

.calendar-header-row {
  display: contents;
}

.calendar-day-header {
  background: #f9fafb;
  padding: 1rem;
  font-weight: 600;
  color: #374151;
  text-align: center;
  border-bottom: 1px solid #e5e7eb;
}

.calendar-day {
  min-height: 120px;
  border-bottom: 1px solid #e5e7eb;
  border-right: 1px solid #e5e7eb;
  padding: 0.5rem;
  position: relative;
}

.calendar-day:nth-child(7n) {
  border-right: none;
}

.calendar-day.other-month {
  background: #f9fafb;
  color: #9ca3af;
}

.calendar-day.today {
  background: #eff6ff;
}

.day-number {
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.day-matches {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.match-item {
  background: #3b82f6;
  color: white;
  padding: 2px 4px;
  border-radius: 3px;
  font-size: 0.75rem;
  cursor: pointer;
  position: relative;
  min-height: 20px;
}

.match-item:hover {
  opacity: 0.8;
}

.match-item.home-match {
  background: #10b981;
}

.match-item.away-match {
  background: #8b5cf6;
}

.match-item.played-match {
  background: #6b7280;
}

.match-item.next-match {
  background: #059669;
  color: white;
}

.match-item.future-match {
  background: #3b82f6;
  color: white;
}

.match-item.home-stripe {
  border-left: 6px solid orange;
}

.match-item.away-stripe {
  border-left: 6px solid #8b5cf6;
}

.match-time {
  font-weight: 600;
  font-size: 0.7rem;
}

.match-name {
  font-size: 0.7rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.match-indicators {
  display: flex;
  gap: 2px;
  margin-top: 2px;
}

.indicator {
  font-size: 0.6rem;
}

.indicator.transport {
  color: #60a5fa;
}

.indicator.accommodation {
  color: #34d399;
}

.indicator.no-travel {
  color: #ef4444;
  font-size: 0.5rem;
}

/* Match Menu */
.match-menu {
  position: fixed;
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  z-index: 1000;
  min-width: 180px;
  max-width: 250px;
}

.menu-item {
  padding: 0.75rem 1rem;
  cursor: pointer;
  border-bottom: 1px solid #f3f4f6;
  transition: background-color 0.2s;
  font-size: 0.875rem;
  color: #374151;
}

.menu-item:hover {
  background-color: #f9fafb;
}

.menu-item:last-child {
  border-bottom: none;
}

.menu-item.danger {
  color: #dc2626;
}

.menu-item.danger:hover {
  background-color: #fef2f2;
}

.match-menu hr {
  margin: 0;
  border: none;
  border-top: 1px solid #e5e7eb;
}

/* Legend */
.legend {
  margin: 2rem 0;
  padding: 1rem;
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.legend h3 {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 600;
}

.legend-items {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.legend-color {
  width: 16px;
  height: 16px;
  border-radius: 3px;
}

.legend-color.next-match {
  background: #059669;
}

.legend-color.future-matches {
  background: #3b82f6;
}

.legend-color.matches-played {
  background: #6b7280;
}

.legend-color.home-matches {
  background: #10b981;
}

.legend-color.away-matches {
  background: #8b5cf6;
}

.legend-color.no-travel-dot {
  background: #ef4444;
  border-radius: 50%;
}

.legend-icon {
  font-size: 1rem;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 0.5rem;
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-header h3 {
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6b7280;
}

.modal-body {
  padding: 1rem;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
  color: #374151;
}

.form-control {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
}

.form-control:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  background: white;
  
  /* Ovo sakriva prirodnu strelicu u različitim browser-ima */
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  
  /* Vaša custom strelica */
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='m6 8 4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
}

.form-select:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-select option {
  color: #374151;
  background: #fff;
  font-size: 0.95rem;
}

.form-check {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.form-check-input {
  width: 1rem;
  height: 1rem;
  margin: 0;
}

.form-check-label {
  margin: 0;
  font-weight: normal;
}

.form-label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
  color: #374151;
}

.mb-3 {
  margin-bottom: 1rem;
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1.5rem;
}

/* Buttons */
.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.375rem;
  font-weight: 500;
  cursor: pointer;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.btn-primary {
  background: var(--color-primary);
  color: white;
}

.btn-primary:hover {
  background: #2563eb;
}

.btn-secondary {
  background: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background: #4b5563;
}

.btn-icon {
  background: transparent;
  color: #6b7280;
  border: 1px solid #d1d5db;
}

.btn-icon:hover {
  background: #f9fafb;
}

/* Responsive */
@media (max-width: 768px) {
  .calendar-header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }
  
  .calendar-controls {
    justify-content: center;
  }
  
  .calendar-day {
    min-height: 80px;
  }
  
  .legend-items {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .header-content {
    flex-direction: column;
    gap: 1rem;
  }
  
  .nav-menu {
    justify-content: center;
  }

  .match-menu {
    max-width: 90vw;
  }
}
</style>