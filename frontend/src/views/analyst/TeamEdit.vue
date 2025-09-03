<template>
  <div class="team-edit">
    <!-- Header Navigation -->
    <header class="header">
      <div class="header-left">
        <div class="logo">📧</div>
      </div>
      
      <div class="header-center">
        <nav class="main-nav">
          <router-link 
            to="/analyst" 
            class="nav-tab"
            :class="{ active: $route.name === 'DataAnalysis' }"
          >
            Data analysis
          </router-link>
          <router-link 
            to="/matches" 
            class="nav-tab"
            :class="{ active: $route.name === 'Matches' }"
          >
            Matches
          </router-link>
          <router-link 
            to="/reports" 
            class="nav-tab"
            :class="{ active: $route.name === 'Reports' }"
          >
            Reports
          </router-link>
        </nav>
      </div>
      
      <div class="header-right">
        <div class="user-icon">👤</div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="main-content">
      <div class="content-header">
        <h1>Edit opponent</h1>
      </div>

      <div v-if="loading" class="loading-state">
        <p>Loading team data...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <p>Error loading team: {{ error }}</p>
        <button @click="fetchTeam" class="btn-primary">Try Again</button>
      </div>

      <form v-else @submit.prevent="submitForm" class="team-form">
        <!-- Left Side - Basic Information -->
        <div class="form-section basic-info">
          <h2>Basic information</h2>
          
          <div class="form-group">
            <label for="teamName">Team name</label>
            <input 
              type="text" 
              id="teamName"
              v-model="formData.name"
              placeholder="Team name"
              required
            />
          </div>

          <div class="form-group">
            <label for="coach">Coach:</label>
            <input 
              type="text" 
              id="coach"
              v-model="formData.coach"
              placeholder="Coach name"
              required
            />
          </div>

          <div class="form-group">
            <label for="state">State:</label>
            <input 
              type="text" 
              id="state"
              v-model="formData.state"
              placeholder="State"
              required
            />
          </div>

          <div class="form-group">
            <label for="city">City:</label>
            <input 
              type="text" 
              id="city"
              v-model="formData.city"
              placeholder="City"
              required
            />
          </div>

          <div class="form-group">
            <label for="foundedDate">Founded (year):</label>
            <input 
              type="number" 
              id="foundedDate"
              v-model="formData.foundedYear"
              placeholder="Year"
              min="1800"
              :max="new Date().getFullYear()"
            />
          </div>

          <div class="form-group">
            <label for="hall">Hall:</label>
            <input 
              type="text" 
              id="hall"
              v-model="formData.hall"
              placeholder="Hall name"
              required
            />
          </div>
        </div>

        <!-- Right Side - Playing Style and Analysis -->
        <div class="form-section playing-style">
          <h2>Playing style, strengths and weaknesses</h2>
          
          <div class="form-group">
            <label for="playingStyle">Playing style:</label>
            <select id="playingStyle" v-model="formData.playingStyle">
              <option value="">Playing style</option>
              <option value="Offensive">Offensive</option>
              <option value="Defensive">Defensive</option>
              <option value="Balanced">Balanced</option>
              <option value="Fast Break">Fast Break</option>
              <option value="Half Court">Half Court</option>
            </select>
          </div>

          <div class="form-group">
            <label for="keyStrengths">Key Strengths:</label>
            <textarea 
              id="keyStrengths"
              v-model="formData.keyStrengths"
              placeholder="Describe team's key strengths..."
              rows="4"
            ></textarea>
          </div>

          <div class="form-group">
            <label for="keyWeaknesses">Key Weaknesses:</label>
            <textarea 
              id="keyWeaknesses"
              v-model="formData.keyWeaknesses"
              placeholder="Describe team's key weaknesses..."
              rows="4"
            ></textarea>
          </div>
        </div>

        <!-- Players Section -->
        <div class="players-section">
          <div class="players-header">
            <h2>Players</h2>
            <button type="button" class="btn-add-players">+ Add players</button>
          </div>
          
          <div class="players-grid">
            <!-- Hardcoded players for now -->
            <div class="player-card" v-for="i in 8" :key="i">
              <h3>Kevin Punter (#0)</h3>
              <p>Position: Guard</p>
              <p>Age: 32</p>
              <p>Height: 193cm</p>
              <p>Weight: 86kg</p>
            </div>
          </div>
        </div>

        <!-- Submit Button -->
        <div class="form-actions">
          <button type="submit" class="btn-accept">Save Changes</button>
        </div>
      </form>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'

const route = useRoute()
const router = useRouter()

const teamId = route.params.id
const loading = ref(false)
const error = ref(null)

// Form data
const formData = ref({
  name: '',
  coach: '',
  state: '',
  city: '',
  hall: '',
  foundedYear: null,
  playingStyle: '',
  keyStrengths: '',
  keyWeaknesses: ''
})

// Fetch team data by ID
const fetchTeam = async () => {
  loading.value = true
  error.value = null

  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`https://localhost:5001/api/team/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })

    const team = response.data.value || response.data
    console.log('Fetched team for edit:', team)

    // Populate form data
    formData.value = {
      name: team.name || '',
      coach: team.coach || '',
      state: team.state || '',
      city: team.city || '',
      hall: team.hall || '',
      foundedYear: team.foundedDate ? new Date(team.foundedDate).getFullYear() : null,
      playingStyle: team.playingStyle || '',
      keyStrengths: team.keyStrengths || '',
      keyWeaknesses: team.keyWeaknesses || ''
    }
  } catch (err) {
    console.error('Error fetching team:', err)
    error.value = err.message || 'Failed to fetch team data'
  } finally {
    loading.value = false
  }
}

const submitForm = async () => {
  try {
    const jwt = localStorage.getItem('token')
    
    // Prepare data for API
    const teamData = {
      idTeam: parseInt(teamId),
      name: formData.value.name,
      coach: formData.value.coach,
      state: formData.value.state,
      city: formData.value.city,
      hall: formData.value.hall,
      foundedDate: formData.value.foundedYear ? new Date(formData.value.foundedYear, 0, 1).toISOString() : null,
      playingStyle: formData.value.playingStyle,
      keyStrengths: formData.value.keyStrengths,
      keyWeaknesses: formData.value.keyWeaknesses
    }

    const response = await axios.put(`https://localhost:5001/api/team/${teamId}`, teamData, {
      headers: {
        Authorization: `Bearer ${jwt}`,
        'Content-Type': 'application/json'
      }
    })

    console.log('Team updated successfully:', response.data)
    router.push('/analyst')
  } catch (error) {
    console.error('Error updating team:', error)
    alert('Error updating team: ' + (error.response?.data?.message || error.message))
  }
}

onMounted(() => {
  fetchTeam()
})
</script>

<style scoped>
.team-edit {
  min-height: 100vh;
  background-color: #f8f9fa;
}

/* Header Styles - Same as DataAnalysis */
.header {
  background-color: white;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 2rem;
  border-bottom: 1px solid #e0e0e0;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.header-left, .header-right {
  flex: 1;
}

.header-right {
  display: flex;
  justify-content: flex-end;
}

.logo, .user-icon {
  font-size: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  background-color: #f0f0f0;
  border-radius: 8px;
}

.header-center {
  flex: 2;
  display: flex;
  justify-content: center;
}

.main-nav {
  display: flex;
  background-color: #f0f0f0;
  border-radius: 8px;
  padding: 4px;
  gap: 4px;
}

.nav-tab {
  padding: 8px 24px;
  text-decoration: none;
  color: #666;
  border-radius: 6px;
  transition: all 0.2s ease;
  font-weight: 500;
}

.nav-tab:hover {
  background-color: #e0e0e0;
  text-decoration: none;
  color: #333;
}

.nav-tab.active {
  background-color: white;
  color: #333;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

/* Main Content */
.main-content {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.content-header h1 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 2rem;
}

/* Loading and Error States */
.loading-state, .error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  text-align: center;
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.loading-state p, .error-state p {
  font-size: 1.1rem;
  color: #666;
  margin-bottom: 1rem;
}

/* Form Styles */
.team-form {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  grid-template-areas: 
    "basic playing"
    "players players"
    "actions actions";
}

.form-section {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid #e0e0e0;
}

.basic-info {
  grid-area: basic;
}

.playing-style {
  grid-area: playing;
}

.players-section {
  grid-area: players;
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid #e0e0e0;
}

.form-actions {
  grid-area: actions;
  display: flex;
  justify-content: center;
  margin-top: 1rem;
}

.form-section h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  color: #333;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #333;
  font-size: 0.9rem;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #d0d0d0;
  border-radius: 6px;
  font-size: 14px;
  transition: border-color 0.2s ease;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

/* Players Section */
.players-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.players-header h2 {
  margin-bottom: 0;
}

.btn-add-players {
  background-color: transparent;
  border: 1px solid #d0d0d0;
  color: #666;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s ease;
}

.btn-add-players:hover {
  background-color: #f8f9fa;
  border-color: #007bff;
  color: #007bff;
}

.players-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.player-card {
  background-color: #f8f9fa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1rem;
  text-align: left;
}

.player-card h3 {
  font-size: 0.9rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #333;
}

.player-card p {
  font-size: 0.8rem;
  color: #666;
  margin: 0.2rem 0;
}

/* Accept Button */
.btn-accept {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 12px 48px;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-accept:hover {
  background-color: #0056b3;
}

.btn-primary {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-primary:hover {
  background-color: #0056b3;
}

/* Responsive Design */
@media (max-width: 768px) {
  .header {
    padding: 1rem;
    flex-direction: column;
    gap: 1rem;
  }

  .header-left, .header-center, .header-right {
    flex: none;
  }

  .main-nav {
    width: 100%;
    justify-content: center;
  }

  .main-content {
    padding: 1rem;
  }

  .team-form {
    grid-template-columns: 1fr;
    grid-template-areas: 
      "basic"
      "playing"
      "players"
      "actions";
  }

  .players-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 480px) {
  .players-grid {
    grid-template-columns: 1fr;
  }
}
</style>
