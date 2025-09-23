<template>
  <div class="pricing-statistics">
    <div class="container">
      <div class="header">
        <h1>Pricing Parameter Statistics</h1>
        <p>Analysis of ticket pricing performance and zone-wise sales data</p>
      </div>

      <!-- Match Selection -->
      <div class="match-selector" v-if="!loading && matches.length > 0">
        <label for="match-select">Select Match:</label>
        <select 
          id="match-select" 
          v-model="selectedMatchId" 
          @change="loadStatistics"
          class="match-select"
        >
          <option value="">Choose a match...</option>
          <option 
            v-for="match in finishedMatches" 
            :key="match.idMatch" 
            :value="match.idMatch"
          >
            {{ match.name }} - {{ formatDate(match.scheduledAt) }}
          </option>
        </select>
      </div>

      <!-- Statistics Content -->
      <div v-if="statisticsData && !statisticsLoading" class="statistics-content">
        <!-- Match Info Card -->
        <div class="match-info-card">
          <div class="match-header">
            <h2>{{ statisticsData.matchName }}</h2>
            <div class="total-revenue-highlight">
              <span class="revenue-label">Total Revenue</span>
              <span class="revenue-amount">{{ formatCurrency(statisticsData.totalRevenue) }}</span>
            </div>
          </div>
          
          <div class="match-details-grid">
            <div class="detail-card">
              <div class="detail-icon">📅</div>
              <div class="detail-content">
                <span class="detail-label">Date & Time</span>
                <span class="detail-value">{{ formatDate(statisticsData.scheduledAt) }}</span>
              </div>
            </div>
            
            <div class="detail-card">
              <div class="detail-icon">🏠</div>
              <div class="detail-content">
                <span class="detail-label">Match Type</span>
                <span class="detail-value">{{ statisticsData.type.charAt(0).toUpperCase() + statisticsData.type.slice(1) }}</span>
              </div>
            </div>
            
            <div class="detail-card">
              <div class="detail-icon">🏟️</div>
              <div class="detail-content">
                <span class="detail-label">Venue</span>
                <span class="detail-value">{{ statisticsData.hall }}</span>
              </div>
            </div>
            
            <div class="detail-card">
              <div class="detail-icon">🏆</div>
              <div class="detail-content">
                <span class="detail-label">Competition</span>
                <span class="detail-value">{{ statisticsData.competitionName }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Zones Statistics -->
        <div class="zones-grid">
          <div 
            v-for="zone in statisticsData.zones" 
            :key="zone.zoneId"
            class="zone-card"
          >
            <div class="zone-header">
              <h3>{{ zone.zoneName }}</h3>
            </div>
            
            <div class="zone-stats">
              <!-- Sales Performance -->
              <div class="stat-group">
                <h4>Sales Performance</h4>
                <div class="stat-row">
                  <span class="stat-label">Tickets Sold:</span>
                  <span class="stat-value">{{ zone.totalTicketsSold }} / {{ zone.totalSeats }}</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">Revenue:</span>
                  <span class="stat-value revenue">{{ formatCurrency(zone.totalRevenue) }}</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">Avg. Price:</span>
                  <span class="stat-value">{{ getAveragePrice(zone) }}</span>
                </div>
              </div>

              <!-- Pricing Parameters -->
              <div class="stat-group">
                <h4>Pricing Parameters</h4>
                <div class="stat-row">
                  <span class="stat-label">Price Factor:</span>
                  <span class="stat-value">{{ zone.priceParameter || 'N/A' }}</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">Time Factor:</span>
                  <span class="stat-value">{{ zone.timeParameter || 'N/A' }}</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">Min Price:</span>
                  <span class="stat-value">{{ formatCurrency(zone.minimumSeatPrice) }}</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">Max Price:</span>
                  <span class="stat-value">{{ formatCurrency(zone.maximumSeatPrice) }}</span>
                </div>
              </div>

              <!-- Performance Metrics -->
              <div class="stat-group">
                <h4>Performance Metrics</h4>
                <div class="stat-row">
                  <span class="stat-label">Fill Rate:</span>
                  <span class="stat-value">{{ getOccupancyPercentage(zone) }}%</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">Revenue Efficiency:</span>
                  <span class="stat-value">{{ getRevenueEfficiency(zone) }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Summary Analytics -->
        <div class="summary-analytics">
          <h3>Summary Analytics</h3>
          <div class="summary-grid">
            <div class="summary-card">
              <div class="summary-icon">📊</div>
              <div class="summary-content">
                <div class="summary-value">{{ getTotalTicketsSold() }}</div>
                <div class="summary-label">Total Tickets Sold</div>
              </div>
            </div>
            <div class="summary-card">
              <div class="summary-icon">💰</div>
              <div class="summary-content">
                <div class="summary-value">{{ formatCurrency(statisticsData.totalRevenue) }}</div>
                <div class="summary-label">Total Revenue</div>
              </div>
            </div>
            <div class="summary-card">
              <div class="summary-icon">🎯</div>
              <div class="summary-content">
                <div class="summary-value">{{ getOverallFillRate() }}%</div>
                <div class="summary-label">Overall Fill Rate</div>
              </div>
            </div>
            <div class="summary-card">
              <div class="summary-icon">📈</div>
              <div class="summary-content">
                <div class="summary-value">{{ getBestPerformingZone() }}</div>
                <div class="summary-label">Best Performing Zone</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Loading States -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>Loading matches...</p>
      </div>

      <div v-if="statisticsLoading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>Loading statistics...</p>
      </div>

      <!-- Error States -->
      <div v-if="error" class="error-container">
        <div class="error-message">
          <h3>Error Loading Data</h3>
          <p>{{ error }}</p>
          <button @click="loadMatches" class="btn btn-primary">Retry</button>
        </div>
      </div>

      <!-- No Data State -->
      <div v-if="!loading && matches.length === 0" class="no-data-container">
        <div class="no-data-message">
          <h3>No Matches Available</h3>
          <p>There are no finished matches with statistics available.</p>
        </div>
      </div>

      <!-- No Selection State -->
      <div v-if="!loading && !selectedMatchId && matches.length > 0" class="no-selection-container">
        <div class="no-selection-message">
          <h3>Select a Match</h3>
          <p>Please select a finished match from the dropdown above to view pricing statistics.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { StatisticsService } from '../../services/ticket_service/statistics_service.js';

// Reactive data
const loading = ref(true);
const statisticsLoading = ref(false);
const error = ref(null);
const matches = ref([]);
const selectedMatchId = ref('');
const statisticsData = ref(null);

// Computed properties
const finishedMatches = computed(() => matches.value);

// Methods
const loadMatches = async () => {
  try {
    loading.value = true;
    error.value = null;
    
    const response = await StatisticsService.getFinishedMatches();
    if (response.isSuccess) {
      matches.value = response.value;
    } else {
      error.value = 'Failed to load matches';
    }
  } catch (err) {
    error.value = 'Failed to load matches. Please try again.';
  } finally {
    loading.value = false;
  }
};

const loadStatistics = async () => {
  if (!selectedMatchId.value) {
    statisticsData.value = null;
    return;
  }

  try {
    statisticsLoading.value = true;
    error.value = null;
    
    const response = await StatisticsService.getMatchStatistics(selectedMatchId.value);
    
    if (response.isSuccess) {
      statisticsData.value = response.value;
    } else {
      error.value = response.error || 'Failed to load statistics';
    }
  } catch (err) {
    error.value = 'Failed to load statistics. Please try again.';
  } finally {
    statisticsLoading.value = false;
  }
};

// Utility functions
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};

const formatCurrency = (amount) => {
  if (!amount) return '0 RSD';
  return `${amount.toLocaleString()} RSD`;
};

const getOccupancyPercentage = (zone) => {
  if (!zone.totalSeats) return 0;
  return Math.round((zone.totalTicketsSold / zone.totalSeats) * 100);
};

const getOccupancyClass = (zone) => {
  const percentage = getOccupancyPercentage(zone);
  if (percentage >= 80) return 'high';
  if (percentage >= 50) return 'medium';
  return 'low';
};

const getAveragePrice = (zone) => {
  if (!zone.totalTicketsSold) return formatCurrency(0);
  const avgPrice = zone.totalRevenue / zone.totalTicketsSold;
  return formatCurrency(avgPrice);
};

const getRevenueEfficiency = (zone) => {
  if (!zone.maximumSeatPrice || !zone.totalSeats) return 0;
  const maxPossibleRevenue = zone.maximumSeatPrice * zone.totalSeats;
  return Math.round((zone.totalRevenue / maxPossibleRevenue) * 100);
};

const getTotalTicketsSold = () => {
  if (!statisticsData.value) return 0;
  return statisticsData.value.zones.reduce((total, zone) => total + zone.totalTicketsSold, 0);
};

const getOverallFillRate = () => {
  if (!statisticsData.value) return 0;
  const totalSold = getTotalTicketsSold();
  const totalSeats = statisticsData.value.zones.reduce((total, zone) => total + zone.totalSeats, 0);
  return totalSeats ? Math.round((totalSold / totalSeats) * 100) : 0;
};

const getBestPerformingZone = () => {
  if (!statisticsData.value) return 'N/A';
  let bestZone = statisticsData.value.zones[0];
  let bestPerformance = 0;
  
  statisticsData.value.zones.forEach(zone => {
    const performance = getOccupancyPercentage(zone);
    if (performance > bestPerformance) {
      bestPerformance = performance;
      bestZone = zone;
    }
  });
  
  return bestZone.zoneName;
};

// Lifecycle
onMounted(() => {
  loadMatches();
});
</script>

<style scoped>
.pricing-statistics {
  min-height: calc(100vh - 4rem);
  padding: var(--spacing-xl) 0;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 var(--spacing-md);
}

.header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.header h1 {
  font-size: 2.5rem;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.header p {
  color: var(--color-text-light);
  font-size: 1.125rem;
}

.match-selector {
  background: white;
  padding: var(--spacing-lg);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  margin-bottom: var(--spacing-xl);
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.match-selector label {
  font-weight: 600;
  color: var(--color-text);
  font-size: 1.1rem;
}

.match-select {
  flex: 1;
  max-width: 400px;
  padding: var(--spacing-sm) var(--spacing-md);
  border: 2px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: 1rem;
  transition: border-color 0.3s ease;
}

.match-select:focus {
  outline: none;
  border-color: var(--color-primary);
}

.statistics-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.match-info-card {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  border: 1px solid var(--color-border);
}

.match-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
  padding-bottom: var(--spacing-lg);
  border-bottom: 2px solid var(--color-border);
}

.match-header h2 {
  color: var(--color-primary);
  margin: 0;
  font-size: 1.875rem;
  font-weight: 700;
}

.total-revenue-highlight {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  padding: var(--spacing-md);
  background: linear-gradient(135deg, var(--color-success), #16a34a);
  border-radius: var(--radius-lg);
  color: white;
  box-shadow: var(--shadow-sm);
}

.revenue-label {
  font-size: 0.875rem;
  opacity: 0.9;
  margin-bottom: var(--spacing-xs);
}

.revenue-amount {
  font-size: 1.5rem;
  font-weight: 700;
}

.match-details-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--spacing-lg);
}

.detail-card {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-lg);
  background: var(--color-surface);
  border-radius: var(--radius-md);
  border: 1px solid var(--color-border);
  transition: all 0.3s ease;
}

.detail-card:hover {
  background: var(--color-primary-light);
  transform: translateY(-2px);
  box-shadow: var(--shadow-sm);
}

.detail-icon {
  font-size: 1.75rem;
  width: 50px;
  height: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border-radius: var(--radius-full);
  box-shadow: var(--shadow-sm);
}

.detail-content {
  display: flex;
  flex-direction: column;
  flex: 1;
}

.detail-label {
  font-size: 0.875rem;
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-xs);
  font-weight: 500;
}

.detail-value {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text);
}

.zones-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--spacing-lg);
}

.zone-card {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  overflow: hidden;
  transition: all 0.3s ease;
}

.zone-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.zone-header {
  background: linear-gradient(135deg, var(--color-primary), var(--color-primary-hover));
  color: white;
  padding: var(--spacing-lg);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.zone-header h3 {
  margin: 0;
  font-size: 1.25rem;
}

.occupancy-badge {
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--radius-full);
  font-weight: 600;
  font-size: 0.875rem;
}

.occupancy-badge.high {
  background: rgba(34, 197, 94, 0.2);
  color: #16a34a;
}

.occupancy-badge.medium {
  background: rgba(251, 191, 36, 0.2);
  color: #d97706;
}

.occupancy-badge.low {
  background: rgba(239, 68, 68, 0.2);
  color: #dc2626;
}

.zone-stats {
  padding: var(--spacing-lg);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.stat-group h4 {
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
  font-size: 1rem;
  border-bottom: 2px solid var(--color-border);
  padding-bottom: var(--spacing-xs);
}

.stat-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-xs) 0;
  border-bottom: 1px solid var(--color-border-light);
}

.stat-row:last-child {
  border-bottom: none;
}

.stat-label {
  color: var(--color-text-muted);
  font-size: 0.9rem;
}

.stat-value {
  font-weight: 600;
  color: var(--color-text);
}

.stat-value.revenue {
  color: var(--color-success);
}

.progress-container {
  padding: var(--spacing-md) var(--spacing-lg);
  background: var(--color-surface);
}

.progress-label {
  font-size: 0.875rem;
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-xs);
}

.progress-bar {
  width: 100%;
  height: 8px;
  background: var(--color-border);
  border-radius: var(--radius-full);
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  transition: width 0.5s ease;
  border-radius: var(--radius-full);
}

.progress-fill.high {
  background: var(--color-success);
}

.progress-fill.medium {
  background: var(--color-warning);
}

.progress-fill.low {
  background: var(--color-danger);
}

.summary-analytics {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
}

.summary-analytics h3 {
  color: var(--color-text);
  margin-bottom: var(--spacing-lg);
  font-size: 1.5rem;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

.summary-card {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-lg);
  background: var(--color-surface);
  border-radius: var(--radius-md);
  transition: all 0.3s ease;
}

.summary-card:hover {
  background: var(--color-primary-light);
  transform: translateY(-2px);
}

.summary-icon {
  font-size: 2rem;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border-radius: var(--radius-full);
  box-shadow: var(--shadow-sm);
}

.summary-content {
  flex: 1;
}

.summary-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-primary);
  margin-bottom: var(--spacing-xs);
}

.summary-label {
  color: var(--color-text-muted);
  font-size: 0.875rem;
}

.loading-container,
.error-container,
.no-data-container,
.no-selection-container {
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

.error-message,
.no-data-message,
.no-selection-message {
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

.no-data-message h3,
.no-selection-message h3 {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-md);
}

.error-message p,
.no-data-message p,
.no-selection-message p {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
}

/* Responsive Design */
@media (max-width: 1024px) {
  .zones-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .summary-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .match-details-grid {
    grid-template-columns: 1fr;
  }
  
  .match-selector {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .match-select {
    max-width: 100%;
  }
  
  .match-header {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--spacing-md);
  }
  
  .total-revenue-highlight {
    align-self: stretch;
    align-items: center;
  }
}

@media (max-width: 480px) {
  .summary-grid {
    grid-template-columns: 1fr;
  }
  
  .detail-card {
    flex-direction: column;
    text-align: center;
    gap: var(--spacing-sm);
  }
  
  .zones-grid {
    gap: var(--spacing-md);
  }
}

/* Animation */
.zone-card {
  animation: fadeInUp 0.6s ease-out;
}

.zone-card:nth-child(1) { animation-delay: 0.1s; }
.zone-card:nth-child(2) { animation-delay: 0.2s; }
.zone-card:nth-child(3) { animation-delay: 0.3s; }
.zone-card:nth-child(4) { animation-delay: 0.4s; }

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
</style>