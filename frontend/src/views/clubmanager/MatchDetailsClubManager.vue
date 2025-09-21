<template>
    <div class="match-details-page">
        <header class="details-header">
            <div class="header-left">
                <button class="btn btn-secondary" @click="goBack">&larr; Back to calendar</button>
            </div>
            <div class="header-center">
                <nav class="nav-menu">
                    <router-link to="/club-manager/matches" class="nav-link active">Matches</router-link>
                    <router-link to="/club-manager/travelinfo" class="nav-link">Travel Info</router-link>
                    <div class="nav-dropdown">
                        <span class="nav-link dropdown-toggle">Requests</span>
                        <div class="dropdown-menu">
							<router-link to="/team-manager/transportation-requests-active" class="dropdown-item">Transportation - Active</router-link>
							<router-link to="/team-manager/transportation-requests-archive" class="dropdown-item">Transportation - Archive</router-link>
							<router-link to="/team-manager/accommodation-requests-active" class="dropdown-item">Accommodation - Active</router-link>
							<router-link to="/team-manager/accommodation-requests-archive" class="dropdown-item">Accommodation - Archive</router-link>
						</div>
                    </div>
                </nav>
            </div>
            <div class="header-right">
                <button 
                    v-if="match && (match.transportationRequired || match.accommodationRequired) && matchStatus !== 'archived'" 
                    class="btn"
                    :class="travelCreated ? 'btn-success' : 'btn-primary'"
                    :disabled="travelCreated"
                    @click="goToCreateTravel">
                    {{ travelCreated ? '✓ Travel Created' : 'Create Travel' }}
                </button>
            </div>
        </header>

        <div class="details-container">
            <div v-if="match" class="match-info-box">
				<div class="match-title">
					{{ match.name }} <span class="type-pill">{{ match.type === 'home' ? 'Home' : 'Away' }}</span>
				</div>
				<div class="match-info-grid">
					<div>
						<div class="info-label">Date</div>
						<div class="info-value">{{ formatDate(match.scheduledAt) }}</div>
					</div>
					<div>
						<div class="info-label">Time</div>
						<div class="info-value">{{ formatTime(match.scheduledAt) }}</div>
					</div>
					<div>
						<div class="info-label">Season</div>
						<div class="info-value">{{ season?.name || '-' }}</div>
					</div>
					<div>
						<div class="info-label">Competition</div>
						<div class="info-value">{{ competition?.name || '-' }}</div>
					</div>
					<div>
						<div class="info-label">Team</div>
						<div class="info-value">{{ team?.name || '-' }}</div>
					</div>
					<div>
						<div class="info-label">Location</div>
						<div class="info-value">{{ match.city }}, {{ match.state }}</div>
					</div>
					<div>
						<div class="info-label">Hall</div>
						<div class="info-value">{{ match.hall }}</div>
					</div>
				</div>
				<div class="status-row">
					<div v-if="matchStatus === 'archived'" class="next-match-badge" style="background:#6b7280">Archived</div>
					<div v-else-if="matchStatus === 'next'" class="next-match-badge">Next Match</div>
					<div v-else-if="matchStatus === 'future'" class="next-match-badge" style="background:#3b82f6">Future Match</div>
				</div>
            </div>
            <div v-else class="match-info-box">
                <div>Loading match details...</div>
            </div>

            <div class="details-sections" v-if="match">
                <div class="details-section">
                    <div class="section-header">
                        Transportation
                        <span class="section-badge" :class="{ 'not-required': !match.transportationRequired, 'required': match.transportationRequired }">
                            {{ match.transportationRequired ? (transportationOffer ? 'Selected offer' : 'Required') : 'Is not required' }}
                        </span>
                    </div>
                    <div class="section-body">
                        <div v-if="!match.transportationRequired" class="info-box">
                            <span class="icon">&#9432;</span>
                            Transportation is not required for this trip, please go to the transportation reservation page
                        </div>
                        <div v-else-if="!transportationOffer" class="info-box warning">
                            <span class="icon">&#9888;</span>
                            Transportation is required for this trip, please go to the transportation reservation page
                        </div>
                        <div v-else class="offer-card-display" :class="{ 'travel-created': travelCreated }">
                            <div class="offer-header">
                                <h3>{{ transportationOffer.agencyName || 'N/A' }}</h3>
                                <span class="status-badge chosen">{{ travelCreated ? 'Final Selection' : 'Chosen' }}</span>
                            </div>
                            
                            <div class="offer-price">
                                {{ transportationOffer.price || 0 }} EUR
                            </div>
                            
                            <div class="offer-details">
                                <div class="detail-row">
                                    <span class="detail-label">Company:</span>
                                    <span>{{ transportationOffer.companyName || 'N/A' }}</span>
                                </div>
                                <div class="detail-row">
                                    <span class="detail-label">Vehicle:</span>
                                    <span>{{ transportationOffer.vehicleType || 'Bus' }}</span>
                                </div>
                                <div class="detail-row">
                                    <span class="detail-label">Capacity:</span>
                                    <span>{{ transportationOffer.capacity || 'N/A' }}</span>
                                </div>
                            </div>
                            
                            <div class="offer-benefits">
                                <h4>Additional benefits:</h4>
                                <div class="benefits-tags">
                                    <span class="benefit-tag">
                                        {{
                                            [
                                                transportationOffer.airConditioning ? 'Air Conditioning' : null,
                                                transportationOffer.tv ? 'TV' : null,
                                                transportationOffer.wifiTransport ? 'WIFI' : null,
                                                transportationOffer.restroom ? 'WC' : null
                                            ].filter(Boolean).join(', ') || 'N/A'
                                        }}
                                    </span>
                                </div>
                            </div>
                            
                            <div class="offer-actions" v-if="!travelCreated">
                                <button @click="goToTransportOffers" class="btn-outline">
                                    <span class="icon">✏️</span> Change Offer
                                </button>
                            </div>
                        </div>
                        <div v-if="match.transportationRequired && !transportationOffer && !travelCreated" class="offer-actions">
                            <button @click="goToTransportOffers" class="btn-outline">Choose an offer</button>
                        </div>
                    </div>
                </div>

                <div class="details-section">
                    <div class="section-header">
                        Accommodation
                        <span class="section-badge" :class="{ 'not-required': !match.accommodationRequired, 'required': match.accommodationRequired }">
                            {{ match.accommodationRequired ? (accommodationOffer ? 'Selected offer' : 'Required') : 'Is not required' }}
                        </span>
                    </div>
                    <div class="section-body">
                        <div v-if="!match.accommodationRequired" class="info-box">
                            <span class="icon">&#9432;</span>
                            Accommodation is required for this trip, please go to the transport booking page
                        </div>
                        <div v-else-if="!accommodationOffer" class="info-box warning">
                            <span class="icon">&#9888;</span>
                            Accommodation is required for this trip, please go to the transport booking page
                        </div>
                        <div v-else class="offer-card-display" :class="{ 'travel-created': travelCreated }">
                            <div class="offer-header">
                                <h3>{{ accommodationOffer.agencyName || 'N/A' }}</h3>
                                <span class="status-badge chosen">{{ travelCreated ? 'Final Selection' : 'Chosen' }}</span>
                            </div>
                            
                            <div class="offer-price">
                                {{ accommodationOffer.price || 0 }} EUR
                            </div>
                            
                            <div class="offer-details">
                                <div class="detail-row">
                                    <span class="detail-label">Name:</span>
                                    <span>{{ accommodationOffer.name || 'N/A' }}</span>
                                </div>
                                <div class="detail-row">
                                    <span class="detail-label">Type:</span>
                                    <span>{{ accommodationOffer.accommodationType || 'Hotel' }}</span>
                                </div>
                                <div class="detail-row">
                                    <span class="detail-label">Capacity:</span>
                                    <span>{{ accommodationOffer.capacity || 'N/A' }}</span>
                                </div>
                                <div class="detail-row">
                                    <span class="detail-label">Room types:</span>
                                    <span>{{
                                        [
                                            accommodationOffer.doubleRoom ? '1/2' : null,
                                            accommodationOffer.tripleRoom ? '1/3' : null,
                                            accommodationOffer.quadrupleRoom ? '1/4' : null
                                        ].filter(Boolean).join(', ') || 'N/A'
                                    }}</span>
                                </div>
                            </div>
                            
                            <div class="offer-benefits">
                                <h4>Additional benefits:</h4>
                                <div class="benefits-tags">
                                    <span class="benefit-tag">
                                        {{
                                            [
                                                accommodationOffer.breakfast ? 'Breakfast' : null,
                                                accommodationOffer.fitnessCenter ? 'Fitness Center' : null,
                                                accommodationOffer.pool ? 'Pool' : null,
                                                accommodationOffer.wifi ? 'WIFI' : null,
                                                accommodationOffer.spa ? 'SPA' : null
                                            ].filter(Boolean).join(', ') || 'N/A'
                                        }}
                                    </span>
                                </div>
                            </div>
                            
                            <div class="offer-actions" v-if="!travelCreated">
                                <button @click="goToAccommodationOffers" class="btn-outline">
                                    <span class="icon">✏️</span> Change Offer
                                </button>
                            </div>
                        </div>
                        <div v-if="match.accommodationRequired && !accommodationOffer && !travelCreated" class="offer-actions">
                            <button @click="goToAccommodationOffers" class="btn-outline">Choose an offer</button>
                        </div>
                    </div>
                </div>
            </div>
		</div>
    </div>

    <!-- Warning Modal -->
    <div v-if="showWarningModal" class="modal-overlay" @click="closeWarningModal">
        <div class="modal-content warning-modal" @click.stop>
            <div class="modal-header">
                <h3>⚠️ Warning</h3>
                <button @click="closeWarningModal" class="close-btn">&times;</button>
            </div>
            <div class="modal-body">
                <p class="warning-text">{{ warningMessage }}</p>
            </div>
            <div class="modal-footer">
                <button @click="closeWarningModal" class="btn btn-primary">OK</button>
            </div>
        </div>
    </div>

    <!-- Create Travel Modal -->
    <div v-if="showCreateTravelModal" class="modal-overlay" @click="closeCreateTravelModal">
        <div class="modal-content" @click.stop>
            <div class="modal-header">
                <h3>Create Travel</h3>
                <button @click="closeCreateTravelModal" class="close-btn">&times;</button>
            </div>
            <div class="modal-body">
                <div v-if="match" class="travel-summary">
                    <h4>Match Details</h4>
                    <p><strong>{{ match.name }}</strong></p>
                    <p>{{ formatDate(match.scheduledAt) }} at {{ formatTime(match.scheduledAt) }}</p>
                    <p>{{ match.city }}, {{ match.state }}</p>
                </div>

                <div class="offer-summary">
                    <h4>Selected Offers</h4>
                    <div v-if="transportationOffer" class="selected-offer">
                        <p><strong>Transportation:</strong> {{ transportationOffer.vehicleType || transportationOffer.companyName || 'Transportation Service' }} - {{ transportationOffer.price }}€</p>
                        <p><strong>Agency:</strong> {{ transportationOffer.agencyName }}</p>
                        <div v-if="transportationOffer.airConditioning || transportationOffer.wifi || transportationOffer.restroom" class="offer-features">
                            <small>Features: 
                                <span v-if="transportationOffer.airConditioning">Air Conditioning, </span>
                                <span v-if="transportationOffer.wifiTransport">WiFi, </span>
                                <span v-if="transportationOffer.restroom">Restroom, </span>
                                <span v-if="transportationOffer.tv">TV</span>
                            </small>
                        </div>
                    </div>
                    <div v-if="accommodationOffer" class="selected-offer">
                        <p><strong>Accommodation:</strong> {{ accommodationOffer.accommodationType || accommodationOffer.name || 'Accommodation' }} - {{ accommodationOffer.price }}€</p>
                        <p><strong>Agency:</strong> {{ accommodationOffer.agencyName }}</p>
                        <p><strong>Capacity:</strong> {{ accommodationOffer.capacity }} people</p>
                        <div v-if="accommodationOffer.breakfast || accommodationOffer.wifi || accommodationOffer.pool || accommodationOffer.spa" class="offer-features">
                            <small>Features: 
                                <span v-if="accommodationOffer.breakfast">Breakfast, </span>
                                <span v-if="accommodationOffer.wifi">WiFi, </span>
                                <span v-if="accommodationOffer.pool">Pool, </span>
                                <span v-if="accommodationOffer.spa">Spa, </span>
                                <span v-if="accommodationOffer.fitnessCenter">Fitness Center</span>
                            </small>
                        </div>
                    </div>
                    <div v-if="!transportationOffer" class="missing-offer">
                        <p><strong>⚠️ Transportation offer not selected</strong></p>
                        <p>Please select a transportation offer before creating travel.</p>
                    </div>
                    <div v-if="match && match.accommodationRequired && !accommodationOffer" class="missing-offer">
                        <p><strong>⚠️ Accommodation offer not selected</strong></p>
                        <p>Accommodation is required for this match. Please select an offer.</p>
                    </div>
                </div>

                <div class="form-group">
                    <label for="travelNotes">Notes (optional)</label>
                    <textarea 
                        id="travelNotes"
                        v-model="travelNotes" 
                        class="form-control"
                        placeholder="Add any additional notes for this travel..."
                        rows="4">
                    </textarea>
                </div>
            </div>
            <div class="modal-footer">
                <button @click="closeCreateTravelModal" class="btn btn-secondary">Cancel</button>
                <button @click="createTravel" class="btn btn-primary" :disabled="loading">
                    {{ loading ? 'Creating...' : 'Create Travel' }}
                </button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { AuthService } from '../../services/auth_service.js'
import axios from 'axios'

const route = useRoute()
const router = useRouter()

const match = ref(null)
const season = ref(null)
const team = ref(null)
const competition = ref(null)
const transportationOffer = ref(null)
const accommodationOffer = ref(null)

// Create Travel Modal
const showCreateTravelModal = ref(false)
const showWarningModal = ref(false)
const warningMessage = ref('')
const travelNotes = ref('')
const loading = ref(false)
const travelCreated = ref(false) // Track if travel has been created

const formatDate = (dt) => {
	if (!dt) return ''
	const d = new Date(dt)
	return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' })
}
const formatTime = (dt) => {
	if (!dt) return ''
	const d = new Date(dt)
	return d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false })
}

const allMatches = ref([])

const matchStatus = computed(() => {
	if (!match.value) return ''
	const now = new Date()
	const matchDate = new Date(match.value.scheduledAt)
	if (matchDate < now) return 'archived'
	// Find all future matches
	const futureMatches = allMatches.value.filter(m => new Date(m.scheduledAt) > now)
	// Sort by date
	futureMatches.sort((a, b) => new Date(a.scheduledAt) - new Date(b.scheduledAt))
	if (futureMatches.length && futureMatches[0].idMatch === match.value.idMatch) return 'next'
	return 'future'
})

const goBack = () => {
	router.push('/club-manager/matches')
}

const goToCreateTravel = () => {
  // Check if travel has already been created
  if (travelCreated.value) {
    return // Travel already created, don't allow recreation
  }
  
  // Check if required offers are selected
  if (!transportationOffer.value) {
    warningMessage.value = 'Transportation offer must be selected before creating travel'
    showWarningModal.value = true
    return
  }
  
  if (match.value.accommodationRequired && !accommodationOffer.value) {
    warningMessage.value = 'Accommodation offer must be selected before creating travel (required for this match)'
    showWarningModal.value = true
    return
  }
  
  showCreateTravelModal.value = true
}

// Create Travel Modal methods
const closeCreateTravelModal = () => {
  showCreateTravelModal.value = false
  travelNotes.value = ''
}

const closeWarningModal = () => {
  showWarningModal.value = false
  warningMessage.value = ''
}

const createTravel = async () => {
  if (loading.value) return
  
  try {
    loading.value = true
    
    // Validate that required offers are selected
    if (!transportationOffer.value) {
      alert('Transportation offer is required')
      return
    }
    
    if (match.value.accommodationRequired && !accommodationOffer.value) {
      alert('Accommodation offer is required for this match')
      return
    }
    
    const travelData = {
      Notes: travelNotes.value || '',
      MatchIdMatch: match.value.idMatch,
      IdTransportationOffer: transportationOffer.value.idOffer,
      IdTransportationAgency: transportationOffer.value.idAgency,
      IdTransportationRequest: transportationOffer.value.idRequest
    }
    
    // Add accommodation data if present
    if (accommodationOffer.value) {
      travelData.IdAccommodationOffer = accommodationOffer.value.idOffer
      travelData.IdAccommodationAgency = accommodationOffer.value.idAgency
      travelData.IdAccommodationRequest = accommodationOffer.value.idRequest
    }
	else{
		// If accommodation is not required, set accommodation fields to null
		travelData.IdAccommodationOffer = null
		travelData.IdAccommodationAgency = null
		travelData.IdAccommodationRequest = null
	}
    
    console.log('Creating travel with data:', travelData)
    
    const response = await axios.post('https://localhost:5007/api/Trip', travelData)
    
    console.log('Travel creation response:', response)
    
    if (response.status === 200 || response.status === 201) {
      travelCreated.value = true // Mark travel as created
      alert('Travel created successfully!')
      closeCreateTravelModal()
    }
    
  } catch (error) {
    console.error('Error creating travel:', error)
    let errorMessage = 'Failed to create travel. Please try again.'
    
    if (error.response?.data?.error) {
      errorMessage = `Failed to create travel: ${error.response.data.error}`
    } else if (error.response?.data?.message) {
      errorMessage = `Failed to create travel: ${error.response.data.message}`
    } else if (error.response?.data) {
      errorMessage = `Failed to create travel: ${JSON.stringify(error.response.data)}`
    } else if (error.message) {
      errorMessage = `Failed to create travel: ${error.message}`
    }
    
    alert(errorMessage)
  } finally {
    loading.value = false
  }
}

const goToTransportOffers = () => {
  router.push(`/offers/transportation/${match.value.idMatch}`)
}

const goToAccommodationOffers = () => {
  router.push(`/offers/accommodation/${match.value.idMatch}`)
}

const fetchChosenOffers = async () => {
  if (!match.value) return

  try {
    // Fetch chosen transportation offer
    const transportResponse = await axios.get(`https://localhost:5007/api/offers/chosen/transportation/${match.value.idMatch}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
	console.log('Transport response:', transportResponse.data)
    if (transportResponse.data.isSuccess) {
		if(transportResponse.data.value.hasChosenOffer == false){
			console.log('No chosen transportation offer')	
		}
		else{
      	transportationOffer.value = transportResponse.data.value.chosenOffer
		}
    }

    // Fetch chosen accommodation offer
    const accommodationResponse = await axios.get(`https://localhost:5007/api/offers/chosen/accommodation/${match.value.idMatch}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
	console.log('Accommodation response:', accommodationResponse.data)
    if (accommodationResponse.data.isSuccess) {
		if(accommodationResponse.data.value.hasChosenOffer == false){
			console.log('No chosen accommodation offer')	
		}
		else{
      	accommodationOffer.value = accommodationResponse.data.value.chosenOffer
		}
    }
  } catch (error) {
    console.error('Error fetching chosen offers:', error)
  }
}

const fetchMatchDetails = async () => {
	const id = route.params.id
	if (!id) return
	try {
		// Fetch all matches for status logic
		const allRes = await axios.get('https://localhost:5007/api/matches', {
			headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
		allMatches.value = Array.isArray(allRes.data.value.matches) ? allRes.data.value.matches : []

		// Fetch this match
		const response = await axios.get(`https://localhost:5007/api/matches/${id}`, {
			headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
		match.value = response.data.value
        console.log('Fetched match:', match.value)
		// Fetch related entities by ID (dummy endpoints for now)
		if (match.value.idSeason) {
            const response1 = await axios.get(`https://localhost:5007/api/seasons/${match.value.idSeason}`, {
			headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
            season.value = response1.data.value
			console.log('Fetched season:', season.value)
		}
		if (match.value.idTeam) {
			const response2 = await axios.get(`https://localhost:5007/api/teams/${match.value.idTeam}`, {
			headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
            team.value = response2.data.value
            console.log('Fetched team:', team.value)
		}
		if (match.value.idCompetition) {
			const response3 = await axios.get(`https://localhost:5007/api/competitions/${match.value.idCompetition}`, {
            headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
            competition.value = response3.data.value
			console.log('Fetched competition:', competition.value)
		}
		
		// Fetch chosen offers
		await fetchChosenOffers()
	} catch (error) {
		console.error('Error fetching match details:', error)
	}
}

const fetchSeason = async (seasonId) => {
	try {
		const response = await axios.get(`https://localhost:5007/api/seasons/${seasonId}`, {
			headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
		return response.data.value
	} catch (error) {
		console.error('Error fetching season:', error)
		return null
	}
}

const fetchTeam = async (teamId) => {
	try {
		const response = await axios.get(`https://localhost:5007/api/teams/${teamId}`, {
			headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
		return response.data.value
	} catch (error) {
		console.error('Error fetching team:', error)
		return null
	}
}

const fetchOpponent = async (opponentId) => {
	try {
		const response = await axios.get(`https://localhost:5007/api/teams/${opponentId}`, {
			headers: {
				'Authorization': `Bearer ${localStorage.getItem('token')}`
			}
		})
		return response.data.value
	} catch (error) {
		console.error('Error fetching opponent:', error)
		return null
	}
}

onMounted(() => {
	fetchMatchDetails()
})
</script>

<style scoped>
/* Header alignment */
.details-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	width: 900px;
	box-sizing: border-box;
	margin: 2rem auto;
}
.header-left {
	flex: 1;
	display: flex;
	align-items: center;
	justify-content: flex-start;
}
.header-center {
	flex: 2;
	display: flex;
	align-items: center;
	justify-content: center;
}
.header-right {
	flex: 1;
	display: flex;
	align-items: center;
	justify-content: flex-end;
}
.nav-menu {
	display: flex;
	gap: 2rem;
	align-items: center;
}
.nav-link {
	text-decoration: none;
	color: #6b7280;
	font-weight: 500;
	padding: 0.5rem 1rem;
	border-radius: 0.375rem;
	transition: all 0.2s;
	cursor: pointer;
}
.nav-link.active {
	color: var(--color-primary);
	background-color: #f3f4f6;
}
.nav-link:hover {
	color: var(--color-primary);
	background-color: #f9fafb;
}

/* Dropdown styles */
.nav-dropdown {
	position: relative;
}

.dropdown-toggle {
	display: flex;
	align-items: center;
	gap: 0.5rem;
}

.dropdown-toggle::after {
	content: '▼';
	font-size: 0.75rem;
	transition: transform 0.2s;
}

.nav-dropdown:hover .dropdown-toggle::after {
	transform: rotate(180deg);
}

.dropdown-menu {
	position: absolute;
	top: 100%;
	left: 0;
	right: 0;
	background: white;
	border: 1px solid #e5e7eb;
	border-radius: 0.5rem;
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
	z-index: 100;
	opacity: 0;
	visibility: hidden;
	transform: translateY(-10px);
	transition: all 0.2s;
	min-width: 200px;
}

.nav-dropdown:hover .dropdown-menu {
	opacity: 1;
	visibility: visible;
	transform: translateY(0);
}

.dropdown-item {
	display: block;
	padding: 0.75rem 1rem;
	color: #374151;
	text-decoration: none;
	font-size: 0.875rem;
	transition: background 0.2s;
	border-bottom: 1px solid #f3f4f6;
}

.dropdown-item:last-child {
	border-bottom: none;
}

.dropdown-item:hover {
	background: #f9fafb;
	color: var(--color-primary);
}
.details-container {
	max-width: 900px;
	margin: 0 auto;
}
.match-info-box {
	background: #fff;
	border-radius: 0.75rem;
	box-shadow: 0 1px 3px rgba(0,0,0,0.07);
	padding: 2rem;
	margin-bottom: 2rem;
	position: relative;
}
.match-title {
	font-size: 1.5rem;
	font-weight: 600;
	margin-bottom: 1.5rem;
}
.match-info-grid {
	display: grid;
	grid-template-columns: repeat(2, 1fr);
	gap: 1.5rem;
	margin-bottom: 1.5rem;
}
.type-pill {
	display: inline-block;
	background: #e0e7ff;
	color: #3730a3;
	font-size: 0.95rem;
	font-weight: 500;
	border-radius: 1rem;
	padding: 0.2rem 1rem;
	margin-left: 1rem;
}
.status-row {
	margin-top: 1rem;
	display: flex;
	gap: 1rem;
	align-items: center;
}
.info-label {
	color: #6b7280;
	font-size: 0.95rem;
	margin-bottom: 0.25rem;
}
.info-value {
	font-size: 1.1rem;
	font-weight: 500;
}
.next-match-badge {
	position: absolute;
	top: 2rem;
	right: 2rem;
	background: #059669;
	color: #fff;
	padding: 0.25rem 0.75rem; 
	border-radius: 2rem; 
	font-weight: 500;
	font-size: 0.875rem; 
}

.details-sections {
	display: flex;
	gap: 2rem;
}
.details-section {
	flex: 1;
	background: #fff;
	border-radius: 0.75rem;
	box-shadow: 0 1px 3px rgba(0,0,0,0.07);
	padding: 1.5rem;
	min-height: 220px;
	position: relative;
}
.section-header {
	font-size: 1.1rem;
	font-weight: 600;
	margin-bottom: 1rem;
	display: flex;
	align-items: center;
	justify-content: space-between;
}
.section-badge {
	font-size: 0.9rem;
	padding: 0.25rem 1rem;
	border-radius: 1rem;
	margin-left: 1rem;
	background: #fca5a5;
	color: #991b1b;
}
.section-badge.required {
	background: #fbbf24;
	color: #92400e;
}
.section-badge.not-required {
	background: #fca5a5;
	color: #991b1b;
}
.section-badge.selected {
	background: #059669;
	color: #fff;
}
.section-body {
	margin-top: 1rem;
}
.info-box {
	background: #f3f4f6;
	border-radius: 0.5rem;
	padding: 1rem;
	font-size: 1rem;
	color: #374151;
	display: flex;
	align-items: flex-start;
	gap: 0.75rem;
	margin-bottom: 0.5rem;
}
.info-box.warning {
	background: #fef3c7;
	color: #92400e;
}
.info-box.selected-offer {
	background: #d1fae5;
	color: #065f46;
}
.icon {
	font-size: 1.2rem;
	margin-right: 0.5rem;
}
.offer-actions {
  margin-top: 1rem;
  display: flex;
  gap: 1rem;
}
.btn-outline {
  background: white;
  color: #2563eb;
  border: 1px solid #2563eb;
  border-radius: 0.375rem;
  padding: 0.5rem 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}
.btn-outline:hover {
  background: #2563eb;
  color: white;
}

.offer-details {
  flex: 1;
}

.offer-details h4 {
  margin: 0 0 0.5rem 0;
  color: #065f46;
  font-weight: 600;
}

.offer-details p {
  margin: 0.25rem 0;
  font-size: 0.9rem;
}

.selected-offer {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
}

.selected-offer .icon {
  margin-top: 0.25rem;
}

/* Offer card display styles */
.offer-card-display {
	background: white;
	border: 2px solid #10b981;
	border-radius: 1rem;
	padding: 1.5rem;
	margin: 1rem 0;
	box-shadow: 0 4px 12px rgba(16, 185, 129, 0.15);
}

.offer-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	margin-bottom: 1rem;
}

.offer-header h3 {
	margin: 0;
	font-size: 1.25rem;
	font-weight: bold;
	color: #065f46;
}

.status-badge {
	padding: 0.25rem 0.75rem;
	border-radius: 1rem;
	font-size: 0.875rem;
	font-weight: 500;
	background: #e5e7eb;
	color: #374151;
}

.status-badge.chosen {
	background: #10b981;
	color: white;
}

.offer-price {
	font-size: 2rem;
	font-weight: bold;
	text-align: center;
	margin: 1rem 0;
	padding: 1rem;
	background: #f0fdf4;
	border-radius: 0.5rem;
	color: #065f46;
}

.detail-row {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 0.5rem 0;
	border-bottom: 1px solid #f3f4f6;
}

.detail-row:last-child {
	border-bottom: none;
}

.detail-label {
	font-weight: 600;
	color: #6b7280;
}

.offer-benefits {
	margin-top: 1rem;
	padding-top: 1rem;
	border-top: 1px solid #f3f4f6;
}

.offer-benefits h4 {
	margin: 0 0 0.5rem 0;
	font-size: 1rem;
	font-weight: 600;
	color: #374151;
}

.benefits-tags {
	display: flex;
	flex-wrap: wrap;
	gap: 0.5rem;
}

.benefit-tag {
	background: #e0f2fe;
	color: #0369a1;
	padding: 0.25rem 0.75rem;
	border-radius: 1rem;
	font-size: 0.875rem;
	font-weight: 500;
}

/* Create Travel Modal */
.modal-overlay {
	position: fixed;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	background: rgba(0, 0, 0, 0.5);
	display: flex;
	justify-content: center;
	align-items: center;
	z-index: 1000;
}

.modal-content {
	background: white;
	border-radius: 12px;
	width: 90%;
	max-width: 600px;
	max-height: 90vh;
	overflow-y: auto;
	box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.modal-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 1.5rem;
	border-bottom: 1px solid #e5e7eb;
}

.modal-header h3 {
	margin: 0;
	font-size: 1.5rem;
	font-weight: 600;
	color: #111827;
}

.close-btn {
	background: none;
	border: none;
	font-size: 1.5rem;
	cursor: pointer;
	color: #6b7280;
	padding: 0;
	line-height: 1;
}

.close-btn:hover {
	color: #374151;
}

.modal-body {
	padding: 1.5rem;
}

.travel-summary {
	margin-bottom: 1.5rem;
	padding: 1rem;
	background: #f9fafb;
	border-radius: 8px;
	border: 1px solid #e5e7eb;
}

.travel-summary h4 {
	margin: 0 0 0.5rem 0;
	color: #374151;
	font-size: 1.1rem;
}

.travel-summary p {
	margin: 0.25rem 0;
	color: #6b7280;
}

.offer-summary {
	margin-bottom: 1.5rem;
}

.offer-summary h4 {
	margin: 0 0 1rem 0;
	color: #374151;
	font-size: 1.1rem;
}

.selected-offer {
	background: #f0fdf4;
	border: 1px solid #bbf7d0;
	border-radius: 8px;
	padding: 1rem;
	margin-bottom: 0.5rem;
}

.selected-offer p {
	margin: 0.25rem 0;
	color: #065f46;
}

.offer-features {
	margin-top: 0.5rem;
	padding-top: 0.5rem;
	border-top: 1px solid #bbf7d0;
}

.offer-features small {
	color: #059669;
	font-style: italic;
}

.missing-offer {
	background: #fef2f2;
	border: 1px solid #fecaca;
	border-radius: 8px;
	padding: 1rem;
	margin-bottom: 0.5rem;
}

.missing-offer p {
	margin: 0.25rem 0;
	color: #dc2626;
}

.form-group {
	margin-bottom: 1rem;
}

.form-group label {
	display: block;
	margin-bottom: 0.5rem;
	font-weight: 600;
	color: #374151;
}

.form-control {
	width: 100%;
	padding: 0.75rem;
	border: 1px solid #d1d5db;
	border-radius: 6px;
	font-size: 1rem;
	transition: border-color 0.2s;
	resize: vertical;
}

.form-control:focus {
	outline: none;
	border-color: #2563eb;
	box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.modal-footer {
	display: flex;
	justify-content: flex-end;
	gap: 0.75rem;
	padding: 1.5rem;
	border-top: 1px solid #e5e7eb;
}

.btn {
	padding: 0.75rem 1.5rem;
	border-radius: 6px;
	font-weight: 600;
	border: none;
	cursor: pointer;
	transition: all 0.2s;
}

.btn-primary {
	background: #2563eb;
	color: white;
}

.btn-primary:hover:not(:disabled) {
	background: #1d4ed8;
}

.btn-primary:disabled {
	background: #9ca3af;
	cursor: not-allowed;
}

.btn-secondary {
	background: #f3f4f6;
	color: #374151;
	border: 1px solid #d1d5db;
}

.btn-secondary:hover {
	background: #e5e7eb;
}

.btn-success {
	background: #10b981;
	color: white;
	border: 1px solid #10b981;
}

.btn-success:disabled {
	background: #6ee7b7;
	cursor: default;
	opacity: 0.8;
}

/* Warning Modal */
.warning-modal {
	max-width: 400px;
}

.warning-text {
	font-size: 1.1rem;
	color: #dc2626;
	text-align: center;
	margin: 0;
	line-height: 1.5;
}

/* Travel Created State */
.offer-card-display.travel-created {
	border-color: #059669;
	background: linear-gradient(135deg, #f0fdf4 0%, #ecfdf5 100%);
	box-shadow: 0 4px 20px rgba(16, 185, 129, 0.25);
	position: relative;
}

.offer-card-display.travel-created::before {
	content: '✓ CONFIRMED';
	position: absolute;
	top: -8px;
	right: 15px;
	background: #059669;
	color: white;
	padding: 4px 12px;
	border-radius: 12px;
	font-size: 0.75rem;
	font-weight: bold;
	letter-spacing: 0.5px;
}

.travel-created .status-badge.chosen {
	background: #059669;
	color: white;
	box-shadow: 0 2px 8px rgba(5, 150, 105, 0.3);
}
</style>
