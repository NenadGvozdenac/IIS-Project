<template>
  <div class="previous-match-detail">
    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <p>Loading match details...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error">
      <p>Error: {{ error }}</p>
      <button @click="fetchMatchDetails" class="btn-primary">Retry</button>
    </div>

    <!-- Match Content -->
    <div v-else-if="match" class="match-content">
      <!-- Combined Match Header and Score Section -->
      <div class="combined-match-section">
        <!-- Back Button positioned absolutely -->
        <button @click="goBack" class="btn-back-absolute">
          ← Back to Matches
        </button>
        
        <!-- Download Report Button for finished matches -->
        <button  
          @click="downloadMatchReport" 
          class="btn-download-report-absolute"
        >
          📊 Download Report
        </button>
        
        <!-- Centered content -->
        <div class="centered-match-content">
          <!-- Match Header Info -->
          <div class="match-header-info">
            <h1>Match Details</h1>
            <div class="match-time-location">
              <div class="time-info">{{ formatMatchTime() }}</div>
              <div class="location-info">{{ match.hall }} - {{ match.state }}, {{ match.city }}</div>
            </div>
          </div>

          <!-- Final Score Display -->
          <div class="score-section">
            <div class="score-container">
              <div class="team-score our-team">
                <div class="team-name">KK Partizan</div>
                <div class="score">{{ match.ourPoints || 0 }}</div>
              </div>
              
              <div class="vs-separator">
                <div class="final-badge">FINAL</div>
              </div>
              
              <div class="team-score opponent-team">
                <div class="team-name">{{ opponentTeam }}</div>
                <div class="score">{{ match.opponentPoints || 0 }}</div>
              </div>
            </div>
            
            <div class="match-result">
              <span :class="['result-badge', getMatchResultClass(match)]">
                {{ getMatchResult(match) === 'W' ? 'WIN' : 'LOSS' }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Team Statistics Section -->
      <div class="team-stats-section">
        <h2>Team Stats</h2>
        
        <div class="team-stats-comparison">
          <div class="team-stats-header">
            <div class="team-column">
              <h3>Partizan</h3>
            </div>
            <div class="stats-labels">
              <div class="stat-label-header">Statistics</div>
            </div>
            <div class="team-column">
              <h3>{{ opponentTeam }}</h3>
            </div>
          </div>
          
          <div class="stats-comparison-grid">
            <!-- Field Goals -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="made-attempts">{{ ourTeamStats.fieldGoals.made }}/{{ ourTeamStats.fieldGoals.attempts }}</span>
                <span class="percentage">{{ ourTeamStats.fieldGoals.percentage }}%</span>
              </div>
              <div class="stat-label">FG</div>
              <div class="team-stat-value opponent-team">
                <span class="made-attempts">{{ opponentTeamStats.fieldGoals.made }}/{{ opponentTeamStats.fieldGoals.attempts }}</span>
                <span class="percentage">{{ opponentTeamStats.fieldGoals.percentage }}%</span>
              </div>
            </div>
            
            <!-- 2-Point Field Goals -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="made-attempts">{{ ourTeamStats.twoPointers.made }}/{{ ourTeamStats.twoPointers.attempts }}</span>
                <span class="percentage">{{ ourTeamStats.twoPointers.percentage }}%</span>
              </div>
              <div class="stat-label">2P</div>
              <div class="team-stat-value opponent-team">
                <span class="made-attempts">{{ opponentTeamStats.twoPointers.made }}/{{ opponentTeamStats.twoPointers.attempts }}</span>
                <span class="percentage">{{ opponentTeamStats.twoPointers.percentage }}%</span>
              </div>
            </div>
            
            <!-- 3-Point Field Goals -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="made-attempts">{{ ourTeamStats.threePointers.made }}/{{ ourTeamStats.threePointers.attempts }}</span>
                <span class="percentage">{{ ourTeamStats.threePointers.percentage }}%</span>
              </div>
              <div class="stat-label">3P</div>
              <div class="team-stat-value opponent-team">
                <span class="made-attempts">{{ opponentTeamStats.threePointers.made }}/{{ opponentTeamStats.threePointers.attempts }}</span>
                <span class="percentage">{{ opponentTeamStats.threePointers.percentage }}%</span>
              </div>
            </div>
            
            <!-- Free Throws -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="made-attempts">{{ ourTeamStats.freeThrows.made }}/{{ ourTeamStats.freeThrows.attempts }}</span>
                <span class="percentage">{{ ourTeamStats.freeThrows.percentage }}%</span>
              </div>
              <div class="stat-label">FT</div>
              <div class="team-stat-value opponent-team">
                <span class="made-attempts">{{ opponentTeamStats.freeThrows.made }}/{{ opponentTeamStats.freeThrows.attempts }}</span>
                <span class="percentage">{{ opponentTeamStats.freeThrows.percentage }}%</span>
              </div>
            </div>
            
            <!-- Rebounds -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="made-attempts">{{ ourTeamStats.rebounds.offensive }}/{{ ourTeamStats.rebounds.defensive }}</span>
                <span class="total">{{ ourTeamStats.rebounds.total }}</span>
              </div>
              <div class="stat-label">REB O/D</div>
              <div class="team-stat-value opponent-team">
                <span class="made-attempts">{{ opponentTeamStats.rebounds.offensive }}/{{ opponentTeamStats.rebounds.defensive }}</span>
                <span class="total">{{ opponentTeamStats.rebounds.total }}</span>
              </div>
            </div>
            
            <!-- Assists -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="total">{{ ourTeamStats.assists }}</span>
              </div>
              <div class="stat-label">AST</div>
              <div class="team-stat-value opponent-team">
                <span class="total">{{ opponentTeamStats.assists }}</span>
              </div>
            </div>
            
            <!-- Steals -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="total">{{ ourTeamStats.steals }}</span>
              </div>
              <div class="stat-label">STL</div>
              <div class="team-stat-value opponent-team">
                <span class="total">{{ opponentTeamStats.steals }}</span>
              </div>
            </div>
            
            <!-- Blocks -->
            <div class="stat-comparison-row">
              <div class="team-stat-value our-team">
                <span class="total">{{ ourTeamStats.blocks }}</span>
              </div>
              <div class="stat-label">BLK</div>
              <div class="team-stat-value opponent-team">
                <span class="total">{{ opponentTeamStats.blocks }}</span>
              </div>
            </div>
            
            <!-- Points -->
            <div class="stat-comparison-row points-row">
              <div class="team-stat-value our-team">
                <span class="total points">{{ ourTeamStats.points }}</span>
              </div>
              <div class="stat-label">PTS</div>
              <div class="team-stat-value opponent-team">
                <span class="total points">{{ opponentTeamStats.points }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Player Statistics Section -->
      <div class="player-stats-section">
        <h2>Player Stats</h2>
        
        <!-- Our Team Players -->
        <div class="team-player-stats">
          <h3>Partizan</h3>
          <div class="stats-table-wrapper">
            <table class="stats-table modern-stats">
              <thead>
                <tr>
                  <th class="player-header"># PLAYER</th>
                  <th class="eff-header">EFF</th>
                  <th class="fg-header">FG</th>
                  <th class="twoP-header">2P</th>
                  <th class="threeP-header">3P</th>
                  <th class="ft-header">FT</th>
                  <th class="reb-header">REB</th>
                  <th class="ast-header">AST</th>
                  <th class="stl-header">STL</th>
                  <th class="blk-header">BLK</th>
                  <th class="pts-header">PTS</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="player in ourTeamPlayers" :key="player.playerId">
                  <td class="player-info-cell">
                    <div class="player-number">{{ player.jerseyNumber }}</div>
                    <div class="player-details">
                      <div class="player-name">{{ player.firstName }} {{ player.lastName }}</div>
                      <div class="foul-dots">
                        <span 
                          v-for="foul in player.fouls" 
                          :key="foul" 
                          class="foul-dot"
                        >●</span>
                      </div>
                    </div>
                  </td>
                  <td class="efficiency-cell" :class="{ 'negative-eff': (player.efficiency || 0) < 0 }">{{ player.efficiency || 0 }}</td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.fieldGoals.made }}/{{ player.fieldGoals.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.fieldGoals.made, player.fieldGoals.attempts) }}%</div>
                  </td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.twoPointers.made }}/{{ player.twoPointers.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.twoPointers.made, player.twoPointers.attempts) }}%</div>
                  </td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.threePointers.made }}/{{ player.threePointers.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.threePointers.made, player.threePointers.attempts) }}%</div>
                  </td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.freeThrows.made }}/{{ player.freeThrows.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.freeThrows.made, player.freeThrows.attempts) }}%</div>
                  </td>
                  <td class="reb-cell">
                    <div class="reb-total">{{ (player.rebounds.offensive || 0) + (player.rebounds.defensive || 0) }}</div>
                    <div class="reb-breakdown">{{ player.rebounds.offensive || 0 }} {{ player.rebounds.defensive || 0 }}</div>
                  </td>
                  <td class="simple-stat">{{ player.assists }}</td>
                  <td class="simple-stat">{{ player.steals }}</td>
                  <td class="simple-stat">{{ player.blocks }}</td>
                  <td class="points-cell">{{ player.points }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Opponent Team Players -->
        <div class="team-player-stats">
          <h3>{{ opponentTeam }}</h3>
          <div class="stats-table-wrapper">
            <table class="stats-table modern-stats">
              <thead>
                <tr>
                  <th class="player-header"># PLAYER</th>
                  <th class="eff-header">EFF</th>
                  <th class="fg-header">FG</th>
                  <th class="twoP-header">2P</th>
                  <th class="threeP-header">3P</th>
                  <th class="ft-header">FT</th>
                  <th class="reb-header">REB</th>
                  <th class="ast-header">AST</th>
                  <th class="stl-header">STL</th>
                  <th class="blk-header">BLK</th>
                  <th class="pts-header">PTS</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="player in opponentTeamPlayers" :key="player.playerId">
                  <td class="player-info-cell">
                    <div class="player-number">{{ player.jerseyNumber }}</div>
                    <div class="player-details">
                      <div class="player-name">{{ player.firstName }} {{ player.lastName }}</div>
                      <div class="foul-dots">
                        <span 
                          v-for="foul in player.fouls" 
                          :key="foul" 
                          class="foul-dot"
                        >●</span>
                      </div>
                    </div>
                  </td>
                  <td class="efficiency-cell" :class="{ 'negative-eff': (player.efficiency || 0) < 0 }">{{ player.efficiency || 0 }}</td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.fieldGoals.made }}/{{ player.fieldGoals.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.fieldGoals.made, player.fieldGoals.attempts) }}%</div>
                  </td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.twoPointers.made }}/{{ player.twoPointers.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.twoPointers.made, player.twoPointers.attempts) }}%</div>
                  </td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.threePointers.made }}/{{ player.threePointers.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.threePointers.made, player.threePointers.attempts) }}%</div>
                  </td>
                  <td class="stat-cell">
                    <div class="stat-made-attempts">{{ player.freeThrows.made }}/{{ player.freeThrows.attempts }}</div>
                    <div class="stat-percentage">{{ calculatePercentage(player.freeThrows.made, player.freeThrows.attempts) }}%</div>
                  </td>
                  <td class="reb-cell">
                    <div class="reb-total">{{ (player.rebounds.offensive || 0) + (player.rebounds.defensive || 0) }}</div>
                    <div class="reb-breakdown">{{ player.rebounds.offensive || 0 }} {{ player.rebounds.defensive || 0 }}</div>
                  </td>
                  <td class="simple-stat">{{ player.assists }}</td>
                  <td class="simple-stat">{{ player.steals }}</td>
                  <td class="simple-stat">{{ player.blocks }}</td>
                  <td class="points-cell">{{ player.points }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Advanced Analytics Loading -->
      <div v-if="advancedAnalyticsLoading" class="advanced-analytics-loading">
        <div class="loading-content">
          <div class="spinner"></div>
          <p>Loading advanced analytics...</p>
        </div>
      </div>

      <!-- Automatic Recommendations Section -->
      <section class="recommendations-section">
        <h2>Automatic Recommendations</h2>
        
        <div v-if="loadingRecommendations" class="loading-recommendations">
          <p>Loading recommendations...</p>
        </div>

        <div v-else-if="recommendations.length === 0" class="no-recommendations">
          <p>No recommendations for this match.</p>
        </div>

        <div v-else class="recommendations-list">
          <div 
            v-for="recommendation in recommendations" 
            :key="recommendation.idRecommendation"
            class="recommendation-card"
            :class="[
              `priority-${recommendation.priority.toLowerCase()}`,
              `status-${recommendation.status.toLowerCase()}`
            ]"
          >
            <div class="recommendation-header">
              <div class="recommendation-type">
                <span class="type-icon">{{ getRecommendationIcon(recommendation.type) }}</span>
                <span class="type-label">{{ recommendation.type }}</span>
              </div>
              <div class="recommendation-meta">
                <span class="priority-badge" :class="`priority-${recommendation.priority.toLowerCase()}`">
                  {{ recommendation.priority }}
                </span>
                <span class="status-badge" :class="`status-${recommendation.status.toLowerCase()}`">
                  {{ recommendation.status }}
                </span>
              </div>
            </div>
            
            <div class="recommendation-body">
              <p class="recommendation-description">{{ recommendation.description }}</p>
            </div>

            <div class="recommendation-footer">
              <span class="recommendation-time">
                {{ recommendation.period }} - {{ formatPeriodTime(recommendation.periodTime) }}
              </span>
              <span class="recommendation-created">
                {{ formatCreationTime(recommendation.creationTime) }}
              </span>
            </div>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'
import matchReportService from '../../services/matchReportService'
import advancedMatchReportService from '../../services/advancedMatchReportService'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const error = ref(null)
const match = ref(null)
const opponentTeam = ref('Unknown Opponent')
const ourTeamPlayers = ref([])
const opponentTeamPlayers = ref([])
const recommendations = ref([])
const loadingRecommendations = ref(false)

// Advanced analytics data
const topPlayers = ref([])
const periodStatistics = ref([])
const advancedAnalyticsLoading = ref(false)

// Team statistics computed from player data
const ourTeamStats = computed(() => {
  return calculateTeamStats(ourTeamPlayers.value)
})

const opponentTeamStats = computed(() => {
  return calculateTeamStats(opponentTeamPlayers.value)
})

// Format period statistics for table display
const formattedPeriodStats = computed(() => {
  const periods = ['1', '2', '3', '4']
  const teams = [
    { id: '1', name: 'KK Partizan', class: 'our-team' },
    { id: '2', name: opponentTeam, class: 'opponent-team' }
  ]
  
  return teams.map(team => {
    const teamRow = {
      teamId: team.id,
      teamName: team.name,
      teamClass: team.class,
      periods: {}
    }
    
    periods.forEach(period => {
      const periodStats = periodStatistics.value.filter(stat => 
        stat.period === period && stat.teamId === team.id
      )
      
      teamRow.periods[period] = {
        '+2p': periodStats.find(s => s.eventType === '+2p')?.count || 0,
        '+3p': periodStats.find(s => s.eventType === '+3p')?.count || 0,
        '+ft': periodStats.find(s => s.eventType === '+ft')?.count || 0,
        'foul': periodStats.find(s => s.eventType === 'foul')?.count || 0,
        'assist': periodStats.find(s => s.eventType === 'assist')?.count || 0
      }
    })
    
    return teamRow
  })
})

// Get player name by ID (helper function)
const getPlayerName = (playerId) => {
  // First try our team players
  const ourPlayer = ourTeamPlayers.value.find(p => p.id?.toString() === playerId || p.playerId?.toString() === playerId)
  if (ourPlayer) {
    return `${ourPlayer.firstName || ''} ${ourPlayer.lastName || ''}`.trim() || `Player ${playerId}`
  }
  
  // Then try opponent players
  const opponentPlayer = opponentTeamPlayers.value.find(p => p.id?.toString() === playerId || p.playerId?.toString() === playerId)
  if (opponentPlayer) {
    return `${opponentPlayer.firstName || ''} ${opponentPlayer.lastName || ''}`.trim() || `Player ${playerId}`
  }
  
  return `Player ${playerId}`
}

// Get match result indicator (W/L)
const getMatchResult = (match) => {
  if (match.ourPoints !== null && match.opponentPoints !== null) {
    return match.ourPoints > match.opponentPoints ? 'W' : 'L'
  }
  return 'L'
}

// Get CSS class for match result
const getMatchResultClass = (match) => {
  const result = getMatchResult(match)
  return result === 'W' ? 'win' : 'loss'
}

// Calculate team statistics from individual player stats
const calculateTeamStats = (players) => {
  const stats = {
    fieldGoals: { made: 0, attempts: 0, percentage: 0 },
    twoPointers: { made: 0, attempts: 0, percentage: 0 },
    threePointers: { made: 0, attempts: 0, percentage: 0 },
    freeThrows: { made: 0, attempts: 0, percentage: 0 },
    rebounds: { offensive: 0, defensive: 0, total: 0 },
    assists: 0,
    turnovers: 0,
    steals: 0,
    blocks: 0,
    points: 0
  }

  players.forEach(player => {
    stats.fieldGoals.made += player.fieldGoals?.made || 0
    stats.fieldGoals.attempts += player.fieldGoals?.attempts || 0
    
    stats.twoPointers.made += player.twoPointers?.made || 0
    stats.twoPointers.attempts += player.twoPointers?.attempts || 0
    
    stats.threePointers.made += player.threePointers?.made || 0
    stats.threePointers.attempts += player.threePointers?.attempts || 0
    
    stats.freeThrows.made += player.freeThrows?.made || 0
    stats.freeThrows.attempts += player.freeThrows?.attempts || 0
    
    stats.rebounds.offensive += player.rebounds?.offensive || 0
    stats.rebounds.defensive += player.rebounds?.defensive || 0
    
    stats.assists += player.assists || 0
    stats.turnovers += player.turnovers || 0
    stats.steals += player.steals || 0
    stats.blocks += player.blocks || 0
    stats.points += player.points || 0
  })

  // Calculate percentages
  stats.fieldGoals.percentage = stats.fieldGoals.attempts > 0 
    ? Math.round((stats.fieldGoals.made / stats.fieldGoals.attempts) * 100) 
    : 0
    
  stats.twoPointers.percentage = stats.twoPointers.attempts > 0 
    ? Math.round((stats.twoPointers.made / stats.twoPointers.attempts) * 100) 
    : 0
    
  stats.threePointers.percentage = stats.threePointers.attempts > 0 
    ? Math.round((stats.threePointers.made / stats.threePointers.attempts) * 100) 
    : 0
    
  stats.freeThrows.percentage = stats.freeThrows.attempts > 0 
    ? Math.round((stats.freeThrows.made / stats.freeThrows.attempts) * 100) 
    : 0

  stats.rebounds.total = stats.rebounds.offensive + stats.rebounds.defensive

  return stats
}

// Fetch advanced analytics data
const fetchAdvancedAnalytics = async (matchId) => {
  advancedAnalyticsLoading.value = true
  
  try {
    // Fetch player performance comparison (top players)
    const playerRankingsResponse = await axios.get(`${MATCHES_URL}/ChronologicalEventInflux/match/${matchId}/player-rankings`)
    if (playerRankingsResponse.data?.isSuccess) {
      // Get top 3 players
      topPlayers.value = playerRankingsResponse.data.value.playerRankings.slice(0, 3)
      console.log('Top 3 players:', topPlayers.value)
    }
    
    // Fetch advanced match statistics (period statistics)  
    const advancedStatsResponse = await axios.get(`${MATCHES_URL}/ChronologicalEventInflux/match/${matchId}/advanced-statistics`)
    if (advancedStatsResponse.data?.isSuccess) {
      periodStatistics.value = advancedStatsResponse.data.value.periodStatistics
      console.log('Period statistics:', periodStatistics.value)
    }
    
  } catch (err) {
    console.error('Error fetching advanced analytics:', err)
  } finally {
    advancedAnalyticsLoading.value = false
  }
}

// Fetch match details and statistics
const fetchMatchDetails = async () => {
  loading.value = true
  error.value = null

  try {
    const matchId = route.params.id
    
    // Fetch match basic info using the same approach as MatchDetail
    const matchResponse = await axios.get(`${MATCHES_URL}/match/${matchId}`)
    match.value = matchResponse.data.value
    console.log('Match data loaded:', match.value)
    
    if (match.value) {
      // Update match data from API response
      updateMatchData()
      
      // Fetch team members and calculate statistics using SQL function
      await calculatePlayerStatisticsSQLFunction(matchId)
      //await fetchPlayerStatistics(matchId) // Alternative approach if needed
      
      // Fetch advanced analytics after basic data
      await fetchAdvancedAnalytics(matchId)
      
      await fetchRecommendations(matchId)
    }
    
  } catch (err) {
    console.error('Error fetching match details:', err)
    error.value = err.response?.data?.message || err.message || 'Failed to fetch match details'
  } finally {
    loading.value = false
  }
}
// Get recommendation icon based on type
const getRecommendationIcon = (type) => {
  const icons = {
    'substitution': '🔄',
    'timeout': '⏸️',
    'strategy': '📋',
    'defense': '🛡️',
    'offense': '⚡',
    'foul_trouble': '⚠️'
  }
  return icons[type] || '💡'
}
// Format period time (milliseconds to MM:SS)
const formatPeriodTime = (milliseconds) => {
  const totalSeconds = Math.floor(milliseconds / 1000)
  const minutes = Math.floor(totalSeconds / 60)
  const secs = totalSeconds % 60
  return `${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
}
// const formattedTime = computed(() => {
//   // Ensure we never show negative time
//   const timeMs = Math.max(0, matchTrackingState.value.remainingTime)
//   const totalSeconds = Math.floor(timeMs / 1000)
//   const minutes = Math.floor(totalSeconds / 60)
//   const seconds = totalSeconds % 60
//   if(minutes === 0 && seconds === 0) {
//     return '0:00'
//   }
//   return `${minutes}:${seconds.toString().padStart(2, '0')}`
// })
// Format creation time
const formatCreationTime = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleString('sr-RS', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
// Fetch all recommendations for the match
const fetchRecommendations = async (matchId) => {
  loadingRecommendations.value = true
  
  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/AutomaticRecommendation/match/${matchId}/all`, {
      headers: { Authorization: `Bearer ${jwt}` }
    })
    
    recommendations.value = response.data.value?.recommendations || response.data?.recommendations || []
    console.log('Recommendations loaded:', recommendations.value)
    
  } catch (err) {
    console.error('Error fetching recommendations:', err)
  } finally {
    loadingRecommendations.value = false
  }
}

// Update match data from API response - similar to MatchDetail
const updateMatchData = () => {
  if (!match.value) return
  
  // Update opponent team name from match data
  if (match.value.teamName) {
    opponentTeam.value = match.value.teamName
  } else if (match.value.opponentName) {
    opponentTeam.value = match.value.opponentName
  }
  
  console.log('Opponent team set to:', opponentTeam.value)
}

// Fetch team members participating in the match - using MatchDetail approach
const fetchTeamMembers = async (matchId) => {
  try {
    // Fetch all team members for this match
    const teamMembersResponse = await axios.get(`${MATCHES_URL}/match/${matchId}/team-members`)
    const teamMembers = teamMembersResponse.data.value.teamMembers

    console.log('Team members:', teamMembers)

    // Separate our team (team ID 1) and opponent team
    const ourTeamMembers = teamMembers.filter(tm => tm.idTeam === 1)
    const opponentTeamMembers = teamMembers.filter(tm => tm.idTeam !== 1)
    
    // Update opponent team name if we have data
    if (opponentTeamMembers.length > 0) {
      opponentTeam.value = opponentTeamMembers[0].teamName
    }
    
    // Convert team members to the format expected by the UI - same as MatchDetail
    const convertToPlayerFormat = (teamMember) => ({
      id: teamMember.idPlayer,
      name: `${teamMember.playerName} ${teamMember.playerSurname}`,
      number: teamMember.jerseyNumber || 0,
      timeInGame: '0:00',
      fouls: 0,
      eff: 0,
      position: teamMember.positionName,
      isActive: teamMember.inGame,
      isStarter: teamMember.startingLineup,
      // Extended stats for full team table - same structure as MatchDetail
      minutes: '0:00',
      points: 0,
      fg: '0/0',
      twoP: '0/0', 
      threeP: '0/0',
      ft: '0/0',
      rebOff: 0,
      rebDef: 0,
      totalReb: 0,
      assists: 0,
      steals: 0,
      blocks: 0,
      turnovers: 0,
      efficiency: 0,
      // Additional tracking fields
      fg_made: 0,
      fg_attempts: 0,
      twoP_made: 0,
      twoP_attempts: 0,
      threeP_made: 0,
      threeP_attempts: 0,
      ft_made: 0,
      ft_attempts: 0
    })
    
    // Convert all team members to player format
    const fullOurTeamStats = ourTeamMembers.map(convertToPlayerFormat)
    const fullOpponentTeamStats = opponentTeamMembers.map(convertToPlayerFormat)
    
    // Calculate comprehensive statistics from events - same as MatchDetail
    await calculatePlayerStatistics(matchId, fullOurTeamStats, fullOpponentTeamStats)
    
    // Convert to format expected by PreviousMatchDetail component
    ourTeamPlayers.value = fullOurTeamStats.map(player => ({
      playerId: player.id,
      firstName: player.name.split(' ')[0],
      lastName: player.name.split(' ').slice(1).join(' '),
      jerseyNumber: player.number,
      fieldGoals: { made: player.fg_made, attempts: player.fg_attempts },
      twoPointers: { made: player.twoP_made, attempts: player.twoP_attempts },
      threePointers: { made: player.threeP_made, attempts: player.threeP_attempts },
      freeThrows: { made: player.ft_made, attempts: player.ft_attempts },
      rebounds: { offensive: player.rebOff, defensive: player.rebDef },
      assists: player.assists,
      turnovers: player.turnovers,
      steals: player.steals,
      blocks: player.blocks,
      fouls: player.fouls,
      points: player.points,
      efficiency: player.efficiency
    }))
    
    opponentTeamPlayers.value = fullOpponentTeamStats.map(player => ({
      playerId: player.id,
      firstName: player.name.split(' ')[0],
      lastName: player.name.split(' ').slice(1).join(' '),
      jerseyNumber: player.number,
      fieldGoals: { made: player.fg_made, attempts: player.fg_attempts },
      twoPointers: { made: player.twoP_made, attempts: player.twoP_attempts },
      threePointers: { made: player.threeP_made, attempts: player.threeP_attempts },
      freeThrows: { made: player.ft_made, attempts: player.ft_attempts },
      rebounds: { offensive: player.rebOff, defensive: player.rebDef },
      assists: player.assists,
      turnovers: player.turnovers,
      steals: player.steals,
      blocks: player.blocks,
      fouls: player.fouls,
      points: player.points,
      efficiency: player.efficiency
    }))
    
    console.log('Our team players with calculated stats:', ourTeamPlayers.value)
    console.log('Opponent team players with calculated stats:', opponentTeamPlayers.value)
    
  } catch (err) {
    console.error('Error fetching team members:', err)
    // Keep empty arrays as fallback
    ourTeamPlayers.value = []
    opponentTeamPlayers.value = []
  }
}

// Fetch player statistics from match events - renamed to match MatchDetail
const fetchPlayerStatistics = async (matchId) => {
  await fetchTeamMembers(matchId)
}

// NEW: Calculate player statistics using SQL function combined with team members
const calculatePlayerStatisticsSQLFunction = async (matchId) => {
  try {
    console.log('Using SQL function to calculate player statistics for match:', matchId)
    
    // 1. First get team members to get complete player information
    const teamMembersResponse = await axios.get(`${MATCHES_URL}/match/${matchId}/team-members`)
    const teamMembers = teamMembersResponse.data.value.teamMembers
    console.log('Team members:', teamMembers)

    // 2. Get statistics from SQL function
    const sqlStatsResponse = await axios.get(`${MATCHES_URL}/match/${matchId}/player-statistics`)
    const sqlStatistics = sqlStatsResponse.data.value.playerStatistics
    console.log('SQL statistics:', sqlStatistics)

    // 3. Combine team member info with SQL statistics
    const combinePlayerData = (teamMembers, sqlStats) => {
      return teamMembers.map(member => {
        // Find matching SQL stats for this player
        const stats = sqlStats.find(s => 
          s.playerId === member.idPlayer && s.teamId === member.idTeam
        )

        if (stats) {
          // Player has stats from SQL function
          return {
            playerId: member.idPlayer,
            firstName: member.playerName,
            lastName: member.playerSurname,
            jerseyNumber: member.jerseyNumber,
            fieldGoals: { 
              made: stats.shooting2PMade + stats.shooting3PMade, 
              attempts: stats.shooting2PAttempted + stats.shooting3PAttempted 
            },
            twoPointers: { 
              made: stats.shooting2PMade, 
              attempts: stats.shooting2PAttempted 
            },
            threePointers: { 
              made: stats.shooting3PMade, 
              attempts: stats.shooting3PAttempted 
            },
            freeThrows: { 
              made: stats.freeThrowsMade, 
              attempts: stats.freeThrowsAttempted 
            },
            rebounds: { 
              offensive: stats.offensiveRebounds, 
              defensive: stats.defensiveRebounds 
            },
            assists: stats.totalAssists,
            turnovers: 0, // Not available in SQL stats yet
            steals: stats.totalSteals,
            blocks: stats.totalBlocks,
            fouls: stats.totalFouls,
            points: stats.totalPoints,
            efficiency: stats.efficiencyRating,
            performanceGrade: stats.performanceGrade,
            minutesPlayed: stats.minutesPlayed,
            teamId: member.idTeam,
            teamName: member.teamName
          }
        } else {
          // No stats available, return empty stats
          return {
            playerId: member.idPlayer,
            firstName: member.playerName,
            lastName: member.playerSurname,
            jerseyNumber: member.jerseyNumber,
            fieldGoals: { made: 0, attempts: 0 },
            twoPointers: { made: 0, attempts: 0 },
            threePointers: { made: 0, attempts: 0 },
            freeThrows: { made: 0, attempts: 0 },
            rebounds: { offensive: 0, defensive: 0 },
            assists: 0,
            turnovers: 0,
            steals: 0,
            blocks: 0,
            fouls: 0,
            points: 0,
            efficiency: 0,
            performanceGrade: 'N/A',
            minutesPlayed: 0,
            teamId: member.idTeam,
            teamName: member.teamName
          }
        }
      })
    }

    // Combine all players
    const allPlayers = combinePlayerData(teamMembers, sqlStatistics)

    // Separate our team (team ID 1) and opponent team
    ourTeamPlayers.value = allPlayers.filter(player => player.teamId === 1)
    opponentTeamPlayers.value = allPlayers.filter(player => player.teamId !== 1)
    
    // Update opponent team name if we have data
    if (opponentTeamPlayers.value.length > 0) {
      opponentTeam.value = opponentTeamPlayers.value[0].teamName
    }
    
    console.log('Our team players with SQL stats:', ourTeamPlayers.value)
    console.log('Opponent team players with SQL stats:', opponentTeamPlayers.value)
    
  } catch (err) {
    console.error('Error calculating player statistics with SQL function:', err)
    // Fallback to original method
    console.log('Falling back to original statistics calculation')
    await fetchTeamMembers(matchId)
  }
}

// Fallback method for fetching player statistics
const fetchPlayerStatisticsFallback = async (matchId) => {
  console.log('Using fallback method to fetch team members')
  
  // Try to get team members from team endpoints directly
  const ourTeamResponse = await axios.get(`${MATCHES_URL}/teammember/team/1`)
  let ourTeamMembers = ourTeamResponse.data.value?.teamPlayers || ourTeamResponse.data?.teamPlayers || []
  
  let opponentTeamMembers = []
  if (match.value && match.value.idTeam) {
    const opponentTeamResponse = await axios.get(`${MATCHES_URL}/teammember/team/${match.value.idTeam}`)
    opponentTeamMembers = opponentTeamResponse.data.value?.teamPlayers || opponentTeamResponse.data?.teamPlayers || []
  }
  
  // Convert to team member format expected by calculatePlayerStatisticsFromEvents
  const convertToTeamMemberFormat = (player, teamId) => ({
    idPlayer: player.playerId || player.idPlayer,
    playerName: player.firstName,
    playerSurname: player.lastName,
    jerseyNumber: player.jerseyNumber,
    idTeam: teamId,
    teamName: teamId === 1 ? 'Naš tim' : opponentTeam.value
  })
  
  ourTeamMembers = ourTeamMembers.map(player => convertToTeamMemberFormat(player, 1))
  opponentTeamMembers = opponentTeamMembers.map(player => convertToTeamMemberFormat(player, match.value?.idTeam || 2))
  
  // Calculate statistics for each player
  ourTeamPlayers.value = await Promise.all(
    ourTeamMembers.map(member => calculatePlayerStatisticsFromEvents(matchId, member))
  )
  
  opponentTeamPlayers.value = await Promise.all(
    opponentTeamMembers.map(member => calculatePlayerStatisticsFromEvents(matchId, member))
  )
  
  console.log('Fallback method completed successfully')
}

// Calculate comprehensive player statistics from match events - identical to MatchDetail
const calculatePlayerStatistics = async (matchId, fullOurTeamStats, fullOpponentTeamStats) => {
  try {
    // Fetch all events for this match
    const eventsResponse = await axios.get(`${MATCHES_URL}/MatchTracking/${matchId}/events`)
    
    if (!eventsResponse.data?.isSuccess || !eventsResponse.data?.value?.events) {
      console.log('No events data available for statistics calculation')
      return
    }

    const events = eventsResponse.data.value.events
    console.log('Calculating statistics from events:', events.length)

    // Initialize statistics for all players - same as MatchDetail
    const resetPlayerStats = (player) => {
      player.points = 0
      player.fg_made = 0
      player.fg_attempts = 0
      player.twoP_made = 0
      player.twoP_attempts = 0
      player.threeP_made = 0
      player.threeP_attempts = 0
      player.ft_made = 0
      player.ft_attempts = 0
      player.rebOff = 0
      player.rebDef = 0
      player.assists = 0
      player.steals = 0
      player.blocks = 0
      player.turnovers = 0
      player.fouls = 0
    }

    // Reset all player stats
    fullOurTeamStats.forEach(resetPlayerStats)
    fullOpponentTeamStats.forEach(resetPlayerStats)

    // Process each event and update player statistics - IDENTICAL to MatchDetail
    events.forEach(event => {
      if (event.eventType === 'personal' && event.playerId) {
        // Find the player in either team
        let player = fullOurTeamStats.find(p => p.id === event.playerId)
        if (!player) {
          player = fullOpponentTeamStats.find(p => p.id === event.playerId)
        }

        if (player) {
          switch (event.type) {
            case '+2p':
              player.points += 2
              player.twoP_made++
              player.twoP_attempts++
              player.fg_made++
              player.fg_attempts++
              break
            case '2p':
              player.twoP_attempts++
              player.fg_attempts++
              break
            case '+3p':
              player.points += 3
              player.threeP_made++
              player.threeP_attempts++
              player.fg_made++
              player.fg_attempts++
              break
            case '3p':
              player.threeP_attempts++
              player.fg_attempts++
              break
            case '+ft':
              player.points += 1
              player.ft_made++
              player.ft_attempts++
              break
            case 'ft':
              player.ft_attempts++
              break
            case 'reb of':
              player.rebOff++
              break
            case 'reb def':
              player.rebDef++
              break
            case 'assist':
              player.assists++
              break
            case 'steal':
              player.steals++
              break
            case 'block':
              player.blocks++
              break
            case 'foul':
              player.fouls++
              break
          }
        }
      }
    })

    // Calculate formatted statistics for display - same as MatchDetail
    fullOurTeamStats.forEach(calculateFormattedStats)
    fullOpponentTeamStats.forEach(calculateFormattedStats)

    console.log('Statistics calculated successfully')

  } catch (error) {
    console.error('Error calculating player statistics:', error)
  }
}

// Helper function to calculate formatted statistics for display - identical to MatchDetail
const calculateFormattedStats = (player) => {
  // Field Goal percentage
  player.fg = player.fg_attempts > 0 ? 
    `${player.fg_made}/${player.fg_attempts}` : '0/0'
  
  // Two-point percentage
  player.twoP = player.twoP_attempts > 0 ? 
    `${player.twoP_made}/${player.twoP_attempts}` : '0/0'
  
  // Three-point percentage
  player.threeP = player.threeP_attempts > 0 ? 
    `${player.threeP_made}/${player.threeP_attempts}` : '0/0'
  
  // Free throw percentage
  player.ft = player.ft_attempts > 0 ? 
    `${player.ft_made}/${player.ft_attempts}` : '0/0'
  
  // Total rebounds
  player.totalReb = player.rebOff + player.rebDef
  
  // Efficiency calculation (basic formula)
  // EFF = (PTS + REB + AST + STL + BLK) - (FGA - FGM + FTA - FTM + TO)
  const positive = player.points + player.totalReb + player.assists + player.steals + player.blocks
  const negative = (player.fg_attempts - player.fg_made) + (player.ft_attempts - player.ft_made) + player.turnovers + player.fouls
  player.eff = positive - negative
  
  // Efficiency for the EFF column in table
  player.efficiency = player.eff
}

// Helper function to calculate shooting percentages - identical to MatchDetail
const calculatePercentage = (made, attempts) => {
  return attempts > 0 ? Math.round((made / attempts) * 100) : 0
}
const formatMatchTime = () => {
  if (!match.value?.scheduledAt) {
    return 'Time TBD'
  }
  
  const matchDate = new Date(match.value.scheduledAt)
  const today = new Date()
  const tomorrow = new Date(today)
  tomorrow.setDate(today.getDate() + 1)
  
  const timeString = matchDate.toLocaleTimeString('en-US', { 
    hour: 'numeric', 
    minute: '2-digit',
    hour12: true 
  })
  
  // Check if match is today
  if (matchDate.toDateString() === today.toDateString()) {
    return `Today, ${timeString}`
  }
  
  // Check if match is tomorrow
  if (matchDate.toDateString() === tomorrow.toDateString()) {
    return `Tomorrow, ${timeString}`
  }
  
  // Otherwise show full date
  return matchDate.toLocaleDateString('en-US', {
    weekday: 'short',
    month: 'short', 
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  })
}

// Navigation
const goBack = () => {
  router.push('/analyst/matches')
}

// Download match report
const downloadMatchReport = () => {
  if (!match.value) {
    console.error('Match data not available')
    return
  }
  
  try {
    // matchReportService.generateMatchReport(
    //   match.value,
    //   ourTeamPlayers.value,
    //   opponentTeamPlayers.value,
    //   ourTeamStats.value,
    //   opponentTeamStats.value
    // )
    advancedMatchReportService.generateAdvancedMatchReport(match.value.idMatch)
  } catch (error) {
    console.error('Error generating match report:', error)
    //alert('Error generating match report. Please try again.')
  }
}

onMounted(() => {
  fetchMatchDetails()
})
</script>

<style scoped>
.previous-match-detail {
  min-height: 100vh;
  background-color: #f8f9fa;
  padding: 1rem 15rem;
}

.loading, .error {
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

.error {
  color: #dc3545;
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

.back-navigation {
  margin-bottom: 2rem;
}

.btn-back {
  background: none;
  border: 1px solid #007bff;
  color: #007bff;
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-back:hover {
  background-color: #007bff;
  color: white;
}

/* Combined Match Section */
.combined-match-section {
  background: white;
  border-radius: 16px;
  padding: 3rem 2rem;
  margin-bottom: 2rem;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1);
  position: relative;
  text-align: center;
}

.btn-back-absolute {
  position: absolute;
  top: 2rem;
  left: 2rem;
  background: none;
  border: 1px solid #007bff;
  color: #007bff;
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
  z-index: 10;
}

.btn-back-absolute:hover {
  background-color: #007bff;
  color: white;
}

.btn-download-report-absolute {
  position: absolute;
  top: 2rem;
  right: 2rem;
  background: linear-gradient(135deg, #28a745, #20c997);
  border: none;
  color: white;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(40, 167, 69, 0.3);
  z-index: 10;
}

.btn-download-report-absolute:hover {
  background: linear-gradient(135deg, #218838, #1ea87a);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(40, 167, 69, 0.4);
}

.centered-match-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2rem;
}

.match-header-info h1 {
  font-size: 2.5rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 1rem;
}

.match-time-location {
  justify-content: center;
  color: #666;
  flex-wrap: wrap;
}

.time-info, .location-info {
  font-size: 1.1rem;
}

.score-section {
  width: 100%;
  max-width: 800px;
}

.match-header {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.header-info h1 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 1rem;
}

.time-info, .location-info {
  font-size: 1.1rem;
}

/* Final Score Section */
.final-score-section {
  background: white;
  border-radius: 16px;
  padding: 2rem 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1);
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.score-container {
  display: flex;
  justify-content: center;
  align-items: center;
  max-width: 600px;
  margin: 0 auto 1.5rem;
  gap: 1rem;
}

.team-score {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  min-width: 250px;
  flex: 1;
}

.team-logo {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background-color: #f0f0f0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.team-logo-img {
  width: 60px;
  height: 60px;
  object-fit: contain;
}

.team-name {
  padding: 0 1rem;
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
  text-align: center;
  word-wrap: break-word;
  max-width: 100%;
}

.score {
  font-size: 3rem;
  font-weight: 700;
  color: #007bff;
}

.vs-separator {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.final-badge {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.match-result {
  margin-top: 1rem;
}

.result-badge {
  padding: 0.75rem 2rem;
  border-radius: 25px;
  font-size: 1.2rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.result-badge.win {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
}

.result-badge.loss {
  background: linear-gradient(135deg, #dc3545 0%, #fd7e14 100%);
  color: white;
}

/* Team Statistics Section */
.team-stats-section {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1);
}

.team-stats-section h2 {
  font-size: 1.8rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

.team-stats-comparison {
  max-width: 800px;
  margin: 0 auto;
}

.team-stats-header {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  align-items: center;
  margin-bottom: 0.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #007bff;
}

.team-column h3 {
  font-size: 1.4rem;
  font-weight: 600;
  color: #333;
  text-align: center;
  margin: 0;
}

.stats-labels .stat-label-header {
  font-size: 1.2rem;
  font-weight: 600;
  color: #007bff;
  text-align: center;
  padding: 0 2rem;
}

.stats-comparison-grid {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.stat-comparison-row {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  align-items: center;
  background: #f8f9fa;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.stat-comparison-row:hover {
  background: #e9ecef;
  transform: translateY(-1px);
}

.points-row {
  background: linear-gradient(135deg, #e8f5e8 0%, #f0f8f0 100%);
  border: 2px solid #28a745;
}

.stat-label {
  font-weight: 700;
  color: #333;
  text-align: center;
  font-size: 1.1rem;
  padding: 0 2rem;
  min-width: 80px;
}

.team-stat-value {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  padding: 0.5rem;
}

.our-team {
  justify-content: flex-end;
  color: #007bff;
}

.opponent-team {
  justify-content: flex-start;
  color: #fd7e14;
}

.made-attempts {
  font-weight: 600;
  color: inherit;
}

.percentage {
  font-weight: 700;
  color: inherit;
  font-size: 1.05rem;
}

.total {
  font-weight: 700;
  color: inherit;
  font-size: 1.1rem;
}

.total.points {
  color: #28a745;
  font-size: 1.3rem;
  font-weight: 800;
}

/* Player Statistics Section */
.player-stats-section {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1);
  margin-bottom: 2rem;
}

.player-stats-section h2 {
  font-size: 1.8rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

.team-player-stats {
  margin-bottom: 2rem;
}

.team-player-stats:last-child {
  margin-bottom: 0;
}

.team-player-stats h3 {
  font-size: 1.4rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 1rem;
  border-bottom: 2px solid #007bff;
  padding-bottom: 0.5rem;
}

.stats-table-wrapper {
  overflow-x: auto;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

/* Modern Table Styles - identical to MatchDetail */
.stats-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  font-size: 13px;
}

.stats-table.modern-stats th {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
  color: white;
  font-weight: 600;
  text-align: center;
  padding: 12px 8px;
  border-bottom: 2px solid #004494;
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stats-table.modern-stats td {
  padding: 8px 8px;
  vertical-align: middle;
}

.player-info-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}

.player-number {
  background: #007bff;
  color: white;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 14px;
  flex-shrink: 0;
}

.player-details {
  flex: 1;
  min-width: 0;
}

.player-details .player-name {
  font-weight: 600;
  font-size: 14px;
  color: #212529;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  margin-bottom: 0;
}

.foul-dots {
  display: flex;
  gap: 2px;
  margin-top: 2px;
}

.foul-dot {
  color: #dc3545;
  font-size: 18px;
  line-height: 1;
}

.efficiency-cell {
  font-weight: 700;
  font-size: 16px;
  color: #28a745;
  text-align: center;
}

.efficiency-cell.negative-eff {
  color: #dc3545;
}

.stat-cell {
  text-align: center;
}

.stat-made-attempts {
  font-weight: 600;
  font-size: 13px;
  color: #212529;
  line-height: 1.2;
}

.stat-percentage {
  font-size: 11px;
  color: #6c757d;
  margin-top: 2px;
}

.reb-cell {
  text-align: center;
}

.reb-total {
  font-weight: 600;
  font-size: 14px;
  color: #212529;
}

.reb-breakdown {
  font-size: 11px;
  color: #6c757d;
  margin-top: 2px;
}

.simple-stat {
  text-align: center;
  font-weight: 500;
  color: #212529;
}

.points-cell {
  text-align: center;
  font-weight: 700;
  font-size: 16px;
  color: #28a745;
}

.stats-table tbody tr:nth-child(even) {
  background-color: #fbfbfb;
}

/* Row separator and improved alignment; hover comes after zebra striping so it overrides */
.stats-table tbody tr {
  transition: background-color 0.15s ease;
  border-bottom: 1px solid rgba(0,0,0,0.06);
  background-color: transparent;
}

.stats-table tbody tr:hover {
  background-color: #f1f5f9;
}

.stats-table td, .stats-table th {
  vertical-align: middle; /* ensure cells align vertically */
  padding: 10px 8px; /* consistent padding for alignment */
}

.player-info-cell {
  display: flex;
  align-items: center; /* center contents vertically */
  gap: 12px;
  padding-left: 8px; /* ensure the first column lines up with others */
}

.player-number {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
}

/* Table Header Styles - identical to MatchDetail */
.player-header {
  width: 180px;
  text-align: left !important;
}

.eff-header,
.fg-header,
.twoP-header,
.threeP-header,
.ft-header,
.reb-header,
.ast-header,
.stl-header,
.blk-header,
.pts-header {
  width: 70px;
  text-align: center !important;
}

/* Responsive Design */
@media (max-width: 768px) {
  .previous-match-detail {
    padding: 1rem;
  }

  .combined-match-section {
    padding: 2rem 1rem;
  }

  .btn-back-absolute {
    position: static;
    margin-bottom: 1rem;
    align-self: flex-start;
  }

  .centered-match-content {
    gap: 1.5rem;
  }

  .match-header-info h1 {
    font-size: 2rem;
  }

  .score-container {
    flex-direction: column;
    gap: 2rem;
  }

  .score {
    font-size: 2.5rem;
  }

  .vs-separator {
    order: 2;
  }

  .team-stats-header {
    grid-template-columns: 1fr;
    gap: 1rem;
    text-align: center;
  }

  .stats-labels {
    order: -1;
  }

  .stat-comparison-row {
    grid-template-columns: 1fr;
    gap: 0.5rem;
    text-align: center;
  }

  .team-stat-value {
    justify-content: center;
  }

  .stat-label {
    order: -1;
    padding: 0.5rem;
    background: #007bff;
    color: white;
    border-radius: 4px;
    margin-bottom: 0.5rem;
  }
}

/* Advanced Analytics Loading */
.advanced-analytics-loading {
  background: white;
  border-radius: 16px;
  padding: 3rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1);
  text-align: center;
}

.loading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #007bff;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Top Players Section */
.top-players-section {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1);
}

.top-players-section h2 {
  font-size: 1.8rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

.top-players-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
}

.top-player-card {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-radius: 16px;
  padding: 1.5rem;
  position: relative;
  transition: all 0.3s ease;
  border: 2px solid transparent;
}

.top-player-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.15);
}

.top-player-card.first-place {
  background: linear-gradient(135deg, #fff3cd 0%, #ffeaa7 100%);
  border-color: #f39c12;
}

.top-player-card.second-place {
  background: linear-gradient(135deg, #f4f4f4 0%, #ddd 100%);
  border-color: #95a5a6;
}

.top-player-card.third-place {
  background: linear-gradient(135deg, #fdeaa7 0%, #fab1a0 100%);
  border-color: #e17055;
}

.rank-badge {
  position: absolute;
  top: -10px;
  right: -10px;
  background: white;
  border-radius: 50%;
  width: 60px;
  height: 60px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  z-index: 2;
}

.rank-number {
  font-size: 1.2rem;
  font-weight: 700;
  color: #333;
}

.rank-icon {
  font-size: 1.5rem;
}

.player-info {
  margin-bottom: 1rem;
}

.player-name {
  font-size: 1.4rem;
  font-weight: 700;
  color: #333;
  margin: 0 0 0.25rem 0;
}

.player-id {
  font-size: 0.9rem;
  color: #666;
  margin: 0;
}

.player-stats-summary {
  background: rgba(255,255,255,0.7);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 1rem;
}

.stat-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.stat-row:last-child {
  margin-bottom: 0;
}

.stat-label {
  font-weight: 600;
  color: #555;
}

.stat-value {
  font-weight: 700;
  font-size: 1.1rem;
}

.stat-value.points {
  color: #28a745;
  font-size: 1.3rem;
}

.stat-value.performance {
  color: #007bff;
}

.event-breakdown h4 {
  font-size: 1.1rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 0.75rem;
}

.breakdown-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(80px, 1fr));
  gap: 0.5rem;
}

.breakdown-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 0.5rem;
  border-radius: 8px;
  background: rgba(255,255,255,0.8);
  transition: all 0.2s ease;
}

.breakdown-item:hover {
  transform: scale(1.05);
}

.breakdown-item.positive-event {
  border-left: 4px solid #28a745;
}

.breakdown-item.negative-event {
  border-left: 4px solid #dc3545;
}

.event-name {
  font-size: 0.8rem;
  font-weight: 600;
  color: #666;
  margin-bottom: 0.25rem;
}

.event-count {
  font-size: 1.1rem;
  font-weight: 700;
  color: #333;
}

/* Period Statistics Section */
.period-statistics-section {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1);
}

.period-statistics-section h2 {
  font-size: 1.8rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

.period-table-container {
  overflow-x: auto;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  max-width: 100%;
}

.period-stats-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  font-size: 13px;
  min-width: 480px;
}

.period-stats-table th {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
  color: white;
  font-weight: 600;
  text-align: center;
  padding: 0.6rem;
  border-bottom: 2px solid #004494;
  font-size: 1rem;
}

.team-header {
  width: 160px;
  text-align: left !important;
  font-size: 0.95rem;
}

.period-header {
  width: 80px;
  font-size: 0.8rem;
}

.period-stats-table td {
  padding: 0.5rem;
  vertical-align: top;
  border-bottom: 1px solid rgba(0,0,0,0.1);
}

.team-name-cell {
  background: #f8f9fa;
  font-weight: 700;
  font-size: 1.1rem;
}

.team-name-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.team-indicator {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  flex-shrink: 0;
}

.team-indicator.our-team {
  background: #007bff;
}

.team-indicator.opponent-team {
  background: #fd7e14;
}

.period-data-cell {
  background: #fdfdfd;
}

.period-stats-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.2rem;
  font-size: 0.78rem;
}

.stat-item {
  max-width: 110px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.2rem 0.17rem;
  border-radius: 6px;
  font-size: 0.78rem;
  transition: all 0.2s ease;
}

.stat-item:hover {
  transform: scale(1.02);
}

.stat-item.positive {
  background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
  border-left: 2px solid #28a745;
}

.stat-item.neutral {
  background: linear-gradient(135deg, #e2e3e5 0%, #d6d8db 100%);
  border-left: 2px solid #6c757d;
}

.stat-item.negative {
  background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
  border-left: 2px solid #dc3545;
}

.stat-item .stat-label {
  font-weight: 600;
  color: #495057;
}

.stat-item .stat-count {
  font-weight: 700;
  font-size: 1rem;
  color: #212529;
  margin-right: 2px;
}

.period-stats-table tbody tr.our-team .period-data-cell {
  background: linear-gradient(135deg, #e7f1ff 0%, #f0f7ff 100%);
}

.period-stats-table tbody tr.opponent-team .period-data-cell {
  background: linear-gradient(135deg, #fff4e6 0%, #fef8f0 100%);
}

/* Responsive Design for Advanced Analytics */
@media (max-width: 768px) {
  .top-players-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .top-player-card {
    padding: 1rem;
  }
  
  .breakdown-grid {
    grid-template-columns: repeat(auto-fit, minmax(60px, 1fr));
    gap: 0.25rem;
  }
  
  .period-table-container {
    font-size: 12px;
  }
  
  .period-stats-table th,
  .period-stats-table td {
    padding: 0.5rem;
  }
  
  .period-stats-grid {
    grid-template-columns: 1fr;
    gap: 0.25rem;
  }
}

/* Recommendations Section Styles */
.recommendations-section {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  margin-top: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.recommendations-section h2 {
  font-size: 1.75rem;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 1.5rem;
  padding-bottom: 0.75rem;
  border-bottom: 3px solid #007bff;
}

.loading-recommendations {
  text-align: center;
  padding: 3rem;
  color: #6c757d;
  font-size: 1.1rem;
}

.no-recommendations {
  text-align: center;
  padding: 3rem;
  color: #6c757d;
  font-size: 1.1rem;
  background: #f8f9fa;
  border-radius: 8px;
  border: 2px dashed #dee2e6;
}

.recommendations-list {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.recommendation-card {
  background: white;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  padding: 1.5rem;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.recommendation-card::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 5px;
  transition: width 0.3s ease;
}

.recommendation-card.priority-high::before {
  background: linear-gradient(180deg, #dc3545 0%, #c82333 100%);
}

.recommendation-card.priority-medium::before {
  background: linear-gradient(180deg, #ffc107 0%, #e0a800 100%);
}

.recommendation-card.priority-low::before {
  background: linear-gradient(180deg, #28a745 0%, #218838 100%);
}

.recommendation-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
  border-color: #007bff;
}

.recommendation-card:hover::before {
  width: 8px;
}

.recommendation-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.recommendation-type {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.type-icon {
  font-size: 1.5rem;
}

.type-label {
  font-size: 1.1rem;
  font-weight: 600;
  color: #2c3e50;
  text-transform: capitalize;
}

.recommendation-meta {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.priority-badge {
  padding: 0.35rem 0.75rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.priority-badge.priority-high {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
  color: white;
}

.priority-badge.priority-medium {
  background: linear-gradient(135deg, #ffc107 0%, #e0a800 100%);
  color: #212529;
}

.priority-badge.priority-low {
  background: linear-gradient(135deg, #28a745 0%, #218838 100%);
  color: white;
}

.status-badge {
  padding: 0.35rem 0.75rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.status-badge.status-pending {
  background: #fff3cd;
  color: #856404;
  border: 1px solid #ffc107;
}

.status-badge.status-accepted {
  background: #d4edda;
  color: #155724;
  border: 1px solid #28a745;
}

.status-badge.status-rejected {
  background: #f8d7da;
  color: #721c24;
  border: 1px solid #dc3545;
}

.recommendation-body {
  margin: 1rem 0;
}

.recommendation-description {
  font-size: 1rem;
  line-height: 1.6;
  color: #495057;
  margin: 0;
}

.recommendation-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #e9ecef;
  font-size: 0.85rem;
  color: #6c757d;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.recommendation-time {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-weight: 600;
  color: #007bff;
}

.recommendation-time::before {
  content: '⏱️';
}

.recommendation-created {
  font-style: italic;
}

/* Responsive Design for Recommendations */
@media (max-width: 768px) {
  .recommendations-section {
    padding: 1.5rem;
  }
  
  .recommendations-section h2 {
    font-size: 1.5rem;
  }
  
  .recommendation-card {
    padding: 1rem;
  }
  
  .recommendation-header {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .recommendation-footer {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .priority-badge,
  .status-badge {
    font-size: 0.75rem;
    padding: 0.3rem 0.6rem;
  }
}
</style>