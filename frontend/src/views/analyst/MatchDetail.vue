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
          <button class="control-btn starting-five" @click="openStartingFiveModal">
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
              class="timer-btn-inline"
              :class="{ 'start': !timerRunning, 'stop': timerRunning }"
              @click="toggleTimer"
            >
              {{ timerRunning ? '⏹ Stop' : '▶ Start' }}
            </button>
          </div>
          <div class="score-display">
            <div class="team-score home">
              <span class="team-name-large">Partizan</span>
              <span class="score">{{ match?.ourPoints }}</span>
            </div>
            <div class="vs">-</div>
            <div class="team-score away">
              <span class="team-name-large">{{ opponentTeam }}</span>
              <span class="score">{{ match?.opponentPoints }}</span>
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
              class="control-btn timeout-btn partizan"
              @click="callTimeout('partizan')"
            >
              Timeout - Partizan
            </button>
            <button 
              v-if="gameStatus === 'Playing'"
              class="control-btn pause-btn"
              @click="pauseGame"
            >
              ⏸ Pause Game
            </button>
            <button 
              v-else-if="gameStatus === 'Paused'"
              class="control-btn resume-btn"
              @click="resumeGame"
            >
              ▶ Resume Game
            </button>
            <button 
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
              <button class="substitution-btn">Substitution</button>
              <button class="undo-btn">UNDO</button>
            </div>
            <div class="active-players-grid">
              <div 
                v-for="player in activeOurPlayers" 
                :key="player.id"
                class="player-card"
                :class="{ 'selected': selectedPlayer?.id === player.id && selectedPlayer?.team === 'our' }"
                @click="selectPlayer(player, 'our')"
              >
                <div class="player-info">
                  <div class="player-name">{{ player.name }}</div>
                  <div class="player-number">#{{ player.number }}</div>
                  <div class="player-stats">
                    <div class="time-fouls">
                      <span>in game: {{ player.timeInGame }}</span>
                      <div class="fouls">
                        <span v-for="foul in player.fouls" :key="foul" class="foul-dot">●</span>
                      </div>
                    </div>
                    <div class="efficiency">eff: {{ player.eff }}</div>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Action Buttons for Our Team -->
            <div class="action-buttons compact">
              <button class="action-btn small success" title="+2pt" @click="recordAction('2p_made')">+2p</button>
              <button class="action-btn small miss" title="2pt" @click="recordAction('2p_miss')">2p</button>
              <button class="action-btn small success" title="+3pt" @click="recordAction('3p_made')">+3p</button>
              <button class="action-btn small miss" title="3pt" @click="recordAction('3p_miss')">3p</button>
              <button class="action-btn small success" title="+FT" @click="recordAction('ft_made')">+ft</button>
              <button class="action-btn small miss" title="FT" @click="recordAction('ft_miss')">ft</button>

              <div class="spacer"></div>

              <button class="action-btn medium assist" title="Assist" @click="recordAction('assist')">asist</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium rebound" title="Offensive rebound" @click="recordAction('reb_off')">reb of</button>
              <button class="action-btn medium rebound" title="Defensive rebound" @click="recordAction('reb_def')">reb def</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium steal" title="Steal" @click="recordAction('steal')">steal</button>
              <button class="action-btn medium block" title="Block" @click="recordAction('block')">block</button>

              <div class="spacer"></div>

              <button class="action-btn large foul" title="Foul" @click="recordAction('foul')">foul</button>
            </div>
          </div>

          <!-- Opponent Team Active Players -->
          <div class="team-players-container">
            <div class="team-header">
              <h3>{{ opponentTeam }}</h3>
              <button class="substitution-btn">Substitution</button>
              <button class="undo-btn">UNDO</button>
            </div>
            <div class="active-players-grid">
              <div 
                v-for="player in activeOpponentPlayers" 
                :key="player.id"
                class="player-card"
                :class="{ 'selected': selectedPlayer?.id === player.id && selectedPlayer?.team === 'opponent' }"
                @click="selectPlayer(player, 'opponent')"
              >
                <div class="player-info">
                  <div class="player-name">{{ player.name }}</div>
                  <div class="player-number">#{{ player.number }}</div>
                  <div class="player-stats">
                    <div class="time-fouls">
                      <span>in game: {{ player.timeInGame }}</span>
                      <div class="fouls">
                        <span v-for="foul in player.fouls" :key="foul" class="foul-dot">●</span>
                      </div>
                    </div>
                    <div class="efficiency">eff: {{ player.eff }}</div>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Action Buttons for Opponent Team -->
            <div class="action-buttons compact">
              <button class="action-btn small success" title="+2pt" @click="recordAction('2p_made')">+2p</button>
              <button class="action-btn small miss" title="2pt" @click="recordAction('2p_miss')">2p</button>
              <button class="action-btn small success" title="+3pt" @click="recordAction('3p_made')">+3p</button>
              <button class="action-btn small miss" title="3pt" @click="recordAction('3p_miss')">3p</button>
              <button class="action-btn small success" title="+FT" @click="recordAction('ft_made')">+ft</button>
              <button class="action-btn small miss" title="FT" @click="recordAction('ft_miss')">ft</button>

              <div class="spacer"></div>

              <button class="action-btn medium assist" title="Assist" @click="recordAction('assist')">asist</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium rebound" title="Offensive rebound" @click="recordAction('reb_off')">reb of</button>
              <button class="action-btn medium rebound" title="Defensive rebound" @click="recordAction('reb_def')">reb def</button>

              <div class="spacer-small"></div>

              <button class="action-btn medium steal" title="Steal" @click="recordAction('steal')">steal</button>
              <button class="action-btn medium block" title="Block" @click="recordAction('block')">block</button>

              <div class="spacer"></div>

              <button class="action-btn large foul" title="Foul" @click="recordAction('foul')">foul</button>
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
                v-for="event in gameEvents" 
                :key="event.id"
                class="event-item"
              >
                <span class="event-time">{{ event.time }}</span>
                <span class="event-description">{{ event.description }}</span>
              </div>
            </div>
            <button class="add-custom-event-btn">Add custom event</button>
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
              <table class="stats-table">
                <thead>
                  <tr>
                    <th>Name</th>
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
                  <tr v-for="player in fullOurTeamStats" :key="player.id">
                    <td class="player-cell">
                      <div class="player-name">{{ player.name }} (#{{ player.number }})</div>
                      <!-- <div class="player-number">#{{ player.number }}</div> -->
                    </td>
                    <td>{{ player.eff }}</td>
                    <td>{{ player.fg }}</td>
                    <td>{{ player.twop }}</td>
                    <td>{{ player.threep }}</td>
                    <td>{{ player.ft }}</td>
                    <td>{{ player.rebOff }}/{{ player.rebDef }}</td>
                    <td>{{ player.ast }}</td>
                    <td>{{ player.stl }}</td>
                    <td>{{ player.blk }}</td>
                    <td>{{ player.pts }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Opponent Team Full Stats -->
          <div class="team-stats-container">
            <h3>{{ opponentTeam }}</h3>
            <div class="stats-table-wrapper">
              <table class="stats-table">
                <thead>
                  <tr>
                    <th>Name</th>
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
                  <tr v-for="player in fullOpponentTeamStats" :key="player.id">
                    <td class="player-cell">
                      <div class="player-name">{{ player.name }} (#{{ player.number }})</div>
                      <!-- <div class="player-number">#{{ player.number }}</div> -->
                    </td>
                    <td>{{ player.eff }}</td>
                    <td>{{ player.fg }}</td>
                    <td>{{ player.twop }}</td>
                    <td>{{ player.threep }}</td>
                    <td>{{ player.ft }}</td>
                    <td>{{ player.rebOff }}/{{ player.rebDef }}</td>
                    <td>{{ player.ast }}</td>
                    <td>{{ player.stl }}</td>
                    <td>{{ player.blk }}</td>
                    <td>{{ player.pts }}</td>
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
  <div v-if="showStartingFiveModal" class="modal-overlay" @click="closeStartingFiveModal">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>Match starting five</h2>
        <p class="match-info">{{ match?.name || 'Match' }}</p>
        <p class="match-details">Date: {{ formatModalDate() }}</p>
        <p class="match-details">Place: {{ isHomeMatch ? 'Home' : 'Away' }}</p>
        <button class="modal-close" @click="closeStartingFiveModal">×</button>
      </div>
      
      <div class="modal-body">
        <div class="teams-selection">
          <!-- Our Team (Partizan) -->
          <div class="team-selection">
            <h3>Our Team - Partizan</h3>
            <div class="player-list">
              <div 
                v-for="player in fullOurTeamStats" 
                :key="player.id"
                class="player-item"
                :class="{ 'selected': selectedOurStartingFive.includes(player.id) }"
                @click="toggleOurPlayerSelection(player.id)"
              >
                <span class="player-checkbox">
                  <input 
                    type="checkbox" 
                    :checked="selectedOurStartingFive.includes(player.id)"
                    @click.stop
                    @change="toggleOurPlayerSelection(player.id)"
                  >
                </span>
                <span class="player-name">{{ player.name }}</span>
                <span class="player-number">#{{ player.number }}</span>
              </div>
            </div>
            <p class="selection-count">Selected: {{ selectedOurStartingFive.length }}/5</p>
          </div>

          <!-- Opponent Team -->
          <div class="team-selection">
            <h3>{{ opponentTeam }}</h3>
            <div class="player-list">
              <div 
                v-for="player in fullOpponentTeamStats" 
                :key="player.id"
                class="player-item"
                :class="{ 'selected': selectedOpponentStartingFive.includes(player.id) }"
                @click="toggleOpponentPlayerSelection(player.id)"
              >
                <span class="player-checkbox">
                  <input 
                    type="checkbox" 
                    :checked="selectedOpponentStartingFive.includes(player.id)"
                    @click.stop
                    @change="toggleOpponentPlayerSelection(player.id)"
                  >
                </span>
                <span class="player-name">{{ player.name }}</span>
                <span class="player-number">#{{ player.number }}</span>
              </div>
            </div>
            <p class="selection-count">Selected: {{ selectedOpponentStartingFive.length }}/5</p>
          </div>
        </div>
      </div>
      
      <div class="modal-footer">
        <button 
          class="btn-decline" 
          @click="closeStartingFiveModal"
        >
          Decline
        </button>
        <button 
          class="btn-accept" 
          :disabled="!canSubmitStartingFive"
          @click="submitStartingFive"
        >
          Accept
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const error = ref(null)
const match = ref(null)

// Match data (will be updated from API)
const isHomeMatch = ref(false)
const opponentTeam = ref('Opponent')
const currentMatchStatus = ref('upcoming')
const currentPeriod = ref('1st')
const currentTime = ref('10:00')
const gameStatus = ref('Not Started')
const timerRunning = ref(false)

// Selected player for actions
const selectedPlayer = ref(null)

// Formation and Defense
const formations = ref(['1-4', '2-3', '3-2', '5 out'])
const selectedFormation = ref('3-2')
const defenseTypes = ref(['Individual', 'Zone', 'Combined'])
const selectedDefense = ref('Zone')

// Active players (5 per team)
const activeOurPlayers = ref([
  { 
    id: 1, 
    name: 'Nikola Jokic', 
    number: 15, 
    timeInGame: '2:36',
    fouls: 3,
    eff: 16
  },
  { 
    id: 2, 
    name: 'Bogdan Bogd.', 
    number: 7, 
    timeInGame: '3:23',
    fouls: 3,
    eff: 8
  },
  { 
    id: 3, 
    name: 'Marko Petrovic', 
    number: 5, 
    timeInGame: '0:30',
    fouls: 1,
    eff: 16
  },
  { 
    id: 4, 
    name: 'Milos Teodosic', 
    number: 4, 
    timeInGame: '1:20',
    fouls: 4,
    eff: 4
  },
  { 
    id: 5, 
    name: 'Stefan Nikolic', 
    number: 12, 
    timeInGame: '0:52',
    fouls: 2,
    eff: 6
  }
])

const activeOpponentPlayers = ref([
  { 
    id: 6, 
    name: 'Vanja Marinkovic', 
    number: 1, 
    timeInGame: '2:36',
    fouls: 2,
    eff: 16
  },
  { 
    id: 7, 
    name: 'Mario Nakic', 
    number: 7, 
    timeInGame: '3:23',
    fouls: 4,
    eff: 8
  },
  { 
    id: 8, 
    name: 'Isaac Bonga', 
    number: 17, 
    timeInGame: '0:30',
    fouls: 1,
    eff: 16
  },
  { 
    id: 9, 
    name: 'Balsa Koprivica', 
    number: 5, 
    timeInGame: '1:20',
    fouls: 3,
    eff: 4
  },
  { 
    id: 10, 
    name: 'Marko Markovic', 
    number: 1, 
    timeInGame: '0:52',
    fouls: 2,
    eff: 6
  }
])

// Game events
const gameEvents = ref([
  { id: 1, time: '2:35 1Q', description: 'Nikola Jokic +3p' },
  { id: 2, time: '2:23 1Q', description: 'Substitution Bogdanovic -> Jokic' },
  { id: 3, time: '9:56 2Q', description: 'Avramovic 2P shot (missed)' }
])

// Full team statistics (all players)
const fullOurTeamStats = ref([
  { id: 1, name: 'Nikola Jokic', number: 15, isActive: true, isStarter: true, minutes: '12:36', points: 18, fg: '7/12', twoP: '5/8', threeP: '2/4', ft: '4/4', offReb: 2, defReb: 6, totalReb: 8, assists: 5, steals: 1, blocks: 2, turnovers: 2, fouls: 2, efficiency: 24 },
  { id: 2, name: 'Bogdan Bogdanovic', number: 7, isActive: true, isStarter: true, minutes: '11:23', points: 15, fg: '5/10', twoP: '2/4', threeP: '3/6', ft: '2/2', offReb: 0, defReb: 3, totalReb: 3, assists: 4, steals: 2, blocks: 0, turnovers: 1, fouls: 1, efficiency: 18 },
  { id: 3, name: 'Marko Petrovic', number: 5, isActive: true, isStarter: true, minutes: '8:30', points: 8, fg: '3/7', twoP: '2/4', threeP: '1/3', ft: '1/2', offReb: 1, defReb: 2, totalReb: 3, assists: 2, steals: 0, blocks: 1, turnovers: 0, fouls: 3, efficiency: 9 },
  { id: 4, name: 'Milos Teodosic', number: 4, isActive: true, isStarter: true, minutes: '10:20', points: 12, fg: '4/8', twoP: '1/3', threeP: '3/5', ft: '1/1', offReb: 0, defReb: 1, totalReb: 1, assists: 6, steals: 1, blocks: 0, turnovers: 3, fouls: 2, efficiency: 13 },
  { id: 5, name: 'Stefan Nikolic', number: 12, isActive: true, isStarter: true, minutes: '7:52', points: 4, fg: '2/5', twoP: '2/4', threeP: '0/1', ft: '0/0', offReb: 2, defReb: 3, totalReb: 5, assists: 1, steals: 0, blocks: 1, turnovers: 1, fouls: 1, efficiency: 7 },
  { id: 6, name: 'Aleksa Avramovic', number: 22, isActive: false, isStarter: false, minutes: '5:15', points: 6, fg: '2/4', twoP: '1/2', threeP: '1/2', ft: '1/1', offReb: 0, defReb: 1, totalReb: 1, assists: 1, steals: 1, blocks: 0, turnovers: 0, fouls: 0, efficiency: 8 },
  { id: 7, name: 'Filip Petrusev', number: 14, isActive: false, isStarter: false, minutes: '3:45', points: 2, fg: '1/2', twoP: '1/2', threeP: '0/0', ft: '0/0', offReb: 1, defReb: 2, totalReb: 3, assists: 0, steals: 0, blocks: 0, turnovers: 0, fouls: 1, efficiency: 4 },
  { id: 8, name: 'Dusan Ristic', number: 16, isActive: false, isStarter: false, minutes: '2:30', points: 0, fg: '0/1', twoP: '0/1', threeP: '0/0', ft: '0/0', offReb: 0, defReb: 1, totalReb: 1, assists: 0, steals: 0, blocks: 1, turnovers: 0, fouls: 0, efficiency: 1 }
])

const fullOpponentTeamStats = ref([
  { id: 9, name: 'Vanja Marinkovic', number: 1, isActive: true, isStarter: true, minutes: '11:36', points: 14, fg: '5/9', twoP: '2/4', threeP: '3/5', ft: '1/1', offReb: 0, defReb: 4, totalReb: 4, assists: 3, steals: 1, blocks: 0, turnovers: 1, fouls: 2, efficiency: 17 },
  { id: 10, name: 'Mario Nakic', number: 7, isActive: true, isStarter: true, minutes: '10:23', points: 20, fg: '8/13', twoP: '5/7', threeP: '3/6', ft: '1/2', offReb: 2, defReb: 5, totalReb: 7, assists: 4, steals: 0, blocks: 1, turnovers: 2, fouls: 3, efficiency: 22 },
  { id: 11, name: 'Isaac Bonga', number: 17, isActive: true, isStarter: true, minutes: '9:30', points: 8, fg: '3/6', twoP: '2/3', threeP: '1/3', ft: '1/2', offReb: 1, defReb: 3, totalReb: 4, assists: 2, steals: 1, blocks: 0, turnovers: 0, fouls: 1, efficiency: 12 },
  { id: 12, name: 'Balsa Koprivica', number: 5, isActive: true, isStarter: true, minutes: '8:20', points: 10, fg: '4/7', twoP: '3/4', threeP: '1/3', ft: '1/1', offReb: 0, defReb: 2, totalReb: 2, assists: 1, steals: 0, blocks: 2, turnovers: 1, fouls: 2, efficiency: 11 },
  { id: 13, name: 'Marko Markovic', number: 1, isActive: true, isStarter: true, minutes: '6:52', points: 6, fg: '2/4', twoP: '2/3', threeP: '0/1', ft: '2/2', offReb: 1, defReb: 2, totalReb: 3, assists: 0, steals: 1, blocks: 0, turnovers: 0, fouls: 1, efficiency: 8 },
  { id: 14, name: 'Nemanja Nedovic', number: 8, isActive: false, isStarter: false, minutes: '4:15', points: 3, fg: '1/3', twoP: '0/1', threeP: '1/2', ft: '0/0', offReb: 0, defReb: 1, totalReb: 1, assists: 2, steals: 0, blocks: 0, turnovers: 1, fouls: 0, efficiency: 4 },
  { id: 15, name: 'Ognjen Dobric', number: 33, isActive: false, isStarter: false, minutes: '3:45', points: 2, fg: '1/2', twoP: '1/2', threeP: '0/0', ft: '0/0', offReb: 0, defReb: 0, totalReb: 0, assists: 1, steals: 0, blocks: 0, turnovers: 0, fouls: 1, efficiency: 2 }
])

// Automatic recommendations (hardcoded for now)
const automaticRecommendations = ref([
  {
    id: 1,
    priority: 'urgent',
    title: 'Focus on defense 3-point!',
    description: 'Opponent shoots 47% for 3-point - too many open shots',
    icon: '🔴',
    timestamp: new Date().toLocaleTimeString()
  },
  {
    id: 2,
    priority: 'medium',
    title: 'Better usage of offensive rebounds',
    description: 'We have more rebounds but we don\'t make points off them',
    icon: '⚠️',
    timestamp: new Date(Date.now() - 60000).toLocaleTimeString()
  },
  {
    id: 3,
    priority: 'low',
    title: 'Continue playing aggressive',
    description: 'Great efficiency of free throws',
    icon: '✅',
    timestamp: new Date(Date.now() - 120000).toLocaleTimeString()
  },
  {
    id: 4,
    priority: 'medium',
    title: 'Consider substitution',
    description: 'Stefan Nikolic has 4 fouls - risk of disqualification',
    icon: '⚠️',
    timestamp: new Date(Date.now() - 180000).toLocaleTimeString()
  }
])

// Functions to manage recommendations
const acceptRecommendation = (recommendationId) => {
  const recommendation = automaticRecommendations.value.find(r => r.id === recommendationId)
  if (recommendation) {
    console.log('Accepted recommendation:', recommendation.title)
    // Add to game events
    const event = {
      id: gameEvents.value.length + 1,
      time: `${currentTime.value} ${currentPeriod.value}`,
      description: `Accepted: ${recommendation.title}`
    }
    gameEvents.value.unshift(event)
    
    // Remove from recommendations
    automaticRecommendations.value = automaticRecommendations.value.filter(r => r.id !== recommendationId)
  }
}

const dismissRecommendation = (recommendationId) => {
  automaticRecommendations.value = automaticRecommendations.value.filter(r => r.id !== recommendationId)
  console.log('Dismissed recommendation:', recommendationId)
}

// Starting Five Modal
const showStartingFiveModal = ref(false)
const selectedOurStartingFive = ref([])
const selectedOpponentStartingFive = ref([])

// Computed property to check if both teams have exactly 5 players selected
const canSubmitStartingFive = computed(() => {
  return selectedOurStartingFive.value.length === 5 && selectedOpponentStartingFive.value.length === 5
})

// Modal functions
const openStartingFiveModal = () => {
  showStartingFiveModal.value = true
  selectedOurStartingFive.value = []
  selectedOpponentStartingFive.value = []
}

const closeStartingFiveModal = () => {
  showStartingFiveModal.value = false
  selectedOurStartingFive.value = []
  selectedOpponentStartingFive.value = []
}

const toggleOurPlayerSelection = (playerId) => {
  const index = selectedOurStartingFive.value.indexOf(playerId)
  if (index > -1) {
    selectedOurStartingFive.value.splice(index, 1)
  } else if (selectedOurStartingFive.value.length < 5) {
    selectedOurStartingFive.value.push(playerId)
  }
}

const toggleOpponentPlayerSelection = (playerId) => {
  const index = selectedOpponentStartingFive.value.indexOf(playerId)
  if (index > -1) {
    selectedOpponentStartingFive.value.splice(index, 1)
  } else if (selectedOpponentStartingFive.value.length < 5) {
    selectedOpponentStartingFive.value.push(playerId)
  }
}

const formatModalDate = () => {
  if (!match.value?.scheduledAt) return 'TBD'
  const date = new Date(match.value.scheduledAt)
  return date.toLocaleDateString('sr-RS', {
    day: '2-digit',
    month: '2-digit', 
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const submitStartingFive = async () => {
  if (!canSubmitStartingFive.value) return
  
  try {
    const matchId = route.params.id
    
    // Prepare data for API - format according to backend structure
    const playersData = []
    
    // Add our team players
    fullOurTeamStats.value.forEach(player => {
      const isSelected = selectedOurStartingFive.value.includes(player.id)
      playersData.push({
        teamId: 1, // Use actual team ID
        playerId: player.id,
        startingLineup: isSelected,
        inGame: isSelected // Set as in game if they're starters
      })
    })
    console.log('MATCH VALUE:', match.value)
    // Add opponent team players
    fullOpponentTeamStats.value.forEach(player => {
      const isSelected = selectedOpponentStartingFive.value.includes(player.id)
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
    updateLocalStartingLineup()
    
    // Close modal
    closeStartingFiveModal()
    
    console.log('Starting five updated successfully')
  } catch (err) {
    console.error('Error updating starting five:', err)
    alert('Failed to update starting five. Please try again.')
  }
}

const updateLocalStartingLineup = () => {
  // Update our team players
  fullOurTeamStats.value.forEach(player => {
    const isStarter = selectedOurStartingFive.value.includes(player.id)
    player.isStarter = isStarter
    player.isActive = isStarter // Set as active if they're starters
  })
  
  // Update opponent team players
  fullOpponentTeamStats.value.forEach(player => {
    const isStarter = selectedOpponentStartingFive.value.includes(player.id)
    player.isStarter = isStarter
    player.isActive = isStarter // Set as active if they're starters
  })
  
  // Update active players arrays
  activeOurPlayers.value = fullOurTeamStats.value.filter(p => p.isActive)
  activeOpponentPlayers.value = fullOpponentTeamStats.value.filter(p => p.isActive)
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
    
    // Fetch team members for this match
    await fetchTeamMembers(matchId)
    
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
    
    console.log('Team members:', teamMembers)
    
    // Separate our team (Partizan - team ID 1) and opponent team
    const ourTeamId = 1
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
      offReb: 0,
      defReb: 0,
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
    
    console.log('Our team players:', activeOurPlayers.value)
    console.log('Opponent team players:', activeOpponentPlayers.value)
    
  } catch (err) {
    console.error('Error fetching team members:', err)
    // Keep hardcoded data as fallback
  }
}

// Player selection
const selectPlayer = (player, team) => {
  selectedPlayer.value = { ...player, team }
  console.log('Selected player:', selectedPlayer.value)
}

// Record action
const recordAction = (action) => {
  if (!selectedPlayer.value) {
    alert('Please select a player first')
    return
  }
  
  const event = {
    id: gameEvents.value.length + 1,
    time: `${currentTime.value} ${currentPeriod.value}`,
    description: `${selectedPlayer.value.name} ${action.replace('_', ' ')}`
  }
  
  gameEvents.value.unshift(event)
  console.log('Recorded action:', action, 'for player:', selectedPlayer.value.name)
}

// Formation selection
const selectFormation = (formation) => {
  selectedFormation.value = formation
  console.log('Selected formation:', formation)
}

// Defense selection
const selectDefense = (defense) => {
  selectedDefense.value = defense
  console.log('Selected defense:', defense)
}

// Game control functions
const callTimeout = (team) => {
  const event = {
    id: gameEvents.value.length + 1,
    time: `${currentTime.value} ${currentPeriod.value}`,
    description: `Timeout called by ${team === 'partizan' ? 'Partizan' : opponentTeam.value}`
  }
  gameEvents.value.unshift(event)
  console.log('Timeout called by:', team)
}

const pauseGame = () => {
  gameStatus.value = 'Paused'
  const event = {
    id: gameEvents.value.length + 1,
    time: `${currentTime.value} ${currentPeriod.value}`,
    description: 'Game paused'
  }
  gameEvents.value.unshift(event)
  console.log('Game paused')
}

const resumeGame = () => {
  gameStatus.value = 'Playing'
  const event = {
    id: gameEvents.value.length + 1,
    time: `${currentTime.value} ${currentPeriod.value}`,
    description: 'Game resumed'
  }
  gameEvents.value.unshift(event)
  console.log('Game resumed')
}

const toggleTimer = () => {
  timerRunning.value = !timerRunning.value
  
  if (timerRunning.value) {
    gameStatus.value = 'Playing'
    const event = {
      id: gameEvents.value.length + 1,
      time: `${currentTime.value} ${currentPeriod.value}`,
      description: 'Match timer started'
    }
    gameEvents.value.unshift(event)
    console.log('Timer started')
  } else {
    gameStatus.value = 'Paused'
    const event = {
      id: gameEvents.value.length + 1,
      time: `${currentTime.value} ${currentPeriod.value}`,
      description: 'Match timer stopped'
    }
    gameEvents.value.unshift(event)
    console.log('Timer stopped')
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
}

.timeout-btn.opponent {
  background-color: #dc3545;
  color: white;
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
  padding: 0.4rem 0.8rem;
  border-radius: 4px;
  font-size: 0.85rem;
  cursor: pointer;
  margin-right: 0.5rem;
}

.undo-btn {
  background-color: #f44336;
  color: white;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 4px;
  font-size: 0.85rem;
  cursor: pointer;
}

.active-players-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 0.5rem;
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
  margin-bottom: 0.3rem;
  color: #333;
}

.player-number {
  font-size: 0.8rem;
  color: #666;
  margin-bottom: 0.5rem;
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

.foul-dot {
  color: #333;
  margin-right: 0.1rem;
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
  max-height: 400px;
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

/* Starting Five Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #dee2e6;
  text-align: center;
  position: relative;
}

.modal-header h2 {
  margin: 0 0 0.5rem 0;
  color: #333;
  font-size: 1.5rem;
}

.match-info {
  margin: 0;
  font-size: 1.1rem;
  color: #333;
  font-weight: 500;
}

.match-details {
  margin: 0.2rem 0;
  color: #666;
  font-size: 0.9rem;
}

.modal-close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-close:hover {
  background-color: #f8f9fa;
  color: #333;
}

.modal-body {
  padding: 1.5rem;
}

.teams-selection {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.team-selection {
  border: 2px solid #dee2e6;
  border-radius: 8px;
  padding: 1rem;
}

.team-selection h3 {
  margin: 0 0 1rem 0;
  text-align: center;
  color: #333;
  border-bottom: 2px solid #1976d2;
  padding-bottom: 0.5rem;
}

.player-list {
  max-height: 300px;
  overflow-y: auto;
  border: 1px solid #dee2e6;
  border-radius: 4px;
}

.player-item {
  display: flex;
  align-items: center;
  padding: 0.8rem;
  border-bottom: 1px solid #dee2e6;
  cursor: pointer;
  transition: background-color 0.2s;
}

.player-item:last-child {
  border-bottom: none;
}

.player-item:hover {
  background-color: #f8f9fa;
}

.player-item.selected {
  background-color: #e3f2fd;
  font-weight: 500;
}

.player-checkbox {
  margin-right: 0.8rem;
}

.player-checkbox input[type="checkbox"] {
  width: 16px;
  height: 16px;
  cursor: pointer;
}

.player-name {
  flex: 1;
  text-align: left;
}

.player-number {
  color: #666;
  font-size: 0.9rem;
  font-weight: 500;
}

.selection-count {
  margin: 0.8rem 0 0 0;
  text-align: center;
  font-size: 0.9rem;
  color: #666;
  font-weight: 500;
}

.modal-footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid #dee2e6;
  display: flex;
  justify-content: space-between;
  gap: 1rem;
}

.btn-accept,
.btn-decline {
  padding: 0.8rem 2rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  min-width: 120px;
}

.btn-accept {
  background-color: #28a745;
  color: white;
}

.btn-accept:hover:not(:disabled) {
  background-color: #218838;
}

.btn-accept:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
  opacity: 0.6;
}

.btn-decline {
  background-color: #6c757d;
  color: white;
}

.btn-decline:hover {
  background-color: #5a6268;
}

/* Main Content Layout */
.main-content-layout {
  display: flex;
  gap: 2rem;
  margin-top: 2rem;
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
</style>
