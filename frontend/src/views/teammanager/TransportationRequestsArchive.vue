<template>
  <div class="transportation-requests-archive">
    <!-- Header Navigation -->
    <div class="header-nav">
      <div class="nav-tabs">
        <span class="tab" @click="navigateToTab('transportRequest')">Transportation Request</span>
        <span class="tab" @click="navigateToTab('accommodationRequest')">Accommodation Request</span>
        <span class="tab" @click="navigateToTab('transportActive')">Transportation Active</span>
        <span class="tab" @click="navigateToTab('accommodationActive')">Accommodation Active</span>
        <span class="tab active" @click="navigateToTab('transportArchive')">Transportation Archive</span>
        <span class="tab" @click="navigateToTab('accommodationArchive')">Accommodation Archive</span>
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
                Request {{ request.idRequest }} - Travel to {{ request.city }}, 
                {{ formatDate(request.startDate) }}
              </h3>
              
              <div class="request-details">
                <div class="detail-left">
                  <p><strong>Number of passengers:</strong> {{ request.numberOfPassengers }}</p>
                  <p><strong>Location:</strong> {{ request.city }}, {{ request.state }}</p>
                  <p><strong>Vehicle type:</strong> {{ getVehicleTypeLabel(request.vehicleType) }}</p>
                </div>
                
                <div class="detail-center">
                  <p><strong>Budget:</strong> {{ request.budget }} EUR</p>
                </div>
              </div>

              <div class="request-dates">
                <span><strong>Start date:</strong> {{ formatDate(request.startDate) }}</span>
                <span><strong>End date:</strong> {{ formatDate(request.endDate) }}</span>
              </div>
            </div>

            <div class="request-actions">
              <div class="status-badge archive">
                Archive
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
        <p>No archived transportation requests found.</p>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-state">
        <div class="loading-spinner"></div>
        <p>Loading archived transportation requests...</p>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'TransportationRequestsArchive',
  data() {
    return {
      archivedRequests: [],
      expandedRequests: [],
      isLoading: false,
      matchId: null
    }
  },
  async mounted() {
    this.matchId = this.$route.params.matchId ? parseInt(this.$route.params.matchId) : null
    await this.loadArchivedRequests()
  },
  methods: {
    async loadArchivedRequests() {
      this.isLoading = true
      try {
        const url = this.matchId 
          ? `https://localhost:5007/api/requests/transportation/${this.matchId}`
          : 'https://localhost:5007/api/requests/transportation/0'
        
        const response = await axios.get(url)
        
        if (response.data.isSuccess) {
          const allRequests = response.data.value.requests || []
          // Filter for archived requests (end date < today)
          const today = new Date().toISOString().split('T')[0]
          this.archivedRequests = allRequests.filter(request => 
            request.endDate < today
          )
        } else {
          console.error('Failed to load requests:', response.data.error)
          alert('Failed to load transportation requests')
        }
      } catch (error) {
        console.error('Error loading archived requests:', error)
        alert('Failed to load transportation requests')
      } finally {
        this.isLoading = false
      }
    },

    loadMockData() {
      // Mock data for demonstration
      this.archivedRequests = [
        {
          idRequest: 1,
          city: 'Atina',
          state: 'Greece',
          budget: 2500,
          numberOfPassengers: 70,
          startDate: '2025-08-04',
          endDate: '2025-08-06',
          vehicleType: 'plane',
          teamMembers: [
            {
              idPlayer: 1,
              playerName: 'Marko',
              playerSurname: 'Nikolic',
              jerseyNumber: 3,
              status: 'active',
              teamName: 'Red Star'
            },
            {
              idPlayer: 2,
              playerName: 'Nikola',
              playerSurname: 'Sretenovic',
              jerseyNumber: 10,
              status: 'active',
              teamName: 'Red Star'
            }
          ],
          managementMembers: [
            {
              memberId: 1,
              memberName: 'Sergej',
              memberSurname: 'Mozic',
              memberRole: 'coach'
            },
            {
              memberId: 2,
              memberName: 'Bojan',
              memberSurname: 'Simic',
              memberRole: 'doctor'
            }
          ]
        },
        {
          idRequest: 2,
          city: 'Novi Sad',
          state: 'Serbia',
          budget: 500,
          numberOfPassengers: 50,
          startDate: '2025-05-29',
          endDate: '2025-05-29',
          vehicleType: 'autobus',
          teamMembers: [
            {
              idPlayer: 3,
              playerName: 'Vukasin',
              playerSurname: 'Peric',
              jerseyNumber: 23,
              status: 'active',
              teamName: 'Red Star'
            }
          ],
          managementMembers: [
            {
              memberId: 1,
              memberName: 'Sergej',
              memberSurname: 'Mozic',
              memberRole: 'coach'
            }
          ]
        }
      ]
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

    getVehicleTypeLabel(vehicleType) {
      const labels = {
        'plane': 'Plane',
        'autobus': 'Bus',
        'train': 'Train',
        'van': 'Van'
      }
      return labels[vehicleType] || vehicleType
    },

    navigateToTab(tab) {
      switch(tab) {
        case 'transportRequest':
            this.$router.push(`/team-manager/transportation-request/${this.matchId}`)
          break
        case 'accommodationRequest':
            this.$router.push(`/team-manager/accommodation-request/${this.matchId}`)
          break
        case 'transportActive':
          this.$router.push(`/team-manager/transportation-requests-active/${this.matchId}`)
          break
        case 'accommodationActive':
          this.$router.push(`/team-manager/accommodation-requests-active/${this.matchId}`)
          break
        case 'accommodationArchive':
          this.$router.push(`/team-manager/accommodation-requests-archive/${this.matchId}`)
          break
        // transportArchive tab is already active
      }
    }
  }
}
</script>

<style scoped>
.transportation-requests-archive {
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
  border-top: 4px solid #1708c0;;
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