<template>
  <nav class="navbar">
    <div class="container navbar-content">
      <div class="navbar-brand">
        <router-link to="/" class="brand-link">
          Sports Hub
        </router-link>
      </div>

      <div class="navbar-menu">
        <div class="navbar-nav">
          <router-link v-if="userInfo && userInfo.userRole === 'admin'" to="/admin/zones-seats"
            class="nav-link admin-link">
            Zones & Seats
          </router-link>
          <router-link v-if="userInfo && userInfo.userRole === 'club owner'" to="/club-owner/seasons"
            class="nav-link club-owner-link">
            Seasons
          </router-link>
          <router-link v-if="userInfo && userInfo.userRole === 'club owner'" to="/club-owner/competitions"
            class="nav-link club-owner-link">
            Competitions
          </router-link>
          <router-link v-if="userInfo && userInfo.userRole === 'analyst'" to="/analyst"
            class="nav-link analyst-link">
            Data analysis
          </router-link>
          <router-link v-if="userInfo && userInfo.userRole === 'analyst'" to="/analyst/matches"
            class="nav-link analyst-link">
            Matches
          </router-link>
        </div>

        <div class="navbar-auth">
          <template v-if="!userInfo">
            <router-link to="/login" class="btn btn-ghost">Login</router-link>
            <router-link to="/register" class="btn btn-primary">Sign Up</router-link>
          </template>
          <template v-else>
            <router-link v-if="isCustomer()" to="/profile" class="btn btn-ghost">Profile</router-link>
            <router-link v-if="isCustomer()" to="/seasonal-tickets" class="btn btn-ghost">Season Tickets</router-link>
            <router-link v-if="isCustomer()" to="/cart" class="btn btn-ghost">Cart</router-link>
            <router-link to="/dashboard" class="btn btn-ghost">Dashboard</router-link>
            <button @click="handleLogout" class="btn btn-secondary">Logout</button>
          </template>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { getUserData, logout } from '../services/auth_service';

const userInfo = ref(getUserData());

// Function to update user info
const updateUserInfo = () => {
  userInfo.value = getUserData();
};

// Listen for storage events (when token changes)
const handleStorageChange = (e) => {
  if (e.key === 'token') {
    updateUserInfo();
  }
};

// Listen for custom events when user logs in/out
const handleAuthChange = () => {
  updateUserInfo();
};

onMounted(() => {
  // Listen for storage changes (logout from another tab)
  window.addEventListener('storage', handleStorageChange);

  // Listen for custom auth events
  window.addEventListener('auth-changed', handleAuthChange);

  // Update user info on mount
  updateUserInfo();
});

onUnmounted(() => {
  window.removeEventListener('storage', handleStorageChange);
  window.removeEventListener('auth-changed', handleAuthChange);
});

// Override logout function to emit event
const handleLogout = () => {
  logout();
  updateUserInfo();
  // Emit custom event for auth change
  window.dispatchEvent(new CustomEvent('auth-changed'));
};

const isCustomer = () => {
  return userInfo.value && userInfo.value.userRole === 'customer';
};
</script>

<style scoped>
.navbar {
  background-color: white;
  border-bottom: 1px solid var(--color-border);
  position: sticky;
  top: 0;
  z-index: 50;
  box-shadow: var(--shadow-sm);
}

.navbar-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 4rem;
}

.navbar-brand {
  flex-shrink: 0;
}

.brand-link {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-primary);
  text-decoration: none;
}

.brand-link:hover {
  color: var(--color-primary-hover);
  text-decoration: none;
}

.navbar-menu {
  display: flex;
  align-items: center;
  gap: var(--spacing-xl);
}

.navbar-nav {
  display: flex;
  align-items: center;
  gap: var(--spacing-lg);
}

.nav-link {
  color: var(--color-text);
  font-weight: 500;
  padding: var(--spacing-sm) var(--spacing-md);
  border-radius: var(--radius-md);
  transition: all 0.2s ease;
  text-decoration: none;
}

.nav-link:hover {
  color: var(--color-primary);
  background-color: var(--color-surface);
  text-decoration: none;
}

.nav-link.router-link-active {
  color: var(--color-primary);
  background-color: var(--color-surface);
}

.admin-link {
  background-color: #fef3c7;
  color: #92400e;
  font-weight: 600;
}

.admin-link:hover {
  background-color: #fcd34d;
  color: #78350f;
}

.club-owner-link {
  background-color: #f3e8ff;
  color: #7c3aed;
  font-weight: 600;
}

.club-owner-link:hover {
  background-color: #e9d5ff;
  color: #6d28d9;
}

.analyst-link {
  background-color: #e0f2fe;
  color: #0277bd;
  font-weight: 600;
}

.analyst-link:hover {
  background-color: #b3e5fc;
  color: #01579b;
}

.navbar-auth {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.user-greeting {
  color: var(--color-text);
  font-weight: 500;
  font-size: 0.875rem;
}

.mobile-menu-btn {
  display: none;
  flex-direction: column;
  background: none;
  border: none;
  cursor: pointer;
  padding: var(--spacing-sm);
  width: 2rem;
  height: 2rem;
  position: relative;
}

.mobile-menu-btn span {
  display: block;
  height: 2px;
  width: 100%;
  background-color: var(--color-text);
  margin: 2px 0;
  transition: all 0.3s ease;
  transform-origin: center;
}

.mobile-menu-btn.active span:nth-child(1) {
  transform: rotate(45deg) translate(5px, 5px);
}

.mobile-menu-btn.active span:nth-child(2) {
  opacity: 0;
}

.mobile-menu-btn.active span:nth-child(3) {
  transform: rotate(-45deg) translate(7px, -6px);
}

.mobile-menu {
  display: none;
  background-color: white;
  border-top: 1px solid var(--color-border);
  padding: var(--spacing-md) 0;
}

.mobile-menu.active {
  display: block;
}

.mobile-nav {
  padding: 0 var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}

.mobile-nav-link {
  display: block;
  color: var(--color-text);
  font-weight: 500;
  padding: var(--spacing-md);
  border-radius: var(--radius-md);
  text-decoration: none;
  transition: all 0.2s ease;
}

.mobile-nav-link:hover {
  color: var(--color-primary);
  background-color: var(--color-surface);
  text-decoration: none;
}

.mobile-nav-link.router-link-active {
  color: var(--color-primary);
  background-color: var(--color-surface);
}

.mobile-auth {
  padding: 0 var(--spacing-md);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.w-full {
  width: 100%;
}

/* Responsive */
@media (max-width: 768px) {
  .navbar-menu {
    display: none;
  }

  .mobile-menu-btn {
    display: flex;
  }
}
</style>
