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
        <div class="header-info">
          <h1>{{ match.name || 'Match name placeholder' }}</h1>
          <div class="match-time-location">
            <div class="time-info">
              <strong>{{ formatMatchTime() }}</strong>
            </div>
            <div class="location-info">
              <span class="location-badge" :class="{ 'home': isHomeMatch, 'away': !isHomeMatch }">
                {{ isHomeMatch ? 'HOME' : 'AWAY' }}
              </span>
            </div>
          </div>
        </div>
        <div class="starting-five-controls">
          <button 
            class="control-btn starting-five" 
            @click="openStartingFiveModal"
            :disabled="!canEditStartingFive"
            :class="{ disabled: !canEditStartingFive }"
          >
            Starting Five
          </button>
        </div>
      </div>

      <!-- Live Match Stats -->
      <div class="live-stats-section">
        <div class="game-header">
          <div class="period-time">
            <span class="period">{{ currentPeriod }}</span>
            <span class="time">{{ currentTime }}</span>
            <span class="game-status">{{ gameStatus }}</span>
            <button 
              v-if="canStartMatch"
              class="timer-btn-inline start"
              @click="startMatchOrPeriod"
            >
              ▶ Start
            </button>
            <button 
              v-else-if="canPauseMatch"
              class="timer-btn-inline stop"
              @click="pauseMatch"
            >
              ⏸ Pause
            </button>
            <button 
              v-else-if="canResumeMatch"
              class="timer-btn-inline start"
              @click="resumeMatch"
            >
              ▶ Resume
            </button>
            <button 
              v-else-if="canEndPeriod"
              class="timer-btn-inline stop"
              @click="endPeriod"
            >
              ⏹ End Period
            </button>
            <button 
              v-else-if="canNextPeriod"
              class="timer-btn-inline start"
              @click="nextPeriod"
            >
              ➡ Next Period
            </button>
            <button 
              v-else-if="canEndMatch"
              class="timer-btn-inline stop"
              @click="endMatch"
            >
              🏁 End Match
            </button>
            <span 
              v-else-if="isMatchFinished"
              class="timer-btn-inline disabled"
            >
              ✅ Finished
            </span>
          </div>
          <div class="score-display">
            <div class="team-score home">
              <span class="team-name-large">Partizan</span>
              <span class="score">{{ matchTrackingState.ourPoints }}</span>
            </div>
            <div class="vs">-</div>
            <div class="team-score away">
              <span class="team-name-large">{{ opponentTeam }}</span>
              <span class="score">{{ matchTrackingState.opponentPoints }}</span>
            </div>
          </div>
          <div class="live-indicator">
            <span class="live-badge">LIVE</span>
          </div>
        </div>
        
        <!-- Game Controls -->
        <div class="game-controls">          
          <div class="center-controls-full">
            <button 
              v-if="canPauseMatch || canResumeMatch"
              class="control-btn timeout-btn partizan"
              @click="callTimeout('partizan')"
            >
              Timeout - Partizan
            </button>
            
            <button 
              v-if="canStartMatch"
              class="control-btn resume-btn"
              @click="startMatchOrPeriod"
            >
              ▶ Start Match
            </button>
            <button 
              v-else-if="canPauseMatch"
              class="control-btn pause-btn"
              @click="pauseMatch"
            >
              ⏸ Pause Game
            </button>
            <button 
              v-else-if="canResumeMatch"
              class="control-btn resume-btn"
              @click="resumeMatch"
            >
              ▶ Resume Game
            </button>
            <button 
              v-else-if="canEndPeriod"
              class="control-btn pause-btn"
              @click="endPeriod"
            >
              ⏹ End Period
            </button>
            <button 
              v-else-if="canNextPeriod"
              class="control-btn resume-btn"
              @click="nextPeriod"
            >
              ➡ Next Period
            </button>
            <button 
              v-else-if="canEndMatch"
              class="control-btn pause-btn"
              @click="endMatch"
            >
              🏁 End Match
            </button>
            <span 
              v-else-if="isMatchFinished"
              class="control-btn pause-btn disabled"
            >
              ✅ Match Finished
            </span>
            
            <button 
              v-if="canPauseMatch || canResumeMatch"
              class="control-btn timeout-btn opponent"
              @click="callTimeout('opponent')"
            >
              Timeout - {{ opponentTeam }}
            </button>
          </div>
        </div>
      </div>

      <!-- Main Game Interface -->
      <div class="game-interface">
        <!-- Left Side: Active Players -->
        <div class="active-players-section">
          <!-- Our Team Active Players -->
          <div class="team-players-container">
            <div class="team-header">
              <h3>Partizan</h3>
              <button class="substitution-btn" @click="openSubstitutionModal(1, 'Partizan')">Substitution</button>
              <button class="undo-btn" @click="undoLastEvent(1)">UNDO</button>
            </div>
            <div class="active-players-grid">
              <div 
                v-for="player in activeOurPlayers" 
                :key="player.id"
                class="player-card modern-card"
                :class="{ 'selected': selectedPlayer?.id === player.id && selectedPlayer?.team === 'our' }"
                @click="selectPlayer(player, 'our')"
              >
                <div class="card-header">
                  <span class="player-name">{{ player.name }}</span>
                  <span class="player-number">#{{ player.number }}</span>
                </div>
                <div class="card-body">
                  <div class="points-stat">
                    <span class="label">points:</span>
                    <span class="value">{{ player.points || 0 }}</span>
                  </div>
                  <div class="fouls-stat">
                    <span class="label">fouls:</span>
                    <div class="foul-dots">
                      <span v-for="foul in player.fouls" :key="foul" class="foul-dot">●</span>
                    </div>
                  </div>
                  <div class="eff-stat">
                    <span class="label">eff:</span>
                    <span class="value">{{ player.eff }}</span>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Action Buttons for Our Team -->
            <div class="action-buttons compact">
              <button class="action-btn small success" title="+2pt" @click="recordAction('+2p', ourTeamId)">+2p</button>
              <button class="action-btn small miss" title="2pt" @click="recordAction('2p', ourTeamId)">2p</button>
              <button class="action-btn small success" title="+3pt" @click="recordAction('+3p', ourTeamId)">+3p</button>
              <button class="action-btn small miss" title="3pt" @click="recordAction('3p', ourTeamId)">3p</button>
              <button class="action-btn small success" title="+FT" @click="recordAction('+ft', ourTeamId)">+ft</button>
              <button class="action-btn small miss" title="FT" @click="recordAction('ft', ourTeamId)">ft</button>

              <div class="spacer"></div>

              <button class="action-btn medium assist" title="Assist" @click="recordAction('assist', ourTeamId)">assist</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium rebound" title="Offensive rebound" @click="recordAction('reb of', ourTeamId)">reb of</button>
              <button class="action-btn medium rebound" title="Defensive rebound" @click="recordAction('reb def', ourTeamId)">reb def</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium steal" title="Steal" @click="recordAction('steal', ourTeamId)">steal</button>
              <button class="action-btn medium block" title="Block" @click="recordAction('block', ourTeamId)">block</button>

              <div class="spacer"></div>

              <button class="action-btn large foul" title="Foul" @click="recordAction('foul', ourTeamId)">foul</button>
            </div>
          </div>

          <!-- Opponent Team Active Players -->
          <div class="team-players-container">
            <div class="team-header">
              <h3>{{ opponentTeam }}</h3>
              <button class="substitution-btn" @click="openSubstitutionModal(match?.idTeam || 2, opponentTeam)">Substitution</button>
              <button class="undo-btn" @click="undoLastEvent(match?.idTeam || 2)">UNDO</button>
            </div>
            <div class="active-players-grid">
              <div 
                v-for="player in activeOpponentPlayers" 
                :key="player.id"
                class="player-card modern-card"
                :class="{ 'selected': selectedPlayer?.id === player.id && selectedPlayer?.team === 'opponent' }"
                @click="selectPlayer(player, 'opponent')"
              >
                <div class="card-header">
                  <span class="player-name">{{ player.name }}</span>
                  <span class="player-number">#{{ player.number }}</span>
                </div>
                <div class="card-body">
                  <div class="points-stat">
                    <span class="label">points:</span>
                    <span class="value">{{ player.points || 0 }}</span>
                  </div>
                  <div class="fouls-stat">
                    <span class="label">fouls:</span>
                    <div class="foul-dots">
                      <span v-for="foul in player.fouls" :key="foul" class="foul-dot">●</span>
                    </div>
                  </div>
                  <div class="eff-stat">
                    <span class="label">eff:</span>
                    <span class="value">{{ player.eff }}</span>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Action Buttons for Opponent Team -->
            <div class="action-buttons compact">
              <button class="action-btn small success" title="+2pt" @click="recordAction('+2p', match?.idTeam)">+2p</button>
              <button class="action-btn small miss" title="2pt" @click="recordAction('2p', match?.idTeam)">2p</button>
              <button class="action-btn small success" title="+3pt" @click="recordAction('+3p', match?.idTeam)">+3p</button>
              <button class="action-btn small miss" title="3pt" @click="recordAction('3p', match?.idTeam)">3p</button>
              <button class="action-btn small success" title="+FT" @click="recordAction('+ft', match?.idTeam)">+ft</button>
              <button class="action-btn small miss" title="FT" @click="recordAction('ft', match?.idTeam)">ft</button>

              <div class="spacer"></div>

              <button class="action-btn medium assist" title="Assist" @click="recordAction('assist', match?.idTeam)">assist</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium rebound" title="Offensive rebound" @click="recordAction('reb of', match?.idTeam)">reb of</button>
              <button class="action-btn medium rebound" title="Defensive rebound" @click="recordAction('reb def', match?.idTeam)">reb def</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium steal" title="Steal" @click="recordAction('steal', match?.idTeam)">steal</button>
              <button class="action-btn medium block" title="Block" @click="recordAction('block', match?.idTeam)">block</button>

              <div class="spacer"></div>

              <button class="action-btn large foul" title="Foul" @click="recordAction('foul', match?.idTeam)">foul</button>
            </div>
          </div>
        </div>

        <!-- Right Side: Formation, Defense & Events -->
        <div class="right-panel">
          <!-- Formation and Defense - Side by Side -->
          <div class="formation-defense-container">
            <!-- Formation -->
            <div class="formation-section">
              <h3>Formation</h3>
              <div class="formation-options">
                <button 
                  v-for="formation in formations" 
                  :key="formation"
                  class="formation-btn"
                  :class="{ 'active': selectedFormation === formation }"
                  @click="selectFormation(formation)"
                >
                  {{ formation }}
                </button>
              </div>
            </div>

            <!-- Defense -->
            <div class="defense-section">
              <h3>Defense</h3>
              <div class="defense-options">
                <button 
                  v-for="defense in defenseTypes" 
                  :key="defense"
                  class="defense-btn"
                  :class="{ 'active': selectedDefense === defense }"
                  @click="selectDefense(defense)"
                >
                  {{ defense }}
                </button>
              </div>
            </div>
          </div>

          <!-- Event Chronology - Full Width -->
          <div class="events-section">
            <h3>Event chronology</h3>
            <div class="events-list">
              <div 
                v-for="event in sortedGameEvents" 
                :key="event.id"
                class="event-item"
              >
                <span class="event-time">{{ event.time }}</span>
                <span class="event-description">{{ event.description }}</span>
              </div>
            </div>
            <button class="add-custom-event-btn" @click="openAddCustomEventModal">Add custom event</button>
          </div>
        </div>
      </div>

      <!-- Main Content Layout - Players Table & Recommendations -->
      <div class="main-content-layout">
        <!-- Left Side: Player Statistics Table -->
        <div class="players-table-section">
          <h2>Team Statistics</h2>
          
          <!-- Our Team Full Stats -->
          <div class="team-stats-container">
            <h3>Partizan</h3>
            <div class="stats-table-wrapper">
              <table class="stats-table modern-stats">
                <thead>
                  <tr>
                    <th class="player-header"># IGRAČ</th>
                    <th>EFF</th>
                    <th>FG</th>
                    <th>2P</th>
                    <th>3P</th>
                    <th>FT</th>
                    <th>REB O/D</th>
                    <th>AST</th>
                    <th>STL</th>
                    <th>BLK</th>
                    <th>PTS</th>
                  </tr>
                </thead>
                <tbody>
                  <tr 
                    v-for="player in fullOurTeamStats" 
                    :key="player.id"
                    :class="{ 'active-player': player.isActive }"
                  >
                    <td class="player-info-cell">
                      <div class="player-number">{{ player.number }}</div>
                      <div class="player-details">
                        <div class="player-name">{{ player.name }}</div>
                        <div class="foul-dots">
                          <span 
                            v-for="foul in player.fouls" 
                            :key="foul" 
                            class="foul-dot"
                          >●</span>
                        </div>
                      </div>
                      <div class="status-container">
                        <span v-if="player.isActive" class="status-badge active">Active</span>
                        <span v-else class="status-badge bench">Bench</span>
                      </div>
                    </td>
                    <td class="efficiency-cell" :class="{ 'negative-eff': player.eff < 0 }">{{ player.eff }}</td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.fg }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.fg_made, player.fg_attempts) }}%</div>
                    </td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.twoP }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.twoP_made, player.twoP_attempts) }}%</div>
                    </td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.threeP }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.threeP_made, player.threeP_attempts) }}%</div>
                    </td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.ft }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.ft_made, player.ft_attempts) }}%</div>
                    </td>
                    <td class="reb-cell">
                      <div class="reb-total">{{ player.rebOff + player.rebDef }}</div>
                      <div class="reb-breakdown">{{ player.rebOff }} {{ player.rebDef }}</div>
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

          <!-- Opponent Team Full Stats -->
          <div class="team-stats-container">
            <h3>{{ opponentTeam }}</h3>
            <div class="stats-table-wrapper">
              <table class="stats-table modern-stats">
                <thead>
                  <tr>
                    <th class="player-header"># IGRAČ</th>
                    <th>EFF</th>
                    <th>FG</th>
                    <th>2P</th>
                    <th>3P</th>
                    <th>FT</th>
                    <th>REB O/D</th>
                    <th>AST</th>
                    <th>STL</th>
                    <th>BLK</th>
                    <th>PTS</th>
                  </tr>
                </thead>
                <tbody>
                  <tr 
                    v-for="player in fullOpponentTeamStats" 
                    :key="player.id"
                    :class="{ 'active-player': player.isActive }"
                  >
                    <td class="player-info-cell">
                      <div class="player-number">{{ player.number }}</div>
                      <div class="player-details">
                        <div class="player-name">{{ player.name }}</div>
                        <div class="foul-dots">
                          <span 
                            v-for="foul in player.fouls" 
                            :key="foul" 
                            class="foul-dot"
                          >●</span>
                        </div>
                      </div>
                      <div class="status-container">
                        <span v-if="player.isActive" class="status-badge active">Active</span>
                        <span v-else class="status-badge bench">Bench</span>
                      </div>
                    </td>
                    <td class="efficiency-cell" :class="{ 'negative-eff': player.eff < 0 }">{{ player.eff }}</td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.fg }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.fg_made, player.fg_attempts) }}%</div>
                    </td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.twoP }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.twoP_made, player.twoP_attempts) }}%</div>
                    </td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.threeP }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.threeP_made, player.threeP_attempts) }}%</div>
                    </td>
                    <td class="stat-cell">
                      <div class="stat-made-attempts">{{ player.ft }}</div>
                      <div class="stat-percentage">{{ calculatePercentage(player.ft_made, player.ft_attempts) }}%</div>
                    </td>
                    <td class="reb-cell">
                      <div class="reb-total">{{ player.rebOff + player.rebDef }}</div>
                      <div class="reb-breakdown">{{ player.rebOff }} {{ player.rebDef }}</div>
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

        <!-- Right Side: Automatic Recommendations -->
        <div class="recommendations-sidebar">
          <h3>Automatic Recommendations</h3>
          <div class="recommendations-container">
            <div 
              v-for="recommendation in automaticRecommendations" 
              :key="recommendation.id"
              class="recommendation-card"
              :class="recommendation.priority"
            >
              <div class="recommendation-header">
                <span class="recommendation-icon">{{ recommendation.icon }}</span>
                <span class="recommendation-priority">{{ recommendation.priority.toUpperCase() }}</span>
                <span class="recommendation-time">{{ recommendation.timestamp }}</span>
              </div>
              <div class="recommendation-content">
                <h4>{{ recommendation.title }}</h4>
                <p>{{ recommendation.description }}</p>
              </div>
              <div class="recommendation-actions">
                <button 
                  @click="acceptRecommendation(recommendation.id)"
                  class="btn-accept"
                >
                  Accept
                </button>
                <button 
                  @click="dismissRecommendation(recommendation.id)"
                  class="btn-dismiss"
                >
                  Dismiss
                </button>
              </div>
            </div>
            <div v-if="automaticRecommendations.length === 0" class="no-recommendations">
              <p>No active recommendations at this time.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Starting Five Modal -->
  <StartingFiveModal
    :is-visible="showStartingFiveModal"
    :match-name="match?.name"
    :scheduled-at="match?.scheduledAt"
    :is-home-match="isHomeMatch"
    :opponent-team="opponentTeam"
    :our-team-players="fullOurTeamStats"
    :opponent-team-players="fullOpponentTeamStats"
    @close="closeStartingFiveModal"
    @submit="handleStartingFiveSubmit"
  />

  <!-- Player Substitution Modal -->
  <PlayerSubstitutionModal
    :is-visible="showSubstitutionModal"
    :team-id="substitutionTeamId"
    :team-name="substitutionTeamName"
    :players-in-game="substitutionPlayersInGame"
    :players-on-bench="substitutionPlayersOnBench"
    @close="closeSubstitutionModal"
    @substitute="handleSubstitution"
  />

  <!-- Add Custom Event Modal -->
  <AddCustomEventModal
    :is-visible="showAddCustomEventModal"
    :match-id="match?.idMatch"
    :our-team-players="fullOurTeamStats"
    :opponent-team-players="fullOpponentTeamStats"
    :opponent-team-name="opponentTeam"
    :opponent-team-id="2"
    @close="closeAddCustomEventModal"
    @event-created="onEventCreated"
  />
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'
import PlayerSubstitutionModal from '../../components/PlayerSubstitutionModal.vue'
import StartingFiveModal from '../../components/StartingFiveModal.vue'
import AddCustomEventModal from '../../components/AddCustomEventModal.vue'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const error = ref(null)
const match = ref(null)

// Match tracking state
const matchTrackingState = ref({
  trackingStatus: 'upcoming', // upcoming, preparation, active, finished
  periodStatus: 'upcoming', // upcoming, active, paused, finished
  currentPeriod: '1',
  periodDuration: 600000, // 10 minutes in milliseconds
  remainingTime: 600000, // milliseconds remaining in current period
  ourPoints: 0,
  opponentPoints: 0
})

// Timer state
const timerInterval = ref(null)

// Match data (will be updated from API)
const isHomeMatch = ref(false)
const opponentTeam = ref('Unknown Opponent')
const currentMatchStatus = ref('Unknown match status')
const currentPeriod = ref('Unknown period')
const currentTime = ref('10:00')
const gameStatus = ref('Unknown game status')
const timerRunning = ref(false)
const ourTeamId = 1
const ourTeamFouls = ref([])
const ourTeamTimeouts = ref([])

// Selected player for actions
const selectedPlayer = ref(null)

// Formation and Defense
const formations = ref(['1-4', '2-3', '3-2', '5 out'])
const selectedFormation = ref('3-2')
const defenseTypes = ref(['Individual', 'Zone', 'Combined'])
const selectedDefense = ref('Zone')

// Active players (5 per team)
const activeOurPlayers = ref([])
const activeOpponentPlayers = ref([])

// Game events (fetched from backend)
const gameEvents = ref([])

// Full team statistics (all players)
const fullOurTeamStats = ref([])
const fullOpponentTeamStats = ref([])

// Computed properties for dynamic UI
const canStartMatch = computed(() => {
  const hasProperStatus = (matchTrackingState.value.trackingStatus === 'preparation' && 
         matchTrackingState.value.periodStatus === 'upcoming') ||
         (matchTrackingState.value.trackingStatus === 'active' && 
         matchTrackingState.value.periodStatus === 'upcoming')
  
  // Can only start if status is correct AND starting lineup is complete
  return hasProperStatus && hasCompleteStartingLineup.value
})

const canPauseMatch = computed(() => {
  return matchTrackingState.value.periodStatus === 'active'
})

const canResumeMatch = computed(() => {
  return matchTrackingState.value.periodStatus === 'paused'
})

const canEndPeriod = computed(() => {
  return matchTrackingState.value.periodStatus === 'active' && 
         matchTrackingState.value.remainingTime <= 0
})

const canNextPeriod = computed(() => {
  return (matchTrackingState.value.periodStatus === 'finished' && 
         matchTrackingState.value.currentPeriod !== '4') &&
         matchTrackingState.value.trackingStatus !== 'finished'
})

const canEndMatch = computed(() => {
  return matchTrackingState.value.periodStatus === 'finished' && 
         matchTrackingState.value.currentPeriod === '4'
})

const isMatchFinished = computed(() => {
  return matchTrackingState.value.trackingStatus === 'finished'
})

// Check if starting lineup is complete (5 players from each team)
const hasCompleteStartingLineup = computed(() => {
  const ourStarters = fullOurTeamStats.value.filter(p => p.isStarter).length
  const opponentStarters = fullOpponentTeamStats.value.filter(p => p.isStarter).length
  return ourStarters === 5 && opponentStarters === 5
})

// Starting five button should only be enabled in preparation phase
const canEditStartingFive = computed(() => {
  return matchTrackingState.value.trackingStatus === 'preparation'
})

const formattedTime = computed(() => {
  // Ensure we never show negative time
  const timeMs = Math.max(0, matchTrackingState.value.remainingTime)
  const totalSeconds = Math.floor(timeMs / 1000)
  const minutes = Math.floor(totalSeconds / 60)
  const seconds = totalSeconds % 60
  if(minutes === 0 && seconds === 0) {
    return '0:00'
  }
  return `${minutes}:${seconds.toString().padStart(2, '0')}`
})

// Reverse events order - newest at top, oldest at bottom
const sortedGameEvents = computed(() => {
  return [...gameEvents.value].reverse()
})

const periodDisplayName = computed(() => {
  const period = matchTrackingState.value.currentPeriod
  switch(period) {
    case '1': return '1st Quarter'
    case '2': return '2nd Quarter'
    case '3': return '3rd Quarter'
    case '4': return '4th Quarter'
    default: return `Period ${period}`
  }
})

// Automatic recommendations (fetched from backend)
const automaticRecommendations = ref([])

// Functions to manage recommendations
const acceptRecommendation = async (recommendationId) => {
  try {
    const response = await axios.put(`${MATCHES_URL}/automaticrecommendation/${recommendationId}/accept`)
    if (response.status === 200) {
      // Remove from local array after successful accept
      automaticRecommendations.value = automaticRecommendations.value.filter(r => r.id !== recommendationId)
      console.log('Recommendation accepted:', recommendationId)
    }
  } catch (error) {
    console.error('Error accepting recommendation:', error)
  }
}

const dismissRecommendation = async (recommendationId) => {
  try {
    const response = await axios.put(`${MATCHES_URL}/automaticrecommendation/${recommendationId}/reject`)
    if (response.status === 200) {
      // Remove from local array after successful reject
      automaticRecommendations.value = automaticRecommendations.value.filter(r => r.id !== recommendationId)
      console.log('Recommendation dismissed:', recommendationId)
    }
  } catch (error) {
    console.error('Error dismissing recommendation:', error)
  }
}

// Match tracking API functions
const startMatchOrPeriod = async () => {
  try {
    const matchId = route.params.id
    const response = await axios.post(`${MATCHES_URL}/matchtracking/${matchId}/start`)
    
    if (response.data.isSuccess) {
      console.log('Match/Period started successfully')
      await fetchMatchTrackingData(matchId)
      startTimer()
    }
  } catch (error) {
    console.error('Error starting match/period:', error)
  }
}

const pauseMatch = async () => {
  try {
    const matchId = route.params.id
    const response = await axios.post(`${MATCHES_URL}/matchtracking/${matchId}/pause`)
    
    if (response.data.isSuccess) {
      console.log('Match paused successfully')
      await fetchMatchTrackingData(matchId)
      stopTimer()
    }
  } catch (error) {
    console.error('Error pausing match:', error)
  }
}

const resumeMatch = async () => {
  try {
    const matchId = route.params.id
    const response = await axios.post(`${MATCHES_URL}/matchtracking/${matchId}/resume`)
    
    if (response.data.isSuccess) {
      console.log('Match resumed successfully')
      await fetchMatchTrackingData(matchId)
      startTimer()
    }
  } catch (error) {
    console.error('Error resuming match:', error)
  }
}

const endPeriod = async () => {
  try {
    const matchId = route.params.id
    const response = await axios.post(`${MATCHES_URL}/matchtracking/${matchId}/end-period`)
    
    if (response.data.isSuccess) {
      console.log('Period ended successfully')
      await fetchMatchTrackingData(matchId)
      stopTimer()
    }
  } catch (error) {
    console.error('Error ending period:', error)
  }
}

const nextPeriod = async () => {
  try {
    const matchId = route.params.id
    const response = await axios.post(`${MATCHES_URL}/matchtracking/${matchId}/next-period`)
    
    if (response.data.isSuccess) {
      console.log('Advanced to next period successfully')
      await fetchMatchTrackingData(matchId)
    }
  } catch (error) {
    console.error('Error advancing to next period:', error)
  }
}

const endMatch = async () => {
  try {
    const matchId = route.params.id
    const response = await axios.post(`${MATCHES_URL}/matchtracking/${matchId}/end`)
    
    if (response.data.isSuccess) {
      console.log('Match ended successfully')
      await fetchMatchTrackingData(matchId)
      stopTimer()
    }
  } catch (error) {
    console.error('Error ending match:', error)
  }
}

const callTimeoutAPI = async (teamId) => {
  try {
    const matchId = route.params.id
    const response = await axios.post(`${MATCHES_URL}/matchtracking/${matchId}/timeout`, {
      teamId: teamId
    })
    
    if (response.data.isSuccess) {
      console.log('Timeout called successfully')
      await fetchMatchTrackingData(matchId)
      await fetchMatchEvents(matchId)
      
      // Track our team's timeouts (team events of type 'timeout')
      ourTeamTimeouts.value = gameEvents.value.filter(e => {
        const type = (e.type || '').toString().toLowerCase()
        const teamId = e.teamId || null
        const isTeamEvent = e.eventType === 'team'
        return isTeamEvent && type === 'timeout' && Number(teamId) === ourTeamId // ourTeamId = 1
      })
      //console.log('Our team timeouts:', ourTeamTimeouts.value)
      //console.log('Our team timeouts count:', ourTeamTimeouts.value.length)

      // If we've just reached 4 timeouts, create a one-time recommendation (warn 1 timeout left)
      try {
        const currentTimeouts = ourTeamTimeouts.value.length
        console.log('Current timeouts used:', currentTimeouts)
        if (currentTimeouts === 4) {
          // Create medium priority recommendation once when reaching 4 timeouts
          createRecommendation('medium priority', 'team timeout warning', 'Team has used 4 timeouts - only 1 left')
        }
      } catch (err) {
        console.error('Error checking timeouts for recommendations:', err)
      }
      stopTimer()
    }
  } catch (error) {
    console.error('Error calling timeout:', error)
  }
}

// Fetch current match tracking data
const fetchMatchTrackingData = async (matchId) => {
  try {
    const response = await axios.get(`${MATCHES_URL}/match/${matchId}/tracking`)
    
    if (response.data.isSuccess) {
      const data = response.data.value
      // Update match tracking state
      matchTrackingState.value = {
        trackingStatus: data.trackingStatus || 'upcoming',
        periodStatus: data.periodStatus || 'upcoming',
        currentPeriod: data.currentPeriod || '1',
        periodDuration: data.periodDuration || 600000,
        remainingTime: data.remainingPeriodTime || data.periodDuration || 600000,
        ourPoints: data.ourPoints || 0,
        opponentPoints: data.opponentPoints || 0
      }
      console.log('FETCHED match tracking data:', matchTrackingState.value)
      
      // Update UI state
      updateUIFromTrackingState()
    }
  } catch (error) {
    console.error('Error fetching match tracking data:', error)
  }
}

// Refresh only scores without affecting timer (for use during active periods)
const refreshScoresOnly = async (matchId) => {
  try {
    const response = await axios.get(`${MATCHES_URL}/match/${matchId}/tracking`)
    
    if (response.data.isSuccess) {
      const data = response.data.value
      // Only update scores, keep timer state intact
      matchTrackingState.value.ourPoints = data.ourPoints || 0
      matchTrackingState.value.opponentPoints = data.opponentPoints || 0
      console.log('REFRESHED scores only:', { 
        ourPoints: matchTrackingState.value.ourPoints, 
        opponentPoints: matchTrackingState.value.opponentPoints 
      })
    }
  } catch (error) {
    console.error('Error refreshing scores:', error)
  }
}

// Update UI state from tracking state
const updateUIFromTrackingState = () => {
  // Only update time display if period is not active (to preserve running timer)
  // Exception: always update on first load when timer is not running yet
  if (matchTrackingState.value.periodStatus !== 'active' || !timerInterval.value) {
    currentTime.value = formattedTime.value
  }
  currentPeriod.value = periodDisplayName.value
  
  // Update game status and handle timer
  if (matchTrackingState.value.trackingStatus === 'finished') {
    gameStatus.value = 'Finished'
    timerRunning.value = false
    stopTimer()
  } else if (matchTrackingState.value.periodStatus === 'active') {
    gameStatus.value = 'Playing'
    timerRunning.value = true
    // Start timer if not already running
    if (!timerInterval.value) {
      startTimer()
    }
  } else if (matchTrackingState.value.periodStatus === 'paused') {
    gameStatus.value = 'Paused'
    timerRunning.value = false
    stopTimer()
  } else if (matchTrackingState.value.periodStatus === 'upcoming') {
    gameStatus.value = 'Ready to Start'
    timerRunning.value = false
    stopTimer()
  } else {
    gameStatus.value = 'Preparing'
    timerRunning.value = false
    stopTimer()
  }
}

// Timer functions
const startTimer = () => {
  if (timerInterval.value) {
    clearInterval(timerInterval.value)
  }
  
  timerInterval.value = setInterval(() => {
    if (matchTrackingState.value.periodStatus === 'active' && matchTrackingState.value.remainingTime > 0) {
      matchTrackingState.value.remainingTime -= 1000 // Decrease by 1000ms (1 second)
      //console.log('AKTIVNA UTAKMICA IDE VREME: ', matchTrackingState.value.remainingTime)

      // Ensure we don't go below 0
      if (matchTrackingState.value.remainingTime <= 0) {
        matchTrackingState.value.remainingTime = 0 // Set exactly to 0
        currentTime.value = formattedTime.value // Update display to show 0:00
        stopTimer() // Stop timer immediately
        endPeriod() // End the period
      } else {
        currentTime.value = formattedTime.value // Update display
      }
    }
  }, 1000)
}

const stopTimer = () => {
  if (timerInterval.value) {
    clearInterval(timerInterval.value)
    timerInterval.value = null
  }
}

// Starting Five Modal
const showStartingFiveModal = ref(false)

// Custom Event Modal
const showAddCustomEventModal = ref(false)

// Substitution Modal
const showSubstitutionModal = ref(false)
const substitutionTeamId = ref(null)
const substitutionTeamName = ref('')
const substitutionPlayersInGame = ref([])
const substitutionPlayersOnBench = ref([])

// Modal functions
const openStartingFiveModal = () => {
  showStartingFiveModal.value = true
}

const closeStartingFiveModal = () => {
  showStartingFiveModal.value = false
}

// Custom Event Modal functions
const openAddCustomEventModal = () => {
  showAddCustomEventModal.value = true
}

const closeAddCustomEventModal = () => {
  showAddCustomEventModal.value = false
}

const onEventCreated = (newEvent) => {
  // Add the new event to the chronology
  gameEvents.value.unshift({
    id: newEvent.id,
    time: currentTime.value,
    description: `${newEvent.category.toUpperCase()}: ${newEvent.type}${newEvent.notes ? ' - ' + newEvent.notes : ''}`
  })
  //console.log('New custom event added:', newEvent)
  
  // Refresh the match events from backend
  const matchId = route.params.id
  fetchMatchEvents(matchId)
}

// Handle starting five submission from modal component
const handleStartingFiveSubmit = async (selectionData) => {
  try {
    const matchId = route.params.id
    
    // Prepare data for API - format according to backend structure
    const playersData = []
    
    // Add our team players
    fullOurTeamStats.value.forEach(player => {
      const isSelected = selectionData.ourTeamPlayers.includes(player.id)
      playersData.push({
        teamId: 1, // Use actual team ID
        playerId: player.id,
        startingLineup: isSelected,
        inGame: isSelected // Set as in game if they're starters
      })
    })
    
    // Add opponent team players
    fullOpponentTeamStats.value.forEach(player => {
      const isSelected = selectionData.opponentTeamPlayers.includes(player.id)
      playersData.push({
        teamId: match.value?.idTeam || 2, // Use actual team ID
        playerId: player.id,
        startingLineup: isSelected,
        inGame: isSelected // Set as in game if they're starters
      })
    })
    
    console.log('Submitting starting five:', playersData)
    
    // Call API to update starting lineup
    await axios.put(`${MATCHES_URL}/TeamMemberMatch/match/${matchId}/starting-lineup`, playersData)
    
    // Update local data
    updateLocalStartingLineup(selectionData)
    
    console.log('Starting five updated successfully')
  } catch (err) {
    console.error('Error updating starting five:', err)
  }
}

const updateLocalStartingLineup = (selectionData) => {
  // Update our team players
  fullOurTeamStats.value.forEach(player => {
    const isStarter = selectionData.ourTeamPlayers.includes(player.id)
    player.isStarter = isStarter
    player.isActive = isStarter // Set as active if they're starters
  })
  
  // Update opponent team players
  fullOpponentTeamStats.value.forEach(player => {
    const isStarter = selectionData.opponentTeamPlayers.includes(player.id)
    player.isStarter = isStarter
    player.isActive = isStarter // Set as active if they're starters
  })
  
  // Update active players arrays
  activeOurPlayers.value = fullOurTeamStats.value.filter(p => p.isActive)
  activeOpponentPlayers.value = fullOpponentTeamStats.value.filter(p => p.isActive)
}

// Substitution Modal functions
const openSubstitutionModal = (teamId, teamName) => {
  substitutionTeamId.value = teamId
  substitutionTeamName.value = teamName
  
  // Get players for this team
  let allTeamPlayers = []
  if (teamId === 1) {
    // Our team (Partizan)
    allTeamPlayers = fullOurTeamStats.value
  } else {
    // Opponent team
    allTeamPlayers = fullOpponentTeamStats.value
  }
  
  // Separate players in game and on bench
  substitutionPlayersInGame.value = allTeamPlayers.filter(p => p.isActive)
  substitutionPlayersOnBench.value = allTeamPlayers.filter(p => !p.isActive)
  
  showSubstitutionModal.value = true
}

const closeSubstitutionModal = () => {
  showSubstitutionModal.value = false
  substitutionTeamId.value = null
  substitutionTeamName.value = ''
  substitutionPlayersInGame.value = []
  substitutionPlayersOnBench.value = []
}

const handleSubstitution = async (substitutionData) => {
  try {
    const matchId = route.params.id
    
    // Call the API to perform the substitution
    const response = await axios.post(`${MATCHES_URL}/TeamMemberMatch/match/${matchId}/substitution`, {
      matchId: parseInt(matchId),
      teamId: substitutionData.teamId,
      playerInId: substitutionData.playerInId,
      playerOutId: substitutionData.playerOutId
    })
    
    if (response.data.isSuccess) {
      // Update local player states
      updateLocalPlayerStates(substitutionData)
      
      // Refresh the match events from backend
      fetchMatchEvents(matchId)
      
      console.log('Substitution completed successfully')
    } else {
      console.error('Error performing substitution:', response.data.message)
    }
  } catch (err) {
    console.error('Error performing substitution:', err)
  }
}

const updateLocalPlayerStates = (substitutionData) => {
  let targetTeamPlayers = []
  let targetActiveList = []
  
  if (substitutionData.teamId === 1) {
    // Our team
    targetTeamPlayers = fullOurTeamStats.value
    targetActiveList = activeOurPlayers.value
  } else {
    // Opponent team
    targetTeamPlayers = fullOpponentTeamStats.value
    targetActiveList = activeOpponentPlayers.value
  }
  
  // Update player states in full team list
  targetTeamPlayers.forEach(player => {
    if (player.id === substitutionData.playerOutId) {
      player.isActive = false
    } else if (player.id === substitutionData.playerInId) {
      player.isActive = true
    }
  })
  
  // Update active players list
  const updatedActiveList = targetTeamPlayers.filter(p => p.isActive)
  
  if (substitutionData.teamId === 1) {
    activeOurPlayers.value = updatedActiveList
  } else {
    activeOpponentPlayers.value = updatedActiveList
  }
}

// Fetch automatic recommendations from backend
const fetchAutomaticRecommendations = async (matchId) => {
  try {
    const response = await axios.get(`${MATCHES_URL}/automaticrecommendation/match/${matchId}`)
    if (response.data.isSuccess) {
      automaticRecommendations.value = response.data.value.recommendations.map(rec => ({
        id: rec.idRecommendation,
        priority: rec.priority,
        title: rec.type,
        description: rec.description,
        icon: rec.priority === 'urgent' ? '🔴' : rec.priority === 'medium priority' ? '⚠️' : '✅',
        timestamp: new Date(rec.creationTime).toLocaleTimeString()
      }))
    }
  } catch (error) {
    console.error('Error fetching automatic recommendations:', error)
  }
}

// Create automatic recommendation
const createRecommendation = async (priority, type, description) => {
  try {
    console.log('priority: ', priority)
    console.log('type: ', type)
    console.log('description: ', description)
    const matchId = parseInt(route.params.id)
    console.log('matchId: ', matchId)
    const response = await axios.post(`${MATCHES_URL}/automaticrecommendation`, {
      matchId,
      priority,
      type,
      description
    })
    if (response.status === 200) {
      // Refresh recommendations after creating new one
      await fetchAutomaticRecommendations(matchId)
    }
  } catch (error) {
    console.error('Error creating recommendation:', error)
  }
}

// Check specific game conditions based on event type
const checkRecommendationsForEvent = async (eventType, playerId = null, teamId = null) => {
  const matchId = parseInt(route.params.id)
  
  // Only check relevant conditions based on the event type
  switch(eventType) {
    case 'foul':
      await checkFoulRecommendations(playerId, teamId)
      break
    case 'reb of':
      await checkOffensiveReboundRecommendations()
      break
    case '+3p':
      await checkThreePointerRecommendations(playerId, teamId)
    case '3p':
    case '+2p':
    case '2p':
      await checkShootingRecommendations(playerId)
      break
    case '+ft':
    case 'ft':
      await checkFreeThrowRecommendations()
      break
    default:
      // For other events, no specific recommendations needed
      break
  }
}

// Check foul-related recommendations
const checkFoulRecommendations = async (playerId, teamId) => {
  // 1. Check if our player reached 4 fouls
  if (teamId === ourTeamId) {
    const player = fullOurTeamStats.value.find(p => p.id === playerId)
    if (player && player.fouls === 4) {
      createRecommendation('urgent', 'player foul limit', 
        `Player ${player.name} has 4 fouls - substitution recommended`)
    }
    // 3. Check if our team reached 4 fouls in current period using recorded foul events
    try {
      // Populate ourTeamFouls with personal 'foul' events that belong to our team
      ourTeamFouls.value = gameEvents.value.filter(e => {
        const type = (e.type || '').toString().toLowerCase()
        // support multiple possible team id field names from backend
        const teamId = e.teamId
        const isPersonal = (e.eventType === 'personal')
        return isPersonal && type === 'foul' && Number(teamId) === ourTeamId
      })

      const currentPeriodStr = String(matchTrackingState.value.currentPeriod || '').trim()
      const foulsThisPeriod = (ourTeamFouls.value || []).filter(ev => {
        const evPeriod = ev.period !== undefined && ev.period !== null ? String(ev.period) : null
        return evPeriod === currentPeriodStr
      }).length
      console.log('FOULS THIS PERIOD: ', foulsThisPeriod)

      if (foulsThisPeriod === 4) {
        createRecommendation('urgent', 'team foul bonus', 
          'Team has 4 fouls in quarter - less aggressive play recommended due to bonus')
      }
    } catch (err) {
      console.error('Error checking team fouls in current period:', err)
    }
  }
}

// Check three-pointer recommendations
const checkThreePointerRecommendations = async (playerId, teamId) => {
  // 2. Check if opponent player made 3rd three-pointer
  if (teamId !== ourTeamId) {
    const player = fullOpponentTeamStats.value.find(p => p.id === playerId)
    if (player && player.threeP_made === 3) {
      createRecommendation('urgent', 'opponent hot shooter', 
        `Opponent player ${player.name} made 3 three-pointers - strengthen defense`)
    }
  }
}

// Check shooting percentage recommendations
const checkShootingRecommendations = async (playerId) => {
  // 4. Check if our player has poor shooting percentage
  const player = fullOurTeamStats.value.find(p => p.id === playerId)
  if (player) {
    const totalShots = (player.twoP_attempts || 0) + (player.threeP_attempts || 0)
    const totalMade = (player.twoP_made || 0) + (player.threeP_made || 0)
    const percentage = totalShots > 0 ? (totalMade / totalShots) * 100 : 0
    //console.log('TOTAL/MADE - ', totalShots, '/', totalMade, ' = ', percentage)
    if (totalShots === 7 && percentage < 30) {
      createRecommendation('medium priority', 'poor shooting', 
        `Player ${player.name} has poor shooting percentage (${percentage.toFixed(1)}%) - consider substitution`)
    }
  }
}

// Check free throw recommendations
const checkFreeThrowRecommendations = async () => {
  // 6. Check if team reached 10 free throws
  const ourFreeThrows = fullOurTeamStats.value.reduce((sum, player) => sum + (player.ft_attempts || 0), 0)
  if (ourFreeThrows === 10) {
    createRecommendation('not priority', 'free throw advantage', 
      'Team achieved 10 free throws - continue with aggressive play')
  }
}

// Check offensive rebound recommendations
const checkOffensiveReboundRecommendations = async () => {
  // 7. Check if team reached 8 offensive rebounds
  const ourOffensiveRebounds = fullOurTeamStats.value.reduce((sum, player) => sum + (player.rebOff || 0), 0)
  if (ourOffensiveRebounds === 8) {
    createRecommendation('not priority', 'offensive rebound dominance', 
      'Team has 8 offensive rebounds - paint dominance')
  }
}

// Fetch match events from backend
const fetchMatchEvents = async (matchId) => {
  try {
    const response = await axios.get(`${MATCHES_URL}/MatchTracking/${matchId}/events`)

    if (response.data?.isSuccess && response.data?.value?.events) {
      // Map backend response to UI format (keep raw event for further checks)
      const rawEvents = response.data.value.events
      gameEvents.value = rawEvents.map(event => ({
        id: event.id,
        time: formatEventTime(event),
        description: formatEventDescription(event),
        eventType: event.eventType || '',
        type: event.type || event.Type || '',
        teamId: event.teamId || null,
        period: event.period || null,
        raw: event // original payload for debugging/logic
      }))

      // // Populate ourTeamFouls with personal 'foul' events that belong to our team
      // ourTeamFouls.value = rawEvents.filter(e => {
      //   const type = (e.type || e.Type || '').toString().toLowerCase()
      //   // support multiple possible team id field names from backend
      //   const teamId = e.teamId
      //   const isPersonal = (e.eventType === 'personal')
      //   return isPersonal && type === 'foul' && Number(teamId) === ourTeamId
      // })

      console.log('Events loaded:', gameEvents.value)
    }
  } catch (err) {
    console.error('Error fetching match events:', err)
    // Keep events empty on error
    gameEvents.value = []
  }
}

// Helper function to format event time for display
const formatEventTime = (event) => {
  if (event.period && event.periodTime !== null) {
    const totalSeconds = Math.floor(event.periodTime / 1000)
    // Round up to next second (e.g., 5:14:23 becomes 5:15)
    // const totalSeconds = Math.ceil(event.periodTime / 1000)
    const minutes = Math.floor(totalSeconds / 60)
    const seconds = totalSeconds % 60
    // Show period first (e.g. "2Q 5:34")
    return `${event.period}Q ${minutes}:${seconds.toString().padStart(2, '0')}`
  }
  // Fallback to creation time
  const date = new Date(event.creationTime)
  return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
}

// Helper function to format event description
const formatEventDescription = (event) => {
  // For personal events show: "Player Name - <type/notes>"
  if (event.eventType === 'personal' && event.playerName) {
    const base = event.type || 'Event'
    return event.notes ? `${event.playerName} - ${base} - ${event.notes}` : `${event.playerName} - ${base}`
  }

  // For team events show: "Team Name - <type/notes>"
  if (event.eventType === 'team' && event.teamName) {
    const base = event.type || 'Event'
    return event.notes ? `${event.teamName} - ${base} - ${event.notes}` : `${event.teamName} - ${base}`
  }

  // Default behavior: prefer player name if present, otherwise show type and notes
  let description = ''
  if (event.playerName) {
    description += event.playerName + ' '
  }
  description += event.type || 'Event'
  if (event.notes) {
    description += ' - ' + event.notes
  }
  return description
}

// Get match details and team members
const fetchMatchDetails = async () => {
  try {
    loading.value = true
    const matchId = route.params.id
    
    // Fetch match basic info (includes tracking data)
    const matchResponse = await axios.get(`${MATCHES_URL}/match/${matchId}`)
    match.value = matchResponse.data.value
    console.log('match.value:', match.value)
    
    // Update real-time data from match response
    updateMatchData()
    
    // Fetch current match tracking data
    await fetchMatchTrackingData(matchId)
    
    // Fetch team members for this match
    await fetchTeamMembers(matchId)
    
    // Fetch match events for event chronology
    await fetchMatchEvents(matchId)
    
  } catch (err) {
    console.error('Error fetching match details:', err)
    error.value = err.response?.data?.message || 'Failed to load match details'
  } finally {
    loading.value = false
  }
}

// Update match data from API response
const updateMatchData = () => {
  if (!match.value) return
  
  // Update opponent team name from match data
  if (match.value.teamName) {
    opponentTeam.value = match.value.teamName
  }

  // Update home/away status
  isHomeMatch.value = match.value.isInOurHall || false
  
  currentTime.value = match.value.currentTime || '10:00'
  // Update match status from tracking data
  if (match.value.trackingStatus) {
    currentMatchStatus.value = match.value.trackingStatus
    
    // Update game status based on tracking status
    if (match.value.trackingStatus === 'active') {
      gameStatus.value = 'Playing'
    } else if (match.value.trackingStatus === 'paused') {
      gameStatus.value = 'Paused'
    } else if (match.value.trackingStatus === 'finished') {
      gameStatus.value = 'Finished'
    } else if (match.value.trackingStatus === 'preparation') {
      gameStatus.value = 'Preparing'
    }
  }
  
  // Update current period if available
  if (match.value.currentPeriod) {
    currentPeriod.value = match.value.currentPeriod + 'Q'
  }
}

// Fetch team members participating in the match
const fetchTeamMembers = async (matchId) => {
  try {
    // Fetch all team members for this match
    const teamMembersResponse = await axios.get(`${MATCHES_URL}/match/${matchId}/team-members`)
    const teamMembers = teamMembersResponse.data.value.teamMembers

    //console.log('Team members:', teamMembers)

    // Separate our team (Partizan - team ID 1) and opponent team
    
    const ourTeamMembers = teamMembers.filter(tm => tm.idTeam === ourTeamId)
    const opponentTeamMembers = teamMembers.filter(tm => tm.idTeam !== ourTeamId)
    
    // Update opponent team name if we have data
    if (opponentTeamMembers.length > 0) {
      opponentTeam.value = opponentTeamMembers[0].teamName
    }
    
    // Convert team members to the format expected by the UI
    const convertToPlayerFormat = (teamMember) => ({
      id: teamMember.idPlayer,
      name: `${teamMember.playerName} ${teamMember.playerSurname}`,
      number: teamMember.jerseyNumber || 0,
      timeInGame: '0:00', // Will be updated during live tracking
      fouls: 0, // Will be updated during live tracking
      eff: 0, // Will be updated during live tracking
      position: teamMember.positionName,
      isActive: teamMember.inGame,
      isStarter: teamMember.startingLineup,
      // Extended stats for full team table
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
      efficiency: 0
    })
    
    // Update active players (currently in game)
    const activeOurTeamMembers = ourTeamMembers.filter(tm => tm.inGame)
    const activeOpponentTeamMembers = opponentTeamMembers.filter(tm => tm.inGame)
    
    activeOurPlayers.value = activeOurTeamMembers.map(convertToPlayerFormat)
    activeOpponentPlayers.value = activeOpponentTeamMembers.map(convertToPlayerFormat)
    
    // Update full team stats (all players)
    fullOurTeamStats.value = ourTeamMembers.map(convertToPlayerFormat)
    fullOpponentTeamStats.value = opponentTeamMembers.map(convertToPlayerFormat)
    
    // Calculate comprehensive statistics from events
    await calculatePlayerStatistics(matchId)
    
    //console.log('Our team players:', activeOurPlayers.value)
    //console.log('Opponent team players:', activeOpponentPlayers.value)
    
  } catch (err) {
    console.error('Error fetching team members:', err)
    // Keep hardcoded data as fallback
  }
}

// Calculate comprehensive player statistics from match events
const calculatePlayerStatistics = async (matchId) => {
  try {
    // Fetch all events for this match
    const eventsResponse = await axios.get(`${MATCHES_URL}/MatchTracking/${matchId}/events`)
    
    if (!eventsResponse.data?.isSuccess || !eventsResponse.data?.value?.events) {
      console.log('No events data available for statistics calculation')
      return
    }

    const events = eventsResponse.data.value.events
    console.log('Calculating statistics from events:', events.length)

    // Initialize statistics for all players
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
    fullOurTeamStats.value.forEach(resetPlayerStats)
    fullOpponentTeamStats.value.forEach(resetPlayerStats)

    // Process each event and update player statistics
    events.forEach(event => {
      if (event.eventType === 'personal' && event.playerId) {
        // Find the player in either team
        let player = fullOurTeamStats.value.find(p => p.id === event.playerId)
        if (!player) {
          player = fullOpponentTeamStats.value.find(p => p.id === event.playerId)
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

    // Calculate formatted statistics for display
    fullOurTeamStats.value.forEach(calculateFormattedStats)
    fullOpponentTeamStats.value.forEach(calculateFormattedStats)

    // Update active players arrays with calculated statistics
    activeOurPlayers.value = fullOurTeamStats.value.filter(p => p.isActive)
    activeOpponentPlayers.value = fullOpponentTeamStats.value.filter(p => p.isActive)

    console.log('Statistics calculated successfully')

  } catch (error) {
    console.error('Error calculating player statistics:', error)
  }
}

// Helper function to calculate formatted statistics for display
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

// Helper function to calculate shooting percentages
const calculatePercentage = (made, attempts) => {
  if (attempts === 0) return 0
  return Math.round((made / attempts) * 100)
}

// Player selection
const selectPlayer = (player, team) => {
  selectedPlayer.value = { ...player, team }
  console.log('Selected player:', selectedPlayer.value)
}

// Record action
const recordAction = async (action, teamId) => {
  if (!selectedPlayer.value) {
    console.log('Please select a player first')
    return
  }
  
  if (matchTrackingState.value.trackingStatus !== 'active') {
    console.log('Match must be active to record events')
    return
  }
  const eventTypeMap = {
      '+2p': '2point made',
      '2p': '2point miss', 
      '+3p': '3point made',
      '3p': '3point miss',
      '+ft': 'free throw made',
      'ft': 'free throw miss',
      'assist': 'assist',
      'reb of': 'rebound offensive',
      'reb def': 'rebound defensive', 
      'steal': 'steal',
      'block': 'block',
      'foul': 'foul'
    }

  try {
    const matchId = route.params.id

    // Create personal event for the selected player
    const eventData = {
      matchId: parseInt(matchId),
      category: 'personal',
      playerId: selectedPlayer.value.id,
      teamId: teamId,
      type: action,
      notes: `${selectedPlayer.value.name} - ${eventTypeMap[action] || action}`
    }

    console.log('Creating personal event:', eventData)
    
    const response = await axios.post(`${MATCHES_URL}/ChronologicalEvent`, eventData)
    
    if (response.data.isSuccess) {
      console.log('Event recorded successfully:', response.data)

      if (matchTrackingState.value.periodStatus === 'active') {
        await refreshScoresOnly(matchId)
      } else {
        await fetchMatchTrackingData(matchId)
      }
      
      // Refresh events from backend to get updated chronology
      await fetchMatchEvents(matchId)
      
      // Refresh player statistics
      await calculatePlayerStatistics(matchId)

      // Check for automatic recommendations based on the specific event type
      await checkRecommendationsForEvent(action, selectedPlayer.value.id, teamId)
      
      console.log(`Recorded ${action} for player ${selectedPlayer.value.name}`)
    }
    
  } catch (error) {
    console.error('Error recording event:', error)
  }
}

// Undo last event for a team
const undoLastEvent = async (teamId) => {
  try {
    const matchId = route.params.id
    
    const response = await axios.delete(`${MATCHES_URL}/ChronologicalEvent/undo/match/${matchId}/team/${teamId}`)
    
    if (response.data.isSuccess) {
      console.log('Event undone successfully:', response.data)
      
      // Refresh scores only if period is active (to avoid resetting timer)
      // Otherwise refresh full tracking data
      if (matchTrackingState.value.periodStatus === 'active') {
        await refreshScoresOnly(matchId)
      } else {
        await fetchMatchTrackingData(matchId)
      }
      
      // Refresh events from backend to get updated chronology
      await fetchMatchEvents(matchId)
      
      // Refresh player statistics
      await calculatePlayerStatistics(matchId)
    }
    
  } catch (error) {
    console.error('Error undoing last event:', error)
    if (error.response?.data?.message) {
      console.log('Undo error:', error.response.data.message)
    }
  }
}

// Formation selection
const selectFormation = async (formation) => {
  selectedFormation.value = formation
  console.log('Selected formation:', formation)
  
  // Create team event for formation change
  try {
    const matchId = route.params.id
    const eventData = {
      matchId: parseInt(matchId),
      category: 'team',
      type: 'formation',
      notes: formation,
      teamId: 1 // Our team (Partizan)
    }
    
    const response = await axios.post(`${MATCHES_URL}/ChronologicalEvent`, eventData)
    
    if (response.data?.isSuccess) {
      // Refresh the match events from backend
      fetchMatchEvents(matchId)
    }
  } catch (error) {
    console.error('Error creating formation event:', error)
  }
}

// Defense selection
const selectDefense = async (defense) => {
  selectedDefense.value = defense
  console.log('Selected defense:', defense)
  
  // Create team event for defense change
  try {
    const matchId = route.params.id
    const eventData = {
      matchId: parseInt(matchId),
      category: 'team',
      type: 'defense',
      notes: defense,
      teamId: 1 // Our team (Partizan)
    }
    
    const response = await axios.post(`${MATCHES_URL}/ChronologicalEvent`, eventData)
    
    if (response.data?.isSuccess) {
      // Refresh the match events from backend
      fetchMatchEvents(matchId)
    }
  } catch (error) {
    console.error('Error creating defense event:', error)
  }
}

// Game control functions (updated to use backend APIs)
const callTimeout = async (team) => {
  const teamId = team === 'partizan' ? 1 : 2 // Assuming opponent team ID is 2
  await callTimeoutAPI(teamId)
}

const pauseGame = async () => {
  await pauseMatch()
}

const resumeGame = async () => {
  await resumeMatch()
}

const toggleTimer = async () => {
  if (canStartMatch.value) {
    await startMatchOrPeriod()
  } else if (canPauseMatch.value) {
    await pauseMatch()
  } else if (canResumeMatch.value) {
    await resumeMatch()
  }
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

onMounted(() => {
  fetchMatchDetails()
  // Fetch recommendations for this match
  const matchId = parseInt(route.params.id)
  if (matchId) {
    fetchAutomaticRecommendations(matchId)
  }
})

onUnmounted(() => {
  // Clean up timer when component is unmounted
  stopTimer()
})
</script>

<style scoped>
.match-detail {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding: 1rem;
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
  min-width: 160px;
}

.btn-back:hover {
  background-color: #5a6268;
}

.match-header {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 1rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-info h1 {
  margin: 0 0 0.5rem 0;
  color: #333;
  font-size: 1.8rem;
}

.match-time-location {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.time-info {
  color: #666;
  font-size: 1.1rem;
}

.location-badge {
  padding: 0.3rem 0.8rem;
  border-radius: 15px;
  font-size: 0.8rem;
  font-weight: bold;
  text-transform: uppercase;
}

.location-badge.home {
  background-color: #e8f5e8;
  color: #2e7d32;
}

.location-badge.away {
  background-color: #fff3e0;
  color: #f57c00;
}

.status-badge {
  padding: 0.5rem 1rem;
  border-radius: 15px;
  font-size: 0.9rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-badge.active {
  background-color: #ffebee;
  color: #d32f2f;
  animation: pulse 2s infinite;
}

.timer-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.timer-btn {
  padding: 0.8rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.timer-btn.start {
  background-color: #28a745;
  color: white;
}

.timer-btn.start:hover {
  background-color: #218838;
  transform: translateY(-1px);
}

.timer-btn.stop {
  background-color: #dc3545;
  color: white;
}

.timer-btn.stop:hover {
  background-color: #c82333;
  transform: translateY(-1px);
}

.starting-five-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.starting-five-controls .starting-five {
  background-color: #6f42c1;
  color: white;
  border: none;
  padding: 0.6rem 1.2rem;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.starting-five-controls .starting-five:hover {
  background-color: #5a35a3;
}

@keyframes pulse {
  0% { opacity: 1; }
  50% { opacity: 0.7; }
  100% { opacity: 1; }
}

.live-stats-section {
  background: white;
  border-radius: 8px;
  margin-bottom: 1rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  overflow: hidden;
}

.game-header {
  background: linear-gradient(135deg, #1976d2, #42a5f5);
  color: white;
  padding: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.period-time {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 6rem;
}

.timer-btn-inline {
  padding: 0.4rem 0.8rem;
  border: none;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.3rem;
}

.timer-btn-inline.start {
  background-color: #28a745;
  color: white;
}

.timer-btn-inline.start:hover {
  background-color: #218838;
  transform: translateY(-1px);
}

.timer-btn-inline.stop {
  background-color: #dc3545;
  color: white;
}

.timer-btn-inline.stop:hover {
  background-color: #c82333;
  transform: translateY(-1px);
}

.timer-btn-inline.disabled {
  background-color: #6c757d;
  color: white;
  cursor: not-allowed;
}

.control-btn.disabled {
  background-color: #6c757d;
  color: white;
  cursor: not-allowed;
  opacity: 0.7;
}

.period {
  font-size: 1.2rem;
  font-weight: bold;
}

.time {
  font-size: 2rem;
  font-weight: bold;
  margin: 0.2rem 0;
}

.game-status {
  font-size: 0.9rem;
  opacity: 0.9;
}

.score-display {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.team-score {
  text-align: center;
}

.team-name {
  display: block;
  font-size: 0.9rem;
  margin-bottom: 0.3rem;
  opacity: 0.9;
}

.team-name-large {
  display: block;
  font-size: 1.3rem;
  margin-bottom: 0.3rem;
  opacity: 0.9;
  font-weight: 600;
}

.score {
  font-size: 3rem;
  font-weight: bold;
  line-height: 1;
}

.vs {
  font-size: 1.5rem;
  font-weight: bold;
}

.live-indicator {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 6rem;
}

.live-badge {
  background-color: #d32f2f;
  padding: 0.3rem 0.8rem;
  border-radius: 15px;
  font-size: 0.8rem;
  font-weight: bold;
  animation: pulse 2s infinite;
}

/* Game Controls */
.game-controls {
  background: #f8f9fa;
  padding: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid #dee2e6;
}

.left-controls,
.center-controls,
.right-controls {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.center-controls {
  flex: 1;
  justify-content: center;
}

.center-controls-full {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  justify-content: center;
  width: 100%;
}

.control-btn {
  padding: 0.6rem 1.2rem;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.starting-five {
  background-color: #6f42c1;
  color: white;
  min-width: 160px;
}

.starting-five:hover {
  background-color: #5a35a3;
}

.timeout-btn.partizan {
  background-color: #007bff;
  color: white;
  margin-right: auto;
}

.timeout-btn.opponent {
  background-color: #dc3545;
  color: white;
  margin-left: auto;
}

.timeout-btn:hover {
  opacity: 0.8;
}

.pause-btn {
  background-color: #ffc107;
  color: #212529;
}

.pause-btn:hover {
  background-color: #e0a800;
}

.resume-btn {
  background-color: #28a745;
  color: white;
}

.resume-btn:hover {
  background-color: #218838;
}

/* Main Game Interface */
.game-interface {
  display: flex;
  gap: 1rem;
}

.active-players-section {
  flex: 2;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.team-players-container {
  background: white;
  border-radius: 8px;
  padding: 1rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.team-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid #1976d2;
}

.team-header h3 {
  margin: 0;
  color: #333;
}

.substitution-btn {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 0.5rem 0.8rem;
  border-radius: 4px;
  font-size: 0.85rem;
  cursor: pointer;
  margin-left: auto;
  margin-right: 1rem;
  transition: all 0.2s;
}
.substitution-btn:hover {
  background-color: #388e3c;
  transform: translateY(-1px);
}

.undo-btn {
  background-color: #f44336;
  color: white;
  border: none;
  padding: 0.5rem 0.8rem;
  border-radius: 4px;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.2s;
}
.undo-btn:hover {
  background-color: #d32f2f;
  transform: translateY(-2px);
}

.active-players-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.player-card {
  border: 2px solid #ddd;
  border-radius: 8px;
  padding: 0.75rem;
  cursor: pointer;
  transition: all 0.2s;
  background: white;
}

/* Modern Card Style */
.player-card.modern-card {
  border: 1px solid #ccc;
  border-radius: 6px;
  padding: 12px;
  background: white;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
  font-family: system-ui, -apple-system, sans-serif;
  font-size: 13px;
  line-height: 1.25;
  min-height: 100px;
}

.player-card.modern-card:hover {
  border-color: #1976d2;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
}

.player-card.modern-card.selected {
  border-color: #1976d2;
  background-color: #e3f2fd;
  box-shadow: 0 0 0 2px rgba(25, 118, 210, 0.2);
}

.modern-card .card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
  padding-bottom: 4px;
  border-bottom: 1px solid #eee;
}

.modern-card .player-name {
  font-weight: bold;
  font-size: 15px;
  color: #333;
  margin: 0;
}

.modern-card .player-number {
  font-weight: 600;
  font-size: 15px;
  color: white;
  margin: 0;
}

.modern-card .card-body {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.modern-card .time-stat,
.modern-card .points-stat,
.modern-card .fouls-stat,
.modern-card .eff-stat {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  gap: 0px;
  vertical-align: middle;
}

.modern-card .label {
  font-size: 18px;
  color: #666;
  font-weight: 500;
  min-width: 64px; /* keep labels aligned and values close */
}

.modern-card .value {
  font-size: 20px;
  color: #333;
  font-weight: 600;
}

.modern-card .foul-dots {
  display: flex;
  gap: 4px;
}

.modern-card .foul-dot {
  color: #dc3545;
  font-size: 18px;
  line-height: 1;
}

.player-card:hover {
  border-color: #1976d2;
}

.player-card.selected {
  border-color: #1976d2;
  background-color: #e3f2fd;
  box-shadow: 0 0 0 2px rgba(25, 118, 210, 0.2);
}

.player-info {
  text-align: center;
}

.player-name {
  font-weight: bold;
  font-size: 0.9rem;
  margin-bottom: 0.2rem;
  color: #333;
  text-align: left;
}

.player-stats {
  font-size: 0.75rem;
}

.time-fouls {
  margin-bottom: 0.3rem;
}

.fouls {
  margin-top: 0.2rem;
}

.efficiency {
  color: #666;
}

/* Action Buttons */
.action-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 0.3rem;
  justify-content: center;
}

.action-btn {
  padding: 0.4rem 0.8rem;
  border: none;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.action-btn.success {
  background-color: #4caf50;
  color: white;
}

.action-btn.miss {
  background-color: #f44336;
  color: white;
}

.action-btn.assist {
  background-color: #2196f3;
  color: white;
}

.action-btn.rebound {
  background-color: #ff9800;
  color: white;
}

.action-btn.steal {
  background-color: #9c27b0;
  color: white;
}

.action-btn.block {
  background-color: #607d8b;
  color: white;
}

.action-btn.foul {
  background-color: #795548;
  color: white;
}

.action-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(0,0,0,0.2);
}

/* Compact grouped layout for quick action buttons */
.action-buttons.compact {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  flex-wrap: nowrap;
  justify-content: space-between;
  width: 100%;
}

.action-btn.small {
  width: 44px;
  height: 36px;
  padding: 0.2rem 0;
  font-size: 0.9rem;
  border-radius: 6px;
}

.action-btn.medium {
  min-width: 70px;
  height: 36px;
  padding: 0 0.6rem;
  font-size: 0.9rem;
  border-radius: 6px;
}

.action-btn.large {
  min-width: 70px;
  height: 36px;
  padding: 0 0.8rem;
  font-size: 0.9rem;
  border-radius: 6px;
}

.spacer {
  flex: 1 1 auto;
  min-width: 8px;
}

.spacer-small {
  width: 8px;
  flex: 0 0 auto;
}

/* Right Panel */
.right-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.formation-defense-container {
  display: flex;
  gap: 1rem;
}

.formation-section,
.defense-section {
  flex: 1;
  background: white;
  border-radius: 8px;
  padding: 1rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.events-section {
  background: white;
  border-radius: 8px;
  padding: 1rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  flex: 1;
}

.formation-section h3,
.defense-section h3,
.events-section h3 {
  margin: 0 0 1rem 0;
  color: #333;
  border-bottom: 2px solid #1976d2;
  padding-bottom: 0.5rem;
}

.formation-options,
.defense-options {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.5rem;
}

.formation-btn,
.defense-btn {
  padding: 0.5rem;
  border: 2px solid #ddd;
  border-radius: 4px;
  background: white;
  cursor: pointer;
  text-align: center;
  font-weight: 500;
  transition: all 0.2s;
}

.formation-btn:hover,
.defense-btn:hover {
  border-color: #1976d2;
}

.formation-btn.active,
.defense-btn.active {
  border-color: #1976d2;
  background-color: #e3f2fd;
  color: #1976d2;
}

.events-list {
  max-height: 225px;
  overflow-y: auto;
  margin-bottom: 1rem;
}

.event-item {
  padding: 0.5rem;
  border-bottom: 1px solid #eee;
  display: flex;
  flex-direction: column;
  gap: 0.2rem;
}

.event-time {
  font-size: 0.8rem;
  color: #666;
  font-weight: 500;
}

.event-description {
  font-size: 0.85rem;
  color: #333;
}

.add-custom-event-btn {
  width: 100%;
  padding: 0.6rem;
  border: 2px dashed #ddd;
  border-radius: 4px;
  background: transparent;
  color: #666;
  cursor: pointer;
  font-size: 0.85rem;
}

.add-custom-event-btn:hover {
  border-color: #1976d2;
  color: #1976d2;
}

/* Automatic Recommendations */
.recommendations-section {
  margin-top: 1.5rem;
}

.recommendations-section h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.1rem;
  border-bottom: 2px solid #1976d2;
  padding-bottom: 0.5rem;
}

.recommendations-container {
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
}

.recommendation-card {
  border-radius: 6px;
  padding: 0.8rem;
  border-left: 3px solid;
  background: white;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  transition: transform 0.2s;
  font-size: 0.85rem;
}

.recommendation-card:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 6px rgba(0,0,0,0.15);
}

.recommendation-card.urgent {
  border-left-color: #dc3545;
  background-color: #fff5f5;
}

.recommendation-card.medium {
  border-left-color: #ffc107;
  background-color: #fffbf0;
}

.recommendation-card.low {
  border-left-color: #28a745;
  background-color: #f8fff8;
}

.recommendation-header {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  margin-bottom: 0.4rem;
}

.recommendation-icon {
  font-size: 1rem;
}

.recommendation-priority {
  font-size: 0.65rem;
  font-weight: bold;
  padding: 0.15rem 0.4rem;
  border-radius: 10px;
  text-transform: uppercase;
}

.recommendation-card.urgent .recommendation-priority {
  background-color: #dc3545;
  color: white;
}

.recommendation-card.medium .recommendation-priority {
  background-color: #ffc107;
  color: #212529;
}

.recommendation-card.low .recommendation-priority {
  background-color: #28a745;
  color: white;
}

.recommendation-time {
  font-size: 0.7rem;
  color: #666;
  margin-left: auto;
}

.recommendation-content h4 {
  margin: 0 0 0.3rem 0;
  color: #333;
  font-size: 0.9rem;
  font-weight: 600;
}

.recommendation-content p {
  margin: 0;
  color: #666;
  font-size: 0.8rem;
  line-height: 1.3;
}

.recommendation-actions {
  display: flex;
  gap: 0.4rem;
  margin-top: 0.8rem;
  justify-content: flex-end;
}

.btn-accept,
.btn-dismiss {
  padding: 0.3rem 0.8rem;
  border: none;
  border-radius: 3px;
  font-size: 0.75rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-accept {
  background-color: #28a745;
  color: white;
}

.btn-accept:hover {
  background-color: #218838;
}

.btn-dismiss {
  background-color: #6c757d;
  color: white;
}

.btn-dismiss:hover {
  background-color: #5a6268;
}

.no-recommendations {
  text-align: center;
  color: #666;
  padding: 1rem;
  font-style: italic;
  font-size: 0.85rem;
}

/* Team Statistics Section */
.team-statistics-section {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  margin-top: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.team-statistics-section h2 {
  margin: 0 0 2rem 0;
  color: #333;
  text-align: center;
  border-bottom: 3px solid #1976d2;
  padding-bottom: 1rem;
}

.team-stats-container {
  margin-bottom: 3rem;
}

.team-stats-container:last-child {
  margin-bottom: 0;
}

.team-stats-container h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.3rem;
  border-bottom: 2px solid #1976d2;
  padding-bottom: 0.5rem;
}

.stats-table-wrapper {
  overflow-x: auto;
}

.full-stats-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.85rem;
  min-width: 1200px;
}

.full-stats-table th,
.full-stats-table td {
  padding: 0.6rem 0.4rem;
  text-align: center;
  border: 1px solid #ddd;
}

.full-stats-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #333;
  position: sticky;
  top: 0;
  z-index: 10;
}

.full-stats-table tr:nth-child(even) {
  background-color: #f9f9f9;
}

.full-stats-table tr.active-player {
  background-color: #e3f2fd;
  font-weight: 500;
}

.full-stats-table tr.starting-player {
  background-color: #f3e5f5;
}

.full-stats-table tr.active-player td {
  border-color: #1976d2;
}

.player-cell {
  text-align: left !important;
  min-width: 120px;
}

.player-cell .player-name {
  font-weight: 600;
  color: #333;
  font-size: 0.9rem;
}

.player-cell .player-number {
  font-size: 0.8rem;
  color: #666;
  margin-top: 0.1rem;
}

.status-badge {
  padding: 0.2rem 0.5rem;
  border-radius: 10px;
  font-size: 0.7rem;
  font-weight: 500;
}

.status-badge.active {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.starter {
  background-color: #cce5ff;
  color: #004085;
}

.status-badge.bench {
  background-color: #f8f9fa;
  color: #6c757d;
}

.points {
  font-weight: bold;
  color: #1976d2;
}

.efficiency {
  font-weight: bold;
  color: #28a745;
}

/* Main Content Layout */
.main-content-layout {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 2rem;
  margin-top: 2rem;
}

/* Align with the game-interface grid layout above */
.game-interface {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 2rem;
  margin-bottom: 1rem;
}

.players-table-section {
  flex: 2;
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.players-table-section h2 {
  margin: 0 0 1.5rem 0;
  color: #333;
  font-size: 1.5rem;
  border-bottom: 2px solid #1976d2;
  padding-bottom: 0.5rem;
}

.team-stats-container {
  margin-bottom: 2rem;
}

.team-stats-container h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.2rem;
  font-weight: 600;
}

.stats-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.stats-table th {
  background-color: #f8f9fa;
  padding: 0.4rem 0.4rem;
  text-align: center;
  font-weight: 600;
  border-bottom: 2px solid #dee2e6;
  color: #333;
  font-size: 0.85rem;
}

.stats-table td {
  padding: 0.4rem 0.4rem;
  text-align: center;
  border-bottom: 1px solid #dee2e6;
  vertical-align: middle;
  font-size: 0.85rem;
}

.stats-table tbody tr:hover {
  background-color: #f8f9fa;
}

.recommendations-sidebar {
  flex: 1;
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  max-height: 600px;
  overflow-y: auto;
}

.recommendations-sidebar h3 {
  margin: 0 0 1.5rem 0;
  color: #333;
  font-size: 1.2rem;
  font-weight: 600;
  border-bottom: 2px solid #1976d2;
  padding-bottom: 0.5rem;
}

/* Responsive Design */
@media (max-width: 1200px) {
  .game-interface {
    flex-direction: column;
  }
  
  .main-content-layout {
    flex-direction: column;
    gap: 1.5rem;
  }
  
  .recommendations-sidebar {
    max-height: 400px;
  }
  
  .right-panel {
    flex-direction: column;
  }
  
  .formation-defense-container {
    flex-direction: row;
  }
  
  .events-section {
    flex: 1;
  }
}

@media (max-width: 768px) {
  .match-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
  
  .game-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .game-controls {
    flex-direction: column;
    gap: 1rem;
  }

  .center-controls {
    flex-wrap: wrap;
    justify-content: center;
  }
  
  .active-players-grid {
    grid-template-columns: repeat(3, 1fr);
  }
  
  .formation-defense-container {
    flex-direction: column;
  }
  
  .action-buttons {
    justify-content: flex-start;
  }

  .full-stats-table {
    font-size: 0.75rem;
  }

  .full-stats-table th,
  .full-stats-table td {
    padding: 0.4rem 0.2rem;
  }
}

/* Enhanced Statistics Table Styles */
.stats-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
  font-size: 0.9rem;
}

.stats-table th {
  background-color: #f8f9fa;
  border: 1px solid #dee2e6;
  padding: 0.75rem 0.5rem;
  text-align: center;
  font-weight: 600;
  color: #495057;
  font-size: 0.85rem;
}

.stats-table td {
  border: 1px solid #dee2e6;
  padding: 0.6rem 0.5rem;
  text-align: center;
  vertical-align: middle;
}

.stats-table tbody tr:hover {
  background-color: #f5f5f5;
}

.stats-table tr.active-player {
  background-color: #e8f5e8;
}

.stats-table tr.active-player td {
  font-weight: 500;
}

.player-cell {
  text-align: left !important;
  min-width: 140px;
}

.player-cell .player-name {
  font-weight: 500;
}

.status-badge {
  display: inline-block;
  padding: 0.15rem 0.4rem;
  border-radius: 10px;
  font-size: 0.7rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-badge.active {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.starter {
  background-color: #fff3cd;
  color: #856404;
}

.status-badge.bench {
  background-color: #f8d7da;
  color: #721c24;
}

.points {
  font-weight: 600;
  color: #007bff;
}

.efficiency {
  font-weight: 600;
  color: #28a745;
}

.efficiency.negative-eff {
  color: #dc3545;
}

/* Modern Basketball Stats Table */
.modern-stats {
  background: #ffffff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.modern-stats th {
  background: #f8f9fa;
  font-weight: 600;
  font-size: 11px;
  color: #6c757d;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 12px 8px;
  border-bottom: 2px solid #e9ecef;
}

.modern-stats th.player-header {
  text-align: left;
  width: 200px;
}

.modern-stats td {
  padding: 8px 8px;
  vertical-align: middle;
}

.player-info-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}
.player-info-cell .player-details .player-name {
  margin-bottom: 0
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
span.player-number{
  color: red;
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
}

.foul-dots {
  display: flex;
  gap: 2px;
}

.foul-dot {
  color: #dc3545;
  font-size: 18px;
  line-height: 1;
}

.status-container {
  flex-shrink: 0;
}

.status-badge {
  font-size: 10px;
  font-weight: 600;
  text-transform: uppercase;
  padding: 2px 6px;
  border-radius: 10px;
  letter-spacing: 0.5px;
}

.status-badge.active {
  background: #d4edda;
  color: #155724;
}

.status-badge.bench {
  background: #f8d7da;
  color: #721c24;
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
  line-height: 1.2;
}

.reb-cell {
  text-align: center;
}

.reb-total {
  font-weight: 600;
  font-size: 14px;
  color: #212529;
  line-height: 1.2;
}

.reb-breakdown {
  font-size: 11px;
  color: #6c757d;
  line-height: 1.2;
}

.simple-stat {
  text-align: center;
  font-weight: 500;
  font-size: 14px;
  color: #212529;
}

.points-cell {
  text-align: center;
  font-weight: 700;
  font-size: 16px;
  color: #007bff;
}

.modern-stats tr:hover {
  background-color: #f8f9fa;
}

.modern-stats tr.active-player {
  background-color: #e8f5e8;
}

.modern-stats tr.active-player:hover {
  background-color: #d4edda;
}

</style>
