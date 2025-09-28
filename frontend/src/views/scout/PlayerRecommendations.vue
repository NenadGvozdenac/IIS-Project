<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Player Recommendations</h1>
          <p>AI-powered player recommendations based on weighted metrics across all sessions</p>
        </div>

        <!-- Metrics Management Section -->
        <div class="metrics-section">
          <div class="metrics-tables-row">
            <!-- Available Metrics Table -->
            <div class="metrics-table-card">
              <div class="table-header">
                <h3>Available Metrics</h3>
                <div class="table-info">{{ availableMetrics.length }} metrics</div>
              </div>
              <div class="small-table-container">
                <table class="compact-table">
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Weight</th>
                      <th>Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="metric in availableMetrics.slice(0, 6)" :key="metric.metricId">
                      <td class="metric-name">{{ truncateText(metric.metricName, 15) }}</td>
                      <td class="metric-weight">{{ metric.weight }}</td>
                      <td>
                        <button @click="addMetric(metric)" class="btn-small btn-add">
                          +
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <div v-if="availableMetrics.length === 0" class="no-data-small">
                  No available metrics
                </div>
                <div v-if="availableMetrics.length > 6" class="table-footer">
                  +{{ availableMetrics.length - 6 }} more metrics
                </div>
              </div>
            </div>

            <!-- Selected Metrics Table -->
            <div class="metrics-table-card">
              <div class="table-header">
                <h3>Selected Metrics</h3>
                <div class="table-actions">
                  <button @click="calculateRecommendations" class="btn-calculate" :disabled="loading || selectedMetrics.length === 0">
                    {{ loading ? 'Loading...' : 'Calculate' }}
                  </button>
                </div>
              </div>
              <div class="small-table-container">
                <table class="compact-table">
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Weight</th>
                      <th>Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="metric in selectedMetrics.slice(0, 6)" :key="metric.metricId">
                      <td class="metric-name">{{ truncateText(metric.metricName, 15) }}</td>
                      <td>
                        <input 
                          type="number" 
                          v-model.number="metric.weight" 
                          min="0" 
                          step="0.1"
                          class="weight-input-small"
                          @input="validateWeight(metric)"
                        />
                      </td>
                      <td>
                        <button @click="removeMetric(metric)" class="btn-small btn-remove">
                          ×
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <div v-if="selectedMetrics.length === 0" class="no-data-small">
                  No metrics selected
                </div>
                <div v-if="selectedMetrics.length > 6" class="table-footer">
                  +{{ selectedMetrics.length - 6 }} more metrics
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Players Recommendations -->
        <div class="content-card" v-if="players.length > 0">
          <h2>Player Recommendations</h2>
          <p class="text-gray-600 mb-4">Players ranked by weighted metric scores</p>
          <div class="players-table-container">
            <table class="players-table">
              <thead>
                <tr>
                  <th>Rank</th>
                  <th>Score (%)</th>
                  <th>Player Name</th>
                  <th v-for="selectedMetric in selectedMetrics" :key="selectedMetric.metricId">
                    {{ selectedMetric.metricName }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(player, index) in players" :key="player.playerId">
                  <td class="rank">{{ index + 1 }}</td>
                  <td class="score">
                    <div class="score-bar-container">
                      <div class="score-bar" :style="{ width: player.scorePercentage + '%' }"></div>
                      <span class="score-text">{{ player.scorePercentage.toFixed(1) }}%</span>
                    </div>
                  </td>
                  <td class="player-name">{{ player.name }} {{ player.surname }}</td>
                  <td v-for="selectedMetric in selectedMetrics" :key="selectedMetric.metricId" class="metric-value">
                    <div class="metric-detail">
                      <span class="average">{{ getMetricValue(player, selectedMetric.metricId)?.averageValue?.toFixed(2) || 'N/A' }}</span>
                      <span class="weighted">({{ getMetricValue(player, selectedMetric.metricId)?.weightedValue?.toFixed(2) || '0' }} weighted)</span>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Empty State -->
        <div class="content-card" v-if="!loading && players.length === 0 && selectedMetrics.length === 0">
          <div class="empty-state">
            <h3>No metrics selected</h3>
            <p>Select some metrics from the available metrics above to start calculating player recommendations.</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { getPlayerRecommendations } from '../../services/player_recommendations_service.js';
import { getAllMetrics } from '../../services/metrics_service.js';

// Reactive data
const selectedMetrics = ref([]);
const availableMetrics = ref([]);
const players = ref([]);
const loading = ref(false);

// Load initial data
onMounted(async () => {
  await loadMetrics();
});

// Load metrics data
const loadMetrics = async () => {
  try {
    const metricsData = await getAllMetrics();
    // Filter only permanent metrics (quantitative metrics)
    availableMetrics.value = metricsData
      .filter(metric => metric.isPermanent === 1) // IsPermanent is 1 for permanent metrics
      .map(metric => ({
        metricId: metric.idMetrics,
        metricName: metric.name,
        weight: 1.0
      }));
  } catch (error) {
    console.error('Error loading metrics:', error);
  }
};

// Add metric to selected
const addMetric = (metric) => {
  selectedMetrics.value.push({
    metricId: metric.metricId,
    metricName: metric.metricName,
    weight: metric.weight
  });
  
  // Remove from available
  availableMetrics.value = availableMetrics.value.filter(m => m.metricId !== metric.metricId);
};

// Remove metric from selected
const removeMetric = (metric) => {
  selectedMetrics.value = selectedMetrics.value.filter(m => m.metricId !== metric.metricId);
  
  // Add back to available
  availableMetrics.value.push({
    metricId: metric.metricId,
    metricName: metric.metricName,
    weight: 1.0
  });
};

// Validate weight input
const validateWeight = (metric) => {
  if (metric.weight < 0) {
    metric.weight = 0;
  }
};

// Calculate recommendations
const calculateRecommendations = async () => {
  if (selectedMetrics.value.length === 0) {
    alert('Please select at least one metric.');
    return;
  }
  
  loading.value = true;
  try {
    const metricWeights = selectedMetrics.value.map(metric => ({
      metricId: metric.metricId,
      weight: metric.weight
    }));
    
    const result = await getPlayerRecommendations(metricWeights);
    players.value = result.players;
    
    // Update selected and available metrics from the backend response
    if (result.selectedMetrics) {
      selectedMetrics.value = result.selectedMetrics.map(metric => ({
        metricId: metric.metricId,
        metricName: metric.metricName,
        weight: metric.weight
      }));
    }
    
    if (result.availableMetrics) {
      availableMetrics.value = result.availableMetrics.map(metric => ({
        metricId: metric.metricId,
        metricName: metric.metricName,
        weight: metric.weight
      }));
    }
  } catch (error) {
    console.error('Error calculating recommendations:', error);
    alert('Error calculating recommendations. Please try again.');
  } finally {
    loading.value = false;
  }
};

// Get metric value for a player
const getMetricValue = (player, metricId) => {
  return player.metricValues.find(mv => mv.metricId === metricId);
};

// Truncate text helper function
const truncateText = (text, maxLength) => {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength - 3) + '...';
};
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

.content-card {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  margin-bottom: var(--spacing-lg);
}

.content-card h2 {
  font-size: 1.5rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.mb-4 {
  margin-bottom: 1rem;
}

.mb-6 {
  margin-bottom: 1.5rem;
}

.btn {
  padding: var(--spacing-sm) var(--spacing-md);
  border: none;
  border-radius: var(--border-radius);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.btn-primary {
  background-color: var(--color-primary);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: var(--color-primary-hover);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-danger {
  background-color: #ef4444;
  color: white;
}

.btn-danger:hover {
  background-color: #dc2626;
}

.btn-sm {
  padding: 0.25rem 0.5rem;
  font-size: 0.875rem;
}

.metrics-table-container,
.players-table-container {
  overflow-x: auto;
  border: 1px solid #e5e7eb;
  border-radius: var(--border-radius);
}

.metrics-table,
.players-table {
  width: 100%;
  border-collapse: collapse;
}

.metrics-table th,
.metrics-table td,
.players-table th,
.players-table td {
  padding: var(--spacing-sm) var(--spacing-md);
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.metrics-table th,
.players-table th {
  background-color: #f9fafb;
  font-weight: 600;
  color: var(--color-text);
}

.weight-input {
  width: 80px;
  padding: 0.25rem 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  font-size: 0.875rem;
}

.weight-input:focus {
  outline: none;
  border-color: var(--color-primary);
}

/* Compact Metrics Section */
.metrics-section {
  margin-bottom: var(--spacing-xl);
}

.metrics-tables-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
}

.metrics-table-card {
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
  overflow: hidden;
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-md);
  background-color: var(--color-primary);
  color: white;
}

.table-header h3 {
  font-size: 1.1rem;
  font-weight: 600;
  margin: 0;
}

.table-info {
  font-size: 0.875rem;
  opacity: 0.9;
}

.table-actions {
  display: flex;
  gap: var(--spacing-xs);
}

.btn-calculate {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--border-radius);
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-calculate:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.3);
}

.btn-calculate:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.small-table-container {
  height: 280px;
  overflow-y: auto;
}

.compact-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}

.compact-table th {
  background-color: #f8fafc;
  color: var(--color-text);
  padding: var(--spacing-sm);
  text-align: left;
  font-weight: 600;
  border-bottom: 1px solid #e5e7eb;
  position: sticky;
  top: 0;
  z-index: 10;
}

.compact-table td {
  padding: var(--spacing-sm);
  border-bottom: 1px solid #f1f5f9;
}

.compact-table tbody tr:hover {
  background-color: #f8fafc;
}

.metric-name {
  font-weight: 500;
}

.metric-weight {
  text-align: center;
  font-weight: 600;
  color: var(--color-primary);
}

.weight-input-small {
  width: 60px;
  padding: 2px 4px;
  border: 1px solid #e5e7eb;
  border-radius: 3px;
  text-align: center;
  font-size: 0.875rem;
}

.weight-input-small:focus {
  outline: none;
  border-color: var(--color-primary);
}

.btn-small {
  width: 24px;
  height: 24px;
  border: none;
  border-radius: 50%;
  font-size: 0.875rem;
  font-weight: bold;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.btn-add {
  background-color: #10b981;
  color: white;
}

.btn-add:hover {
  background-color: #059669;
  transform: scale(1.1);
}

.btn-remove {
  background-color: #ef4444;
  color: white;
}

.btn-remove:hover {
  background-color: #dc2626;
  transform: scale(1.1);
}

.no-data-small {
  padding: var(--spacing-lg);
  text-align: center;
  color: var(--color-text-light);
  font-style: italic;
}

.table-footer {
  padding: var(--spacing-xs) var(--spacing-sm);
  background-color: #f1f5f9;
  text-align: center;
  font-size: 0.75rem;
  color: var(--color-text-light);
  border-top: 1px solid #e5e7eb;
}

/* Responsive Design */
@media (max-width: 768px) {
  .metrics-tables-row {
    grid-template-columns: 1fr;
  }
  
  .small-table-container {
    height: 200px;
  }
}

.rank {
  font-weight: 600;
  color: var(--color-primary);
  width: 60px;
}

.score {
  width: 150px;
}

.score-bar-container {
  position: relative;
  background-color: #f3f4f6;
  border-radius: 4px;
  height: 24px;
  overflow: hidden;
}

.score-bar {
  position: absolute;
  top: 0;
  left: 0;
  height: 100%;
  background: linear-gradient(90deg, #10b981, #059669);
  transition: width 0.3s ease;
}

.score-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 0.75rem;
  font-weight: 600;
  color: white;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.player-name {
  font-weight: 500;
  color: var(--color-text);
  min-width: 150px;
}

.metric-value {
  text-align: center;
  min-width: 120px;
}

.metric-detail {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2px;
}

.metric-detail .average {
  font-weight: 600;
  color: var(--color-text);
  font-size: 0.875rem;
}

.metric-detail .weighted {
  font-size: 0.75rem;
  color: var(--color-text-light);
}

.empty-state {
  text-align: center;
  padding: var(--spacing-xl);
}

.empty-state h3 {
  font-size: 1.25rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.empty-state p {
  color: var(--color-text-light);
}

.flex {
  display: flex;
}

.items-center {
  align-items: center;
}

.justify-between {
  justify-content: space-between;
}

.text-gray-600 {
  color: #6b7280;
}

/* CSS Variables */
:root {
  --color-surface: #f8fafc;
  --color-text: #1f2937;
  --color-text-light: #6b7280;
  --color-primary: #3b82f6;
  --color-primary-hover: #2563eb;
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --border-radius: 0.375rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 0.75rem;
  --spacing-lg: 1rem;
  --spacing-xl: 1.5rem;
}
</style>
