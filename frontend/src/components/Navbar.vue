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
          <router-link to="/" class="nav-link">Home</router-link>
          <router-link to="/about" class="nav-link">About</router-link>
          <router-link to="/services" class="nav-link">Services</router-link>
        </div>
        
        <div class="navbar-auth">
          <template v-if="!isAuthenticated">
            <router-link to="/login" class="btn btn-ghost">Login</router-link>
            <router-link to="/register" class="btn btn-primary">Sign Up</router-link>
          </template>
          <template v-else>
            <span class="user-greeting">Hello, {{ userName }}</span>
            <router-link to="/dashboard" class="btn btn-ghost">Dashboard</router-link>
            <button @click="logout" class="btn btn-secondary">Logout</button>
          </template>
        </div>
      </div>
      
      <!-- Mobile menu button -->
      <button 
        class="mobile-menu-btn"
        @click="toggleMobileMenu"
        :class="{ 'active': isMobileMenuOpen }"
      >
        <span></span>
        <span></span>
        <span></span>
      </button>
    </div>
    
    <!-- Mobile menu -->
    <div class="mobile-menu" :class="{ 'active': isMobileMenuOpen }">
      <div class="mobile-nav">
        <router-link to="/" class="mobile-nav-link" @click="closeMobileMenu">Home</router-link>
        <router-link to="/about" class="mobile-nav-link" @click="closeMobileMenu">About</router-link>
        <router-link to="/services" class="mobile-nav-link" @click="closeMobileMenu">Services</router-link>
      </div>
      
      <div class="mobile-auth">
        <template v-if="!isAuthenticated">
          <router-link to="/login" class="btn btn-ghost w-full" @click="closeMobileMenu">Login</router-link>
          <router-link to="/register" class="btn btn-primary w-full" @click="closeMobileMenu">Sign Up</router-link>
        </template>
        <template v-else>
          <span class="user-greeting">Hello, {{ userName }}</span>
          <router-link to="/dashboard" class="btn btn-ghost w-full" @click="closeMobileMenu">Dashboard</router-link>
          <button @click="logout" class="btn btn-secondary w-full">Logout</button>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'

const router = useRouter()
const { isAuthenticated, userName, logout: authLogout } = useAuth()
const isMobileMenuOpen = ref(false)

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

const logout = () => {
  authLogout()
  closeMobileMenu()
  router.push('/')
}
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
