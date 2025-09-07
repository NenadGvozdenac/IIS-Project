<template>
  <div class="club-owner-dashboard">
    <div class="container">
      <div class="dashboard-header">
        <h1>Club Owner Dashboard</h1>
        <p>Hi {{ userInfo?.userName || 'Club Owner' }}, manage your club operations from here.</p>
      </div>

      <div class="dashboard-content">
        <div class="dashboard-grid">
          <!-- Club Owner Profile Card -->
          <div class="dashboard-card">
            <div class="card-header">
              <h3>Your Profile</h3>
            </div>
            <div class="card-content">
              <div class="profile-info">
                <div class="info-row">
                  <span class="label">Name:</span>
                  <span class="value">{{ userInfo?.userName || 'N/A' }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Email:</span>
                  <span class="value">{{ userInfo?.userEmail || 'N/A' }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Role:</span>
                  <span class="value">Club Owner</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions Card -->
          <div class="dashboard-card">
            <div class="card-header">
              <h3>Quick Actions</h3>
            </div>
            <div class="card-content">
              <div class="action-buttons">
                <router-link to="/club-manager/matches" class="btn btn-primary btn-sm">View Matches</router-link>
                <router-link to="/club-manager/travelinfo" class="btn btn-secondary btn-sm">Travel Info</router-link>
                <button class="btn btn-secondary btn-sm">Club Settings</button>
              </div>
            </div>
          </div>

          <!-- Club Overview Card -->
          <div class="dashboard-card full-width">
            <div class="card-header">
              <h3>Club Overview</h3>
            </div>
            <div class="card-content">
              <div class="stats-grid">
                <div class="stat-item">
                  <div class="stat-icon">⚽</div>
                  <div class="stat-content">
                    <h4>Active Matches</h4>
                    <p class="stat-number">12</p>
                  </div>
                </div>
                <div class="stat-item">
                  <div class="stat-icon">👥</div>
                  <div class="stat-content">
                    <h4>Team Members</h4>
                    <p class="stat-number">25</p>
                  </div>
                </div>
                <div class="stat-item">
                  <div class="stat-icon">✈️</div>
                  <div class="stat-content">
                    <h4>Travel Bookings</h4>
                    <p class="stat-number">8</p>
                  </div>
                </div>
                <div class="stat-item">
                  <div class="stat-icon">🏆</div>
                  <div class="stat-content">
                    <h4>Wins This Season</h4>
                    <p class="stat-number">7</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Club Management Services Card -->
          <div class="dashboard-card full-width">
            <div class="card-header">
              <h3>Club Management Services</h3>
            </div>
            <div class="card-content">
              <div class="services-grid">
                <div class="service-item">
                  <div class="service-icon">⚽</div>
                  <h4>Match Management</h4>
                  <p>View and manage upcoming matches and events</p>
                  <router-link to="/club-manager/matches" class="service-link">View Matches →</router-link>
                </div>
                <div class="service-item">
                  <div class="service-icon">✈️</div>
                  <h4>Travel Arrangements</h4>
                  <p>Organize travel for team and support staff</p>
                  <router-link to="/club-manager/travelinfo" class="service-link">Manage Travel →</router-link>
                </div>
                <div class="service-item">
                  <div class="service-icon">👥</div>
                  <h4>Team Management</h4>
                  <p>Manage team roster and staff information</p>
                  <span class="service-link disabled">Coming Soon</span>
                </div>
                <div class="service-item">
                  <div class="service-icon">📊</div>
                  <h4>Performance Analytics</h4>
                  <p>View team performance metrics and reports</p>
                  <span class="service-link disabled">Coming Soon</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { getUserData } from '../../services/auth_service';

const userInfo = getUserData();
</script>

<style scoped>
.club-owner-dashboard {
  min-height: calc(100vh - 4rem);
  padding: var(--spacing-xl) 0;
  background-color: var(--color-surface);
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

.dashboard-content {
  max-width: 1200px;
  margin: 0 auto;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: var(--spacing-lg);
}

.dashboard-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.dashboard-card.full-width {
  grid-column: 1 / -1;
}

.card-header {
  background-color: var(--color-primary);
  color: white;
  padding: var(--spacing-md);
}

.card-header h3 {
  margin: 0;
  font-size: 1.25rem;
}

.card-content {
  padding: var(--spacing-lg);
}

.profile-info {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.label {
  font-weight: 600;
  color: var(--color-text-light);
}

.value {
  color: var(--color-text);
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.btn-sm {
  padding: var(--spacing-sm) var(--spacing-md);
  font-size: 0.875rem;
  text-decoration: none;
  text-align: center;
  display: inline-block;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

.stat-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  background-color: var(--color-surface);
  border-radius: 8px;
}

.stat-icon {
  font-size: 2rem;
}

.stat-content h4 {
  margin: 0 0 var(--spacing-xs) 0;
  color: var(--color-text-light);
  font-size: 0.875rem;
  font-weight: 600;
}

.stat-number {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-text);
}

.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: var(--spacing-lg);
}

.service-item {
  padding: var(--spacing-lg);
  background-color: var(--color-surface);
  border-radius: 8px;
  text-align: center;
}

.service-icon {
  font-size: 2.5rem;
  margin-bottom: var(--spacing-sm);
}

.service-item h4 {
  margin: 0 0 var(--spacing-sm) 0;
  color: var(--color-text);
}

.service-item p {
  margin: 0 0 var(--spacing-md) 0;
  color: var(--color-text-light);
  font-size: 0.875rem;
}

.service-link {
  color: var(--color-primary);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.875rem;
}

.service-link:hover {
  text-decoration: underline;
}

.service-link.disabled {
  color: var(--color-text-light);
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .services-grid {
    grid-template-columns: 1fr;
  }
  
  .dashboard-header h1 {
    font-size: 2rem;
  }
}
</style>
