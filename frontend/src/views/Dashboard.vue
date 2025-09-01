<template>
  <div class="dashboard">
    <nav class="navbar">
      <div class="nav-content">
        <h1>Dashboard</h1>
        <div class="nav-actions">
          <span class="user-greeting">Welcome, {{ userInfo?.name || 'User' }}!</span>
          <button @click="handleLogout" class="btn btn-outline">Logout</button>
        </div>
      </div>
    </nav>

    <main class="main-content">
      <div class="dashboard-grid">
        <div class="card">
          <h3>Profile Information</h3>
          <div class="profile-info">
            <p><strong>Name:</strong> {{ userInfo?.name || 'N/A' }} {{ userInfo?.surname || '' }}</p>
            <p><strong>Email:</strong> {{ userInfo?.email || 'N/A' }}</p>
          </div>
        </div>

        <div class="card">
          <h3>Quick Actions</h3>
          <div class="actions">
            <button class="btn btn-primary">Update Profile</button>
            <button class="btn btn-secondary">Settings</button>
          </div>
        </div>

        <div class="card">
          <h3>Recent Activity</h3>
          <p>No recent activity to display.</p>
        </div>

        <div class="card">
          <h3>Statistics</h3>
          <div class="stats">
            <div class="stat-item">
              <span class="stat-number">0</span>
              <span class="stat-label">Total Items</span>
            </div>
            <div class="stat-item">
              <span class="stat-number">1</span>
              <span class="stat-label">Active Sessions</span>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { useAuth } from '../composables/useAuth'
import { useRouter } from 'vue-router'

const { userInfo, logout } = useAuth()
const router = useRouter()

const handleLogout = () => {
  logout()
  router.push('/')
}
</script>

<style scoped>
.dashboard {
  min-height: 100vh;
  background-color: #f9fafb;
}

.navbar {
  background-color: white;
  border-bottom: 1px solid #e5e7eb;
  padding: 1rem 0;
}

.nav-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.navbar h1 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-greeting {
  color: #6b7280;
  font-weight: 500;
}

.main-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.card {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
  border: 1px solid #e5e7eb;
}

.card h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 1rem 0;
}

.profile-info p {
  margin: 0.5rem 0;
  color: #374151;
}

.actions {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.stats {
  display: flex;
  gap: 2rem;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-number {
  font-size: 2rem;
  font-weight: 700;
  color: #4f46e5;
}

.stat-label {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.25rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  text-decoration: none;
  display: inline-block;
}

.btn-primary {
  background-color: #4f46e5;
  color: white;
}

.btn-primary:hover {
  background-color: #4338ca;
}

.btn-secondary {
  background-color: #f3f4f6;
  color: #374151;
}

.btn-secondary:hover {
  background-color: #e5e7eb;
}

.btn-outline {
  background-color: transparent;
  color: #6b7280;
  border: 1px solid #d1d5db;
}

.btn-outline:hover {
  background-color: #f9fafb;
  color: #374151;
}

@media (max-width: 768px) {
  .nav-content {
    flex-direction: column;
    gap: 1rem;
  }

  .nav-actions {
    flex-direction: column;
    gap: 0.5rem;
  }

  .dashboard-grid {
    grid-template-columns: 1fr;
  }

  .stats {
    justify-content: space-around;
  }
}
</style>
