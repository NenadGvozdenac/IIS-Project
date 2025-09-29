<template>
  <div class="transportation-request">

    <!-- Main Content -->
    <div class="main-content">
      <!-- Request Form -->
      <div class="request-form-card">
        <h2>Creating a transport request</h2>
        
        <div class="form-grid">
          <div class="form-group">
            <label for="passengers">Number of passengers *</label>
            <input 
              type="number" 
              id="passengers" 
              v-model="formData.numberOfPassengers"
              required
            >
          </div>

          <div class="form-group">
            <label for="startDate">Start date *</label>
            <input 
              type="date" 
              id="startDate" 
              v-model="formData.startDate"
              :min="today"
              @change="onStartDateChange"
              required
            >
          </div>

          <div class="form-group">
            <label for="endDate">End date *</label>
            <input 
              type="date" 
              id="endDate" 
              v-model="formData.endDate"
              :min="formData.startDate || today"
              required
            >
          </div>
          
          <div class="form-group">
            <label for="state">State *</label>
            <input 
              type="text" 
              id="state" 
              v-model="formData.state"
              placeholder="Country"
              required
            >
          </div>

          

          <div class="form-group">
            <label for="city">City *</label>
            <input 
              type="text" 
              id="city" 
              v-model="formData.city"
              placeholder="City"
              required
            >
          </div>

          <div class="form-group">
            <label for="hall">Hall *</label>
            <input 
              type="text" 
              id="hall" 
              v-model="formData.hall"
              placeholder="Hall/Venue name"
              required
            >
          </div>

          <div class="form-group">
            <label for="vehicleType">Vehicle type *</label>
            <select 
              id="vehicleType" 
              v-model="formData.vehicleType"
              required
            >
              <option value="">Select vehicle type</option>
              <option value="autobus">Autobus</option>
              <option value="plane">Plane</option>
              <option value="train">Train</option>
              <option value="van">Van</option>
            </select>
          </div>

          <div class="form-group budget-group">
            <label for="budget">Budget *</label>
            <div class="budget-input">
              <input 
                type="number" 
                id="budget" 
                v-model="formData.budget"
                required
              >
              <span class="currency">EUR</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Passengers Section -->
      <div class="passengers-card">
        <h2>Passengers</h2>
        
        <div class="passengers-grid">
          <!-- Team Members -->
          <div 
            v-for="member in teamMembers" 
            :key="`team-${member.idTeam}-${member.idPlayer}`"
            class="passenger-item"
          >
            <label class="passenger-checkbox">
              <input 
                type="checkbox" 
                :value="{idTeam: member.idTeam, idPlayer: member.idPlayer}"
                v-model="selectedTeamMembers"
              >
              <span class="checkmark"></span>
              <div class="passenger-info">
                <div class="passenger-name">{{ member.playerName }} {{ member.playerSurname }}</div>
                <div class="passenger-role">player #{{ member.jerseyNumber }}</div>
              </div>
            </label>
          </div>

          <!-- Management Members -->
          <div 
            v-for="manager in managementMembers" 
            :key="`mgmt-${manager.memberId}`"
            class="passenger-item"
          >
            <label class="passenger-checkbox">
              <input 
                type="checkbox" 
                :value="manager.memberId"
                v-model="selectedManagementMembers"
              >
              <span class="checkmark"></span>
              <div class="passenger-info">
                <div class="passenger-name">{{ manager.memberName }} {{ manager.memberSurname }}</div>
                <div class="passenger-role">{{ manager.memberRole }}</div>
              </div>
            </label>
          </div>
        </div>
      </div>

      <!-- Send Request Button -->
      <div class="send-request-section">
        <button class="send-request-btn" @click="openAgencyModal">
          Send a request to agencies
        </button>
      </div>
    </div>

    <!-- Agency Selection Modal -->
    <div v-if="showAgencyModal" class="modal-overlay" @click="closeAgencyModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Select Transportation Agencies</h3>
          <button class="close-btn" @click="closeAgencyModal">&times;</button>
        </div>
        
        <div class="modal-body">
          <div class="agency-list">
            <div 
              v-for="agency in transportationAgencies" 
              :key="agency.idAgency"
              class="agency-item"
            >
              <label class="agency-checkbox">
                <input 
                  type="checkbox" 
                  :value="agency.idAgency"
                  v-model="selectedAgencies"
                >
                <span class="checkmark"></span>
                <div class="agency-info">
                  <div class="agency-name">{{ agency.name }}</div>
                  <div class="agency-email">{{ agency.email }}</div>
                </div>
              </label>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button class="cancel-btn" @click="closeAgencyModal">Cancel</button>
          <button class="send-btn" @click="sendRequest" :disabled="selectedAgencies.length === 0">
            Send Request
          </button>
        </div>
      </div>
    </div>

    <!-- Loading Overlay -->
    <div v-if="isLoading" class="loading-overlay">
      <div class="loading-spinner"></div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import { AuthService } from '../../services/auth_service.js'

export default {
  name: 'TransportationRequest',
  data() {
    return {
      formData: {
        numberOfPassengers: null,
        startDate: '',
        endDate: '',
        state: '',
        city: '',
        hall: '',
        vehicleType: '',
        budget: null
      },
      teamMembers: [],
      managementMembers: [],
      transportationAgencies: [],
      selectedTeamMembers: [],
      selectedManagementMembers: [],
      selectedAgencies: [],
      showAgencyModal: false,
      isLoading: false,
      matchId: null,
      userId: null
    }
  },
  computed: {
    today() {
      return new Date().toISOString().split('T')[0]
    }
  },
  async mounted() {
    this.matchId = parseInt(this.$route.params.matchId) || 1
    this.userId = this.getUserId()
    await this.loadData()
  },
  methods: {
    onStartDateChange() {
      // If end date is before start date, reset end date
      if (this.formData.endDate && this.formData.startDate && this.formData.endDate < this.formData.startDate) {
        this.formData.endDate = this.formData.startDate
      }
    },
    getUserId() {
      const token = localStorage.getItem('token')
      if (!token) return null
      
      const userData = AuthService.decode(token)
      return userData?.userID || null
    },
    async loadData() {
      this.isLoading = true
      try {
        await Promise.all([
          this.loadTeamMembers(),
          this.loadManagementMembers(),
          this.loadTransportationAgencies()
        ])
      } catch (error) {
        console.error('Error loading data:', error)
        alert('Failed to load data')
      } finally {
        this.isLoading = false
      }
    },

    async loadTeamMembers() {
      try {
        const response = await axios.get('https://localhost:5007/api/teammembers')
        if (response.data.isSuccess) {
          this.teamMembers = response.data.value.map(tm => ({
            idTeam: tm.idTeam,
            idPlayer: tm.idPlayer,
            playerName: tm.fullName?.split(' ')[0] || 'Unknown',
            playerSurname: tm.fullName?.split(' ').slice(1).join(' ') || 'Player',
            jerseyNumber: tm.jerseyNumber
          }))
        }
      } catch (error) {
        console.error('Error loading team members:', error)
      }
    },

    async loadManagementMembers() {
      try {
        const response = await axios.get('https://localhost:5007/api/managementmembers')
        if (response.data.isSuccess) {
          this.managementMembers = response.data.value.map(m => ({
            memberId: m.memberId,
            memberName: m.memberName,
            memberSurname: m.memberSurname,
            memberRole: m.memberRole
          }))
        }
      } catch (error) {
        console.error('Error loading management members:', error)
      }
    },

    async loadTransportationAgencies() {
      try {
        const response = await axios.get('https://localhost:5007/api/agencies/transportation')
        this.transportationAgencies =  Array.isArray(response.data.value.agencies)
      ? response.data.value.agencies
      : []
        console.log(this.transportationAgencies)
      } catch (error) {
        console.error('Error loading agencies:', error)
      }
    },

    openAgencyModal() {
      if (!this.validateForm()) {
        return
      }
      this.showAgencyModal = true
    },

    closeAgencyModal() {
      this.showAgencyModal = false
      this.selectedAgencies = []
    },

    validateForm() {
      if (!this.formData.numberOfPassengers || !this.formData.startDate || 
          !this.formData.endDate || !this.formData.state || 
          !this.formData.city || !this.formData.hall ||
          !this.formData.vehicleType || !this.formData.budget) {
        alert('Please fill in all required fields')
        return false
      }

      // Validate dates
      const today = new Date().toISOString().split('T')[0]
      if (this.formData.startDate < today) {
        alert('Start date cannot be in the past')
        return false
      }

      if (this.formData.endDate < this.formData.startDate) {
        alert('End date must be the same as or after start date')
        return false
      }

      if (this.selectedTeamMembers.length === 0 && this.selectedManagementMembers.length === 0) {
        alert('Please select at least one passenger')
        return false
      }

      return true
    },

    async sendRequest() {
      if (this.selectedAgencies.length === 0) {
        alert('Please select at least one agency')
        return
      }

      this.isLoading = true
      try {
        const requestData = {
          userId: this.userId,
          state: this.formData.state,
          city: this.formData.city,
          hall: this.formData.hall,
          budget: this.formData.budget,
          idMatch: this.matchId,
          type: 'transportation',
          numberOfPassengers: this.formData.numberOfPassengers,
          startDate: this.formData.startDate,
          endDate: this.formData.endDate,
          vehicleType: this.formData.vehicleType,
          teamMemberRequests: this.selectedTeamMembers,
          managementMemberIds: this.selectedManagementMembers,
          agencyIds: this.selectedAgencies
        }
        console.log('Request Data:', requestData); 
        console.log('Debug log', this.userId);
        const response = await axios.post(`https://localhost:5007/api/requests/transportation/${this.userId}`, requestData)
        
        if (response.data.isSuccess) {
          
          this.closeAgencyModal()
          this.resetForm()
          this.$router.push({
            name: 'TransportationRequestsActive',
            params: { matchId: this.matchId } // prosleđuješ matchId
        })
        } else {
          alert(response.data.error || 'Failed to create request')
        }
      } catch (error) {
        console.error('Error creating request:', error)
        alert('Failed to create transportation request')
      } finally {
        this.isLoading = false
      }
    },

    resetForm() {
      this.formData = {
        numberOfPassengers: null,
        startDate: '',
        endDate: '',
        state: '',
        city: '',
        hall: '',
        vehicleType: '',
        budget: null
      }
      this.selectedTeamMembers = []
      this.selectedManagementMembers = []
    },

    navigateToTab(tab) {
      switch(tab) {
        case 'accommodationRequest':
          this.$router.push(`/team-manager/accommodation-request/${this.matchId}`)
          break
        case 'transportActive':
          this.$router.push(`/team-manager/transportation-requests-active/${this.matchId}`)
          break
        case 'accommodationActive':
          this.$router.push(`/team-manager/accommodation-requests-active/${this.matchId}`)
          break
        case 'transportArchive':
          this.$router.push(`/team-manager/transportation-requests-archive/${this.matchId}`)
          break
        case 'accommodationArchive':
          this.$router.push(`/team-manager/accommodation-requests-archive/${this.matchId}`)
          break
        // transportRequest tab is already active
      }
    }
  }
}
</script>

<style scoped>
.transportation-request {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.header-nav {
  margin-bottom: 30px;
}

.nav-tabs {
  display: flex;
  gap: 20px;
  border-bottom: 2px solid #e0e0e0;
}

.tab {
  padding: 10px 20px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.3s;
}

.tab.active {
  border-bottom-color: #1708c0;;
  font-weight: bold;
}

.main-content {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.request-form-card {
  background: white;
  border: 2px solid #000;
  border-radius: 15px;
  padding: 30px;
}

.request-form-card h2 {
  margin: 0 0 30px 0;
  font-size: 24px;
  font-weight: bold;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 20px;
  align-items: end;
}

.budget-group {
  grid-column: 3;
  grid-row: 2;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 8px;
  font-weight: bold;
}

.form-group input,
.form-group select {
  padding: 12px;
  border: 2px solid #ccc;
  border-radius: 5px;
  font-size: 16px;
}

.budget-input {
  position: relative;
}

.budget-input input {
  padding-right: 40px;
}

.currency {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  font-weight: bold;
  font-size: 18px;
}

.passengers-card {
  background: white;
  border: 2px solid #000;
  border-radius: 15px;
  padding: 30px;
}

.passengers-card h2 {
  margin: 0 0 30px 0;
  font-size: 24px;
  font-weight: bold;
}

.passengers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.passenger-item {
  background: #f5f5f5;
  border-radius: 10px;
  padding: 15px;
}

.passenger-checkbox {
  display: flex;
  align-items: center;
  cursor: pointer;
  gap: 15px;
}

.passenger-checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  margin: 0;
}

.passenger-info {
  flex: 1;
}

.passenger-name {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 4px;
}

.passenger-role {
  color: #666;
  font-size: 14px;
}

.send-request-section {
  text-align: right;
}

.send-request-btn {
  background: white;
  border: 2px solid #000;
  border-radius: 10px;
  padding: 15px 30px;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s;
}

.send-request-btn:hover {
  background: #f0f0f0;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 15px;
  max-width: 600px;
  width: 90%;
  max-height: 80vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.modal-header {
  padding: 20px;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
  font-size: 20px;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
}

.agency-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.agency-item {
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 15px;
}

.agency-checkbox {
  display: flex;
  align-items: center;
  cursor: pointer;
  gap: 15px;
}

.agency-checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  margin: 0;
}

.agency-info {
  flex: 1;
}

.agency-name {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 4px;
}

.agency-email {
  color: #666;
  font-size: 14px;
}

.modal-footer {
  padding: 20px;
  border-top: 1px solid #e0e0e0;
  display: flex;
  gap: 15px;
  justify-content: flex-end;
}

.cancel-btn,
.send-btn {
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  border: none;
  transition: all 0.3s;
}

.cancel-btn {
  background: #f5f5f5;
  color: #333;
}

.cancel-btn:hover {
  background: #e0e0e0;
}

.send-btn {
  background: #1708c0;;
  color: white;
}

.send-btn:hover:not(:disabled) {
  background: #3074da;
}

.send-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
}

/* Loading Styles */
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 5px solid #f3f3f3;
  border-top: 5px solid #1708c0;;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
