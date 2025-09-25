<template>
  <div v-if="visible" class="modal-overlay" @click="closeModal">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>InfluxDB Analytics Report</h2>
        <button class="close-btn" @click="closeModal">&times;</button>
      </div>
      
      <form @submit.prevent="generateReport" class="report-form">
        <div class="form-group">
          <label for="playerId">Player ID:</label>
          <input
            type="text"
            id="playerId"
            v-model="formData.playerId"
            required
            placeholder="Enter Player ID (e.g., 1, 2, 3...)"
          />
        </div>

        <div class="form-group">
          <label for="matchId">Match ID:</label>
          <input
            type="text"
            id="matchId"
            v-model="formData.matchId"
            required
            placeholder="Enter Match ID (e.g., match123)"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="startDate">Start Date:</label>
            <input
              type="date"
              id="startDate"
              v-model="formData.startDate"
              required
            />
          </div>

          <div class="form-group">
            <label for="endDate">End Date:</label>
            <input
              type="date"
              id="endDate"
              v-model="formData.endDate"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label for="teamId">Team ID (optional):</label>
          <input
            type="text"
            id="teamId"
            v-model="formData.teamId"
            placeholder="Default: 1"
          />
        </div>

        <div class="report-description">
          <h3>This report will generate:</h3>
          <ul>
            <li><strong>Match Scoring Events:</strong> List of scoring events for the specified match (+2p, +3p, +ft)</li>
            <li><strong>Player Event Counts:</strong> Event statistics for the player across multiple matches</li>
            <li><strong>Team Player Averages:</strong> Average statistics for all team players in the date range</li>
          </ul>
        </div>

        <div class="modal-actions">
          <button 
            type="button" 
            class="btn-cancel" 
            @click="closeModal"
            :disabled="loading"
          >
            Cancel
          </button>
          <button 
            type="submit" 
            class="btn-generate" 
            :disabled="loading || !isFormValid"
          >
            <span v-if="loading">Generating...</span>
            <span v-else>Generate PDF Report</span>
          </button>
        </div>
      </form>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import influxReportService from '../services/influxReportService'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['close'])

const loading = ref(false)
const error = ref(null)

const formData = ref({
  playerId: '',
  matchId: '',
  startDate: '',
  endDate: '',
  teamId: '1'
})

// Computed property to validate form
const isFormValid = computed(() => {
  return formData.value.playerId.trim() !== '' &&
         formData.value.matchId.trim() !== '' &&
         formData.value.startDate !== '' &&
         formData.value.endDate !== '' &&
         new Date(formData.value.startDate) <= new Date(formData.value.endDate)
})

// Watch for modal visibility to reset form
watch(() => props.visible, (newVisible) => {
  if (newVisible) {
    resetForm()
  }
})

const resetForm = () => {
  formData.value = {
    playerId: '',
    matchId: '',
    startDate: '',
    endDate: '',
    teamId: '1'
  }
  error.value = null
}

const closeModal = () => {
  if (!loading.value) {
    emit('close')
  }
}

const generateReport = async () => {
  if (!isFormValid.value) return

  loading.value = true
  error.value = null

  try {
    await influxReportService.generateInfluxReport({
      playerId: formData.value.playerId.trim(),
      matchId: formData.value.matchId.trim(),
      startDate: formData.value.startDate,
      endDate: formData.value.endDate,
      teamId: formData.value.teamId.trim() || '1'
    })
    
    // Close modal on successful generation
    emit('close')
  } catch (err) {
    console.error('Error generating InfluxDB report:', err)
    error.value = err.message || 'Failed to generate report. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.modal-content {
  background: white;
  border-radius: 16px;
  padding: 0;
  max-width: 600px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.3);
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2rem;
  border-bottom: 1px solid #e9ecef;
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
  color: white;
  border-radius: 16px 16px 0 0;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.4rem;
  font-weight: 600;
}

.close-btn {
  background: none;
  border: none;
  font-size: 2rem;
  color: white;
  cursor: pointer;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s ease;
}

.close-btn:hover {
  background-color: rgba(255, 255, 255, 0.2);
}

.report-form {
  padding: 2rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
  font-size: 0.9rem;
}

.form-group input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 0.9rem;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
  box-sizing: border-box;
}

.form-group input:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
}

.form-group input:invalid {
  border-color: #dc3545;
}

.report-description {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
  border-left: 4px solid #007bff;
}

.report-description h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1rem;
  font-weight: 600;
}

.report-description ul {
  margin: 0;
  padding-left: 1.2rem;
}

.report-description li {
  margin-bottom: 0.5rem;
  color: #666;
  font-size: 0.9rem;
  line-height: 1.4;
}

.report-description li strong {
  color: #333;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e9ecef;
}

.btn-cancel,
.btn-generate {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  min-width: 120px;
}

.btn-cancel {
  background-color: #6c757d;
  color: white;
}

.btn-cancel:hover:not(:disabled) {
  background-color: #5a6268;
}

.btn-generate {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
}

.btn-generate:hover:not(:disabled) {
  background: linear-gradient(135deg, #218838 0%, #1aa179 100%);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
}

.btn-generate:disabled,
.btn-cancel:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.error-message {
  margin-top: 1rem;
  padding: 1rem;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  border-radius: 8px;
  color: #721c24;
  font-size: 0.9rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .modal-overlay {
    padding: 1rem;
  }
  
  .modal-content {
    max-width: 100%;
  }
  
  .modal-header {
    padding: 1rem 1.5rem;
  }
  
  .report-form {
    padding: 1.5rem;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 0;
  }
  
  .modal-actions {
    flex-direction: column-reverse;
  }
  
  .btn-cancel,
  .btn-generate {
    width: 100%;
  }
}
</style>