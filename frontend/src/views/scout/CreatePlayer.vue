<template>
  <div class="scout-page">
    <ScoutNavbar />
    <div class="main-content">
      <div class="container">
        <div class="page-header">
          <h1>Create Player Profile</h1>
          <p>Add a new player to the database</p>
        </div>
        <div class="form-container">
          <form @submit.prevent="createPlayer" class="player-form">
            <div class="form-grid">
              <div class="form-group">
                <label for="firstName">First Name</label>
                <input id="firstName" v-model="player.firstName" type="text" required />
              </div>
              <div class="form-group">
                <label for="lastName">Last Name</label>
                <input id="lastName" v-model="player.lastName" type="text" required />
              </div>
              <div class="form-group">
                <label for="dateOfBirth">Date of Birth</label>
                <input id="dateOfBirth" v-model="player.dateOfBirth" type="date" required />
              </div>
              <div class="form-group">
                <label for="position">Position</label>
                <select id="position" v-model="player.position" required>
                  <option value="">Select Position</option>
                  <option value="Point Guard">Point Guard</option>
                  <option value="Shooting Guard">Shooting Guard</option>
                  <option value="Small Forward">Small Forward</option>
                  <option value="Power Forward">Power Forward</option>
                  <option value="Center">Center</option>
                </select>
              </div>
              <div class="form-group">
                <label for="nationality">Nationality</label>
                <input id="nationality" v-model="player.nationality" type="text" required />
              </div>
              <div class="form-group">
                <label for="height">Height (cm)</label>
                <input id="height" v-model="player.height" type="number" min="150" max="250" />
              </div>
              <div class="form-group">
                <label for="wingspan">Wingspan (cm)</label>
                <input id="wingspan" v-model="player.wingspan" type="number" min="150" max="300" />
              </div>
              <div class="form-group">
                <label for="verticalJump">Vertical Jump (cm)</label>
                <input id="verticalJump" v-model="player.verticalJump" type="number" min="30" max="150" />
              </div>
            </div>
            <div class="form-actions">
              <button type="button" @click="resetForm" class="btn btn-secondary">Reset</button>
              <button type="submit" class="btn btn-primary">Create Player</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import ScoutNavbar from '../../components/scout/ScoutNavbar.vue'

const router = useRouter()

const player = ref({
  firstName: '',
  lastName: '',
  dateOfBirth: '',
  position: '',
  nationality: '',
  height: null,
  wingspan: null,
  verticalJump: null
})

const createPlayer = () => {
  // Here you would typically make an API call to create the player
  console.log('Creating player:', player.value)
  alert('Player profile created successfully!')
  router.push('/scout')
}

const resetForm = () => {
  player.value = {
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    position: '',
    nationality: '',
    height: null,
    wingspan: null,
    verticalJump: null
  }
}
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
  max-width: 800px;
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

.form-container {
  background: white;
  padding: var(--spacing-xl);
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-sm);
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-weight: 500;
  margin-bottom: var(--spacing-sm);
  color: var(--color-text);
}

.form-group input,
.form-group select {
  padding: var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--border-radius);
  font-size: 1rem;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.1);
}

.form-actions {
  display: flex;
  gap: var(--spacing-md);
  justify-content: flex-end;
}

.btn {
  padding: var(--spacing-md) var(--spacing-xl);
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-primary {
  background-color: var(--color-primary);
  color: white;
}

.btn-primary:hover {
  background-color: var(--color-primary-dark);
}

.btn-secondary {
  background-color: var(--color-secondary);
  color: white;
}

.btn-secondary:hover {
  background-color: var(--color-secondary-dark);
}

:root {
  --color-primary: #3b82f6;
  --color-primary-dark: #2563eb;
  --color-secondary: #6b7280;
  --color-secondary-dark: #4b5563;
  --color-surface: #f8fafc;
  --color-text: #1f2937;
  --color-text-light: #6b7280;
  --color-border: #e5e7eb;
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --border-radius: 0.375rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 0.75rem;
  --spacing-lg: 1rem;
  --spacing-xl: 1.5rem;
}
</style>
