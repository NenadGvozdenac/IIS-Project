<template>
    <div class="match-details-page">
        <header class="details-header">
            <div class="header-left">
                <button class="btn btn-secondary" @click="goBack">&larr; Back to calendar</button>
            </div>
            <div class="header-center">
                <nav class="nav-menu">
                    <router-link to="/club-manager/matches" class="nav-link active">Matches</router-link>
                    <router-link to="/club-manager/travelinfo" class="nav-link">Players</router-link>
                    <router-link to="/club-manager/travel" class="nav-link">Travel Organization</router-link>
                </nav>
            </div>
            <div class="header-right">
                <button v-if="match && (match.transportationRequired || match.accommodationRequired)" class="btn btn-primary" @click="goToCreateTravel">Create Travel</button>
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
                            Transportation is not required for this trip, you can optionally go to the transportation reservation page
                        </div>
                        <div v-else-if="!transportationOffer" class="info-box warning">
                            <span class="icon">&#9888;</span>
                            Transportation is required for this trip, please go to the transportation reservation page
                        </div>
                        <div v-else class="info-box selected-offer">
                            <span class="icon">&#10003;</span>
                            <div class="offer-details">
                                <h4>{{ transportationOffer.agencyName }}</h4>
                                <p><strong>{{ transportationOffer.price }} EUR</strong></p>
                                <p>{{ transportationOffer.transportationOffer?.vehicleType || 'Bus' }} - Capacity: {{ transportationOffer.transportationOffer?.capacity || 'N/A' }}</p>
                                <p>Departure: {{ transportationOffer.transportationOffer?.departureTime || 'N/A' }}</p>
                            </div>
                            <div class="offer-actions">
                                <button @click="goToTransportOffers" class="btn-outline">
                                    Change Selection
                                </button>
                            </div>
                        </div>
                        <div v-if="match.transportationRequired && !transportationOffer" class="offer-actions">
                            <button class="btn btn-primary" @click="goToTransportOffers">View Transport Offers</button>
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
                            Accommodation is not required for this trip, you can optionally go to the accommodation booking page
                        </div>
                        <div v-else-if="!accommodationOffer" class="info-box warning">
                            <span class="icon">&#9888;</span>
                            Accommodation is required for this trip, please go to the accommodation booking page
                        </div>
                        <div v-else class="info-box selected-offer">
                            <span class="icon">&#10003;</span>
                            <div class="offer-details">
                                <h4>{{ accommodationOffer.agencyName }}</h4>
                                <p><strong>{{ accommodationOffer.price }} EUR</strong></p>
                                <p>{{ accommodationOffer.accommodationOffer?.accommodationType || 'Hotel' }} - Capacity: {{ accommodationOffer.accommodationOffer?.capacity || 'N/A' }}</p>
                                <p>Room types: {{ accommodationOffer.accommodationOffer?.roomType || 'N/A' }}</p>
                            </div>
                            <div class="offer-actions">
                                <button @click="goToAccommodationOffers" class="btn-outline">
                                    Change Selection
                                </button>
                            </div>
                        </div>
                        <div v-if="match.accommodationRequired && !accommodationOffer" class="offer-actions">
                            <button class="btn btn-primary" @click="goToAccommodationOffers">View Accommodation Offers</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'

const route = useRoute()
const router = useRouter()

const match = ref(null)
const season = ref(null)
const team = ref(null)
const competition = ref(null)
const transportationOffer = ref(null)
const accommodationOffer = ref(null)

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
  // Implement navigation to create travel page or modal
  alert('Go to create travel (implement route/modal)')
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
    if (transportResponse.data.isSuccess) {
      transportationOffer.value = transportResponse.data.value
    }

    // Fetch chosen accommodation offer
    const accommodationResponse = await axios.get(`https://localhost:5007/api/offers/chosen/accommodation/${match.value.idMatch}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    if (accommodationResponse.data.isSuccess) {
      accommodationOffer.value = accommodationResponse.data.value
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
}
.nav-link {
	text-decoration: none;
	color: #6b7280;
	font-weight: 500;
	padding: 0.5rem 1rem;
	border-radius: 0.375rem;
	transition: all 0.2s;
}
.nav-link.active {
	color: var(--color-primary);
	background-color: #f3f4f6;
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
	padding: 0.5rem 1.5rem;
	border-radius: 1rem;
	font-weight: 500;
	font-size: 1rem;
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
</style>
