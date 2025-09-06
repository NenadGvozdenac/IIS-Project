<template>
  <div class="dashboard">
    <div class="container">
      <div class="dashboard-header">
        <h1>Welcome to your Dashboard</h1>
        <p>Hi {{ userInfo?.userName || 'User' }}, manage your Sports Hub account and activities here.</p>
      </div>

      <div class="dashboard-content">
        <div class="dashboard-grid">
          <!-- User Info Card -->
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
                  <span class="label">User Type:</span>
                  <span class="value">{{ userInfo?.userRole || 'N/A' }}</span>
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
                <button class="btn btn-primary btn-sm">Book Tickets</button>
                <button class="btn btn-secondary btn-sm">Find Events</button>
                <button class="btn btn-secondary btn-sm">Update Profile</button>
              </div>
            </div>
          </div>

          <!-- Recent Activity Card -->
          <div class="dashboard-card full-width">
            <div class="card-header">
              <h3>Recent Activity</h3>
            </div>
            <div class="card-content">
              <div class="activity-list">
                <div class="activity-item">
                  <div class="activity-icon">🎫</div>
                  <div class="activity-content">
                    <p class="activity-title">Welcome to Sports Hub!</p>
                    <p class="activity-time">Account created successfully</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Services Card -->
          <div class="dashboard-card full-width">
            <div class="card-header">
              <h3>Available Services</h3>
            </div>
            <div class="card-content">
              <div class="services-grid">
                <div class="service-item">
                  <div class="service-icon">🎫</div>
                  <h4>Ticket Services</h4>
                  <p>Book tickets for sports events</p>
                </div>
                <div class="service-item">
                  <div class="service-icon">🔍</div>
                  <h4>Scouting Services</h4>
                  <p>Professional talent scouting</p>
                </div>
                <div class="service-item">
                  <div class="service-icon">✈️</div>
                  <h4>Travel Services</h4>
                  <p>Travel arrangements for teams</p>
                </div>
                <div class="service-item">
                  <div class="service-icon">🏆</div>
                  <h4>Match Services</h4>
                  <p>Match organization and management</p>
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
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { getUserData } from '../services/auth_service';

const router = useRouter();
const userInfo = getUserData();

onMounted(() => {
  // Redirect customers to their specific dashboard
  if (userInfo && userInfo.userRole === 'customer') {
    router.replace('/customer-dashboard');
    return;
  }
  else if(userInfo && userInfo.userRole === 'team manager') {
    router.replace('/team-manager/matches');
    return;
  }
});
</script>

<style scoped>
.dashboard {
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
  padding: 0 var(--spacing-md);
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--spacing-xl);
}

.dashboard-card {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  overflow: hidden;
  transition: box-shadow 0.2s ease;
}

.dashboard-card:hover {
  box-shadow: var(--shadow-md);
}

.dashboard-card.full-width {
  grid-column: 1 / -1;
}

.card-header {
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
  background-color: var(--color-surface);
}

.card-header h3 {
  color: var(--color-text);
  font-size: 1.25rem;
  font-weight: 600;
}

.card-content {
  padding: var(--spacing-lg);
}

/* Profile Info */
.profile-info {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.info-row .label {
  font-weight: 500;
  color: var(--color-text-light);
}

.info-row .value {
  color: var(--color-text);
  font-weight: 500;
}

/* Action Buttons */
.action-buttons {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.btn-sm {
  padding: var(--spacing-xs) var(--spacing-md);
  font-size: 0.875rem;
}

/* Activity List */
.activity-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.activity-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  background-color: var(--color-surface);
  border-radius: var(--radius-md);
}

.activity-icon {
  font-size: 1.5rem;
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: white;
  border-radius: var(--radius-md);
}

.activity-content {
  flex: 1;
}

.activity-title {
  color: var(--color-text);
  font-weight: 500;
  margin-bottom: var(--spacing-xs);
}

.activity-time {
  color: var(--color-text-light);
  font-size: 0.875rem;
}

/* Services Grid */
.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

.service-item {
  text-align: center;
  padding: var(--spacing-lg);
  background-color: var(--color-surface);
  border-radius: var(--radius-md);
  transition: transform 0.2s ease;
}

.service-item:hover {
  transform: translateY(-2px);
}

.service-icon {
  font-size: 2rem;
  margin-bottom: var(--spacing-md);
}

.service-item h4 {
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
  font-size: 1.125rem;
}

.service-item p {
  color: var(--color-text-light);
  font-size: 0.875rem;
  line-height: 1.5;
}

/* Responsive */
@media (max-width: 768px) {
  .dashboard-header h1 {
    font-size: 2rem;
  }
  
  .dashboard-header p {
    font-size: 1rem;
  }
  
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
  
  .info-row {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--spacing-xs);
  }
  
  .services-grid {
    grid-template-columns: 1fr;
  }
}
</style>
