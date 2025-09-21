<template>
    <div class="match-details-page">
        <header class="details-header">
            <div class="header-left">
                <button class="btn btn-secondary" @click="goBack">&larr; Back to calendar</button>
            </div>
            <div class="header-center">
                <nav class="nav-menu">
                    <router-link to="/team-manager/matches" class="nav-link active">Matches</router-link>
                    <router-link to="/team-manager/players" class="nav-link">Players</router-link>
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
                    v-if="isTeamManager && canCreateTravel" 
                    @click="showCreateTravelModal = true"
                    class="btn btn-primary">
                    🧳 Create Travel
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
                        <div v-else class="offer-card-display">
                            <div class="offer-header">
                                <h3>{{ transportationOffer.agencyName || 'N/A' }}</h3>
                                <span class="status-badge chosen">Chosen</span>
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
                        </div>
                        <div v-if="match.transportationRequired && !transportationOffer" class="offer-actions">
                            <button @click="goToTransportOffers" class="btn-outline">View offers</button>
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
                        <div v-else class="offer-card-display">
                            <div class="offer-header">
                                <h3>{{ accommodationOffer.agencyName || 'N/A' }}</h3>
                                <span class="status-badge chosen">Chosen</span>
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
                        </div>
                        <div v-if="match.accommodationRequired && !accommodationOffer" class="offer-actions">
                            <button @click="goToAccommodationOffers" class="btn-outline">View offers</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Create Travel Modal -->
        <div v-if="showCreateTravelModal" class="modal-overlay" @click="closeCreateTravelModal">
            <div class="modal-content" @click.stop>
                <h2>Create Travel</h2>
                <form @submit.prevent="createTravel">
                    <div class="form-group">
                        <label for="travelNotes">Add Travel Note</label>
                        <textarea 
                            id="travelNotes"
                            v-model="travelNotes" 
                            rows="5"
                            placeholder="Enter any additional notes for this travel..."
                            class="form-control">
                        </textarea>
                    </div>

                    <div class="modal-actions">
                        <button type="button" @click="closeCreateTravelModal" class="btn btn-outline">Cancel</button>
                        <button type="submit" :disabled="loading" class="btn btn-primary">Create</button>
                    </div>
                </form>
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
const travelNotes = ref('')
const loading = ref(false)

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
	router.push('/team-manager/matches')
}

const getUserId = () => {
  const token = localStorage.getItem('token')
  if (!token) return null
  
  const userData = AuthService.decode(token)
  return userData?.userID || null
}
//const isTeamManager = computed(() => getUserId() == 6)

const canCreateTravel = computed(() => {
  if (!match.value) return false
  
  // Check if transportation is required and chosen
  const transportationOk = !match.value.transportationRequired || transportationOffer.value
  
  // Check if accommodation is required and chosen
  const accommodationOk = !match.value.accommodationRequired || accommodationOffer.value
  
  return transportationOk && accommodationOk
})

// Navigation methods
const goToTransportOffers = () => {
  router.push(`/offers/transportation/${match.value.idMatch}`)
}

const goToAccommodationOffers = () => {
  router.push(`/offers/accommodation/${match.value.idMatch}`)
}

// Create Travel Modal methods
const closeCreateTravelModal = () => {
  showCreateTravelModal.value = false
  travelNotes.value = ''
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

const createTravel = async () => {
  if (loading.value || !match.value) return
  
  loading.value = true
  
  try {
    const travelData = {
      notes: travelNotes.value,
      matchIdMatch: match.value.idMatch,
      idTransportationOffer: transportationOffer.value?.idOffer,
      idTransportationAgency: transportationOffer.value?.idAgency,
      idTransportationRequest: transportationOffer.value?.idRequest,
      idAccommodationOffer: accommodationOffer.value?.idOffer || null,
      idAccommodationAgency: accommodationOffer.value?.idAgency || null,
      idAccommodationRequest: accommodationOffer.value?.idRequest || null
    }
    
    const response = await axios.post('https://localhost:5007/api/trip', travelData)
    
    if (response.data.isSuccess) {
      alert('Travel created successfully!')
      closeCreateTravelModal()
    } else {
      alert(response.data.error || 'Failed to create travel')
    }
  } catch (error) {
    console.error('Error creating travel:', error)
    alert('Failed to create travel')
  } finally {
    loading.value = false
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
</style>
