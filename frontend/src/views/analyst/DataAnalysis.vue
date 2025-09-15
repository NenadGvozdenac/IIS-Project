<template>
  <div class="data-analysis">
    <!-- Main Content -->
    <main class="main-content">
      <div class="content-header">
        <h1>Data analysis</h1>
        <div class="header-actions">
          <div class="search-container">
            <input 
              type="text" 
              placeholder="Search opponents" 
              class="search-input"
              v-model="searchQuery"
            />
            <span class="search-icon">🔍</span>
          </div>
          <button class="btn-secondary" @click="checkMyTeam">
            Check my team
          </button>
          <button class="btn-primary" @click="addOpponent">
            + Add opponent
          </button>
        </div>
      </div>

      <!-- Teams Grid -->
      <div v-if="loading" class="loading-state">
        <p>Loading teams...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <p>Error loading teams: {{ error }}</p>
        <button @click="fetchTeams" class="btn-primary">Try Again</button>
      </div>

      <div v-else-if="filteredTeams.length === 0" class="empty-state">
        <p>No teams found.</p>
      </div>

      <div v-else class="teams-grid">
        <div 
          v-for="team in filteredTeams" 
          :key="team.idTeam"
          class="team-card"
        >
          <div class="team-info">
            <h3 class="team-name">{{ team.name }}</h3>
            <p class="team-detail">Coach: {{ team.coach || 'N/A' }}</p>
            <p class="team-detail">Location: {{ team.city }}, {{ team.state }}</p>
            <p class="team-detail">Hall: {{ team.hall }}</p>
            <p class="team-detail" v-if="team.foundedDate">Founded: {{ formatDate(team.foundedDate) }}</p>
            <p class="team-detail" v-if="team.playingStyle">Playing style: {{ team.playingStyle }}</p>
          </div>
          <div class="team-actions">
            <button 
              class="btn-outline" 
              @click="viewDetailedProfile(team.idTeam)"
            >
              Detailed profile
            </button>
            <button 
              class="btn-gray" 
              @click="editTeam(team.idTeam)"
            >
              Edit
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'

const router = useRouter()
const searchQuery = ref('')
const teams = ref([])
const loading = ref(false)
const error = ref(null)

// API call to fetch teams
const fetchTeams = async () => {
  loading.value = true
  error.value = null
  console.log("fetchTeams called");

  try {
    const jwt = localStorage.getItem('token');
    console.log("JWT token:", jwt);
    console.log("Fetching teams from:", `${MATCHES_URL}/team`);
    const response = await axios.get(`${MATCHES_URL}/team`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    });
    console.log("Fetched teams:", response.data.value);
    teams.value = response.data.value.teams || [];
  } catch (err) {
    console.error("Error fetching teams:", err);
    error.value = err.message || 'Failed to fetch teams';
  } finally {
    loading.value = false;
  }
}

const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  
  try {
    const date = new Date(dateString)
    return date.getFullYear().toString()
  } catch {
    return 'N/A'
  }
}

const filteredTeams = computed(() => {
  // Exclude team with ID 1 (my team)
  const teamsWithoutMyTeam = teams.value.filter(team => team.idTeam !== 1)
  
  if (!searchQuery.value) {
    return teamsWithoutMyTeam
  }
  return teamsWithoutMyTeam.filter(team => 
    team.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    (team.coach && team.coach.toLowerCase().includes(searchQuery.value.toLowerCase())) ||
    team.city.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    team.state.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

const viewDetailedProfile = (teamId) => {
  router.push(`/analyst/team/${teamId}`)
}

const editTeam = (teamId) => {
  router.push(`/analyst/team/${teamId}/edit`)
}

const addOpponent = () => {
  router.push('/analyst/team/add')
}

const checkMyTeam = () => {
  router.push(`/analyst/team/1`)
}

onMounted(() => {
  fetchTeams()
})
</script>

<style scoped>
.data-analysis {
  min-height: 100vh;
  background-color: #f8f9fa;
}

/* Header Styles */
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

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.content-header h1 {
  font-size: 2rem;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.search-container {
  position: relative;
  display: flex;
  align-items: center;
}

.search-input {
  padding: 8px 12px;
  padding-right: 35px;
  border: 1px solid #d0d0d0;
  border-radius: 6px;
  font-size: 14px;
  width: 200px;
}

.search-input:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
}

.search-icon {
  position: absolute;
  right: 10px;
  color: #666;
  pointer-events: none;
}

/* Buttons */
.btn-primary, .btn-secondary, .btn-outline, .btn-gray {
  padding: 8px 16px;
  border-radius: 6px;
  font-weight: 500;
  text-decoration: none;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 14px;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover {
  background-color: #0056b3;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background-color: #545b62;
}

.btn-outline {
  background-color: white;
  color: #007bff;
  border: 1px solid #007bff;
}

.btn-outline:hover {
  background-color: #007bff;
  color: white;
}

.btn-gray {
  background-color: #e9ecef;
  color: #495057;
  border: 1px solid #ced4da;
}

.btn-gray:hover {
  background-color: #d1ecf1;
  border-color: #bee5eb;
}

/* Teams Grid */
.teams-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 1.5rem;
}

/* Loading, Error, and Empty States */
.loading-state, .error-state, .empty-state {
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

.loading-state p, .error-state p, .empty-state p {
  font-size: 1.1rem;
  color: #666;
  margin-bottom: 1rem;
}

.error-state button {
  margin-top: 1rem;
}

.team-card {
  background-color: white;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.team-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0,0,0,0.15);
}

.team-info {
  margin-bottom: 1.5rem;
}

.team-name {
  font-size: 1.25rem;
  font-weight: 600;
  color: #333;
  margin: 0 0 0.5rem 0;
}

.team-detail {
  font-size: 0.9rem;
  color: #666;
  margin: 0.25rem 0;
}

.team-actions {
  display: flex;
  gap: 0.75rem;
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

  .content-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .header-actions {
    width: 100%;
    justify-content: flex-start;
  }

  .search-input {
    width: 150px;
  }

  .teams-grid {
    grid-template-columns: 1fr;
  }

  .team-actions {
    flex-direction: column;
  }
}

@media (max-width: 480px) {
  .header-actions {
    flex-direction: column;
    align-items: stretch;
  }

  .search-container {
    order: -1;
  }

  .search-input {
    width: 100%;
  }
}
</style>
