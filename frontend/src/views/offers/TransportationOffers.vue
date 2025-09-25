<template>
  <div class="transportation-offers">
    <!-- Header with navigation -->
    <div class="details-header">
      <div class="header-left">
        <button @click="goBack" class="btn btn-back">
          ← Back
        </button>
      </div>
      <div class="header-center">
        <div class="nav-menu">
          <router-link :to="userRole === 'TeamManager' ? '/team-manager/matches' : '/club-manager/matches'" class="nav-link">Matches</router-link>
          <router-link :to="userRole === 'TeamManager' ? '/team-manager/players' : '/club-manager/travelinfo'" class="nav-link">Players</router-link>
          <div class="nav-dropdown">
                <span class="nav-link dropdown-toggle">Requests</span>
                <div class="dropdown-menu">
                    <router-link to="/team-manager/transportation-requests-active" class="dropdown-item">Transportation - Active</router-link>
                    <router-link to="/team-manager/transportation-requests-archive" class="dropdown-item">Transportation - Archive</router-link>
                    <router-link to="/team-manager/accommodation-requests-active" class="dropdown-item">Accommodation - Active</router-link>
                    <router-link to="/team-manager/accommodation-requests-archive" class="dropdown-item">Accommodation - Archive</router-link>
                </div>
            </div>
        </div>
      </div>
      <div class="header-right">
        <div class="user-info">
        </div>
      </div>
    </div>

    <!-- Page title and new offer button -->
    <div class="page-header">
      <h1>Transportation offers</h1>
      <div class="header-buttons">
        <button 
          v-if="isTeamManager" 
          @click="showAddOfferModal = true"
          class="btn btn-primary">
          + New transportation offer
        </button>
      </div>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="loading-state">
      <div class="loading-spinner"></div>
      <p>Loading offers...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button @click="fetchOffers" class="btn btn-outline">Try Again</button>
    </div>

    <!-- Offers grid -->
    <div v-else class="offers-container">
      <div v-if="offers.length === 0" class="empty-state">
        <p>No transportation offers available for this match.</p>
      </div>
      <div v-else class="offers-grid">
        <div 
          v-for="offer in offers" 
          :key="`${offer.idOffer}-${offer.idAgency}-${offer.idRequest}`"
          class="offer-card"
          :class="{ 'chosen': offer.chosen }">
          
          <!-- Agency name and status -->
          <div class="offer-header">
            <h3>Agency: {{ offer.agencyName }} Transportation: {{ offer.companyName }}</h3>
            <span class="status-badge" :class="{ 'chosen': offer.chosen }">
              {{ offer.chosen ? 'Chosen' : 'Available' }}
            </span>
          </div>

          <!-- Price -->
          <div class="offer-price">
            {{ offer.price }} EUR
          </div>

          <!-- Transportation details -->
          <div class="offer-details">
            <div class="detail-row">
              <span class="detail-label">Vehicle:</span>
              <span class="vehicle-type">{{ offer.vehicleType || 'N/A' }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Capacity:</span>
              <span>{{ offer.capacity || 'N/A' }}</span>
            </div>
          </div>

          <!-- Additional benefits -->
          <div class="offer-benefits">
            <h4>Additional benefits:</h4>
            <div class="benefits-tags">
              <span class="benefit-tag">
                {{
                [
                    offer.airConditioning ? 'Air Conditioning' : null,
                    offer.tv ? 'TV' : null,
                    offer.wifiTransport ? 'WIFI' : null,
                    offer.restroom ? 'WC' : null
                ].filter(Boolean).join(', ') || 'N/A'
                }}
              </span>
            </div>
          </div>

          <!-- Choose button -->
          <div class="offer-actions" v-if="!isTeamManager">
            <button 
              @click="chooseOffer(offer)"
              class="btn btn-choose"
              :class="{ 'chosen': offer.chosen }"
              :disabled="loading">
              {{ offer.chosen ? 'Chosen' : 'Choose' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Offer Modal -->
    <div v-if="showAddOfferModal" class="modal-overlay" @click="closeAddOfferModal">
      <div class="modal-content" @click.stop>
        <h2>Add transportation offer</h2>
        <form @submit.prevent="addOffer">
          <div class="form-group">
            <label for="agency">Agency *</label>
            <select 
              id="agency" 
              v-model="newOffer.selectedAgency" 
              required
              class="form-control">
              <option value="">Select agency</option>
              <option 
                v-for="agency in availableAgencies" 
                :key="agency.idAgency"
                :value="agency">
                {{ agency.agencyName }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="companyName">Company Name *</label>
            <input 
              id="companyName"
              type="text" 
              v-model="newOffer.companyName" 
              required
              placeholder="Enter company name"
              class="form-control">
          </div>

          <div class="form-group">
            <label for="capacity">Capacity *</label>
            <input 
              id="capacity"
              type="number" 
              v-model="newOffer.capacity" 
              required
              min="1"
              class="form-control">
          </div>

          <div class="form-group">
            <label for="vehicleType">Vehicle type *</label>
            <select id="vehicleType" v-model="newOffer.vehicleType" required class="form-control">
              <option value="">Select vehicle type</option>
              <option value="Bus">Bus</option>
              <option value="Van">Van</option>
              <option value="Plane">Plane</option>
              <option value="Train">Train</option>
            </select>
          </div>

          <div class="form-group">
            <label for="equipmentSpace">Place for equipment *</label>
            <select id="equipmentSpace" v-model="newOffer.placeForEquipment" required class="form-control">
              <option value="">Select option</option>
              <option :value="true">YES</option>
              <option :value="false">NO</option>
            </select>
          </div>

          <div class="form-group">
            <label for="price">Price *</label>
            <input 
              id="price"
              type="number" 
              v-model="newOffer.price" 
              required
              min="0"
              step="0.01"
              class="form-control">
          </div>

          <div class="form-group">
            <label>Additional benefits *</label>
            <div class="checkbox-group">
              <label class="checkbox-item">
                <input type="checkbox" v-model="newOffer.benefits.airConditioner">
                <span>Air conditioner</span>
              </label>
              <label class="checkbox-item">
                <input type="checkbox" v-model="newOffer.benefits.tv">
                <span>TV</span>
              </label>
              <label class="checkbox-item">
                <input type="checkbox" v-model="newOffer.benefits.wifi">
                <span>WIFI</span>
              </label>
              <label class="checkbox-item">
                <input type="checkbox" v-model="newOffer.benefits.wc">
                <span>WC</span>
              </label>
            </div>
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeAddOfferModal" class="btn btn-outline">Cancel</button>
            <button type="submit" :disabled="loading" class="btn btn-primary">Add Offer</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Auto Select Result Modal -->
    <div v-if="showAutoSelectResult" class="modal-overlay" @click="closeAutoSelectResult">
      <div class="modal-content auto-select-modal" @click.stop>
        <div class="modal-header">
          <h2>🤖 Auto Selection Completed!</h2>
          <button @click="closeAutoSelectResult" class="close-btn">×</button>
        </div>
        
        <div class="result-content" v-if="autoSelectResult">
          <div class="success-badge">
            ✅ Best offer selected successfully!
          </div>
          
          <div class="result-details">
            <div class="detail-card">
              <div class="detail-icon">🏢</div>
              <div class="detail-info">
                <h4>Selected Agency</h4>
                <p>{{ autoSelectResult.selectedAgencyName }}</p>
              </div>
            </div>
            
            <div class="detail-card">
              <div class="detail-icon">⭐</div>
              <div class="detail-info">
                <h4>Selection Score</h4>
                <p>{{ (autoSelectResult.selectionScore * 100).toFixed(1) }}%</p>
              </div>
            </div>
            
            <div class="detail-card">
              <div class="detail-icon">📊</div>
              <div class="detail-info">
                <h4>Offers Analyzed</h4>
                <p>{{ autoSelectResult.totalOffersAnalyzed }} offers</p>
              </div>
            </div>
          </div>
          
          <div class="reason-section">
            <h4>📋 Selection Analysis</h4>
            <div class="reason-text">
              {{ autoSelectResult.selectionReason }}
            </div>
          </div>
          
          <div class="modal-actions">
            <button @click="closeAutoSelectResult" class="btn btn-primary">
              Perfect! Close
            </button>
          </div>
        </div>
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

// Get user role from token
const getUserRole = () => {
  try {
    const token = localStorage.getItem('token')
    if (!token) return null
    
    const payload = JSON.parse(atob(token.split('.')[1]))
    return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  } catch (error) {
    console.error('Error getting user role:', error)
    return null
  }
}

const userRole = computed(() => getUserRole())
// }

// Get user ID from token
const getUserId = () => {
  const token = localStorage.getItem('token')
  if (!token) return null
  
  const userData = AuthService.decode(token)
  return userData?.userID || null
}

// Reactive data
const loading = ref(false)
const autoSelectLoading = ref(false)
const error = ref('')
const offers = ref([])
const availableAgencies = ref([])
const showAddOfferModal = ref(false)
const showAutoSelectResult = ref(false)
const autoSelectResult = ref(null)

// Computed
const isTeamManager = computed(() => getUserId() == 6)
const isClubManager = computed(() => getUserId() == 2)
const matchId = computed(() => parseInt(route.params.matchId))

// New offer form data
const newOffer = ref({
  selectedAgency: '',
  companyName: '',
  capacity: '',
  vehicleType: '',
  placeForEquipment: '',
  price: '',
  benefits: {
    airConditioner: false,
    tv: false,
    wifi: false,
    wc: false
  }
})

// Methods
const goBack = () => {
  router.go(-1)
}

const fetchOffers = async () => {
  if (!matchId.value) return
  
  loading.value = true
  error.value = ''
  
  try {
    const response = await axios.get(`https://localhost:5007/api/offers/transportation/${matchId.value}`)
    if (response.data.isSuccess) {
        offers.value = Array.isArray(response.data.value.offers)
      ? response.data.value.offers
      : []
    } else {
      error.value = response.data.error || 'Failed to load offers'
    }
  } catch (err) {
    console.error('Error fetching offers:', err)
    error.value = 'Failed to load transportation offers'
  } finally {
    loading.value = false
  }
}

const fetchAvailableAgencies = async () => {
  if (!matchId.value) return
  
  try {
    const response = await axios.get(`https://localhost:5007/api/agencies/${matchId.value}/transportation`)
    if (response.data.isSuccess) {
      availableAgencies.value = response.data.value.agencies || []
    }
  } catch (err) {
    console.error('Error fetching agencies:', err)
  }
}

const chooseOffer = async (offer) => {
  if (loading.value) return
  
  loading.value = true
  
  try {
    const updateData = {
      idOffer: offer.idOffer,
      idAgency: offer.idAgency,
      idRequest: offer.idRequest,
      chosen: !offer.chosen,
      type: 'transportation',
      idMatch: matchId.value
    }
    
    const response = await axios.put('https://localhost:5007/api/offers/status', updateData)
    
    if (response.data.isSuccess) {
      // Refresh offers to get updated status
      await fetchOffers()
    } else {
      error.value = response.data.error || 'Failed to update offer status'
    }
  } catch (err) {
    console.error('Error updating offer status:', err)
    error.value = 'Failed to update offer status'
  } finally {
    loading.value = false
  }
}
const mapVehicleType = (vehicleType) => {
  const mapping = {
    'bus': 'autobus',
    'plane': 'plane',
    'train': 'train',
    'van': 'van'
  }
  return mapping[vehicleType.toLowerCase()] || vehicleType.toLowerCase()
}
const addOffer = async () => {
  if (loading.value || !newOffer.value.selectedAgency) return
  
  loading.value = true
  
  try {
    const offerData = {
      userId: getUserId(),
      idAgency: newOffer.value.selectedAgency.idAgency,
      idRequest: newOffer.value.selectedAgency.idRequest,
      idMatch: matchId.value,
      price: parseFloat(newOffer.value.price),
      chosen: false,
      type: 'transportation',
      companyName: newOffer.value.companyName,
      capacity: parseInt(newOffer.value.capacity),
      vehicleType: mapVehicleType(newOffer.value.vehicleType),
      equipmentSpace: newOffer.value.placeForEquipment,
      airConditioning: newOffer.value.benefits.airConditioner,
      tv: newOffer.value.benefits.tv,
      wifiTransport: newOffer.value.benefits.wifi,
      restroom: newOffer.value.benefits.wc
    }
    
    console.log('Submitting offer data:', offerData)
    const response = await axios.post(`https://localhost:5007/api/offers/transportation/${getUserId()}`, offerData)
    
    if (response.data.isSuccess) {
      closeAddOfferModal()
      await fetchOffers()
    } else {
      error.value = response.data.error || 'Failed to create offer'
    }
  } catch (err) {
    console.error('Error creating offer:', err)
    error.value = 'Failed to create offer'
  } finally {
    loading.value = false
  }
}

const closeAddOfferModal = () => {
  showAddOfferModal.value = false
  resetNewOffer()
}

const closeAutoSelectResult = () => {
  showAutoSelectResult.value = false
  autoSelectResult.value = null
}

const resetNewOffer = () => {
  newOffer.value = {
    selectedAgency: '',
    companyName: '',
    capacity: '',
    vehicleType: '',
    placeForEquipment: '',
    price: '',
    benefits: {
      airConditioner: false,
      tv: false,
      wifi: false,
      wc: false
    }
  }
}

const parseBenefits = (benefitsString) => {
  if (!benefitsString) return []
  return benefitsString.split(',').map(b => b.trim()).filter(b => b.length > 0)
}

const autoSelectBestOffer = async () => {
  if (autoSelectLoading.value || offers.value.length <= 1) return
  
  autoSelectLoading.value = true
  error.value = ''
  
  try {
    const autoSelectData = {
      matchId: matchId.value,
      offerType: 'transportation',
      weightPrice: 0.4,      // 40% weight for price
      weightCapacity: 0.3,   // 30% weight for capacity
      weightBenefits: 0.2,   // 20% weight for benefits
      weightAgency: 0.1      // 10% weight for agency reliability
    }
    
    const response = await axios.post('https://localhost:5007/api/offers/auto-select', autoSelectData)
    
    if (response.data.isSuccess) {
      const result = response.data.value
      
      // Store result and show modal
      autoSelectResult.value = {
        selectedAgencyName: result.selectedAgencyName,
        selectionScore: result.selectionScore,
        totalOffersAnalyzed: result.totalOffersAnalyzed,
        selectionReason: result.selectionReason
      }
      showAutoSelectResult.value = true
      
      // Refresh offers to show the selected one
      await fetchOffers()
    } else {
      error.value = response.data.error || 'Auto-selection failed'
      alert(`❌ Auto-selection failed: ${error.value}`)
    }
  } catch (err) {
    console.error('Error in auto-select:', err)
    error.value = 'Failed to auto-select best offer'
    alert(`❌ Error: ${error.value}`)
  } finally {
    autoSelectLoading.value = false
  }
}

// Lifecycle
onMounted(() => {
  fetchOffers()
  if (isTeamManager.value) {
    fetchAvailableAgencies()
  }
})
</script>

<style scoped>
/* Header styles */
/* Dropdown styles */
.nav-dropdown {
  position: relative;
  padding: 0.5rem;
}

.dropdown-toggle {
  cursor: pointer;
}

.dropdown-toggle::after {
  content: ' ▼';
  font-size: 12px;
}

.nav-dropdown:hover .dropdown-toggle::after {
  content: ' ▲';
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  min-width: 200px;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
  z-index: 1000;
}

.nav-dropdown:hover .dropdown-menu {
  opacity: 1;
  visibility: visible;
}

.dropdown-item {
  display: block;
  padding: 10px 15px;
  text-decoration: none;
  color: #333;
  transition: background-color 0.3s;
}

.dropdown-item:last-child {
  border-bottom: none;
}

.dropdown-item:hover {
  background-color: #f0f0f0;
}

.details-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 1200px;
  margin: 2rem auto;
}

.header-left, .header-right {
  flex: 1;
}

.header-center {
  flex: 2;
  display: flex;
  justify-content: center;
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
  color: var(--color-primary, #1708c0);
  background-color: #f3f4f6;
}

.btn-back {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border: 2px solid #000;
  background: white;
  border-radius: 0.375rem;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-back:hover {
  background: #f0f0f0;
}

.user-info {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.user-icon {
  font-size: 1.5rem;
}

/* Page content */
.transportation-offers {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  font-weight: bold;
  margin: 0;
}

.header-buttons {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 0.375rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-primary {
  background: #1708c0;
  color: white;
  border: 2px solid #1708c0;
}

.btn-primary:hover {
  background: #1406a0;
}

.btn-auto-select {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: 2px solid #667eea;
  font-weight: 600;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.btn-auto-select:hover:not(:disabled) {
  background: linear-gradient(135deg, #5a6fd8 0%, #6a4190 100%);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-auto-select:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.btn-outline {
  background: white;
  color: #374151;
  border: 2px solid #d1d5db;
}

.btn-outline:hover {
  background: #f9fafb;
}

/* Offers grid */
.offers-container {
  margin-top: 2rem;
}

.offers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 2rem;
}

.offer-card {
  background: white;
  border: 2px solid #000;
  border-radius: 1rem;
  padding: 1.5rem;
  transition: all 0.3s ease;
}

.offer-card.chosen {
  border-color: #10b981;
  background: #f0fdf4;
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
  background: #f9fafb;
  border-radius: 0.5rem;
}

.offer-details {
  margin: 1rem 0;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  border-bottom: 1px solid #e5e7eb;
}

.detail-row:last-child {
  border-bottom: none;
}

.detail-label {
  font-weight: 500;
  color: #6b7280;
}

.vehicle-type {
  font-weight: 500;
  text-transform: capitalize;
}

.offer-benefits {
  margin: 1rem 0;
}

.offer-benefits h4 {
  margin: 0 0 0.5rem 0;
  font-size: 1rem;
  font-weight: 600;
}

.benefits-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.benefit-tag {
  padding: 0.25rem 0.5rem;
  background: #ddd6fe;
  color: #7c3aed;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.offer-actions {
  margin-top: 1.5rem;
}

.btn-choose {
  width: 100%;
  padding: 0.75rem;
  background: white;
  color: #374151;
  border: 2px solid #000;
  font-weight: bold;
}

.btn-choose:hover {
  background: #f9fafb;
}

.btn-choose.chosen {
  background: #10b981;
  color: white;
  border-color: #10b981;
}

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 1rem;
  padding: 2rem;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-content h2 {
  margin: 0 0 1.5rem 0;
  font-size: 1.5rem;
  font-weight: bold;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #374151;
}

.form-control {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 1rem;
}

.form-control:focus {
  outline: none;
  border-color: #1708c0;
}

.checkbox-group {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.5rem;
}

.checkbox-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.checkbox-item input[type="checkbox"] {
  width: auto;
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
}

/* Loading and error states */
.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.loading-spinner {
  width: 3rem;
  height: 3rem;
  border: 3px solid #e5e7eb;
  border-top: 3px solid #1708c0;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Auto Select Result Modal */
.auto-select-modal {
  max-width: 600px;
  width: 95%;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e5e7eb;
}

.modal-header h2 {
  margin: 0;
  color: #1f2937;
  font-size: 1.5rem;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6b7280;
  padding: 0.25rem;
  border-radius: 50%;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.close-btn:hover {
  background: #f3f4f6;
  color: #374151;
}

.success-badge {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
  padding: 1rem 1.5rem;
  border-radius: 0.75rem;
  text-align: center;
  font-weight: 600;
  font-size: 1.1rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 15px rgba(16, 185, 129, 0.3);
}

.result-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.detail-card {
  background: #f8fafc;
  border: 2px solid #e2e8f0;
  border-radius: 0.75rem;
  padding: 1rem;
  text-align: center;
  transition: all 0.3s ease;
}

.detail-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.detail-icon {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

.detail-info h4 {
  margin: 0 0 0.25rem 0;
  font-size: 0.875rem;
  color: #6b7280;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.detail-info p {
  margin: 0;
  font-size: 1.25rem;
  font-weight: bold;
  color: #1f2937;
}

.reason-section {
  background: #f0f9ff;
  border: 2px solid #0ea5e9;
  border-radius: 0.75rem;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
}

.reason-section h4 {
  margin: 0 0 1rem 0;
  color: #0c4a6e;
  font-weight: 600;
}

.reason-text {
  color: #0f172a;
  line-height: 1.6;
  font-size: 0.95rem;
}
</style>