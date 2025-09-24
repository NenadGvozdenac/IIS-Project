<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <!-- Page Header -->
        <div class="page-header">
          <button class="btn btn-secondary back-btn" @click="goBack">
            ← Back to Sessions
          </button>
          <h1>Edit Session #{{ sessionId }}</h1>
          <p v-if="session">{{ session.playerName }} - {{ formatDate(session.startTime) }}</p>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="content-card">
          <p>Loading session details...</p>
        </div>

        <!-- Error State -->
        <div v-if="error" class="content-card error">
          <p>{{ error }}</p>
        </div>

        <!-- Session Edit Form -->
        <div v-if="session && !loading" class="content-card">
          <form @submit.prevent="updateSessionData" class="session-edit-form">
            <div class="form-row">
              <div class="form-group">
                <label>Player:</label>
                <input 
                  type="text" 
                  :value="session.playerName" 
                  readonly 
                  class="readonly-input"
                />
              </div>

              <div class="form-group">
                <label>Scout:</label>
                <input 
                  type="text" 
                  :value="session.userName" 
                  readonly 
                  class="readonly-input"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Start Date:</label>
                <input 
                  type="date" 
                  v-model="editForm.startTime" 
                  required 
                />
              </div>

              <div class="form-group">
                <label>End Date:</label>
                <input 
                  type="date" 
                  v-model="editForm.endTime" 
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Type:</label>
                <select v-model="editForm.idSessionType" required>
                  <option value="">Select Type</option>
                  <option 
                    v-for="type in sessionTypes" 
                    :key="type.id" 
                    :value="type.id"
                  >
                    {{ type.name }}
                  </option>
                </select>
              </div>

              <div class="form-group">
                <label>Status:</label>
                <select v-model="editForm.idSessionStatus" required>
                  <option value="">Select Status</option>
                  <option 
                    v-for="status in sessionStatuses" 
                    :key="status.id" 
                    :value="status.id"
                  >
                    {{ status.name }}
                  </option>
                </select>
              </div>
            </div>

            <div class="form-group full-width">
              <label>Note:</label>
              <textarea 
                v-model="editForm.note" 
                rows="4"
                placeholder="Session notes..."
              ></textarea>
            </div>

            <div class="form-actions">
              <button 
                type="button" 
                class="btn btn-secondary" 
                @click="goBack"
              >
                Cancel
              </button>
              <button 
                type="submit" 
                class="btn btn-primary"
                :disabled="isUpdating"
              >
                {{ isUpdating ? 'Updating...' : 'Update Session' }}
              </button>
            </div>
          </form>
        </div>

        <!-- Metrics Section -->
        <div v-if="session && !loading" class="content-card metrics-section">
          <h2>Session Metrics</h2>
          
          <!-- Current Session Metrics -->
          <div class="metrics-subsection">
            <h3>Current Session Metrics</h3>
            <div v-if="sessionMetrics.length === 0" class="no-metrics">
              No metrics recorded for this session yet.
            </div>
            <div v-else class="metrics-grid">
              <div 
                v-for="metric in sessionMetrics" 
                :key="`session-${metric.id}`"
                class="metric-card session-metric"
              >
                <div class="metric-header">
                  <h4>{{ metric.name }}</h4>
                  <button 
                    class="btn btn-sm btn-edit"
                    @click="startEditMetric(metric)"
                    :disabled="metric.isEditing"
                  >
                    Edit
                  </button>
                </div>
                
                <!-- Display mode -->
                <div v-if="!metric.isEditing" class="metric-display">
                  <p class="metric-value">{{ metric.value }}</p>
                  <p class="metric-type">{{ metric.type }}</p>
                </div>
                
                <!-- Edit mode -->
                <div v-else class="metric-edit">
                  <div class="edit-form">
                    <input 
                      v-model="metric.editValue" 
                      type="text" 
                      class="form-control"
                      :placeholder="getMetricPlaceholder({ metricTypeName: metric.type })"
                      @keyup.enter="saveMetricEdit(metric)"
                      @keyup.escape="cancelMetricEdit(metric)"
                    />
                    <div class="edit-actions">
                      <button 
                        class="btn btn-sm btn-success"
                        @click="saveMetricEdit(metric)"
                        :disabled="metric.isSaving"
                      >
                        {{ metric.isSaving ? 'Saving...' : 'Save' }}
                      </button>
                      <button 
                        class="btn btn-sm btn-secondary"
                        @click="cancelMetricEdit(metric)"
                        :disabled="metric.isSaving"
                      >
                        Cancel
                      </button>
                    </div>
                  </div>
                  <p class="metric-type">{{ metric.type }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Available Metrics -->
          <div class="metrics-subsection">
            <h3>Available Metrics</h3>
            
            <!-- Permanent Metrics -->
            <div class="metric-category" v-if="availablePermanentMetrics.length > 0">
              <h4>Permanent Metrics</h4>
              <div class="metrics-grid">
                <div 
                  v-for="metric in availablePermanentMetrics" 
                  :key="metric.idMetrics"
                  class="metric-card available"
                  @click="addMetricToSession(metric)"
                >
                  <h5>{{ metric.name }}</h5>
                  <p class="metric-type">{{ metric.metricTypeName }}</p>
                  <span class="add-icon">+</span>
                </div>
              </div>
            </div>

            <!-- Current Season Metrics -->
            <div class="metric-category" v-if="availableCurrentSeasonMetrics.length > 0">
              <h4>Current Season Metrics</h4>
              <div class="metrics-grid">
                <div 
                  v-for="metric in availableCurrentSeasonMetrics" 
                  :key="metric.idMetrics"
                  class="metric-card available"
                  @click="addMetricToSession(metric)"
                >
                  <h5>{{ metric.name }}</h5>
                  <p class="metric-type">{{ metric.metricTypeName }}</p>
                  <span class="add-icon">+</span>
                </div>
              </div>
            </div>
            
            <div v-if="availablePermanentMetrics.length === 0 && availableCurrentSeasonMetrics.length === 0" class="no-metrics">
              All available metrics have been added to this session.
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Metric Modal -->
    <div v-if="showAddMetricModal" class="modal-overlay" @click="closeAddMetricModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>Add Metric: {{ selectedMetric?.name }}</h2>
          <button class="close-btn" @click="closeAddMetricModal">&times;</button>
        </div>
        
        <div class="modal-body">
          <form @submit.prevent="saveMetricValue">
            <div class="form-group">
              <label>Value:</label>
              <input 
                type="text" 
                v-model="metricValue" 
                :placeholder="getMetricPlaceholder(selectedMetric)"
                required 
              />
            </div>

            <div class="form-actions">
              <button 
                type="button" 
                class="btn btn-secondary" 
                @click="closeAddMetricModal"
              >
                Cancel
              </button>
              <button 
                type="submit" 
                class="btn btn-primary"
                :disabled="isAddingMetric"
              >
                {{ isAddingMetric ? 'Adding...' : 'Add Metric' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { getSessionTypes, getSessionStatuses, getAllSessionMetrics, getSessionMetricsBySessionId, createSessionMetric, updateSessionMetric, updateSession, getSessionById } from '../../services/session_service.js'
import { getAllMetrics } from '../../services/metrics_service.js'

// Router
const router = useRouter()
const route = useRoute()
const sessionId = route.params.id

// Reactive data
const session = ref(null)
const sessionTypes = ref([])
const sessionStatuses = ref([])
const sessionMetrics = ref([])
const permanentMetrics = ref([])
const currentSeasonMetrics = ref([])
const loading = ref(true)
const error = ref('')
const isUpdating = ref(false)
const isAddingMetric = ref(false)
const showAddMetricModal = ref(false)
const selectedMetric = ref(null)
const metricValue = ref('')

const editForm = ref({
  startTime: '',
  endTime: '',
  idSessionType: '',
  idSessionStatus: '',
  note: ''
})

// Computed properties for available metrics (excluding ones already in session)
const existingMetricIds = computed(() => {
  return new Set(sessionMetrics.value.map(metric => metric.id))
})

const availablePermanentMetrics = computed(() => {
  return permanentMetrics.value.filter(metric => !existingMetricIds.value.has(metric.idMetrics))
})

const availableCurrentSeasonMetrics = computed(() => {
  return currentSeasonMetrics.value.filter(metric => !existingMetricIds.value.has(metric.idMetrics))
})

// Methods
const goBack = () => {
  router.push('/scout/sessions')
}

const loadSession = async () => {
  try {
    // Load session from the API by ID
    const sessionData = await getSessionById(sessionId)
    
    session.value = {
      idSession: sessionData.idSession,
      idPlayer: sessionData.idPlayer,
      idUser: sessionData.idUser,
      playerName: sessionData.playerName,
      userName: sessionData.userName,
      startTime: sessionData.startTime,
      endTime: sessionData.endTime,
      sessionTypeName: sessionData.sessionTypeName,
      sessionStatusName: sessionData.sessionStatusName,
      note: sessionData.note
    }

    editForm.value = {
      startTime: sessionData.startTime ? sessionData.startTime.split('T')[0] : '',
      endTime: sessionData.endTime ? sessionData.endTime.split('T')[0] : '',
      idSessionType: sessionData.idSessionType,
      idSessionStatus: sessionData.idSessionStatus,
      note: sessionData.note || ''
    }
  } catch (err) {
    console.error('Error loading session:', err)
    error.value = 'Failed to load session: ' + err.message
  }
}

const loadSessionTypes = async () => {
  try {
    const data = await getSessionTypes()
    sessionTypes.value = data || []
  } catch (err) {
    console.error('Error loading session types:', err)
  }
}

const loadSessionStatuses = async () => {
  try {
    const data = await getSessionStatuses()
    sessionStatuses.value = data || []
  } catch (err) {
    console.error('Error loading session statuses:', err)
  }
}

const loadMetrics = async () => {
  try {
    // Load all available metrics from the database
    const allMetrics = await getAllMetrics();
    console.log('Loaded all metrics:', allMetrics); // Debug log
    
    // Separate permanent metrics from seasonal ones
    permanentMetrics.value = allMetrics.filter(metric => metric.isPermanent === 1);
    currentSeasonMetrics.value = allMetrics.filter(metric => metric.isPermanent === 0);
    
    console.log('Permanent metrics:', permanentMetrics.value); // Debug log
    console.log('Season metrics:', currentSeasonMetrics.value); // Debug log
    
    // Load session-specific metrics
    await loadSessionMetrics();
  } catch (err) {
    console.error('Error loading metrics:', err);
    permanentMetrics.value = [];
    currentSeasonMetrics.value = [];
  }
}

const loadSessionMetrics = async () => {
  try {
    const metrics = await getSessionMetricsBySessionId(parseInt(sessionId));
    console.log('Loaded session metrics:', metrics); // Debug log
    
    sessionMetrics.value = metrics.map(metric => ({
      id: metric.idMetrics,
      sessionId: metric.idSession,
      name: metric.metricName || 'Unknown Metric',
      value: metric.value || '',
      type: metric.metricTypeName || 'Unknown Type',
      isEditing: false,
      editValue: '',
      isSaving: false
    }));
    
    console.log('Mapped session metrics:', sessionMetrics.value); // Debug log
  } catch (err) {
    console.error('Error loading session metrics:', err);
    sessionMetrics.value = [];
  }
}

const updateSessionData = async () => {
  isUpdating.value = true
  try {
    const updateData = {
      StartTime: editForm.value.startTime,
      EndTime: editForm.value.endTime,
      IdSessionType: parseInt(editForm.value.idSessionType),
      IdSessionStatus: parseInt(editForm.value.idSessionStatus),
      Note: editForm.value.note || '',
      IdPlayer: session.value.idPlayer, // Keep the original player
      IdUser: session.value.idUser // Keep the original user
    }
    
    console.log('Updating session with data:', updateData)
    
    await updateSession(parseInt(sessionId), updateData)
    
    // Reload session data to reflect changes
    await loadSession()
    
    alert('Session updated successfully!')
  } catch (err) {
    console.error('Error updating session:', err)
    alert('Error updating session: ' + err.message)
  } finally {
    isUpdating.value = false
  }
}

const addMetricToSession = (metric) => {
  selectedMetric.value = metric
  metricValue.value = ''
  showAddMetricModal.value = true
}

const closeAddMetricModal = () => {
  showAddMetricModal.value = false
  selectedMetric.value = null
  metricValue.value = ''
}

const saveMetricValue = async () => {
  isAddingMetric.value = true
  try {
    // Create session metric data
    const sessionMetricData = {
      value: metricValue.value,
      idSession: parseInt(sessionId),
      idMetrics: selectedMetric.value.idMetrics
    };
    
    console.log('Saving metric:', sessionMetricData); // Debug log
    
    // Call API to save metric
    await createSessionMetric(sessionMetricData);
    
    // Add to session metrics display with proper metric info
    const newSessionMetric = {
      id: selectedMetric.value.idMetrics,
      sessionId: parseInt(sessionId),
      name: selectedMetric.value.name || 'Unknown Metric',
      value: metricValue.value,
      type: selectedMetric.value.metricTypeName || 'Unknown Type',
      isEditing: false,
      editValue: '',
      isSaving: false
    };
    
    console.log('Adding new session metric to display:', newSessionMetric); // Debug log
    
    sessionMetrics.value.push(newSessionMetric);
    
    closeAddMetricModal();
    alert('Metric added successfully!');
  } catch (err) {
    console.error('Error adding metric:', err);
    alert('Error adding metric: ' + err.message);
  } finally {
    isAddingMetric.value = false;
  }
}

// Inline editing methods
const startEditMetric = (metric) => {
  metric.isEditing = true;
  metric.editValue = metric.value;
}

const cancelMetricEdit = (metric) => {
  metric.isEditing = false;
  metric.editValue = '';
  metric.isSaving = false;
}

const saveMetricEdit = async (metric) => {
  if (!metric.editValue.trim()) {
    alert('Please enter a value');
    return;
  }
  
  metric.isSaving = true;
  
  try {
    const updateData = {
      value: metric.editValue,
      idSession: parseInt(sessionId),
      idMetrics: metric.id
    };
    
    console.log('Updating session metric:', updateData);
    
    // Call API to update session metric
    await updateSessionMetric(parseInt(sessionId), metric.id, updateData);
    
    // Update the display value
    metric.value = metric.editValue;
    metric.isEditing = false;
    metric.editValue = '';
    
    console.log('Session metric updated successfully');
  } catch (err) {
    console.error('Error updating session metric:', err);
    alert('Error updating metric: ' + err.message);
  } finally {
    metric.isSaving = false;
  }
}

const getMetricPlaceholder = (metric) => {
  if (!metric) return ''
  const typeName = metric.metricTypeName?.toLowerCase() || ''
  if (typeName.includes('physical')) return 'e.g., 25.5 km/h'
  if (typeName.includes('technical')) return 'e.g., 85%'
  if (typeName.includes('performance')) return 'e.g., 3'
  return 'Enter value'
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  try {
    return new Date(dateString).toLocaleDateString()
  } catch {
    return '-'
  }
}

// Lifecycle
onMounted(async () => {
  loading.value = true
  try {
    await Promise.all([
      loadSession(),
      loadSessionTypes(),
      loadSessionStatuses(),
      loadMetrics()
    ])
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.scout-page {
  min-height: 100vh;
  background-color: var(--color-surface);
}

.main-content {
  padding: var(--spacing-xl);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: var(--spacing-xl);
  position: relative;
}

.page-header h1 {
  font-size: 2.5rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.page-header p {
  color: var(--color-text-light);
  font-size: 1.125rem;
}

.back-btn {
  margin-bottom: 1rem;
}

.content-card {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  margin-bottom: var(--spacing-xl);
}

.error {
  background-color: #fef2f2;
  border: 1px solid #fecaca;
  color: #dc2626;
}

/* Form Styles */
.session-edit-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  font-weight: 600;
  color: var(--color-text);
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.readonly-input {
  background-color: #f9fafb;
  color: #6b7280;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

/* Button Styles */
.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.375rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #2563eb;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background-color: #4b5563;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Metrics Styles */
.metrics-section h2 {
  margin-bottom: 2rem;
  color: var(--color-text);
}

.metrics-subsection {
  margin-bottom: 3rem;
}

.metrics-subsection h3 {
  margin-bottom: 1rem;
  color: var(--color-text);
  border-bottom: 2px solid #e5e7eb;
  padding-bottom: 0.5rem;
}

.metric-category {
  margin-bottom: 2rem;
}

.metric-category h4 {
  margin-bottom: 1rem;
  color: var(--color-text-light);
  font-size: 1.1rem;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1rem;
}

.metric-card {
  padding: 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  background: white;
}

.metric-card.session-metric {
  border-left: 4px solid #3b82f6;
}

.metric-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.metric-header h4 {
  margin: 0;
  color: var(--color-text);
}

.btn-sm {
  padding: 0.375rem 0.75rem;
  font-size: 0.875rem;
}

.btn-edit {
  background-color: #f59e0b;
  color: white;
}

.btn-edit:hover:not(:disabled) {
  background-color: #d97706;
}

.btn-success {
  background-color: #10b981;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background-color: #059669;
}

.metric-display {
  /* existing styles */
}

.metric-edit {
  /* edit mode styles */
}

.edit-form {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.edit-actions {
  display: flex;
  gap: 0.5rem;
}

.form-control {
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.25rem;
  font-size: 0.875rem;
}

.form-control:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.1);
}

.metric-card.available {
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.metric-card.available:hover {
  border-color: #3b82f6;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.metric-card h4,
.metric-card h5 {
  margin: 0 0 0.5rem 0;
  color: var(--color-text);
}

.metric-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #3b82f6;
  margin: 0.5rem 0;
}

.metric-type {
  color: var(--color-text-light);
  font-size: 0.875rem;
  margin: 0;
}

.add-icon {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 24px;
  height: 24px;
  background-color: #3b82f6;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  font-weight: bold;
}

.no-metrics {
  color: var(--color-text-light);
  font-style: italic;
  padding: 2rem;
  text-align: center;
  background-color: #f9fafb;
  border-radius: 0.375rem;
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
  border-radius: 0.375rem;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-header h2 {
  margin: 0;
  color: var(--color-text);
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: var(--color-text-light);
}

.close-btn:hover {
  color: var(--color-text);
}

.modal-body {
  padding: 1.5rem;
}

/* CSS Variables */
:root {
  --color-surface: #f8fafc;
  --color-text: #1f2937;
  --color-text-light: #6b7280;
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --border-radius: 0.375rem;
  --spacing-sm: 0.5rem;
  --spacing-xl: 1.5rem;
}
</style>
