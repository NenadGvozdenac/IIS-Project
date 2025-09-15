<template>
  <div class="team-edit">
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
            <input 
              type="text" 
              id="playingStyle"
              v-model="formData.playingStyle"
              placeholder="Playing style"
              required
            />
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
            <button type="button" class="btn-add-players" @click="showAddPlayerModal = true">+ Add players</button>
          </div>
          
          <div class="players-grid">
            <div v-if="loadingPlayers" class="loading-players">
              <p>Loading players...</p>
            </div>
            <div v-else-if="teamPlayers.length === 0" class="no-players">
              <p>No players found for this team.</p>
            </div>
            <div v-else class="player-card" v-for="player in teamPlayers" :key="player.playerId">
              <h3>{{ player.playerName }} {{ player.playerSurname }} (#{{ player.jerseyNumber || 'N/A' }})</h3>
              <p>Position: {{ player.positionName || 'N/A' }}</p>
              <p>Age: {{ player.age || 'N/A' }}</p>
              <p>Height: {{ player.height ? player.height + 'cm' : 'N/A' }}</p>
              <p>Weight: {{ player.weight ? player.weight + 'kg' : 'N/A' }}</p>
              <p>Status: {{ player.status || 'N/A' }}</p>
            </div>
          </div>
        </div>

        <!-- Submit Button -->
        <div class="form-actions">
          <button type="submit" class="btn-accept">Save Changes</button>
        </div>
      </form>
    </main>

    <!-- Add Player Modal -->
    <div v-if="showAddPlayerModal" class="modal-overlay" @click="closeModal">
      <div class="modal-content" @click.stop>
        <h2>Add new player</h2>
        
        <form @submit.prevent="addPlayer" class="player-form">
          <div class="form-row">
            <div class="form-group">
              <label for="firstName">First name:</label>
              <input 
                type="text" 
                id="firstName"
                v-model="playerForm.firstName"
                placeholder="First name"
                required
              />
            </div>
            
            <div class="form-group">
              <label for="lastName">Last name:</label>
              <input 
                type="text" 
                id="lastName"
                v-model="playerForm.lastName"
                placeholder="Last name"
                required
              />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="jerseyNumber">Number:</label>
              <input 
                type="number" 
                id="jerseyNumber"
                v-model="playerForm.jerseyNumber"
                placeholder="Number"
                min="0"
                max="99"
                required
              />
            </div>

            <div class="form-group">
              <label for="position">Position:</label>
              <select id="position" v-model="playerForm.position" required>
                <option value="">Position</option>
                <option v-for="pos in positions" :key="pos.idPosition" :value="pos.idPosition">
                  {{ pos.name }}
                </option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="birthday">Date of birth:</label>
              <input 
                type="date" 
                id="birthday"
                v-model="playerForm.birthday"
                required
              />
            </div>

            <div class="form-group">
              <label for="height">Height:</label>
              <input 
                type="number" 
                id="height"
                v-model="playerForm.height"
                placeholder="Height (cm)"
                min="150"
                max="250"
              />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="weight">Weight:</label>
              <input 
                type="number" 
                id="weight"
                v-model="playerForm.weight"
                placeholder="Weight (kg)"
                min="50"
                max="200"
              />
            </div>
          </div>

          <div class="modal-actions">
            <button type="submit" class="btn-accept">Accept</button>
            <button type="button" class="btn-decline" @click="closeModal">Decline</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { MATCHES_URL } from '../../services/const_service'

const route = useRoute()
const router = useRouter()

const teamId = route.params.id
const loading = ref(false)
const error = ref(null)
const showAddPlayerModal = ref(false)
const positions = ref([])
const teamPlayers = ref([])
const loadingPlayers = ref(false)

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

// Player form data
const playerForm = ref({
  firstName: '',
  lastName: '',
  jerseyNumber: null,
  position: '',
  birthday: '',
  height: null,
  weight: null
})
// Fetch team data by ID
const fetchTeam = async () => {
  loading.value = true
  error.value = null

  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/team/${teamId}`, {
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

// Fetch all positions for dropdown
const fetchPositions = async () => {
  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/position`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    //console.log('111Fetched positions:', response.data.value.positions)
    positions.value = response.data.value.positions || []
    console.log('Fetched positions:', positions.value)
  } catch (err) {
    console.error('Error fetching positions:', err)
  }
}

// Fetch team players
const fetchPlayers = async () => {
  loadingPlayers.value = true
  try {
    const jwt = localStorage.getItem('token')
    const response = await axios.get(`${MATCHES_URL}/teammember/team/${teamId}`, {
      headers: {
        Authorization: `Bearer ${jwt}`
      }
    })
    
    teamPlayers.value = response.data.value?.teamPlayers || []
    console.log('Fetched team players:', teamPlayers.value)
  } catch (err) {
    console.error('Error fetching team players:', err)
  } finally {
    loadingPlayers.value = false
  }
}

// Close modal and reset form
const closeModal = () => {
  showAddPlayerModal.value = false
  playerForm.value = {
    firstName: '',
    lastName: '',
    jerseyNumber: null,
    position: '',
    birthday: '',
    height: null,
    weight: null
  }
}

// Add new player
const addPlayer = async () => {
  try {
    const jwt = localStorage.getItem('token')
    
    // Convert birthday string to DateOnly format for backend
    const birthdayDate = new Date(playerForm.value.birthday)
    const formattedBirthday = birthdayDate.toISOString().split('T')[0] // YYYY-MM-DD format
    
    const playerData = {
      name: playerForm.value.firstName,
      surname: playerForm.value.lastName,
      birthday: formattedBirthday,
      weight: playerForm.value.weight,
      height: playerForm.value.height,
      idPosition: parseInt(playerForm.value.position),
      jerseyNumber: playerForm.value.jerseyNumber,
      idTeam: parseInt(teamId)
    }
    console.log("playerData", playerData)

    const response = await axios.post(`${MATCHES_URL}/player/with-team-member`, playerData, {
      headers: {
        Authorization: `Bearer ${jwt}`,
        'Content-Type': 'application/json'
      }
    })

    console.log('Player added successfully:', response.data)
    
    // Close modal and reset form
    closeModal()
    
    // Refresh players list to show new player
    await fetchPlayers()
    
    //alert('Player added successfully!')
  } catch (error) {
    console.error('Error adding player:', error)
    alert('Error adding player: ' + (error.response?.data?.message || error.message))
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
      foundedDate: formData.value.foundedYear 
          ? `${formData.value.foundedYear}-01-01`
          : null,
      playingStyle: formData.value.playingStyle,
      keyStrengths: formData.value.keyStrengths,
      keyWeaknesses: formData.value.keyWeaknesses
    }
    console.log('Team data to be updated:', teamData)

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
    //alert('Error updating team: ' + (error.response?.data?.message || error.message))
  }
}

onMounted(() => {
  fetchTeam()
  fetchPositions()
  fetchPlayers()
})
</script>

<style scoped>
.team-edit {
  min-height: 100vh;
  background-color: #f8f9fa;
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

.loading-players, .no-players {
  grid-column: 1 / -1;
  text-align: center;
  padding: 2rem;
  color: #666;
  font-style: italic;
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  max-width: 600px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.modal-content h2 {
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
  text-align: center;
}

.player-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.modal-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 2rem;
}

.btn-decline {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-decline:hover {
  background-color: #5a6268;
}

/* Responsive Design */
@media (max-width: 768px) {
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

  .form-row {
    grid-template-columns: 1fr;
  }

  .modal-content {
    padding: 1.5rem;
    margin: 1rem;
  }
}

@media (max-width: 480px) {
  .players-grid {
    grid-template-columns: 1fr;
  }

  .modal-actions {
    flex-direction: column;
  }

  .modal-content {
    padding: 1rem;
    margin: 0.5rem;
  }
}
</style>
