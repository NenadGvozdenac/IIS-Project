<template>
  <div class="customer-dashboard">
    <div class="container">
      <!-- Welcome Section -->
      <div class="welcome-section">
        <h1 class="welcome-title">Welcome back, {{ userInfo?.userName || 'Customer' }}!</h1>
        <p class="welcome-subtitle">Discover upcoming matches and secure your tickets</p>
      </div>

      <!-- Matches With Tickets For Sale Section -->
      <div class="matches-section" v-if="matchesWithTickets.length > 0">
        <div class="section-header">
          <h2>Tickets Available Now</h2>
          <div class="matches-count">
            {{ matchesWithTickets.length }} match{{ matchesWithTickets.length !== 1 ? 'es' : '' }} with tickets for sale
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="loading-state">
          <div class="spinner"></div>
          <p>Loading matches...</p>
        </div>

        <!-- Matches With Tickets Grid -->
        <div class="matches-grid">
          <div v-for="match in matchesWithTickets" :key="match.idMatch" class="match-card">
            <div class="match-header">
              <div class="match-date">
                {{ formatMatchDate(match.scheduledAt) }}
              </div>
              <div class="match-time">
                {{ formatMatchTime(match.scheduledAt) }}
              </div>
            </div>
            
            <div class="match-teams">
              <div class="match-title">
                <h3>{{ match.name }}</h3>
              </div>
            </div>

            <div class="match-details">
              <div class="match-info">
                <div class="info-item">
                  <i class="icon-location"></i>
                  <span>{{ match.hall }}, {{ match.city }}</span>
                </div>
                <div class="info-item" v-if="match.competitionName">
                  <i class="icon-trophy"></i>
                  <span>{{ match.competitionName }}</span>
                </div>
                <div class="info-item">
                  <i class="icon-team"></i>
                  <span>{{ match.teamName }}</span>
                </div>
                <div class="info-item tickets-available">
                  <i class="icon-tickets"></i>
                  <span>Tickets on sale since {{ formatTicketSaleDate(match.ticketsWentOnSale) }}</span>
                </div>
                <br v-if="!match.competitionName" />
              </div>
              
              <div class="match-status">
                <span class="status-badge home-match">
                  {{ match.type }} match
                </span>
                <span class="status-badge tickets-available">
                  Tickets Available
                </span>
              </div>
            </div>

            <div class="match-actions">
              <button @click="viewMatchDetails(match)" class="btn btn-primary">
                View Details & Buy Tickets
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Upcoming Matches Without Tickets Section -->
      <div class="matches-section" v-if="matchesWithoutTickets.length > 0">
        <div class="section-header">
          <h2>Upcoming Matches</h2>
          <div class="matches-count">
            {{ matchesWithoutTickets.length }} match{{ matchesWithoutTickets.length !== 1 ? 'es' : '' }} scheduled
          </div>
        </div>

        <!-- Matches Without Tickets Grid -->
        <div class="matches-grid">
          <div v-for="match in matchesWithoutTickets" :key="match.idMatch" class="match-card match-card-no-tickets">
            <div class="match-header">
              <div class="match-date">
                {{ formatMatchDate(match.scheduledAt) }}
              </div>
              <div class="match-time">
                {{ formatMatchTime(match.scheduledAt) }}
              </div>
            </div>
            
            <div class="match-teams">
              <div class="match-title">
                <h3>{{ match.name }}</h3>
              </div>
            </div>

            <div class="match-details">
              <div class="match-info">
                <div class="info-item">
                  <i class="icon-location"></i>
                  <span>{{ match.hall }}, {{ match.city }}</span>
                </div>
                <div class="info-item" v-if="match.competitionName">
                  <i class="icon-trophy"></i>
                  <span>{{ match.competitionName }}</span>
                </div>
                <div class="info-item">
                  <i class="icon-team"></i>
                  <span>{{ match.teamName }}</span>
                </div>
                <br v-if="!match.competitionName" />
              </div>
              
              <div class="match-status">
                <span class="status-badge home-match">
                  {{ match.type }} match
                </span>
                <span class="status-badge tickets-coming-soon">
                  Tickets Coming Soon
                </span>
              </div>
            </div>

            <div class="match-actions">
              <div class="info-notice">
                <i class="icon-info"></i>
                <span>Tickets will be available soon. Stay tuned!</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="!loading && matches.length === 0" class="empty-state">
        <div class="empty-icon">🎟️</div>
        <h3>No Upcoming Matches</h3>
        <p>There are currently no upcoming matches scheduled. Check back later for new events!</p>
      </div>

      <!-- Quick Actions Section -->
      <div class="quick-actions-section">
        <h3>Quick Actions</h3>
        <div class="actions-grid">
          <div class="action-card" @click="goToProfile">
            <div class="action-icon">👤</div>
            <div class="action-content">
              <h4>My Profile</h4>
              <p>Manage your account and credit cards</p>
            </div>
          </div>
          
          <div class="action-card" @click="goToTickets">
            <div class="action-icon">🎫</div>
            <div class="action-content">
              <h4>My Tickets</h4>
              <p>View your purchased tickets</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { MatchService } from '../../services/ticket_service/match_service.js';
import { getUserData } from '../../services/auth_service.js';

const router = useRouter();

// Reactive data
const matches = ref([]);
const matchesWithTickets = ref([]);
const matchesWithoutTickets = ref([]);
const loading = ref(false);
const userInfo = ref(getUserData());

// Methods
const loadUpcomingMatches = async () => {
  try {
    loading.value = true;
    const response = await MatchService.getMatchesInOurHall();

    const allMatches = response.value || response || [];
    
    // Filter out past matches - only show matches that are in the future
    const now = new Date();
    matches.value = allMatches.filter(match => {
      if (!match.scheduledAt) return true; // Include matches without scheduled date
      const matchDate = new Date(match.scheduledAt);
      return matchDate > now; // Only include future matches
    });
    
    // Separate matches based on ticket availability
    matchesWithTickets.value = matches.value.filter(match => match.ticketsForSale);
    matchesWithoutTickets.value = matches.value.filter(match => !match.ticketsForSale);

  } catch (error) {
    console.error('Error loading matches:', error);
    matches.value = [];
    matchesWithTickets.value = [];
    matchesWithoutTickets.value = [];
  } finally {
    loading.value = false;
  }
};

const formatMatchDate = (dateString) => {
  if (!dateString) return 'TBD';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    weekday: 'short',
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
};

const formatMatchTime = (dateString) => {
  if (!dateString) return 'TBD';
  const date = new Date(dateString);
  return date.toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  });
};

const formatTicketSaleDate = (dateString) => {
  if (!dateString) return 'recently';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric'
  });
};

const viewMatchDetails = (match) => {
  // Navigate to seat selection page
  router.push(`/seat-selection/${match.idMatch}`);
};

const goToProfile = () => {
  router.push('/profile');
};

const goToTickets = () => {
  // Navigate to tickets page (to be implemented)
  router.push('/my-tickets');
};

// Lifecycle
onMounted(() => {
  loadUpcomingMatches();
});
</script>

<style scoped>
.customer-dashboard {
  padding: var(--spacing-xl) 0;
  min-height: calc(100vh - 200px);
}

/* Welcome Section */
.welcome-section {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
  padding: var(--spacing-2xl) 0;
  background: linear-gradient(135deg, var(--color-primary), var(--color-primary-hover));
  border-radius: var(--radius-lg);
  color: white;
}

.welcome-title {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: var(--spacing-md);
}

.welcome-subtitle {
  font-size: 1.125rem;
  opacity: 0.9;
  margin: 0;
}

/* Matches Section */
.matches-section {
  margin-bottom: var(--spacing-2xl);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
}

.section-header h2 {
  font-size: 1.875rem;
  font-weight: 600;
  color: var(--color-text);
  margin: 0;
}

.matches-count {
  color: var(--color-text-muted);
  font-size: 0.875rem;
}

/* Loading State */
.loading-state {
  text-align: center;
  padding: var(--spacing-2xl);
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--color-border);
  border-top: 4px solid var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto var(--spacing-md);
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Matches Grid */
.matches-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: var(--spacing-xl);
}

.match-card {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  padding: var(--spacing-xl);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.match-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-lg);
}

.match-card-no-tickets {
  border: 2px dashed var(--color-border);
  background: #fafafa;
  opacity: 0.8;
}

.match-card-no-tickets:hover {
  transform: none;
  box-shadow: var(--shadow-md);
}

.match-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
  padding-bottom: var(--spacing-md);
  border-bottom: 1px solid var(--color-border);
}

.match-date {
  font-weight: 600;
  color: var(--color-text);
}

.match-time {
  color: var(--color-text-muted);
  font-size: 0.875rem;
}

.match-teams {
  margin-bottom: var(--spacing-lg);
  text-align: center;
}

.match-title h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--color-text);
  margin: 0;
}

.info-item .icon-team::before {
  content: "👥";
}

.info-item .icon-tickets::before {
  content: "🎫";
}

.info-item .icon-info::before {
  content: "ℹ️";
}

.status-badge {
  margin-right: 2px;
}

.status-badge.home-match {
  background-color: #e8f5e8;
  color: #2e7d32;
}

.status-badge.tickets-available {
  background-color: #e3f2fd;
  color: #1565c0;
}

.status-badge.tickets-coming-soon {
  background-color: #fff3e0;
  color: #ef6c00;
}

.match-details {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-lg);
}

.match-info {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.info-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  color: var(--color-text-muted);
  font-size: 0.875rem;
}

.info-item.tickets-available {
  color: var(--color-primary);
  font-weight: 500;
}

.info-item .icon-location::before {
  content: "📍";
}

.info-item .icon-trophy::before {
  content: "🏆";
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: var(--radius-md);
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  margin-bottom: 0.25rem;
  display: inline-block;
}

.status-badge.scheduled {
  background-color: #e0f2fe;
  color: #0277bd;
}

.status-badge.live {
  background-color: #ffebee;
  color: #c62828;
}

.status-badge.finished {
  background-color: #f3e5f5;
  color: #7b1fa2;
}

.status-badge.cancelled {
  background-color: #fafafa;
  color: #616161;
}

.match-actions {
  text-align: center;
}

.match-actions .btn {
  width: 100%;
  padding: var(--spacing-md) var(--spacing-lg);
  font-weight: 600;
}

.info-notice {
  text-align: center;
  padding: var(--spacing-md);
  background-color: #fff3e0;
  border: 1px solid #ffcc02;
  border-radius: var(--radius-md);
  color: #ef6c00;
  font-weight: 500;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: var(--spacing-2xl);
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: var(--spacing-lg);
}

.empty-state h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.empty-state p {
  color: var(--color-text-muted);
  margin: 0;
}

/* Quick Actions */
.quick-actions-section {
  margin-bottom: var(--spacing-2xl);
}

.quick-actions-section h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-lg);
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: var(--spacing-lg);
}

.action-card {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  padding: var(--spacing-lg);
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.action-card:hover {
  transform: translateY(-1px);
  box-shadow: var(--shadow-md);
}

.action-icon {
  font-size: 2rem;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-primary);
  border-radius: var(--radius-md);
  filter: grayscale(1);
}

.action-content h4 {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text);
  margin: 0 0 var(--spacing-xs) 0;
}

.action-content p {
  color: var(--color-text-muted);
  font-size: 0.875rem;
  margin: 0;
}

/* Responsive */
@media (max-width: 768px) {
  .welcome-title {
    font-size: 2rem;
  }
  
  .matches-grid {
    grid-template-columns: 1fr;
  }
  
  .match-teams {
    flex-direction: column;
    gap: var(--spacing-sm);
  }
  
  .vs-separator {
    margin: var(--spacing-sm) 0;
  }
  
  .match-details {
    flex-direction: column;
    gap: var(--spacing-md);
  }
  
  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--spacing-sm);
  }
  
  .actions-grid {
    grid-template-columns: 1fr;
  }
}
</style>
