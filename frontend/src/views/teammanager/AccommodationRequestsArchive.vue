<template>
  <div class="accommodation-requests-archive">
    <!-- Header Navigation -->
    <div class="header-nav">
      <div class="nav-tabs">
        <span class="tab" @click="navigateToTab('transportActive')">Transportation Active</span>
        <span class="tab" @click="navigateToTab('accommodationActive')">Accommodation Active</span>
        <span class="tab" @click="navigateToTab('transportArchive')">Transportation Archive</span>
        <span class="tab active" @click="navigateToTab('accommodationArchive')">Accommodation Archive</span>
      </div>
    </div>

    <div class="filter-section">
      <div class="filter-controls">
        <div class="filter-group">
          <label for="match-filter">Filter by Match:</label>
          <select id="match-filter" v-model="selectedMatchId" @change="filterByMatch">
            <option value="">All Matches</option>
            <option v-for="match in matches" :key="match.idMatch" :value="match.idMatch">
              {{ match.name }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <div class="requests-container">
        <div 
          v-for="request in archivedRequests" 
          :key="request.idRequest"
          class="request-card"
          :class="{ expanded: expandedRequests.includes(request.idRequest) }"
        >
          <!-- Request Header -->
          <div class="request-header">
            <div class="request-info">
              <h3 class="request-title">
                Request {{ request.idRequest }} - Stay in {{ request.city }}, 
                {{ formatDate(request.checkInDate) }}
              </h3>

              <!-- Match Information -->
             <div class="match-info" v-if="request.matchData">
                <div class="match-details">
                  <h4>Match: {{ request.matchData.name }}</h4>
                  <p><strong>Type: </strong>{{ request.matchData.type }}</p>
                  <p><strong>Location:</strong> {{ request.matchData.state }}, {{ request.matchData.city }}, {{ request.matchData.hall }}</p>
                </div>
              </div>
              
              <div class="request-details">
                <div class="detail-left">
                  <p><strong>Number of guests:</strong> {{ request.numberOfGuests }}</p>
                  <p><strong>Number of rooms:</strong> {{ request.numberOfRooms }}</p>
                  <p><strong>Location:</strong> {{ request.city }}, {{ request.state }}</p>
                </div>
                
                <div class="detail-center">
                  <p><strong>Budget:</strong> {{ request.budget }} EUR</p>
                  <p><strong>Accommodation type:</strong> {{ getAccommodationTypeLabel(request.accommodationType) }}</p>
                </div>
              </div>

              <div class="request-dates">
                <span><strong>Check-in date:</strong> {{ formatDate(request.checkInDate) }}</span>
                <span><strong>Check-out date:</strong> {{ formatDate(request.checkOutDate) }}</span>
              </div>
            </div>

            <div class="request-actions">
              <div class="accommodation-type-badge" :class="getAccommodationTypeClass(request.accommodationType)">
                {{ getAccommodationTypeLabel(request.accommodationType) }}
              </div>
              <div class="status-badge archive">
                Archived
              </div>

              <div class="action-buttons">
                <button 
                  class="action-btn view-details-btn"
                  @click="toggleRequestDetails(request.idRequest)"
                >
                  View details
                </button>
              </div>
            </div>
          </div>

          <!-- Expanded Details -->
          <div 
            v-if="expandedRequests.includes(request.idRequest)" 
            class="request-expanded-details"
          >
            <div class="details-grid">
              <!-- Team Members -->
              <div class="details-section" v-if="request.teamMembers && request.teamMembers.length > 0">
                <h4>Team Members</h4>
                <div class="members-list">
                  <div 
                    v-for="member in request.teamMembers" 
                    :key="`team-${member.idPlayer}`"
                    class="member-card"
                  >
                    <div class="member-info">
                      <div class="member-name">{{ member.playerName }} {{ member.playerSurname }}</div>
                      <div class="member-details">
                        <span class="member-role">Player #{{ member.jerseyNumber }}</span>
                        <span class="member-status" :class="member.status.toLowerCase()">
                          {{ member.status }}
                        </span>
                      </div>
                      <div class="member-team">Team: {{ member.teamName }}</div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Management Members -->
              <div class="details-section" v-if="request.managementMembers && request.managementMembers.length > 0">
                <h4>Management Members</h4>
                <div class="members-list">
                  <div 
                    v-for="manager in request.managementMembers" 
                    :key="`mgmt-${manager.memberId}`"
                    class="member-card"
                  >
                    <div class="member-info">
                      <div class="member-name">{{ manager.memberName }} {{ manager.memberSurname }}</div>
                      <div class="member-details">
                        <span class="member-role">{{ manager.memberRole }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="archivedRequests.length === 0 && !isLoading" class="empty-state">
        <p>No archived accommodation requests found.</p>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-state">
        <div class="loading-spinner"></div>
        <p>Loading archived accommodation requests...</p>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'AccommodationRequestsArchive',
  data() {
    return {
      archivedRequests: [],
      allRequests: [],
      matches: [],
      expandedRequests: [],
      isLoading: false,
      matchId: null,
      selectedMatchId: '',
      existingRequests: {
        accommodation: false,
        transportation: false
      }
    }
  },
  async mounted() {
    this.matchId = this.$route.params.matchId ? parseInt(this.$route.params.matchId) : null
    
    await this.loadMatches()
    await this.loadArchivedRequests()
    await this.checkExistingRequests()
  },
  methods: {
     async loadMatches() {
      try {
        const response = await axios.get('https://localhost:5007/api/matches', {
            headers: {
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
          })
          console.log('Fetched matches:', response.data.value.matches)
          this.matches = Array.isArray(response.data.value.matches)
            ? response.data.value.matches
            : []

      } catch (error) {
        console.error('Error loading matches:', error)
      }
    },

    async loadArchivedRequests() {
      this.isLoading = true
      try {
        let requests = []
        
        if (this.selectedMatchId !== '') {
          const response = await axios.get(`https://localhost:5007/api/requests/by-match/${this.selectedMatchId}`)
          if (response.data.isSuccess) {
            requests = response.data.value.requests || []
          }
        } else {
          // Load all accommodation requests
          const response = await axios.get('https://localhost:5007/api/requests/accommodation/0')
          if (response.data.isSuccess) {
            requests = response.data.value.requests || []
          }
        }

        // Add match data to each request
        await this.addMatchDataToRequests(requests)
        
        // Filter for archived accommodation requests
        const today = new Date().toISOString().split('T')[0]
        this.allRequests = requests
        this.archivedRequests = requests.filter(request => 
          request.type === 'accommodation' && request.checkOutDate < today
        )

      } catch (error) {
        console.error('Error loading archived requests:', error)
        alert('Failed to load accommodation requests')
      } finally {
        this.isLoading = false
      }
    },

    async addMatchDataToRequests(requests) {
      for (let request of requests) {
        if (request.idMatch) {
          try {
            const matchResponse = await axios.get(`https://localhost:5007/api/matches/${request.idMatch}`)
            if (matchResponse.data.isSuccess) {
              request.matchData = matchResponse.data.value
            }
          } catch (error) {
            console.error(`Error loading match ${request.idMatch}:`, error)
          }
        }
      }
    },

    async checkExistingRequests() {
      if (!this.matchId) return

      try {
        // Check for existing accommodation request
        const accommResponse = await axios.get(`https://localhost:5007/api/requests/check-existing/${this.matchId}/accommodation`)
        if (accommResponse.data.isSuccess) {
          this.existingRequests.accommodation = accommResponse.data.value.requestExists
        }

        // Check for existing transportation request
        const transResponse = await axios.get(`https://localhost:5007/api/requests/check-existing/${this.matchId}/transportation`)
        if (transResponse.data.isSuccess) {
          this.existingRequests.transportation = transResponse.data.value.requestExists
        }
      } catch (error) {
        console.error('Error checking existing requests:', error)
      }
    },

    async filterByMatch() {
      if (this.selectedMatchId) {
        this.matchId = parseInt(this.selectedMatchId)
        await this.checkExistingRequests()
      } else {
        this.matchId = null
        this.existingRequests = { accommodation: false, transportation: false }
      }
      await this.loadArchivedRequests()
    },

    hasExistingRequest(type) {
      return this.existingRequests[type]
    },

    toggleRequestDetails(requestId) {
      const index = this.expandedRequests.indexOf(requestId)
      if (index > -1) {
        this.expandedRequests.splice(index, 1)
      } else {
        this.expandedRequests.push(requestId)
      }
    },

    formatDate(dateString) {
      const date = new Date(dateString)
      return date.toLocaleDateString('en-GB', {
        day: '2-digit',
        month: 'long',
        year: 'numeric'
      })
    },

    formatDateTime(dateTimeString) {
      const date = new Date(dateTimeString)
      return date.toLocaleDateString('en-GB', {
        day: '2-digit',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      })
    },

    getAccommodationTypeLabel(accommodationType) {
      const labels = {
        'hotel': 'Hotel',
        'house': 'House',
        'villa': 'Villa',
      }
      return labels[accommodationType] || accommodationType
    },
    getAccommodationTypeClass(accommodationType) {
      return `accommodation-${accommodationType}`
    },
    navigateToTab(tab) {
      switch(tab) {
        case 'transportActive':
          this.$router.push(`/team-manager/transportation-requests-active/${this.matchId}`)
          break
        case 'accommodationActive':
          this.$router.push(`/team-manager/accommodation-requests-active/${this.matchId}`)
          break
        case 'transportArchive':
          this.$router.push(`/team-manager/transportation-requests-archive/${this.matchId}`)
          break
        // accommodationArchive tab is already active
      }
    }
  }
}
</script>

<style scoped>
.accommodation-requests-archive {
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
  padding: 10px 50px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.3s;
}

.tab.active {
  border-bottom-color: #1708c0;
  font-weight: bold;
}

.tab.disabled {
  opacity: 0.5;
  cursor: not-allowed;
  background-color: #f5f5f5;
}

.filter-section {
  margin-bottom: 25px;
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 10px;
  padding: 20px;
}

.filter-controls {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 10px;
}

.filter-group label {
  font-weight: bold;
  color: #333;
}

.filter-group select {
  padding: 8px 12px;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
  min-width: 250px;
  cursor: pointer;
}

.filter-group select:focus {
  outline: none;
  border-color: #1708c0;
}

.refresh-btn {
  padding: 8px 16px;
  background: #1708c0;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 5px;
  transition: background 0.3s;
}

.refresh-btn:hover {
  background: #0f0690;
}

.match-info {
  background: #f8f9fa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 15px;
  margin-bottom: 15px;
}

.match-details h4 {
  margin: 0 0 10px 0;
  color: #1708c0;
  font-size: 16px;
}

.match-details p {
  margin: 5px 0;
  font-size: 14px;
  color: #666;
}

.main-content {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.requests-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.request-card {
  background: white;
  border: 2px solid #000;
  border-radius: 15px;
  padding: 25px;
  transition: all 0.3s ease;
}

.request-card.expanded {
  border-color: #ff9800;
}

.request-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 20px;
}

.request-info {
  flex: 1;
}

.request-title {
  margin: 0 0 15px 0;
  font-size: 20px;
  font-weight: bold;
}

.request-details {
  display: flex;
  gap: 40px;
  margin-bottom: 15px;
}

.detail-left,
.detail-center {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.request-details p {
  margin: 0;
  font-size: 16px;
}

.request-dates {
  display: flex;
  gap: 40px;
  font-size: 16px;
}

.request-actions {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 15px;
}
.accommodation-type-badge {
  padding: 8px 16px;
  border-radius: 20px;
  font-weight: bold;
  font-size: 14px;
  text-align: center;
  min-width: 80px;
}

.accommodation-hotel {
  background: #e3f2fd;
  color: #1976d2;
}

.accommodation-house {
  background: #fff3e0;
  color: #f57c00;
}

.accommodation-villa {
  background: #e8f5e8;
  color: #388e3c;
}

.accommodation-guesthouse {
  background: #fce4ec;
  color: #c2185b;
}
.status-badge {
  padding: 8px 16px;
  border-radius: 20px;
  font-weight: bold;
  font-size: 14px;
  text-align: center;
  min-width: 80px;
}

.status-badge.archive {
  background: #ffebee;
  color: #d32f2f;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.action-btn {
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  border: 2px solid #000;
  background: white;
  transition: all 0.3s;
  min-width: 120px;
}

.action-btn:hover {
  background: #f0f0f0;
}

.view-details-btn {
  border-color: #000;
}

/* Expanded Details */
.request-expanded-details {
  margin-top: 25px;
  padding-top: 25px;
  border-top: 2px solid #e0e0e0;
}

.details-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 30px;
}

.details-section h4 {
  margin: 0 0 15px 0;
  font-size: 18px;
  font-weight: bold;
  color: #333;
}

.members-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.member-card {
  background: #f8f9fa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 15px;
}

.member-name {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 8px;
}

.member-details {
  display: flex;
  gap: 15px;
  margin-bottom: 5px;
}

.member-role {
  color: #666;
  font-size: 14px;
}

.member-status {
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: bold;
  text-transform: uppercase;
}

.member-status.active {
  background: #e8f5e8;
  color: #06b226;
}

.member-status.injured {
  background: #ffebee;
  color: #f44336;
}

.member-status.suspended {
  background: #fff3e0;
  color: #ff9800;
}

.member-team {
  color: #666;
  font-size: 14px;
}

/* Empty and Loading States */
.empty-state,
.loading-state {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #1708c0;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 20px auto;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Responsive Design */
@media (max-width: 768px) {
  .request-header {
    flex-direction: column;
    align-items: stretch;
  }

  .request-details {
    flex-direction: column;
    gap: 10px;
  }

  .request-dates {
    flex-direction: column;
    gap: 5px;
  }

  .request-actions {
    align-items: stretch;
  }

  .details-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
}
</style>
