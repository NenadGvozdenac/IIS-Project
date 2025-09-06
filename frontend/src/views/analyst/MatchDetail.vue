<template>
  <div class="match-detail">
    <div v-if="loading" class="loading">
      <p>Loading match details...</p>
    </div>
    
    <div v-else-if="error" class="error">
      <p>{{ error }}</p>
      <button @click="$router.go(-1)" class="btn-back">Go Back</button>
    </div>
    
    <div v-else-if="match" class="match-content">
      <!-- Match Header -->
      <div class="match-header">
        <button @click="$router.go(-1)" class="btn-back">← Back to Matches</button>
        <h1>{{ match.name }}</h1>
        <div class="match-status">
          <span class="status-badge" :class="match.trackingStatus">
            {{ formatStatus(match.trackingStatus) }}
          </span>
        </div>
      </div>

      <!-- Match Info -->
      <div class="match-info-section">
        <div class="info-grid">
          <div class="info-item">
            <label>Date & Time:</label>
            <span>{{ formatDateTime(match.scheduledAt) }}</span>
          </div>
          <div class="info-item">
            <label>Location:</label>
            <span>{{ match.isInOurHall ? 'Home' : 'Away' }} - {{ match.hall }}, {{ match.city }}</span>
          </div>
          <div class="info-item">
            <label>Competition:</label>
            <span>{{ match.competition || 'N/A' }}</span>
          </div>
          <div class="info-item">
            <label>Opponent:</label>
            <span>{{ match.opponentTeam || 'N/A' }}</span>
          </div>
        </div>
      </div>

      <!-- Score Section (only for active/finished matches) -->
      <div v-if="matchTracking && (matchTracking.trackingStatus === 'active' || matchTracking.trackingStatus === 'finished')" class="score-section">
        <div class="scoreboard">
          <div class="team-score our-team">
            <h3>Partizan</h3>
            <div class="score">{{ matchTracking.ourPoints || 0 }}</div>
          </div>
          <div class="vs-separator">VS</div>
          <div class="team-score opponent-team">
            <h3>{{ match.opponentTeam || 'Opponent' }}</h3>
            <div class="score">{{ matchTracking.opponentPoints || 0 }}</div>
          </div>
        </div>
        
        <div v-if="matchTracking.trackingStatus === 'active'" class="game-time">
          <div class="period">{{ matchTracking.currentPeriod || 'Period 1' }}</div>
          <div class="time">{{ formatGameTime(matchTracking.elapsedPeriodTime) }}</div>
          <div class="status">{{ formatPeriodStatus(matchTracking.periodStatus) }}</div>
        </div>
      </div>

      <!-- Team Rosters -->
      <div class="teams-section">
        <h2>Team Rosters</h2>
        <div class="teams-grid">
          <!-- Our Team -->
          <div class="team-roster">
            <h3>Our Team - Partizan</h3>
            <div v-if="ourTeamPlayers.length === 0" class="no-players">
              <p>No players selected yet</p>
            </div>
            <div v-else class="players-list">
              <div 
                v-for="player in ourTeamPlayers" 
                :key="player.playerId"
                class="player-card"
                :class="{ 
                  'starting-lineup': player.startingLineup,
                  'in-game': player.inGame 
                }"
              >
                <div class="player-info">
                  <div class="player-name">{{ player.playerName }} {{ player.playerSurname }}</div>
                  <div class="player-details">
                    <span class="jersey">#{{ player.jerseyNumber || 'N/A' }}</span>
                    <span class="position">{{ player.positionName || 'N/A' }}</span>
                  </div>
                </div>
                <div class="player-status">
                  <span v-if="player.startingLineup" class="badge starting">Starting</span>
                  <span v-if="player.inGame" class="badge playing">Playing</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Opponent Team -->
          <div class="team-roster">
            <h3>Opponent Team</h3>
            <div v-if="opponentTeamPlayers.length === 0" class="no-players">
              <p>No players selected yet</p>
            </div>
            <div v-else class="players-list">
              <div 
                v-for="player in opponentTeamPlayers" 
                :key="player.playerId"
                class="player-card"
                :class="{ 
                  'starting-lineup': player.startingLineup,
                  'in-game': player.inGame 
                }"
              >
                <div class="player-info">
                  <div class="player-name">{{ player.playerName }} {{ player.playerSurname }}</div>
                  <div class="player-details">
                    <span class="jersey">#{{ player.jerseyNumber || 'N/A' }}</span>
                    <span class="position">{{ player.positionName || 'N/A' }}</span>
                  </div>
                </div>
                <div class="player-status">
                  <span v-if="player.startingLineup" class="badge starting">Starting</span>
                  <span v-if="player.inGame" class="badge playing">Playing</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Match Actions -->
      <div v-if="match.trackingStatus === 'preparation'" class="match-actions">
        <button class="btn-primary btn-start-match" @click="startMatch">
          Start Match
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const error = ref(null)
const match = ref(null)
const matchTracking = ref(null)
const ourTeamPlayers = ref([])
const opponentTeamPlayers = ref([])

// Get match details
const fetchMatchDetails = async () => {
  try {
    loading.value = true
    const matchId = route.params.id
    
    // Fetch match basic info
    const matchResponse = await axios.get(`${MATCHES_URL}/match/${matchId}`)
    match.value = matchResponse.data.value
    console.log('match.value:', matchResponse.data);
    
    // Fetch match tracking info
    try {
      const trackingResponse = await axios.get(`${MATCHES_URL}/match/${matchId}/tracking`)
      matchTracking.value = trackingResponse.data.data
    } catch (trackingError) {
      console.log('No tracking data available yet')
    }
    
    // Fetch team players if match is in preparation or later
    if (match.value.trackingStatus === 'preparation' || 
        match.value.trackingStatus === 'active' || 
        match.value.trackingStatus === 'finished') {
      //await fetchTeamPlayers(matchId)
    }
    
  } catch (err) {
    console.error('Error fetching match details:', err)
    error.value = err.response?.data?.message || 'Failed to load match details'
  } finally {
    loading.value = false
  }
}

// Fetch team players for this match
const fetchTeamPlayers = async (matchId) => {
  try {
    // This would be a new endpoint to get team players for a specific match
    // For now, we'll simulate this or use existing endpoints
    const response = await axios.get(`${MATCHES_URL}/${matchId}/players`)
    
    // Separate our team and opponent team players
    ourTeamPlayers.value = response.data.data.filter(p => p.isOurTeam)
    opponentTeamPlayers.value = response.data.data.filter(p => !p.isOurTeam)
  } catch (err) {
    console.log('Could not fetch team players:', err)
  }
}

// Utility functions
const formatStatus = (status) => {
  const statusMap = {
    'upcoming': 'Upcoming',
    'preparation': 'In Preparation', 
    'active': 'Live',
    'finished': 'Finished'
  }
  return statusMap[status] || status
}

const formatDateTime = (dateTime) => {
  return new Date(dateTime).toLocaleString()
}

const formatGameTime = (seconds) => {
  if (!seconds) return '00:00'
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`
}

const formatPeriodStatus = (status) => {
  const statusMap = {
    'active': 'Playing',
    'paused': 'Paused',
    'break': 'Break'
  }
  return statusMap[status] || status
}

onMounted(() => {
  fetchMatchDetails()
})
</script>

<style scoped>
.match-detail {
  min-height: 100vh;
  background-color: #f8f9fa;
  padding: 2rem;
}

.loading, .error {
  text-align: center;
  padding: 3rem;
}

.error {
  color: #dc3545;
}

.btn-back {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  margin-bottom: 1rem;
}

.btn-back:hover {
  background-color: #5a6268;
}

.match-header {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.match-header h1 {
  margin: 0.5rem 0;
  color: #333;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.875rem;
  font-weight: 500;
}

.status-badge.upcoming {
  background-color: #e3f2fd;
  color: #1976d2;
}

.status-badge.preparation {
  background-color: #fff3e0;
  color: #f57c00;
}

.status-badge.active {
  background-color: #e8f5e8;
  color: #2e7d32;
}

.status-badge.finished {
  background-color: #f3e5f5;
  color: #7b1fa2;
}

.match-info-section {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.info-item {
  display: flex;
  flex-direction: column;
}

.info-item label {
  font-weight: 600;
  color: #666;
  margin-bottom: 0.25rem;
}

.score-section {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  margin-bottom: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  text-align: center;
}

.scoreboard {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 2rem;
  margin-bottom: 1rem;
}

.team-score h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.team-score .score {
  font-size: 3rem;
  font-weight: bold;
  color: #007bff;
}

.vs-separator {
  font-size: 1.5rem;
  font-weight: bold;
  color: #666;
}

.game-time {
  display: flex;
  justify-content: center;
  gap: 2rem;
  align-items: center;
}

.game-time .period {
  font-weight: 600;
  color: #333;
}

.game-time .time {
  font-size: 1.5rem;
  font-weight: bold;
  color: #007bff;
}

.teams-section {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.teams-section h2 {
  margin-top: 0;
  margin-bottom: 1.5rem;
  color: #333;
}

.teams-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.team-roster h3 {
  margin-top: 0;
  margin-bottom: 1rem;
  color: #333;
  border-bottom: 2px solid #007bff;
  padding-bottom: 0.5rem;
}

.no-players {
  text-align: center;
  color: #666;
  font-style: italic;
  padding: 2rem;
}

.players-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.player-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 6px;
  background-color: #fafafa;
}

.player-card.starting-lineup {
  border-color: #28a745;
  background-color: #f8fff8;
}

.player-card.in-game {
  border-color: #007bff;
  background-color: #f0f8ff;
}

.player-name {
  font-weight: 600;
  color: #333;
}

.player-details {
  margin-top: 0.25rem;
  color: #666;
  font-size: 0.875rem;
}

.jersey {
  margin-right: 0.5rem;
}

.player-status {
  display: flex;
  gap: 0.5rem;
}

.badge {
  padding: 0.25rem 0.5rem;
  border-radius: 10px;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge.starting {
  background-color: #d4edda;
  color: #155724;
}

.badge.playing {
  background-color: #cce5ff;
  color: #004085;
}

.match-actions {
  text-align: center;
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.btn-primary {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 0.75rem 2rem;
  border-radius: 6px;
  font-size: 1.1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-primary:hover {
  background-color: #0056b3;
}

@media (max-width: 768px) {
  .teams-grid {
    grid-template-columns: 1fr;
  }
  
  .scoreboard {
    flex-direction: column;
    gap: 1rem;
  }
  
  .game-time {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
