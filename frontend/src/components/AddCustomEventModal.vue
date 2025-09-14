<template>
  <div v-if="isVisible" class="modal-overlay" @click="closeModal">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>Add custom event</h2>
        <button class="modal-close" @click="closeModal">×</button>
      </div>
      
      <div class="modal-body">
        <div class="form-group">
          <label for="category">Category:</label>
          <select 
            id="category" 
            v-model="selectedCategory" 
            @change="onCategoryChange"
            class="form-control"
          >
            <option value="">Category (Personal, Team, General)</option>
            <option value="personal">Personal</option>
            <option value="team">Team</option>
            <option value="general">General</option>
          </select>
        </div>

        <div class="form-group">
          <label for="type">Type:</label>
          <select 
            id="type" 
            v-model="selectedType" 
            :disabled="!selectedCategory || loadingTypes"
            class="form-control"
          >
            <option value="">Event type</option>
            <option 
              v-for="type in eventTypes" 
              :key="type" 
              :value="type"
            >
              {{ type }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="notes">Additional note:</label>
          <textarea 
            id="notes" 
            v-model="notes" 
            class="form-control notes-textarea"
            placeholder="Enter additional notes..."
            rows="4"
          ></textarea>
        </div>

        <!-- Additional fields for personal events -->
        <div v-if="selectedCategory === 'personal'" class="form-group">
          <label for="player">Player:</label>
          <select 
            id="player" 
            v-model="selectedPlayerId" 
            class="form-control"
          >
            <option value="">Select player</option>
            <option 
              v-for="player in allPlayers" 
              :key="player.id" 
              :value="player.id"
            >
              {{ player.name }} (#{{ player.number }}) - {{ player.teamName }}
            </option>
          </select>
        </div>

        <!-- Team selection for team events -->
        <div v-if="selectedCategory === 'team'" class="form-group">
          <label for="team">Team:</label>
          <select 
            id="team" 
            v-model="selectedTeamId" 
            class="form-control"
          >
            <option value="">Select team</option>
            <option value="1">Partizan</option>
            <option :value="opponentTeamId">{{ opponentTeamName }}</option>
          </select>
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
          :disabled="!canSubmit"
          @click="submitEvent"
        >
          Accept
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import axios from 'axios'
import { MATCHES_URL } from '../services/const_service'

// Props
const props = defineProps({
  isVisible: {
    type: Boolean,
    default: false
  },
  matchId: {
    type: Number,
    required: true
  },
  ourTeamPlayers: {
    type: Array,
    default: () => []
  },
  opponentTeamPlayers: {
    type: Array,
    default: () => []
  },
  opponentTeamName: {
    type: String,
    default: 'Opponent'
  },
  opponentTeamId: {
    type: Number,
    default: 2
  }
})

// Emits
const emit = defineEmits(['close', 'eventCreated'])

// Local state
const selectedCategory = ref('')
const selectedType = ref('')
const notes = ref('')
const eventTypes = ref([])
const loadingTypes = ref(false)
const selectedPlayerId = ref('')
const selectedTeamId = ref('')

// Computed properties
const allPlayers = computed(() => {
  const ourPlayers = props.ourTeamPlayers.map(p => ({
    ...p,
    teamName: 'Partizan',
    teamId: 1
  }))
  const opponentPlayers = props.opponentTeamPlayers.map(p => ({
    ...p,
    teamName: props.opponentTeamName,
    teamId: props.opponentTeamId
  }))
  return [...ourPlayers, ...opponentPlayers]
})

const canSubmit = computed(() => {
  const hasBasicFields = selectedCategory.value && selectedType.value
  
  if (selectedCategory.value === 'personal') {
    return hasBasicFields && selectedPlayerId.value
  }
  
  if (selectedCategory.value === 'team') {
    return hasBasicFields && selectedTeamId.value
  }
  
  return hasBasicFields // general events don't need additional fields
})

// Watch for modal visibility to reset form
watch(() => props.isVisible, (newVal) => {
  if (newVal) {
    resetForm()
  }
})

// Methods
const closeModal = () => {
  resetForm()
  emit('close')
}

const resetForm = () => {
  selectedCategory.value = ''
  selectedType.value = ''
  notes.value = ''
  eventTypes.value = []
  selectedPlayerId.value = ''
  selectedTeamId.value = ''
}

const onCategoryChange = async () => {
  selectedType.value = ''
  eventTypes.value = []
  
  if (!selectedCategory.value) return
  
  try {
    loadingTypes.value = true
    const response = await axios.get(`${MATCHES_URL}/ChronologicalEvent/types/${selectedCategory.value}`)
    
    if (response.data?.isSuccess && response.data?.value?.eventTypes) {
      eventTypes.value = response.data.value.eventTypes
    }
  } catch (error) {
    console.error('Error fetching event types:', error)
    eventTypes.value = []
  } finally {
    loadingTypes.value = false
  }
}

const submitEvent = async () => {
  if (!canSubmit.value) return
  
  try {
    const eventData = {
      matchId: props.matchId,
      category: selectedCategory.value,
      type: selectedType.value,
      notes: notes.value || null
    }
    
    // Add specific fields based on category
    if (selectedCategory.value === 'personal') {
      const selectedPlayer = allPlayers.value.find(p => p.id === selectedPlayerId.value)
      eventData.playerId = selectedPlayer.id
      eventData.teamId = selectedPlayer.teamId
    } else if (selectedCategory.value === 'team') {
      eventData.teamId = selectedTeamId.value
    }
    console.log('eventData: ', eventData)
    const response = await axios.post(`${MATCHES_URL}/ChronologicalEvent`, eventData)
    //console.log('response.data: ', response.data)
    if (response.data?.isSuccess) {
      emit('eventCreated', response.data.value)
      closeModal()
    } else {
      alert('Failed to create event: ' + (response.data?.message || 'Unknown error'))
    }
  } catch (error) {
    console.error('Error creating event:', error)
    alert('Failed to create event. Please try again.')
  }
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
  max-width: 500px;
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
  margin: 0;
  color: #333;
  font-size: 1.5rem;
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

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #333;
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #dee2e6;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-control:focus {
  outline: none;
  border-color: #1976d2;
}

.form-control:disabled {
  background-color: #f8f9fa;
  color: #6c757d;
  cursor: not-allowed;
}

.notes-textarea {
  resize: vertical;
  min-height: 100px;
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
  
  .modal-header,
  .modal-body,
  .modal-footer {
    padding: 1rem;
  }
}
</style>