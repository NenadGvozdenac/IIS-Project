<template>
  <div class="scout-page">
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Player Recommendations</h1>
          <p>AI-powered player recommendations based on weighted metrics across all sessions</p>
        </div>

        <!-- Selected Metrics -->
        <div class="content-card mb-6" v-if="selectedMetrics.length > 0">
          <div class="flex items-center justify-between mb-4">
            <h2>Selected Metrics</h2>
            <button @click="calculateRecommendations" class="btn btn-primary" :disabled="loading">
              {{ loading ? 'Calculating...' : 'Calculate Recommendations' }}
            </button>
          </div>
          <div class="metrics-table-container">
            <table class="metrics-table">
              <thead>
                <tr>
                  <th>Metric Name</th>
                  <th>Weight</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="metric in selectedMetrics" :key="metric.metricId">
                  <td>{{ metric.metricName }}</td>
                  <td>
                    <input 
                      type="number" 
                      v-model.number="metric.weight" 
                      min="0" 
                      step="0.1"
                      class="weight-input"
                      @input="validateWeight(metric)"
                    />
                  </td>
                  <td>
                    <button @click="removeMetric(metric)" class="btn btn-danger btn-sm">
                      Remove
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Available Metrics -->
        <div class="content-card mb-6" v-if="availableMetrics.length > 0">
          <h2>Available Metrics</h2>
          <p class="text-gray-600 mb-4">Click on a metric to add it to your selection</p>
          <div class="metrics-table-container">
            <table class="metrics-table">
              <thead>
                <tr>
                  <th>Metric Name</th>
                  <th>Default Weight</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="metric in availableMetrics" :key="metric.metricId">
                  <td>{{ metric.metricName }}</td>
                  <td>{{ metric.weight }}</td>
                  <td>
                    <button @click="addMetric(metric)" class="btn btn-primary btn-sm">
                      Add
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
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
