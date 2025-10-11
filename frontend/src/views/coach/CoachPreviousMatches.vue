<template>
  <div class="coach-previous-matches">
    <!-- Main Content -->
    <main class="main-content">
      <div class="content-header">
        <h1>Previous Matches</h1>
      </div>

      <div v-if="loading" class="loading-state">
        <p>Loading matches...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <p>Error loading matches: {{ error }}</p>
        <button @click="fetchMatches" class="btn-primary">Try Again</button>
      </div>

      <div v-else class="matches-container">
        <!-- Previous Matches -->
        <div class="matches-section previous-matches">
          <h2>Finished Matches</h2>
          <div class="matches-list">
            <div v-if="previousMatches.length === 0" class="no-matches">
              <p>No finished matches.</p>
            </div>
            <div v-else class="matches-grid">
              <div 
                v-for="match in previousMatches" 
                :key="match.idMatch"
                class="match-card previous clickable"
                :class="getMatchResultClass(match)"
                @click="viewMatchDetails(match)"
              >
                <div class="match-info">
                  <div class="result-indicator">{{ getMatchResult(match) }}</div>
                  <h3>{{ match.name }}</h3>
                  <p class="match-date">Date: {{ formatDate(match.scheduledAt) }}</p>
                  <p class="match-place">Location: {{ match.type === 'home' ? 'Home' : 'Away' }}</p>
                  <div v-if="getMatchScore(match)" class="match-score final-score">
                    Score: {{ getMatchScore(match) }}
                  </div>
                </div>
                <div class="match-action-indicator">
                  <span class="view-details">View Details →</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'

const router = useRouter()
const loading = ref(false)
const error = ref(null)
const previousMatches = ref([])

// Fetch all matches and filter only finished ones
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
    
    // Filter only finished matches
    previousMatches.value = allMatches
      .filter(match => match.trackingStatus === 'finished')
      .sort((a, b) => new Date(b.scheduledAt) - new Date(a.scheduledAt))
    
    console.log('Previous matches loaded:', previousMatches.value)
  } catch (err) {
    console.error('Error fetching matches:', err)
    error.value = err.message || 'Error loading matches'
  } finally {
    loading.value = false
  }
}

// Get match result indicator (W/L for previous matches)
const getMatchResult = (match) => {
  if (match.ourPoints !== null && match.opponentPoints !== null) {
    return match.ourPoints > match.opponentPoints ? 'W' : 'L'
  }
  return 'N/A'
}

// Get match score display
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

// View match details
const viewMatchDetails = (match) => {
  console.log('Viewing match details:', match)
  router.push(`/coach/matches/${match.idMatch}/previous`)
}

onMounted(() => {
  fetchMatches()
})
</script>

<style scoped>
.coach-previous-matches {
  min-height: 100vh;
  background-color: #f8f9fa;
}

/* Main Content */
.main-content {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.content-header h1 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin: 0;
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
  margin-left: 1.5rem;
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
  display: inline-block;
  text-align: center;
  font-size: 1rem;
  font-weight: 700;
  margin: auto;
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.final-score {
  background: linear-gradient(135deg, #28a745 0%, #1e7e34 100%);
  color: white;
}

/* When the previous match is a loss, make the final score badge red to indicate loss */
.match-card.previous.loss .final-score {
  background: linear-gradient(135deg, #ff6b6b 0%, #e53935 100%);
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

/* Match Action Indicator for Previous Matches */
.match-action-indicator {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px 20px;
}

.view-details {
  color: #007bff;
  font-size: 14px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
}

.match-card.clickable:hover .view-details {
  color: #0056b3;
  transform: translateX(4px);
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
}
</style>
