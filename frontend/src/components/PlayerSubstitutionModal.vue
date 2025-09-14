<template>
  <div v-if="isVisible" class="modal-overlay" @click="closeModal">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>Player Substitution</h2>
        <p class="team-info">{{ teamName }}</p>
        <button class="modal-close" @click="closeModal">×</button>
      </div>
      
      <div class="modal-body">
        <div class="substitution-selection">
          <!-- Players In Game (Left Side) -->
          <div class="players-section in-game">
            <h3>Players in Game</h3>
            <div class="players-list">
              <div 
                v-for="player in playersInGame" 
                :key="player.id"
                class="player-item"
                :class="{ 'selected': selectedPlayerOut?.id === player.id }"
                @click="selectPlayerOut(player)"
              >
                <div class="player-info">
                  <span class="player-name">{{ player.name }}</span>
                  <span class="player-number">#{{ player.number }}</span>
                </div>
                <div class="player-stats">
                  <span class="time-played">{{ player.timeInGame }}</span>
                  <div class="fouls">
                    <span v-for="foul in player.fouls" :key="foul" class="foul-dot">●</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Substitution Arrow -->
          <div class="substitution-arrow">
            <div class="arrow-container">
              <span class="arrow">→</span>
              <span class="action-text">Substitute</span>
            </div>
          </div>

          <!-- Players on Bench (Right Side) -->
          <div class="players-section on-bench">
            <h3>Players on Bench</h3>
            <div class="players-list">
              <div 
                v-for="player in playersOnBench" 
                :key="player.id"
                class="player-item"
                :class="{ 'selected': selectedPlayerIn?.id === player.id }"
                @click="selectPlayerIn(player)"
              >
                <div class="player-info">
                  <span class="player-name">{{ player.name }}</span>
                  <span class="player-number">#{{ player.number }}</span>
                </div>
                <div class="player-stats">
                  <span class="rest-time">{{ player.restTime || 'Fresh' }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Selection Summary -->
        <div v-if="selectedPlayerOut && selectedPlayerIn" class="substitution-summary">
          <div class="summary-content">
            <span class="player-out">
              OUT: {{ selectedPlayerOut.name }} (#{{ selectedPlayerOut.number }})
            </span>
            <span class="separator">⇄</span>
            <span class="player-in">
              IN: {{ selectedPlayerIn.name }} (#{{ selectedPlayerIn.number }})
            </span>
          </div>
        </div>
      </div>
      
      <div class="modal-footer">
        <button class="btn-cancel" @click="closeModal">
          Cancel
        </button>
        <button 
          class="btn-substitute" 
          :disabled="!canSubstitute"
          @click="performSubstitution"
        >
          Make Substitution
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
  teamId: {
    type: Number,
    required: true
  },
  teamName: {
    type: String,
    required: true
  },
  playersInGame: {
    type: Array,
    default: () => []
  },
  playersOnBench: {
    type: Array,
    default: () => []
  }
})

// Emits
const emit = defineEmits(['close', 'substitute'])

// Reactive data
const selectedPlayerOut = ref(null)
const selectedPlayerIn = ref(null)

// Computed properties
const canSubstitute = computed(() => {
  return selectedPlayerOut.value && selectedPlayerIn.value
})

// Methods
const selectPlayerOut = (player) => {
  selectedPlayerOut.value = player
}

const selectPlayerIn = (player) => {
  selectedPlayerIn.value = player
}

const closeModal = () => {
  selectedPlayerOut.value = null
  selectedPlayerIn.value = null
  emit('close')
}

const performSubstitution = () => {
  if (!canSubstitute.value) return
  
  const substitutionData = {
    teamId: props.teamId,
    playerOutId: selectedPlayerOut.value.id,
    playerInId: selectedPlayerIn.value.id,
    playerOutName: selectedPlayerOut.value.name,
    playerInName: selectedPlayerIn.value.name
  }
  
  emit('substitute', substitutionData)
  closeModal()
}

// Watch for modal visibility changes to reset selections
watch(() => props.isVisible, (newValue) => {
  if (!newValue) {
    selectedPlayerOut.value = null
    selectedPlayerIn.value = null
  }
})
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
  border-radius: 8px;
  width: 90%;
  max-width: 900px;
  max-height: 85vh;
  overflow-y: auto;
  position: relative;
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #dee2e6;
  text-align: center;
  position: relative;
  background: linear-gradient(135deg, #1976d2, #42a5f5);
  color: white;
  border-radius: 8px 8px 0 0;
}

.modal-header h2 {
  margin: 0 0 0.5rem 0;
  font-size: 1.5rem;
  font-weight: 600;
}

.team-info {
  margin: 0;
  font-size: 1.1rem;
  opacity: 0.9;
}

.modal-close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: white;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.modal-close:hover {
  background: rgba(255, 255, 255, 0.3);
}

.modal-body {
  padding: 1.5rem;
}

.substitution-selection {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  gap: 1.5rem;
  align-items: start;
}

.players-section {
  border: 2px solid #dee2e6;
  border-radius: 8px;
  overflow: hidden;
}

.players-section.in-game {
  border-color: #4caf50;
}

.players-section.on-bench {
  border-color: #ff9800;
}

.players-section h3 {
  margin: 0;
  padding: 1rem;
  text-align: center;
  color: white;
  font-weight: 600;
  font-size: 1.1rem;
}

.players-section.in-game h3 {
  background-color: #4caf50;
}

.players-section.on-bench h3 {
  background-color: #ff9800;
}

.players-list {
  background: white;
  max-height: 300px;
  overflow-y: auto;
}

.player-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #dee2e6;
  cursor: pointer;
  transition: all 0.2s;
}

.player-item:last-child {
  border-bottom: none;
}

.player-item:hover {
  background-color: #f8f9fa;
}

.player-item.selected {
  background-color: #e3f2fd;
  border-left: 4px solid #1976d2;
  font-weight: 500;
}

.player-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.player-name {
  font-size: 1rem;
  font-weight: 500;
  color: #333;
  margin-bottom: 0.2rem;
}

.player-number {
  font-size: 0.9rem;
  color: #666;
}

.player-stats {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  font-size: 0.8rem;
  color: #666;
}

.time-played,
.rest-time {
  margin-bottom: 0.2rem;
}

.fouls {
  display: flex;
  gap: 0.1rem;
}

.foul-dot {
  color: #d32f2f;
  font-weight: bold;
}

.substitution-arrow {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 200px;
}

.arrow-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  border: 2px dashed #1976d2;
  border-radius: 8px;
  background-color: #f8f9fa;
}

.arrow {
  font-size: 2rem;
  color: #1976d2;
  font-weight: bold;
}

.action-text {
  font-size: 0.9rem;
  color: #1976d2;
  font-weight: 500;
  text-align: center;
}

.substitution-summary {
  margin-top: 1.5rem;
  padding: 1rem;
  background: linear-gradient(135deg, #e3f2fd, #f3e5f5);
  border-radius: 8px;
  border: 2px solid #1976d2;
}

.summary-content {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  font-size: 1.1rem;
  font-weight: 500;
}

.player-out {
  color: #d32f2f;
}

.player-in {
  color: #2e7d32;
}

.separator {
  font-size: 1.5rem;
  color: #1976d2;
  font-weight: bold;
}

.modal-footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid #dee2e6;
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  background-color: #f8f9fa;
  border-radius: 0 0 8px 8px;
}

.btn-cancel,
.btn-substitute {
  padding: 0.8rem 2rem;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  min-width: 120px;
}

.btn-cancel {
  background-color: #6c757d;
  color: white;
}

.btn-cancel:hover {
  background-color: #5a6268;
  transform: translateY(-1px);
}

.btn-substitute {
  background-color: #4caf50;
  color: white;
}

.btn-substitute:hover:not(:disabled) {
  background-color: #45a049;
  transform: translateY(-1px);
}

.btn-substitute:disabled {
  background-color: #dee2e6;
  color: #6c757d;
  cursor: not-allowed;
  transform: none;
}

/* Responsive Design */
@media (max-width: 768px) {
  .modal-content {
    width: 95%;
    max-height: 90vh;
  }
  
  .substitution-selection {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .substitution-arrow {
    min-height: auto;
    order: -1;
  }
  
  .arrow-container {
    flex-direction: row;
    padding: 0.5rem 1rem;
  }
  
  .arrow {
    font-size: 1.5rem;
    transform: rotate(90deg);
  }
  
  .modal-footer {
    flex-direction: column;
  }
  
  .btn-cancel,
  .btn-substitute {
    width: 100%;
  }
}
</style>