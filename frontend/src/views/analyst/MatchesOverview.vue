<template>
  <div class="matches-overview">
    <!-- Main Content -->
    <main class="main-content">
      <div class="content-header">
        <h1>Partizan profile</h1>
      </div>

      <div v-if="loading" class="loading-state">
        <p>Loading matches...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <p>Error loading matches: {{ error }}</p>
        <button @click="fetchMatches" class="btn-primary">Try Again</button>
      </div>

      <div v-else class="matches-container">
        <!-- Live Matches -->
        <div class="matches-section live-matches">
          <h2>Live matches</h2>
          <div class="matches-list">
            <div v-if="matches.liveMatches.length === 0" class="no-matches">
              <p>No live matches at the moment.</p>
            </div>
            <div v-else>
              <div 
                v-for="match in matches.liveMatches" 
                :key="match.idMatch"
                class="match-card live clickable"
                @click="handleMatchAction(match)"
                :class="{ disabled: match.trackingStatus === 'preparation' }"
              >
                <div class="match-info">
                  <h3>{{ match.name }}</h3>
                  <p class="match-date">Date: {{ formatDate(match.scheduledAt) }}</p>
                  <p class="match-place">Place: {{ match.isInOurHall ? 'Home' : 'Opponent' }}</p>
                  <div v-if="getMatchScore(match)" class="match-score live-score">
                    Score: {{ getMatchScore(match) }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Upcoming Matches -->
        <div class="matches-section upcoming-matches">
          <h2>Upcoming matches</h2>
          <div class="matches-list">
            <div v-if="matches.upcomingMatches.length === 0" class="no-matches">
              <p>No upcoming matches scheduled.</p>
            </div>
            <div v-else class="matches-grid">
              <div 
                v-for="match in matches.upcomingMatches" 
                :key="match.idMatch"
                class="match-card upcoming"
              >
                <div class="match-info">
                  <h3>{{ match.name }}</h3>
                  <p class="match-date">Date: {{ formatDate(match.scheduledAt) }}</p>
                  <p class="match-place">Place: {{ match.isInOurHall ? 'Home' : 'Opponent' }}</p>
                </div>
                <button 
                  class="match-action start-up" 
                  @click="handleMatchAction(match)"
                  :class="{ disabled: match.trackingStatus === 'preparation' }"
                >
                  {{ match.trackingStatus === 'preparation' ? 'View Match' : 'Start-up' }}
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Previous Matches -->
        <div class="matches-section previous-matches">
          <h2>Previous matches</h2>
          <div class="matches-list">
            <div v-if="matches.previousMatches.length === 0" class="no-matches">
              <p>No previous matches found.</p>
            </div>
            <div v-else class="matches-grid">
              <div 
                v-for="match in matches.previousMatches" 
                :key="match.idMatch"
                class="match-card previous"
                :class="getMatchResultClass(match)"
              >
                <div class="match-info">
                  <div class="result-indicator">{{ getMatchResult(match) }}</div>
                  <h3>{{ match.name }}</h3>
                  <p class="match-date">Date: {{ formatDate(match.scheduledAt) }}</p>
                  <p class="match-place">Place: {{ match.isInOurHall ? 'Home' : 'Opponent' }}</p>
                  <div v-if="getMatchScore(match)" class="match-score final-score">
                    Final Score: {{ getMatchScore(match) }}
                  </div>
                </div>
                <button class="match-action report">Report</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Startup Modal -->
    <div v-if="showStartupModal" class="modal-overlay" @click="closeStartupModal">
      <div class="modal-content" @click.stop>
        <h2>Match Startup - {{ selectedMatch?.name }}</h2>
        
        <div v-if="loadingPlayers" class="loading-players">
          <p>Loading players...</p>
        </div>
        
        <div v-else class="teams-container">
          <!-- Our Team -->
          <div class="team-section">
            <div class="team-header">
              <h3>Our Team (Partizan) - {{ selectedOurPlayers.length }}/{{ ourTeamPlayers.length }}</h3>
              <div class="team-actions">
                <button class="btn-select-all" @click="selectAllOurPlayers">Select All</button>
                <button class="btn-uncheck-all" @click="uncheckAllOurPlayers">Uncheck All</button>
              </div>
            </div>
            <div class="players-list">
              <div 
                v-for="player in ourTeamPlayers" 
                :key="player.playerId"
                class="player-item"
                :class="{ selected: isPlayerSelected(player, true) }"
                @click="togglePlayerSelection(player, true)"
              >
                <div class="player-info">
                  <h4>{{ player.playerName }} {{ player.playerSurname }}</h4>
                  <p>#{{ player.jerseyNumber || 'N/A' }} - {{ player.positionName || 'N/A' }}</p>
                  <p>Age: {{ player.age || 'N/A' }}</p>
                </div>
                <div class="selection-indicator">
                  <span v-if="isPlayerSelected(player, true)">✓</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Opponent Team -->
          <div class="team-section">
            <div class="team-header">
              <h3>Opponent Team - {{ selectedOpponentPlayers.length }}/{{ opponentTeamPlayers.length }}</h3>
              <div class="team-actions">
                <button class="btn-select-all" @click="selectAllOpponentPlayers">Select All</button>
                <button class="btn-uncheck-all" @click="uncheckAllOpponentPlayers">Uncheck All</button>
              </div>
            </div>
            <div class="players-list">
              <div 
                v-for="player in opponentTeamPlayers" 
                :key="player.playerId"
                class="player-item"
                :class="{ selected: isPlayerSelected(player, false) }"
                @click="togglePlayerSelection(player, false)"
              >
                <div class="player-info">
                  <h4>{{ player.playerName }} {{ player.playerSurname }}</h4>
                  <p>#{{ player.jerseyNumber || 'N/A' }} - {{ player.positionName || 'N/A' }}</p>
                  <p>Age: {{ player.age || 'N/A' }}</p>
                </div>
                <div class="selection-indicator">
                  <span v-if="isPlayerSelected(player, false)">✓</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-actions">
          <div v-if="!canAcceptStartup" class="validation-message">
            <p>⚠️ You must select at least 5 players from each team to proceed</p>
          </div>
          <div class="action-buttons">
            <button 
              class="btn-accept" 
              @click="acceptMatchStartup"
              :disabled="!canAcceptStartup"
              :class="{ disabled: !canAcceptStartup }"
            >
              Accept
            </button>
            <button class="btn-cancel" @click="closeStartupModal">Cancel</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'

const router = useRouter()
const loading = ref(false)
const error = ref(null)
const showStartupModal = ref(false)
const selectedMatch = ref(null)
const ourTeamPlayers = ref([])
const opponentTeamPlayers = ref([])
const selectedOurPlayers = ref([])
const selectedOpponentPlayers = ref([])
const loadingPlayers = ref(false)

const matches = ref({
  liveMatches: [],
  upcomingMatches: [],
  previousMatches: []
})

// Fetch all matches
const fetchMatches = async () => {
  loading.value = true
  error.value = null

  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/match`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })

    const allMatches = response.data.value?.matches || response.data?.matches || []
    
    // Categorize matches based on MatchTracking status
    const now = new Date()

    matches.value = {
      liveMatches: allMatches.filter(match => {
        return match.trackingStatus === 'active'
      }),
      upcomingMatches: allMatches.filter(match => {
        return match.trackingStatus === 'preparation' || match.trackingStatus === 'upcoming' || (!match.trackingStatus && new Date(match.scheduledAt) > now)
      }),
      previousMatches: allMatches.filter(match => {
        return match.trackingStatus === 'finished'
      }).sort((a, b) => new Date(b.scheduledAt) - new Date(a.scheduledAt))
    }
    
    console.log('Categorized matches:', matches.value)
    console.log('Live matches:', matches.value.liveMatches)
    console.log('Upcoming matches:', matches.value.upcomingMatches)
    console.log('Previous matches:', matches.value.previousMatches)
  } catch (err) {
    console.error('Error fetching matches:', err)
    error.value = err.message || 'Failed to fetch matches'
  } finally {
    loading.value = false
  }
}

// Get match result indicator (W/L for previous matches)
const getMatchResult = (match) => {
  if (match.ourPoints !== null && match.opponentPoints !== null) {
    return match.ourPoints > match.opponentPoints ? 'W' : 'L'
  }
  // Fallback for matches without scores
  return Math.random() > 0.5 ? 'W' : 'L'
}

// Get match score display for live and previous matches
const getMatchScore = (match) => {
  if (match.ourPoints !== null && match.opponentPoints !== null) {
    return `${match.ourPoints} - ${match.opponentPoints}`
  }
  return null
}

// Get CSS class for match result
const getMatchResultClass = (match) => {
  const result = getMatchResult(match)
  return result === 'W' ? 'win' : 'loss'
}

// Format date for display
const formatDate = (dateString) => {
  const date = new Date(dateString)
  const day = date.getDate().toString().padStart(2, '0')
  const month = (date.getMonth() + 1).toString().padStart(2, '0')
  const year = date.getFullYear()
  const hours = date.getHours().toString().padStart(2, '0')
  const minutes = date.getMinutes().toString().padStart(2, '0')
  return `${day}.${month}.${year}, ${hours}:${minutes}`
}

// Handle match action based on status
const handleMatchAction = (match) => {
  console.log('handleMatchAction called with match:', match)
  console.log('match.trackingStatus:', match.trackingStatus)
  
  if (match.trackingStatus === 'active' || match.trackingStatus === 'preparation') {
    // Navigate to match detail page
    console.log('Navigating to match detail for match:', match)
    router.push(`/analyst/matches/${match.idMatch}`)
  } else {
    // Open startup modal for upcoming matches
    console.log('Opening startup modal for match:', match)
    openStartupModal(match)
  }
}

// Open startup modal
const openStartupModal = async (match) => {
  console.log('openStartupModal called with match:', match)
  selectedMatch.value = match
  console.log('selectedMatch.value set to:', selectedMatch.value)
  showStartupModal.value = true
  await fetchTeamPlayers()
}

// Close startup modal
const closeStartupModal = () => {
  showStartupModal.value = false
  selectedMatch.value = null
  ourTeamPlayers.value = []
  opponentTeamPlayers.value = []
  selectedOurPlayers.value = []
  selectedOpponentPlayers.value = []
}

// Fetch team players for both teams
const fetchTeamPlayers = async () => {
  if (!selectedMatch.value) return
  
  loadingPlayers.value = true
  try {
    const jwt = localStorage.getItem('token')
    console.log('Fetching players for match:', selectedMatch.value.idTeam)
    // Fetch our team players (team ID = 1)
    const ourTeamResponse = await axios.get(`${MATCHES_URL}/teammember/team/1`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    console.log('Our Team Response:', ourTeamResponse)
    
    // Fetch opponent team players
    const opponentTeamResponse = await axios.get(`${MATCHES_URL}/teammember/team/${selectedMatch.value.idTeam}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    console.log('Opponent Team Response:', opponentTeamResponse)

    ourTeamPlayers.value = ourTeamResponse.data.value?.teamPlayers || []
    opponentTeamPlayers.value = opponentTeamResponse.data.value?.teamPlayers || []
    
    console.log('Our team players:', ourTeamPlayers.value)
    console.log('Opponent team players:', opponentTeamPlayers.value)
    
  } catch (err) {
    console.error('Error fetching team players:', err)
  } finally {
    loadingPlayers.value = false
  }
}

// Toggle player selection
const togglePlayerSelection = (player, isOurTeam) => {
  if (isOurTeam) {
    const index = selectedOurPlayers.value.findIndex(p => p.playerId === player.playerId)
    if (index > -1) {
      selectedOurPlayers.value.splice(index, 1)
    } else {
      selectedOurPlayers.value.push(player)
    }
  } else {
    const index = selectedOpponentPlayers.value.findIndex(p => p.playerId === player.playerId)
    if (index > -1) {
      selectedOpponentPlayers.value.splice(index, 1)
    } else {
      selectedOpponentPlayers.value.push(player)
    }
  }
}

// Check if player is selected
const isPlayerSelected = (player, isOurTeam) => {
  if (isOurTeam) {
    return selectedOurPlayers.value.some(p => p.playerId === player.playerId)
  } else {
    return selectedOpponentPlayers.value.some(p => p.playerId === player.playerId)
  }
}

// Computed property for validation
const canAcceptStartup = computed(() => {
  return selectedOurPlayers.value.length >= 5 && selectedOpponentPlayers.value.length >= 5
})

// Select all our players
const selectAllOurPlayers = () => {
  selectedOurPlayers.value = [...ourTeamPlayers.value]
  console.log('Selected match in selectAllOurPlayers:', selectedMatch.value)
}

// Uncheck all our players  
const uncheckAllOurPlayers = () => {
  selectedOurPlayers.value = []
}

// Select all opponent players
const selectAllOpponentPlayers = () => {
  selectedOpponentPlayers.value = [...opponentTeamPlayers.value]
}

// Uncheck all opponent players
const uncheckAllOpponentPlayers = () => {
  selectedOpponentPlayers.value = []
}

// Accept match startup
const acceptMatchStartup = async () => {
  if (!canAcceptStartup.value) {
    return
  }
  console.log('selectedMatchBRAAAAAAAAAAAAAAAAA: ', selectedMatch.value)
  var SelectedMatchId = selectedMatch.value.idMatch
  try {
    const prepareMatchRequest = {
      ourTeamPlayerIds: selectedOurPlayers.value.map(p => p.playerId),
      opponentTeamPlayerIds: selectedOpponentPlayers.value.map(p => p.playerId),
      analystId: 4 // Current user ID (analyst) hardcoded to 4 for now
    }

    await axios.post(`${MATCHES_URL}/match/${selectedMatch.value.idMatch}/prepare`, prepareMatchRequest)
    
    // Close modal
    closeStartupModal()
    console.log('Match prepared successfully, selectedMatch.value: ', selectedMatch.value)
    // Navigate to match detail page
    router.push(`/analyst/matches/${SelectedMatchId}`)
  } catch (err) {
    console.error('Error preparing match:', err)
    error.value = err.response?.data?.message || 'Failed to prepare match'
  }
}

onMounted(() => {
  fetchMatches()
})
</script>

<style scoped>
.matches-overview {
  min-height: 100vh;
  background-color: #f8f9fa;
}

/* Main Content */
.main-content {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.content-header h1 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
}

/* Loading and Error States */
.loading-state, .error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  text-align: center;
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.btn-primary {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-primary:hover {
  background-color: #0056b3;
}

/* Matches Container */
.matches-container {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.matches-section {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.matches-section h2 {
  font-size: 1.3rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  color: #333;
}

.live-matches h2 {
  color: #dc3545;
}

.matches-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1rem;
}

.no-matches {
  text-align: center;
  padding: 2rem;
  color: #666;
  font-style: italic;
}

/* Match Cards */
.match-card {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
  border: none;
  border-radius: 16px;
  padding: 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 4px 12px rgba(0,0,0,0.08);
  position: relative;
  overflow: hidden;
}

.match-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 4px;
  height: 100%;
  background: linear-gradient(180deg, #007bff 0%, #0056b3 100%);
  transition: width 0.3s ease;
}

.match-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.15);
}

.match-card:hover::before {
  width: 8px;
}

.match-card.live {
  background: linear-gradient(135deg, #fff5f5 0%, #ffe6e6 100%);
  box-shadow: 0 4px 16px rgba(220, 53, 69, 0.2);
}

.match-card.live::before {
  background: linear-gradient(180deg, #dc3545 0%, #c82333 100%);
}

.match-card.upcoming {
  background: linear-gradient(135deg, #f0f8ff 0%, #e6f3ff 100%);
  box-shadow: 0 4px 16px rgba(0, 123, 255, 0.2);
}

.match-card.upcoming::before {
  background: linear-gradient(180deg, #007bff 0%, #0056b3 100%);
}

.match-card.previous.win {
  background: linear-gradient(135deg, #f0fff4 0%, #e6ffe6 100%);
  box-shadow: 0 4px 16px rgba(40, 167, 69, 0.2);
}

.match-card.clickable {
  cursor: pointer;
  user-select: none;
}

.match-card.clickable:hover {
  transform: translateY(-6px);
  box-shadow: 0 12px 32px rgba(0,0,0,0.2);
}

.match-card.clickable.disabled {
  cursor: not-allowed;
  opacity: 0.7;
}

.match-card.clickable.disabled:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(0,0,0,0.1);
}

.match-status {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.status-badge {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.status-badge.live {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(220, 53, 69, 0.3);
}

.click-indicator {
  font-size: 1.5rem;
  font-weight: bold;
  color: #dc3545;
  transition: transform 0.3s ease;
}

.match-card.clickable:hover .click-indicator {
  transform: translateX(4px);
}

.match-card.previous.win::before {
  background: linear-gradient(180deg, #28a745 0%, #1e7e34 100%);
}

.match-card.previous.loss {
  background: linear-gradient(135deg, #fff5f5 0%, #ffe6e6 100%);
  box-shadow: 0 4px 16px rgba(220, 53, 69, 0.2);
}

.match-card.previous.loss::before {
  background: linear-gradient(180deg, #dc3545 0%, #c82333 100%);
}

.match-info {
  flex: 1;
  position: relative;
}

.match-info h3 {
  font-size: 1.1rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  color: #2c3e50;
  text-shadow: 0 1px 2px rgba(0,0,0,0.05);
}

.match-date, .match-place {
  font-size: 0.9rem;
  color: #6c757d;
  margin: 0.3rem 0;
  font-weight: 500;
}

.match-score {
  font-size: 1rem;
  font-weight: 700;
  margin: 0.4rem 0;
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.live-score {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
  color: white;
}

.final-score {
  background: linear-gradient(135deg, #28a745 0%, #1e7e34 100%);
  color: white;
}

.result-indicator {
  position: absolute;
  top: -5px;
  left: -5px;
  width: 25px;
  height: 25px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 0.8rem;
  color: white;
}

.match-card.win .result-indicator {
  background-color: #28a745;
}

.match-card.loss .result-indicator {
  background-color: #dc3545;
}

/* Match Action Buttons */
.match-action {
  padding: 10px 20px;
  border: none;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.match-action.start-up {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
  color: white;
}

.match-action.start-up:hover {
  background: linear-gradient(135deg, #0056b3 0%, #004494 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 123, 255, 0.3);
}

.match-action.report {
  background: linear-gradient(135deg, #6c757d 0%, #545b62 100%);
  color: white;
}

.match-action.report:hover {
  background: linear-gradient(135deg, #545b62 0%, #3d4449 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(108, 117, 125, 0.3);
}

/* Modal Styles */
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
  border-radius: 12px;
  padding: 2rem;
  max-width: 800px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.modal-content h2 {
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
  text-align: center;
}

.loading-players {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.teams-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin-bottom: 2rem;
}

.team-section h3 {
  margin-bottom: 1rem;
  font-size: 1.2rem;
  font-weight: 600;
  color: #333;
  text-align: center;
  border-bottom: 2px solid #007bff;
  padding-bottom: 0.5rem;
}

.team-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 1rem;
}

.team-header h3 {
  margin-bottom: 0.5rem;
  border-bottom: 2px solid #007bff;
  padding-bottom: 0.5rem;
}

.team-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.btn-select-all,
.btn-uncheck-all {
  padding: 0.3rem 0.8rem;
  border: none;
  border-radius: 4px;
  font-size: 0.8rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-select-all {
  background-color: #28a745;
  color: white;
}

.btn-select-all:hover {
  background-color: #218838;
}

.btn-uncheck-all {
  background-color: #6c757d;
  color: white;
}

.btn-uncheck-all:hover {
  background-color: #5a6268;
}

.players-list {
  max-height: 400px;
  overflow-y: auto;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 0.5rem;
}

.player-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  margin-bottom: 0.5rem;
  border: 1px solid #e0e0e0;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.player-item:hover {
  background-color: #f8f9fa;
  border-color: #007bff;
}

.player-item.selected {
  background-color: #e7f3ff;
  border-color: #007bff;
  border-width: 2px;
}

.player-info h4 {
  margin: 0 0 0.25rem 0;
  font-size: 0.9rem;
  font-weight: 600;
  color: #333;
}

.player-info p {
  margin: 0;
  font-size: 0.8rem;
  color: #666;
}

.selection-indicator {
  width: 24px;
  height: 24px;
  border: 2px solid #007bff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  color: #007bff;
  transition: all 0.2s ease;
}

.player-item.selected .selection-indicator {
  background-color: #007bff;
  color: white;
}

.modal-actions {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.validation-message {
  background-color: #fff3cd;
  border: 1px solid #ffeaa7;
  border-radius: 6px;
  padding: 0.75rem 1rem;
  color: #856404;
  text-align: center;
  width: 100%;
}

.validation-message p {
  margin: 0;
  font-size: 0.9rem;
  font-weight: 500;
}

.modal-actions .btn-accept,
.modal-actions .btn-cancel {
  min-width: 120px;
}

.action-buttons {
  display: flex;
  justify-content: center;
  gap: 1rem;
}

.btn-accept {
  background-color: #28a745;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-accept:hover:not(.disabled) {
  background-color: #218838;
}

.btn-accept.disabled {
  background-color: #6c757d;
  cursor: not-allowed;
  opacity: 0.6;
}

.btn-cancel {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-cancel:hover {
  background-color: #5a6268;
}

/* Responsive Design */
@media (max-width: 768px) {
  .main-content {
    padding: 1rem;
  }

  .matches-grid {
    grid-template-columns: 1fr;
  }

  .match-card {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }

  .match-info {
    margin-bottom: 1rem;
  }

  .teams-container {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .modal-content {
    padding: 1.5rem;
    margin: 1rem;
  }

  .modal-actions {
    flex-direction: column;
  }
}
</style>
