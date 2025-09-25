<template>
  <nav class="scout-navbar">
    <div class="navbar-container">
      <div class="navbar-brand">
        <h2>Scout Portal</h2>
      </div>
      <div class="navbar-menu">
        <router-link to="/scout" class="nav-item" active-class="active">
          <i class="icon">🏠</i>
          Homepage
        </router-link>
        <router-link to="/scout/create-player" class="nav-item" active-class="active">
          <i class="icon">👤</i>
          Create Player Profile
        </router-link>
        <router-link to="/scout/metrics" class="nav-item" active-class="active">
          <i class="icon">📊</i>
          Metrics
        </router-link>
        <router-link to="/scout/sessions" class="nav-item" active-class="active">
          <i class="icon">📅</i>
          Sessions
        </router-link>
        <router-link to="/scout/analysis" class="nav-item" active-class="active">
          <i class="icon">📈</i>
          Player Analysis
        </router-link>
        <router-link to="/scout/recommendations" class="nav-item" active-class="active">
          <i class="icon">⭐</i>
          Player Recommendation
        </router-link>
        <button @click="handleLogout" class="nav-item logout-btn">
          <i class="icon">🚪</i>
          Log Out
        </button>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { logout } from '../../services/auth_service'

const router = useRouter()

const handleLogout = () => {
  logout()
  router.push('/login')
}
</script>

<style scoped>
.scout-navbar {
  background-color: var(--color-primary);
  color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 100;
}

.navbar-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
  display: flex;
  justify-content: space-between;
  align-items: center;
  min-height: 4rem;
}

.navbar-brand h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
}

.navbar-menu {
  display: flex;
  gap: var(--spacing-md);
  align-items: center;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  padding: var(--spacing-sm) var(--spacing-md);
  text-decoration: none;
  color: white;
  border-radius: var(--border-radius);
  transition: all 0.2s;
  font-weight: 500;
  border: none;
  background: none;
  cursor: pointer;
  font-size: 1rem;
}

.nav-item:hover {
  background-color: rgba(255, 255, 255, 0.1);
}

.nav-item.active {
  background-color: rgba(255, 255, 255, 0.2);
  font-weight: 600;
}

.logout-btn {
  margin-left: var(--spacing-lg);
  background-color: rgba(220, 38, 38, 0.8);
}

.logout-btn:hover {
  background-color: rgba(220, 38, 38, 1);
}

.icon {
  font-size: 1.1rem;
}

/* Responsive design */
@media (max-width: 768px) {
  .navbar-container {
    flex-direction: column;
    padding: var(--spacing-md);
    min-height: auto;
  }
  
  .navbar-brand {
    margin-bottom: var(--spacing-md);
  }
  
  .navbar-menu {
    flex-wrap: wrap;
    justify-content: center;
    gap: var(--spacing-sm);
  }
  
  .nav-item {
    font-size: 0.875rem;
    padding: var(--spacing-xs) var(--spacing-sm);
  }
}

:root {
  --color-primary: #3b82f6;
  --spacing-xs: 0.25rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 0.75rem;
  --spacing-lg: 1rem;
  --border-radius: 0.375rem;
}
</style>
