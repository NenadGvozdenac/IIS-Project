<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Metrics Management</h1>
          <p>Define and manage metric types and metric definitions</p>
        </div>
        
        <div v-if="loading" class="loading">
          Loading metrics data...
        </div>
        
        <div v-else class="metrics-layout">
          <!-- Left Side - Metrics Table -->
          <div class="metrics-table-section">
            <div class="table-header">
              <h3>All Metrics</h3>
              <div class="table-filters">
                <select v-model="selectedMetricType" @change="filterMetrics" class="filter-select">
                  <option value="">All Types</option>
                  <option v-for="type in metricTypes" :key="type.idType" :value="type.idType">
                    {{ type.type }}
                  </option>
                </select>
              </div>
            </div>
            
            <div class="table-container">
              <table class="metrics-table">
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Weight</th>
                    <th>Created By</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="metric in filteredMetrics" 
                      :key="metric.idMetrics" 
                      :class="{ 'selected': selectedMetric?.idMetrics === metric.idMetrics }"
                      @click="selectMetric(metric)">
                    <td>{{ metric.name }}</td>
                    <td>{{ metric.metricTypeName || 'Unknown Type' }}</td>
                    <td>{{ metric.metricWeight }}</td>
                    <td>{{ metric.userName || 'Unknown' }}</td>
                    <td>
                      <button @click.stop="selectMetric(metric)" class="btn btn-sm btn-primary">
                        Edit
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
              
              <div v-if="filteredMetrics.length === 0" class="no-data">
                <p>No metrics found.</p>
              </div>
            </div>
          </div>
          
          <!-- Right Side - Forms -->
          <div class="forms-section">
            <!-- Metric Types Section -->
            <div class="form-card">
              <h3>Metric Types</h3>
              <div class="metric-types-list">
                <div v-for="type in metricTypes" :key="type.idType" class="metric-type-item">
                  <span>{{ type.type }}</span>
                </div>
              </div>
              
              <form @submit.prevent="createMetricType" class="simple-form">
                <div class="form-group">
                  <label for="newTypeName">Add New Type</label>
                  <div class="input-with-button">
                    <input 
                      v-model="newMetricType.type" 
                      type="text" 
                      id="newTypeName" 
                      placeholder="Enter type name"
                      class="form-control" 
                      required>
                    <button type="submit" class="btn btn-sm btn-primary" :disabled="creatingMetricType">
                      {{ creatingMetricType ? 'Adding...' : 'Add' }}
                    </button>
                  </div>
                </div>
              </form>
            </div>
            
            <!-- Create Metric Form -->
            <div class="form-card">
              <h3>Add New Metric</h3>
              <form @submit.prevent="createMetric" class="metrics-form">
                <div class="form-group">
                  <label for="metricName">Metric Name</label>
                  <input v-model="newMetric.name" type="text" id="metricName" class="form-control" required>
                </div>
                
                <div class="form-group">
                  <label for="metricType">Metric Type</label>
                  <select v-model="newMetric.idMetricType" id="metricType" required class="form-control">
                    <option value="">Select a type</option>
                    <option v-for="type in metricTypes" :key="type.idType" :value="type.idType">
                      {{ type.type }}
                    </option>
                  </select>
                </div>
                
                <div class="form-row">
                  <div class="form-group">
                    <label for="metricWeight">Weight</label>
                    <input v-model.number="newMetric.metricWeight" type="number" id="metricWeight" class="form-control" required min="1" max="100">
                  </div>
                  <div class="form-group">
                    <label for="isPermanent">Permanent</label>
                    <select v-model="newMetric.isPermanent" id="isPermanent" class="form-control" required>
                      <option value="">Select</option>
                      <option :value="true">Yes</option>
                      <option :value="false">No</option>
                    </select>
                  </div>
                </div>
                
                <button type="submit" class="btn btn-primary" :disabled="creatingMetric">
                  {{ creatingMetric ? 'Creating...' : 'Create Metric' }}
                </button>
              </form>
            </div>
            
            <!-- Edit Metric Form -->
            <div v-if="selectedMetric" class="form-card">
              <h3>Edit Metric</h3>
              <div class="selected-metric-info">
                <p><strong>Current:</strong> {{ selectedMetric.name }}</p>
                <p><strong>Type:</strong> {{ selectedMetric.metricTypeName }}</p>
              </div>
              <form @submit.prevent="updateMetric" class="metrics-form">
                <div class="form-group">
                  <label for="editMetricName">Metric Name</label>
                  <input v-model="editMetric.name" type="text" id="editMetricName" class="form-control" required>
                </div>
                
                <div class="form-group">
                  <label for="editMetricType">Metric Type</label>
                  <select v-model="editMetric.idMetricType" id="editMetricType" required class="form-control">
                    <option value="">Select a type</option>
                    <option v-for="type in metricTypes" :key="type.idType" :value="type.idType">
                      {{ type.type }}
                    </option>
                  </select>
                </div>
                
                <div class="form-group">
                  <label for="editMetricWeight">Weight</label>
                  <input v-model.number="editMetric.metricWeight" type="number" id="editMetricWeight" class="form-control" required min="1" max="100">
                </div>
                
                <div class="form-actions">
                  <button type="submit" class="btn btn-primary" :disabled="updatingMetric">
                    {{ updatingMetric ? 'Updating...' : 'Update Metric' }}
                  </button>
                  <button type="button" @click="clearSelection" class="btn btn-secondary">
                    Cancel
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
        
        <div v-if="error" class="error-message">
          {{ error }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { 
  getAllMetrics, 
  createMetric as createMetricAPI, 
  updateMetric as updateMetricAPI,
  getAllMetricTypes, 
  createMetricType as createMetricTypeAPI 
} from '../../services/metrics_service.js'
import { getUserData } from '../../services/auth_service.js'

// Reactive data
const loading = ref(true)
const error = ref(null)
const metrics = ref([])
const metricTypes = ref([])
const selectedMetricType = ref('')
const selectedMetric = ref(null)
const creatingMetric = ref(false)
const updatingMetric = ref(false)
const creatingMetricType = ref(false)

// Current user data
const currentUser = ref(null)

// Form data for creating new metric types
const newMetricType = ref({
  type: ''
})

// Form data for creating new metrics
const newMetric = ref({
  name: '',
  isPermanent: '',
  metricWeight: 1,
  idUser: 0,
  idMetricType: ''
})

// Form data for editing metrics
const editMetric = ref({
  name: '',
  isPermanent: '',
  metricWeight: 1,
  idUser: 0,
  idMetricType: ''
})

// Computed property for filtered metrics
const filteredMetrics = computed(() => {
  if (!selectedMetricType.value) {
    return metrics.value
  }
  return metrics.value.filter(metric => metric.idMetricType === parseInt(selectedMetricType.value))
})

// Load initial data
onMounted(async () => {
  await loadData()
})

const loadData = async () => {
  try {
    loading.value = true
    error.value = null
    
    // Get current user
    currentUser.value = getUserData()
    console.log('Current user data:', currentUser.value)
    
    if (currentUser.value) {
      newMetric.value.idUser = currentUser.value.userID || currentUser.value.id
    }
    
    // Load metrics and metric types in parallel
    const [metricsData, typesData] = await Promise.all([
      getAllMetrics(),
      getAllMetricTypes()
    ])
    
    console.log('Metrics data received:', metricsData)
    console.log('Types data received:', typesData)
    
    metrics.value = metricsData || []
    metricTypes.value = typesData || []
  } catch (err) {
    console.error('Error loading data:', err)
    error.value = 'Failed to load data. Please try again.'
  } finally {
    loading.value = false
  }
}

const filterMetrics = () => {
  // The computed property will handle the filtering automatically
}

const selectMetric = (metric) => {
  selectedMetric.value = metric
  // Populate edit form with selected metric data
  editMetric.value = {
    name: metric.name,
    isPermanent: Boolean(metric.isPermanent),
    metricWeight: metric.metricWeight || 1,
    idUser: metric.idUser,
    idMetricType: metric.idMetricType
  }
}

const clearSelection = () => {
  selectedMetric.value = null
  editMetric.value = {
    name: '',
    isPermanent: '',
    metricWeight: 1,
    idUser: 0,
    idMetricType: ''
  }
}

const createMetricType = async () => {
  try {
    creatingMetricType.value = true
    error.value = null
    
    console.log('Creating metric type:', newMetricType.value);
    
    if (!newMetricType.value.type || newMetricType.value.type.trim() === '') {
      throw new Error('Please enter a type name')
    }
    
    await createMetricTypeAPI(newMetricType.value)
    
    // Reload data after successful creation
    await loadData()
    
    // Reset form
    newMetricType.value.type = ''
    
    alert('Metric type created successfully!')
  } catch (err) {
    console.error('Error creating metric type:', err)
    error.value = `Failed to create metric type: ${err.message}`
  } finally {
    creatingMetricType.value = false
  }
}

const createMetric = async () => {
  try {
    creatingMetric.value = true
    error.value = null
    
    if (!currentUser.value) {
      throw new Error('User not authenticated')
    }
    
    console.log('Current user:', currentUser.value);
    console.log('Form data:', newMetric.value);
    
    // Validate required fields
    if (!newMetric.value.name || !newMetric.value.idMetricType || newMetric.value.isPermanent === '') {
      throw new Error('Please fill in all required fields')
    }
    
    const metricData = {
      name: newMetric.value.name,
      isPermanent: newMetric.value.isPermanent,
      metricWeight: newMetric.value.metricWeight || 1,
      idUser: currentUser.value.userID || currentUser.value.id,
      idMetricType: newMetric.value.idMetricType
    }
    
    console.log('Sending metric data:', metricData);
    
    await createMetricAPI(metricData)
    
    // Reload data after successful creation
    await loadData()
    
    // Reset form
    newMetric.value = {
      name: '',
      isPermanent: '',
      metricWeight: 1,
      idUser: currentUser.value.userID || currentUser.value.id,
      idMetricType: ''
    }
    
    alert('Metric created successfully!')
  } catch (err) {
    console.error('Error creating metric:', err)
    error.value = `Failed to create metric: ${err.message}`
  } finally {
    creatingMetric.value = false
  }
}

const updateMetric = async () => {
  try {
    updatingMetric.value = true
    error.value = null
    
    if (!selectedMetric.value) {
      throw new Error('No metric selected for update')
    }
    
    if (!currentUser.value) {
      throw new Error('User not authenticated')
    }
    
    // Validate required fields
    if (!editMetric.value.name || !editMetric.value.idMetricType) {
      throw new Error('Please fill in all required fields')
    }
    
    const metricData = {
      name: editMetric.value.name,
      isPermanent: editMetric.value.isPermanent,
      metricWeight: editMetric.value.metricWeight || 1,
      idUser: currentUser.value.userID || currentUser.value.id,
      idMetricType: editMetric.value.idMetricType
    }
    
    console.log('Updating metric with ID:', selectedMetric.value.idMetrics);
    console.log('Update data:', metricData);
    
    await updateMetricAPI(selectedMetric.value.idMetrics, metricData)
    
    // Reload data after successful update
    await loadData()
    clearSelection()
    
    alert('Metric updated successfully!')
  } catch (err) {
    console.error('Error updating metric:', err)
    error.value = `Failed to update metric: ${err.message}`
  } finally {
    updatingMetric.value = false
  }
}
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
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: var(--spacing-xl);
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

.loading {
  text-align: center;
  padding: var(--spacing-xl);
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  color: var(--color-text-light);
}

.metrics-layout {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: var(--spacing-xl);
  align-items: start;
}

/* Left side - Metrics Table */
.metrics-table-section {
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  overflow: hidden;
}

.table-header {
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-header h3 {
  margin: 0;
  color: var(--color-text);
}

.table-filters {
  display: flex;
  gap: var(--spacing-md);
}

.filter-select {
  padding: var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius);
  background: white;
  color: var(--color-text);
  min-width: 200px;
}

.table-container {
  overflow-x: auto;
  max-height: 70vh;
  overflow-y: auto;
}

.metrics-table {
  width: 100%;
  border-collapse: collapse;
}

.metrics-table th,
.metrics-table td {
  padding: var(--spacing-md);
  text-align: left;
  border-bottom: 1px solid var(--color-border);
}

.metrics-table th {
  background-color: var(--color-surface);
  font-weight: 600;
  color: var(--color-text);
  position: sticky;
  top: 0;
  z-index: 1;
}

.metrics-table tr {
  cursor: pointer;
  transition: background-color 0.2s;
}

.metrics-table tr:hover {
  background-color: var(--color-surface);
}

.metrics-table tr.selected {
  background-color: var(--color-primary-light);
}

.no-data {
  padding: var(--spacing-xl);
  text-align: center;
  color: var(--color-text-light);
}

/* Right side - Forms */
.forms-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.form-card {
  background: white;
  padding: var(--spacing-lg);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
}

.form-card h3 {
  margin: 0 0 var(--spacing-lg) 0;
  color: var(--color-text);
  padding-bottom: var(--spacing-sm);
  border-bottom: 2px solid var(--color-border);
}

.selected-metric-info {
  background-color: var(--color-surface);
  padding: var(--spacing-md);
  border-radius: var(--border-radius);
  margin-bottom: var(--spacing-lg);
}

.selected-metric-info p {
  margin: var(--spacing-xs) 0;
  color: var(--color-text);
}

.metrics-form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-weight: 500;
  color: var(--color-text);
  margin-bottom: var(--spacing-xs);
  font-size: 0.875rem;
}

.form-control {
  padding: var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius);
  font-size: 1rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.form-control:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-actions {
  display: flex;
  gap: var(--spacing-md);
  margin-top: var(--spacing-md);
}

/* Buttons */
.btn {
  padding: var(--spacing-md) var(--spacing-lg);
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background-color: var(--color-primary);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: var(--color-primary-dark);
}

.btn-secondary {
  background-color: var(--color-secondary);
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background-color: var(--color-secondary-dark);
}

.btn-sm {
  padding: var(--spacing-xs) var(--spacing-sm);
  font-size: 0.875rem;
}

.status-badge {
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius);
  font-size: 0.75rem;
  font-weight: 500;
  text-transform: uppercase;
}

.status-badge.permanent {
  background-color: #dcfce7;
  color: #166534;
}

.status-badge.temporary {
  background-color: #fef3c7;
  color: #92400e;
}

/* Metric Types Section */
.metric-types-list {
  margin-bottom: var(--spacing-lg);
  max-height: 150px;
  overflow-y: auto;
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius);
  padding: var(--spacing-sm);
}

.metric-type-item {
  padding: var(--spacing-xs) var(--spacing-sm);
  background-color: var(--color-surface);
  border-radius: var(--border-radius);
  margin-bottom: var(--spacing-xs);
  font-size: 0.875rem;
}

.metric-type-item:last-child {
  margin-bottom: 0;
}

.simple-form {
  margin-top: var(--spacing-md);
}

.input-with-button {
  display: flex;
  gap: var(--spacing-sm);
}

.input-with-button .form-control {
  flex: 1;
}

.error-message {
  background-color: #fee2e2;
  color: #dc2626;
  padding: var(--spacing-md);
  border-radius: var(--border-radius);
  margin-top: var(--spacing-lg);
  border: 1px solid #fecaca;
}

/* Responsive design */
@media (max-width: 1200px) {
  .metrics-layout {
    grid-template-columns: 1fr;
    gap: var(--spacing-lg);
  }
  
  .table-container {
    max-height: 50vh;
  }
}

@media (max-width: 768px) {
  .container {
    max-width: 100%;
    padding: 0 var(--spacing-md);
  }
  
  .main-content {
    padding: var(--spacing-lg);
  }
  
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .table-header {
    flex-direction: column;
    gap: var(--spacing-md);
    align-items: stretch;
  }
  
  .metrics-table {
    font-size: 0.875rem;
  }
  
  .metrics-table th,
  .metrics-table td {
    padding: var(--spacing-sm);
  }
  
  .form-actions {
    flex-direction: column;
  }
}

/* CSS Variables */
:root {
  --color-primary: #3b82f6;
  --color-primary-dark: #2563eb;
  --color-primary-light: #dbeafe;
  --color-secondary: #6b7280;
  --color-secondary-dark: #4b5563;
  --color-surface: #f8fafc;
  --color-text: #1f2937;
  --color-text-light: #6b7280;
  --color-border: #e5e7eb;
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --border-radius: 0.375rem;
  --spacing-xs: 0.25rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 0.75rem;
  --spacing-lg: 1rem;
  --spacing-xl: 1.5rem;
}
</style>
