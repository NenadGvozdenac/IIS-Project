<template>
  <div class="club-owner-dashboard">
    <div class="container">
      <div class="dashboard-header">
        <h1>Club Owner Dashboard</h1>
        <p>Analytics and data overview for your club operations</p>
      </div>

      <div class="dashboard-content" v-if="!loading">
        <div class="charts-grid">
          <!-- Seasons Chart -->
          <div class="chart-card">
            <div class="card-header">
              <h3>Active Seasons</h3>
            </div>
            <div class="card-content">
              <div class="chart-container">
                <canvas ref="seasonsChart" class="chart"></canvas>
              </div>
              <div class="chart-stats">
                <div class="stat-item">
                  <span class="stat-label">Total Seasons:</span>
                  <span class="stat-value">{{ seasonsData.length }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Active Seasons:</span>
                  <span class="stat-value">{{ activeSeasonsCount }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Matches Chart -->
          <div class="chart-card">
            <div class="card-header">
              <h3>Matches Overview</h3>
            </div>
            <div class="card-content">
              <div class="chart-container">
                <canvas ref="matchesChart" class="chart"></canvas>
              </div>
              <div class="chart-stats">
                <div class="stat-item">
                  <span class="stat-label">Total Matches:</span>
                  <span class="stat-value">{{ matchesData.length }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Home Matches:</span>
                  <span class="stat-value">{{ homeMatchesCount }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Competitions Chart -->
          <div class="chart-card full-width">
            <div class="card-header">
              <h3>Competitions Analysis</h3>
            </div>
            <div class="card-content">
              <div class="chart-container">
                <canvas ref="competitionsChart" class="chart"></canvas>
              </div>
              <div class="chart-stats">
                <div class="stat-item">
                  <span class="stat-label">Total Competitions:</span>
                  <span class="stat-value">{{ competitionsData.length }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Active Competitions:</span>
                  <span class="stat-value">{{ activeCompetitionsCount }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Planned Matches:</span>
                  <span class="stat-value">{{ totalCompetitionMatches }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Added Matches:</span>
                  <span class="stat-value">{{ totalAddedMatches }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Avg. Completion:</span>
                  <span class="stat-value">{{ averageCompletionRate }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-else class="loading-container">
        <div class="loading-spinner"></div>
        <p>Loading dashboard data...</p>
      </div>

      <!-- Error State -->
      <div v-if="error" class="error-container">
        <div class="error-message">
          <h3>Error Loading Data</h3>
          <p>{{ error }}</p>
          <button @click="loadData" class="btn btn-primary">Retry</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, nextTick, watch, onBeforeUnmount } from 'vue';
import { AdminService } from '../../services/ticket_service/admin_service';
import Chart from 'chart.js/auto';

// Reactive data
const loading = ref(true);
const error = ref(null);
const seasonsData = ref([]);
const matchesData = ref([]);
const competitionsData = ref([]);

// Chart refs
const seasonsChart = ref(null);
const matchesChart = ref(null);
const competitionsChart = ref(null);

// Chart instances
let seasonsChartInstance = null;
let matchesChartInstance = null;
let competitionsChartInstance = null;

// Computed properties
const activeSeasonsCount = computed(() => 
  seasonsData.value.filter(season => season.isActive).length
);

const homeMatchesCount = computed(() => 
  matchesData.value.filter(match => match.type === 'home').length
);

const activeCompetitionsCount = computed(() => 
  competitionsData.value.filter(competition => competition.isActive).length
);

const totalCompetitionMatches = computed(() => 
  competitionsData.value.reduce((total, competition) => total + competition.numberOfMatches, 0)
);

const totalAddedMatches = computed(() => 
  competitionsData.value.reduce((total, competition) => 
    total + (competition.matches ? competition.matches.length : 0), 0)
);

const averageCompletionRate = computed(() => {
  if (competitionsData.value.length === 0) return 0;
  
  const totalRate = competitionsData.value.reduce((sum, competition) => {
    const planned = competition.numberOfMatches;
    const actual = competition.matches ? competition.matches.length : 0;
    return sum + (planned > 0 ? (actual / planned) * 100 : 0);
  }, 0);
  
  return Math.round(totalRate / competitionsData.value.length);
});

// Load data from API
const loadData = async () => {
  try {
    loading.value = true;
    error.value = null;

    const [seasonsResponse, matchesResponse, competitionsResponse] = await Promise.all([
      AdminService.getSeasons(),
      AdminService.getMatches(),
      AdminService.getCompetitions()
    ]);

    if (seasonsResponse.isSuccess) {
      seasonsData.value = seasonsResponse.value;
    }
    if (matchesResponse.isSuccess) {
      matchesData.value = matchesResponse.value;
    }
    if (competitionsResponse.isSuccess) {
      competitionsData.value = competitionsResponse.value;
    }

    await nextTick();
    // Add a small delay to ensure canvas elements are fully rendered
    setTimeout(() => {
      createCharts();
    }, 100);
  } catch (err) {
    error.value = 'Failed to load dashboard data. Please try again.';
  } finally {
    loading.value = false;
  }
};

// Create charts
const createCharts = () => {
  createSeasonsChart();
  createMatchesChart();
  createCompetitionsChart();
};

const createSeasonsChart = () => {
  if (seasonsChartInstance) {
    seasonsChartInstance.destroy();
  }

  const ctx = seasonsChart.value?.getContext('2d');
  if (!ctx) return;

  const activeSeasons = seasonsData.value.filter(season => season.isActive).length;
  const inactiveSeasons = seasonsData.value.length - activeSeasons;

  seasonsChartInstance = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ['Active Seasons', 'Inactive Seasons'],
      datasets: [{
        data: [activeSeasons, inactiveSeasons],
        backgroundColor: ['#4CAF50', '#FFA726'],
        borderWidth: 2,
        borderColor: '#fff'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  });
};

const createMatchesChart = () => {
  if (matchesChartInstance) {
    matchesChartInstance.destroy();
  }

  const ctx = matchesChart.value?.getContext('2d');
  if (!ctx) return;

  // Group matches by month
  const matchesByMonth = {};
  matchesData.value.forEach(match => {
    const date = new Date(match.scheduledAt);
    const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
    matchesByMonth[monthKey] = (matchesByMonth[monthKey] || 0) + 1;
  });

  const sortedMonths = Object.keys(matchesByMonth).sort();
  const matchCounts = sortedMonths.map(month => matchesByMonth[month]);

  matchesChartInstance = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: sortedMonths.map(month => {
        const [year, monthNum] = month.split('-');
        const date = new Date(year, monthNum - 1);
        return date.toLocaleDateString('en-US', { month: 'short', year: 'numeric' });
      }),
      datasets: [{
        label: 'Matches per Month',
        data: matchCounts,
        backgroundColor: '#2196F3',
        borderColor: '#1976D2',
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            stepSize: 1
          }
        }
      },
      plugins: {
        legend: {
          display: false
        }
      }
    }
  });
};

const createCompetitionsChart = () => {
  if (competitionsChartInstance) {
    competitionsChartInstance.destroy();
  }

  const ctx = competitionsChart.value?.getContext('2d');
  if (!ctx) return;

  const competitionNames = competitionsData.value.map(comp => comp.name);
  const plannedMatches = competitionsData.value.map(comp => comp.numberOfMatches);
  const actualMatches = competitionsData.value.map(comp => comp.matches ? comp.matches.length : 0);

  competitionsChartInstance = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: competitionNames,
      datasets: [
        {
          label: 'Planned Matches',
          data: plannedMatches,
          backgroundColor: '#FF5722',
          borderColor: '#D84315',
          borderWidth: 1
        },
        {
          label: 'Added Matches',
          data: actualMatches,
          backgroundColor: '#4CAF50',
          borderColor: '#2E7D32',
          borderWidth: 1
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            stepSize: 1
          }
        }
      },
      plugins: {
        legend: {
          display: true,
          position: 'top'
        },
        tooltip: {
          callbacks: {
            afterLabel: function(context) {
              const datasetIndex = context.datasetIndex;
              const competitionIndex = context.dataIndex;
              const competition = competitionsData.value[competitionIndex];
              
              if (datasetIndex === 0) {
                return `Total planned: ${competition.numberOfMatches}`;
              } else {
                const actual = competition.matches ? competition.matches.length : 0;
                const planned = competition.numberOfMatches;
                const percentage = planned > 0 ? Math.round((actual / planned) * 100) : 0;
                return `Progress: ${percentage}% (${actual}/${planned})`;
              }
            }
          }
        }
      }
    }
  });
};

// Cleanup charts on unmount
const cleanup = () => {
  if (seasonsChartInstance) seasonsChartInstance.destroy();
  if (matchesChartInstance) matchesChartInstance.destroy();
  if (competitionsChartInstance) competitionsChartInstance.destroy();
};

onMounted(() => {
  loadData();
});

// Watch for loading state changes to recreate charts when data is loaded
watch(loading, (newLoading) => {
  if (!newLoading && seasonsData.value.length > 0) {
    // Additional delay to ensure DOM is fully rendered
    setTimeout(() => {
      createCharts();
    }, 200);
  }
});

// Cleanup on component unmount
onBeforeUnmount(cleanup);
</script>

<style scoped>
.club-owner-dashboard {
  min-height: calc(100vh - 4rem);
  padding: var(--spacing-xl) 0;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 var(--spacing-md);
}

.dashboard-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.dashboard-header h1 {
  font-size: 2.5rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.dashboard-header p {
  color: var(--color-text-light);
  font-size: 1.125rem;
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: var(--spacing-xl);
}

.chart-card {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  overflow: hidden;
  transition: all 0.3s ease;
}

.chart-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.chart-card.full-width {
  grid-column: 1 / -1;
}

.card-header {
  background: linear-gradient(135deg, var(--color-primary), var(--color-primary-hover));
  color: white;
  padding: var(--spacing-lg);
  position: relative;
  overflow: hidden;
}

.card-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 20"><defs><pattern id="grain" width="100" height="20" patternUnits="userSpaceOnUse"><circle cx="10" cy="10" r="0.5" fill="rgba(255,255,255,0.1)"/></pattern></defs><rect width="100" height="20" fill="url(%23grain)"/></svg>');
  opacity: 0.5;
}

.card-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  position: relative;
  z-index: 1;
}

.card-content {
  padding: var(--spacing-lg);
}

.chart-container {
  position: relative;
  height: 300px;
  margin-bottom: var(--spacing-lg);
}

.chart {
  max-width: 100%;
  max-height: 100%;
}

.chart-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: var(--spacing-md);
  padding-top: var(--spacing-md);
  border-top: 1px solid var(--color-border);
}

.stat-item {
  text-align: center;
  padding: var(--spacing-sm);
  background: var(--color-surface);
  border-radius: var(--radius-md);
  transition: all 0.2s ease;
}

.stat-item:hover {
  background: var(--color-primary-light);
  transform: translateY(-1px);
}

.stat-label {
  display: block;
  font-size: 0.875rem;
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-xs);
  font-weight: 500;
}

.stat-value {
  display: block;
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-primary);
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-2xl);
  text-align: center;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--color-border);
  border-top: 4px solid var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: var(--spacing-md);
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-2xl);
  text-align: center;
}

.error-message {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  max-width: 500px;
}

.error-message h3 {
  color: var(--color-danger);
  margin-bottom: var(--spacing-md);
}

.error-message p {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
}

/* Responsive Design */
@media (max-width: 768px) {
  .charts-grid {
    grid-template-columns: 1fr;
    gap: var(--spacing-md);
  }
  
  .chart-container {
    height: 250px;
  }
  
  .dashboard-header h1 {
    font-size: 2rem;
  }
  
  .chart-stats {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .stat-value {
    font-size: 1.25rem;
  }
}

@media (max-width: 480px) {
  .chart-stats {
    grid-template-columns: 1fr;
  }
  
  .card-content {
    padding: var(--spacing-md);
  }
  
  .chart-container {
    height: 200px;
  }
}

/* Animation for chart cards */
.chart-card {
  animation: fadeInUp 0.6s ease-out;
}

.chart-card:nth-child(1) { animation-delay: 0.1s; }
.chart-card:nth-child(2) { animation-delay: 0.2s; }
.chart-card:nth-child(3) { animation-delay: 0.3s; }

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Custom scrollbar for chart tooltips */
::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: var(--color-surface);
}

::-webkit-scrollbar-thumb {
  background: var(--color-primary);
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: var(--color-primary-hover);
}
</style>