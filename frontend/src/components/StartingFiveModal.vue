<template>
  <div v-if="isVisible" class="modal-overlay" @click="closeModal">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>Match starting five</h2>
        <p class="match-info">{{ matchName || 'Match' }}</p>
        <p class="match-details">Date: {{ formattedDate }}</p>
        <p class="match-details">Place: {{ isHomeMatch ? 'Home' : 'Away' }}</p>
        <button class="modal-close" @click="closeModal">×</button>
      </div>
      
      <div class="modal-body">
        <div class="teams-selection">
          <!-- Our Team (Partizan) -->
          <div class="team-selection">
            <h3>Our Team - Partizan</h3>
            <div class="player-list">
              <div 
                v-for="player in ourTeamPlayers" 
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
                v-for="player in opponentTeamPlayers" 
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
          @click="closeModal"
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
import { ref, computed, watch } from 'vue'

// Props
const props = defineProps({
  isVisible: {
    type: Boolean,
    default: false
  },
  matchName: {
    type: String,
    default: ''
  },
  scheduledAt: {
    type: String,
    default: null
  },
  isHomeMatch: {
    type: Boolean,
    default: false
  },
  opponentTeam: {
    type: String,
    default: 'Unknown Opponent'
  },
  ourTeamPlayers: {
    type: Array,
    default: () => []
  },
  opponentTeamPlayers: {
    type: Array,
    default: () => []
  }
})

// Emits
const emit = defineEmits(['close', 'submit'])

// Local state
const selectedOurStartingFive = ref([])
const selectedOpponentStartingFive = ref([])

// Computed properties
const canSubmitStartingFive = computed(() => {
  return selectedOurStartingFive.value.length === 5 && selectedOpponentStartingFive.value.length === 5
})

const formattedDate = computed(() => {
  if (!props.scheduledAt) return 'TBD'
  const date = new Date(props.scheduledAt)
  return date.toLocaleDateString('sr-RS', {
    day: '2-digit',
    month: '2-digit', 
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
})

// Watch for modal visibility to reset selections
watch(() => props.isVisible, (newVal) => {
  if (newVal) {
    selectedOurStartingFive.value = []
    selectedOpponentStartingFive.value = []
  }
})

// Methods
const closeModal = () => {
  selectedOurStartingFive.value = []
  selectedOpponentStartingFive.value = []
  emit('close')
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

const submitStartingFive = () => {
  if (!canSubmitStartingFive.value) return
  
  emit('submit', {
    ourTeamPlayers: selectedOurStartingFive.value,
    opponentTeamPlayers: selectedOpponentStartingFive.value
  })
  
  closeModal()
}
</script>

<style scoped>
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
  border-radius: 12px;
  width: 90%;
  max-width: 900px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #dee2e6;
  position: relative;
}

.modal-header h2 {
  margin: 0 0 0.5rem 0;
  color: #333;
  font-size: 1.5rem;
}

.match-info {
  font-size: 1.1rem;
  color: #666;
  margin: 0.3rem 0;
}

.match-details {
  font-size: 0.9rem;
  color: #666;
  margin: 0.2rem 0;
}

.modal-close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #666;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.modal-close:hover {
  background-color: #f8f9fa;
}

.modal-body {
  padding: 1.5rem;
}

.teams-selection {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.team-selection h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.2rem;
  border-bottom: 2px solid #1976d2;
  padding-bottom: 0.5rem;
}

.player-list {
  max-height: 300px;
  overflow-y: auto;
  border: 1px solid #dee2e6;
  border-radius: 6px;
}

.player-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-bottom: 1px solid #eee;
  cursor: pointer;
  transition: background-color 0.2s;
}

.player-item:hover {
  background-color: #f8f9fa;
}

.player-item.selected {
  background-color: #e3f2fd;
  border-color: #1976d2;
}

.player-item:last-child {
  border-bottom: none;
}

.player-checkbox input {
  margin: 0;
}

.player-name {
  font-weight: 500;
  color: #333;
  flex: 1;
}

.player-number {
  color: #666;
  font-size: 0.9rem;
}

.selection-count {
  margin-top: 0.75rem;
  text-align: center;
  font-size: 0.9rem;
  color: #666;
  font-weight: 500;
}

.modal-footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid #dee2e6;
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.btn-decline,
.btn-accept {
  padding: 0.6rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-decline {
  background-color: #6c757d;
  color: white;
}

.btn-decline:hover {
  background-color: #5a6268;
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

/* Responsive adjustments */
@media (max-width: 768px) {
  .modal-content {
    width: 95%;
    max-height: 95vh;
  }
  
  .teams-selection {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .modal-header,
  .modal-body,
  .modal-footer {
    padding: 1rem;
  }
}
</style>