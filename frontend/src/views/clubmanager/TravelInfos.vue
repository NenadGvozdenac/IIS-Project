<template>
  <div class="players-page">
    <header class="matches-header">
      <div class="container">
        <div class="header-content">
          <nav class="nav-menu" v-if="isTeamManager">
            <router-link to="/club-manager/matches" class="nav-link">Matches</router-link>
            <router-link to="/club-manager/travelinfo" class="nav-link">Players</router-link>
            <router-link to="/club-manager/travel" class="nav-link">Travel Organization</router-link>
          </nav>
        </div>
      </div>
    </header>

    <div class="container">
      <div class="player-overview">
        <h1>Player overview</h1>
        <div class="overview-actions">
         
        </div>
        <div class="search-box">
          <input type="text" v-model="searchQuery" placeholder="Search.." class="form-control" />
        </div>
        
        <div v-if="loading" class="loading">
          Loading players...
        </div>
        
        <div v-else class="player-cards">
          <div v-for="player in filteredPlayers" :key="player.id" class="player-card">
            <div class="card-header">
              <span class="player-name">{{ player.fullPlayerName }}</span>
              <div class="card-actions">
                </div>
            </div>
            <div class="card-body">
              <div>Passport Number: {{ player.passportNumber }}</div>
              <div>Passport Expiry Date: {{ formatDate(player.passportExpirationDate) }}</div>
              <div>Nationality: {{ player.nationality }}</div>
              <div class="visas-row">
                Visas:
                <span class="visa-icon" @click="showVisaList(player)">
                  <svg width="20" height="20" viewBox="0 0 20 20"><rect x="2" y="6" width="16" height="2" fill="#333"/><rect x="2" y="10" width="16" height="2" fill="#333"/><rect x="2" y="14" width="16" height="2" fill="#333"/></svg>
                </span>
                <div v-if="player.showVisas" class="visa-list">
                  <div v-for="visa in player.visas" :key="visa.id" class="visa-item">
                    <div class="visa-info">
                      {{ visa.visaNumber }} | {{ visa.state }} | {{ formatDate(visa.creationDate) }} | {{ formatDate(visa.expirationDate) }}
                    </div>
                    
                  </div>
                </div>
              </div>
              <div>Phone Number: {{ player.phone }}</div>
              <div>Email Address: {{ player.email }}</div>
            </div>
          </div>
        </div>
      </div>
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

const travelInfos = ref([])
const players = ref([])
const loading = ref(false)
const teamPlayers = ref([])
const nationalities = ref([])
const myTeamId = ref(null)

const fetchTeamPlayers = async () => {
  try {
    const response = await axios.get('https://localhost:5007/api/teammembers', {
      headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
    })
    const allTeamPlayers = Array.isArray(response.data.value) ? response.data.value : []
    const existingPlayerIds = new Set(players.value.map(p => p.idPlayer))
    console.log('Fetched team players:', existingPlayerIds)
    teamPlayers.value = allTeamPlayers.filter(tp => !existingPlayerIds.has(tp.idPlayer))
  } catch (error) {
    console.error('Error fetching team players:', error)
  }
}

const fetchNationalities = async () => {
  try {
    const response = await axios.get('https://localhost:5007/api/nationality', {
      headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
    })
    nationalities.value = Array.isArray(response.data.value) ? response.data.value : []
  } catch (error) {
    console.error('Error fetching nationalities:', error)
  }
}

const fetchMyTeam = async () => {
  try {
    const response = await axios.get('https://localhost:5007/api/teams', {
      headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
    })
    const allTeams = response.data.value || []
    if (allTeams.length > 0) {
      myTeamId.value = allTeams[0].idTeam // Prvi tim je "moj tim"
    }
  } catch (error) {
    console.error('Error fetching my team:', error)
  }
}

const fetchPlayers = async () => {
  try {
    loading.value = true
    const response = await axios.get('https://localhost:5007/api/travelinformations', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    console.log('Fetched players:', response.data.value.travelInfos)
    players.value = Array.isArray(response.data.value.travelInfos)
      ? response.data.value.travelInfos
      : []
  } catch (error) {
    console.error('Error fetching players:', error)
  } finally {
    loading.value = false
  }
}



// 5. Dohvatanje viza za igrača
const fetchVisas = async (playerId) => {
  try {
    const response = await axios.get(`https://localhost:5007/api/visas/by-travel-info/${playerId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    const visas = Array.isArray(response.data.value.visas)
      ? response.data.value.visas
      : []
    const player = players.value.find(p => p.idTravelInfo == playerId)
    if (player) player.visas = visas
  } catch (error) {
    console.error('Error fetching visas:', error)
  }
}

const searchQuery = ref('')

// Filter players using the search input
const filteredPlayers = computed(() => {
  const q = (searchQuery.value || '').toString().trim().toLowerCase()
  if (!q) return players.value
  return players.value.filter(p => {
    const name = (p.fullPlayerName || p.fullName || '').toString().toLowerCase()
    const passport = (p.passportNumber || '').toString().toLowerCase()
    const nationality = (p.nationality || '').toString().toLowerCase()
    const phone = (p.phone || '').toString().toLowerCase()
    const email = (p.email || '').toString().toLowerCase()
    return name.includes(q) || passport.includes(q) || nationality.includes(q) || phone.includes(q) || email.includes(q)
  })
})
const showNoteModal = ref(false)
const showVisaModal = ref(false)
const isTeamManager = ref(true) 
const userName = ref('Team Manager')
const isEditMode = ref(false)
const editNoteData = ref({
  playerId: '',
  name: '',
  number: '',
  passportNumber: '',
  passportExpirationDate: '',
  nationality: '',
  phone: '',
  email: '',
  userId: getUserId()
})
const visaData = ref({
  place: '',
  number: '',
  issueDate: '',
  expiryDate: ''
})


function formatDate(dateVal) {
  if (!dateVal) return ''
  if (typeof dateVal === 'string') {
    // "2028-11-15"
    return new Date(dateVal).toLocaleDateString('en-GB')
  }
  if (typeof dateVal === 'object' && dateVal.year && dateVal.month && dateVal.day) {
    // { year: 2028, month: 11, day: 15 }
    return `${dateVal.day.toString().padStart(2, '0')}.${dateVal.month.toString().padStart(2, '0')}.${dateVal.year}.`
  }
  return ''
}


function openAddNoteModal() {
  isEditMode.value = false
  resetNoteData()
  showNoteModal.value = true
}

function openEditNoteModal(player) {
  isEditMode.value = true
  editNoteData.value = {
    playerId: player.idTravelInfo,
    name: player.fullPlayerName,
    number: player.number,
    passportNumber: player.passportNumber,
    passportExpirationDate: player.passportExpirationDate,
    nationality: player.nationality,
    phone: player.phone,
    email: player.email,
    userId: getUserId()
  }
  showNoteModal.value = true
}
function closeNoteModal() {
  showNoteModal.value = false
}

const resetNoteData = () => {
  editNoteData.value = {
    playerId: '',
    name: '',
    number: '',
    passportNumber: '',
    passportExpirationDate: '',
    nationality: '',
    phone: '',
    email: '',
    userId: getUserId()
  }
}
function showVisaList(player) {
  player.showVisas = !player.showVisas
  if (player.showVisas) {
    fetchVisas(player.idTravelInfo)
  }
}

const handleClickOutside = (event) => {
  // Close visa lists when clicking outside
  if (!event.target.closest('.visa-icon') && !event.target.closest('.visa-list')) {
    travelInfos.value.forEach(player => {
      if (player.showVisas) {
        player.showVisas = false
      }
    })
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
  
  fetchPlayers()
  fetchTeamPlayers()
  fetchNationalities()
  fetchMyTeam()
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

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem; /* prevent children from touching viewport edges on small screens */
}

.player-overview {
  max-width: 1200px;
  margin: 2rem auto;
}

.overview-actions {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 1rem;
  padding: 0 0.25rem; /* small inner spacing so buttons aren't flush to container */
}

.search-box {
  margin-bottom: 2rem;
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

.loading {
  text-align: center;
  padding: 2rem;
  color: #666;
  font-style: italic;
}

.player-cards {
  display: flex;
  gap: 2rem;
  flex-wrap: wrap;
}

.player-card {
  background: #fff;
  border: 2px solid #333;
  border-radius: 12px;
  padding: 1.5rem;
  min-width: 280px;
  max-width: 320px;
  margin-bottom: 2rem;
  position: relative;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.card-actions {
  display: flex;
  gap: 0.5rem;
}

.player-name {
  font-weight: 700;
  font-size: 1.1rem;
  color: #374151;
}

.player-number {
  font-size: 0.9rem;
  color: #6b7280;
  margin-left: 0.5rem;
}

.card-body > div {
  margin-bottom: 0.5rem;
  color: #374151;
  font-size: 0.875rem;
}

.visas-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  position: relative;
}

.visa-icon {
  cursor: pointer;
  margin-left: 0.5rem;
  padding: 0.25rem;
  border-radius: 0.25rem;
  transition: background-color 0.2s;
}

.visa-icon:hover {
  background-color: #f3f4f6;
}

.visa-list {
  position: absolute;
  top: 2rem;
  left: 0;
  background: #fff;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  padding: 0.5rem;
  z-index: 10;
  min-width: 280px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

.visa-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
  padding: 0.5rem;
  border-radius: 0.25rem;
  background-color: #f9fafb;
}

.visa-item:last-child {
  margin-bottom: 0;
}

.visa-info {
  flex: 1;
  font-size: 0.8rem;
  color: #374151;
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
  margin: 0 1rem; /* avoid touching viewport edges on very small screens */
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
  color: #374151;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6b7280;
}

.close-btn:hover {
  color: #374151;
}

.add-visa-btn {
  margin-left: 1rem;
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
  font-size: 0.875rem;
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

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
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
  font-size: 0.875rem;
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

.btn-danger {
  background: #dc2626;
  color: white;
}

.btn-danger:hover {
  background: #b91c1c;
}

.btn-sm {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn:disabled:hover {
  background: inherit !important;
}

/* Responsive */
@media (max-width: 768px) {
  .player-cards {
    flex-direction: column;
    gap: 1rem;
  }
  
  .player-card {
    min-width: 220px;
    max-width: 100%;
  }
  
  .card-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }
  
  .card-actions {
    align-self: flex-end;
  }
  
  .header-content {
    flex-direction: column;
    gap: 1rem;
  }
  
  .nav-menu {
    justify-content: center;
  }

  .visa-list {
    min-width: 250px;
    max-width: 90vw;
  }
  
  .modal {
    width: 95%;
    max-width: 400px;
  }
}
</style>
