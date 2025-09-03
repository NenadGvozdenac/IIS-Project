<template>
  <div class="customer-profile">
    <div class="container">
      <h1 class="page-title">My Profile</h1>
      
      <!-- Navigation tabs -->
      <div class="tabs">
        <button 
          class="tab-btn" 
          :class="{ active: activeTab === 'info' }"
          @click="activeTab = 'info'"
        >
          Personal Info
        </button>
        <button 
          class="tab-btn" 
          :class="{ active: activeTab === 'cards' }"
          @click="switchToCardsTab"
        >
          Credit Cards
        </button>
      </div>

      <!-- Personal Info Tab -->
      <div v-if="activeTab === 'info'" class="tab-content">
        <div class="info-section">
          <h2>Personal Information</h2>
          <div class="info-grid">
            <div class="info-item">
              <label>Name:</label>
              <span>{{ userInfo?.userName || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <label>Email:</label>
              <span>{{ userInfo?.userEmail || 'N/A' }}</span>
            </div>
            <div class="info-item">
              <label>Role:</label>
              <span>{{ userInfo?.userRole || 'N/A' }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Credit Cards Tab -->
      <div v-if="activeTab === 'cards'" class="tab-content">
        <div class="section-header">
          <h2>My Credit Cards</h2>
          <button @click="openAddCardModal" class="btn btn-primary">
            <i class="icon-plus"></i>
            Add New Card
          </button>
        </div>

        <!-- Credit Cards Grid -->
        <div v-if="creditCards.length > 0" class="cards-grid">
          <div v-for="card in creditCards" :key="card.idCreditCard" class="credit-card">
            <div class="card-header">
              <div class="card-type">Credit Card</div>
              <div class="card-actions">
                <button @click="editCard(card)" class="btn btn-sm">
                  Edit
                </button>
                <button @click="deleteCard(card.idCreditCard)" class="btn btn-sm btn-danger">
                  Delete
                </button>
              </div>
            </div>
            
            <div class="card-body">
              <div class="card-number">
                **** **** **** {{ card.number.slice(-4) }}
              </div>
              <div class="card-details">
                <div class="card-name">{{ card.name }}</div>
                <div class="card-expiry">{{ formatExpiryDate(card.expirationDate) }}</div>
              </div>
            </div>
          </div>
        </div>

        <div v-else class="empty-state">
          <p>No credit cards added yet.</p>
          <button @click="openAddCardModal" class="btn btn-primary">
            Add Your First Card
          </button>
        </div>
      </div>
    </div>

    <!-- Add/Edit Credit Card Modal -->
    <div v-if="showCardModal" class="modal-overlay" @click="closeCardModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h3>{{ isEditingCard ? 'Edit Credit Card' : 'Add New Credit Card' }}</h3>
          <button @click="closeCardModal" class="close-btn">&times;</button>
        </div>
        
        <!-- Edit card info notice -->
        <div v-if="isEditingCard" class="edit-notice">
          <i class="icon-info"></i>
          <span>Please re-enter all card information for security purposes.</span>
        </div>
        
        <form @submit.prevent="saveCard" class="modal-body">
          <div class="form-group">
            <label for="cardNumber">Card Number * (16 digits)</label>
            <input 
              id="cardNumber"
              v-model="cardForm.number" 
              type="text" 
              class="form-control"
              :class="{ 'invalid': cardForm.number && (cardForm.number || '').replace(/\s/g, '').length !== 16 }"
              required
              placeholder="1234 5678 9012 3456"
              maxlength="19"
              @input="formatCardNumber"
            />
          </div>
          
          <div class="form-group">
            <label for="cardName">Cardholder Name *</label>
            <input 
              id="cardName"
              v-model="cardForm.name" 
              type="text" 
              class="form-control"
              required
              placeholder="John Doe"
            />
          </div>
          
          <div class="form-row">
            <div class="form-group half-width">
              <label for="cardExpiry">Expiry Date *</label>
              <input 
                id="cardExpiry"
                v-model="cardForm.expirationDate" 
                type="date" 
                class="form-control"
                required
                :min="minExpiryDate"
              />
            </div>
            
            <div class="form-group half-width">
              <label for="cardCVV">CVV *</label>
              <input 
                id="cardCVV"
                v-model="cardForm.cvv" 
                type="text" 
                class="form-control"
                required
                placeholder="123"
                maxlength="4"
                @input="formatCVV"
              />
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" @click="closeCardModal" class="btn btn-secondary">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" :disabled="loading || !isFormValid">
              {{ loading ? 'Saving...' : (isEditingCard ? 'Update Card' : 'Add Card') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { CreditCardService } from '../../services/credit_card_service.js';
import { getUserData } from '../../services/auth_service.js';

// Reactive data
const activeTab = ref('info');
const creditCards = ref([]);
const loading = ref(false);
const userInfo = ref(getUserData());

// Modal data
const showCardModal = ref(false);
const isEditingCard = ref(false);
const cardForm = ref({
  number: '',
  name: '',
  cvv: '',
  expirationDate: ''
});

// Computed properties
const minExpiryDate = computed(() => {
  const today = new Date();
  const year = today.getFullYear();
  const month = String(today.getMonth() + 1).padStart(2, '0');
  return `${year}-${month}`;
});

const isFormValid = computed(() => {
  const cardNumber = (cardForm.value.number || '').replace(/\s/g, '');
  return (
    cardNumber.length === 16 &&
    (cardForm.value.name || '').trim().length > 0 &&
    (cardForm.value.cvv || '').length >= 3 &&
    (cardForm.value.expirationDate || '').length > 0
  );
});

// Methods
const loadCreditCards = async () => {
  try {
    loading.value = true;
    const response = await CreditCardService.getCreditCardsByUser();
    creditCards.value = response.value || [];
  } catch (error) {
    console.error('Error loading credit cards:', error);
    alert('Error loading credit cards');
  } finally {
    loading.value = false;
  }
};

const formatCardNumber = (event) => {
  let value = event.target.value.replace(/\s/g, '').replace(/[^0-9]/gi, '');
  
  // Limit to 16 digits
  if (value.length > 16) {
    value = value.slice(0, 16);
  }
  
  const formattedValue = value.match(/.{1,4}/g)?.join(' ') || value;
  cardForm.value.number = formattedValue;
};

const formatCVV = (event) => {
  const value = event.target.value.replace(/[^0-9]/gi, '');
  cardForm.value.cvv = value;
};

const formatExpiryDate = (dateString) => {
  if (!dateString) return 'N/A';
  const date = new Date(dateString);
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = String(date.getFullYear()).slice(-2);
  return `${month}/${year}`;
};

const openAddCardModal = () => {
  isEditingCard.value = false;
  cardForm.value = {
    number: '',
    name: '',
    cvv: '',
    expirationDate: ''
  };
  showCardModal.value = true;
};

const editCard = (card) => {
  isEditingCard.value = true;
  cardForm.value = {
    idCreditCard: card.idCreditCard,
    number: card.number,
    name: card.name,
    cvv: card.cvv,
    expirationDate: card.expirationDate.split('T')[0]
  };
  showCardModal.value = true;
};

const closeCardModal = () => {
  showCardModal.value = false;
  cardForm.value = {
    number: '',
    name: '',
    cvv: '',
    expirationDate: ''
  };
};

const saveCard = async () => {
  try {
    loading.value = true;
    
    const cardData = {
      number: cardForm.value.number.replace(/\s/g, ''), 
      name: cardForm.value.name,
      cvv: cardForm.value.cvv,
      expirationDate: cardForm.value.expirationDate
    };
    
    if (isEditingCard.value) {
      await CreditCardService.updateCreditCard(cardForm.value.idCreditCard, cardData);
      alert('Credit card updated successfully!');
    } else {
      await CreditCardService.createCreditCard(cardData);
      alert('Credit card added successfully!');
    }
    
    closeCardModal();
    await loadCreditCards();
  } catch (error) {
    console.error('Error saving credit card:', error);
    alert('Error saving credit card');
  } finally {
    loading.value = false;
  }
};

const deleteCard = async (cardId) => {
  if (!confirm('Are you sure you want to delete this credit card?')) {
    return;
  }
  
  try {
    await CreditCardService.deleteCreditCard(cardId);
    alert('Credit card deleted successfully!');
    await loadCreditCards();
  } catch (error) {
    console.error('Error deleting credit card:', error);
    alert('Error deleting credit card');
  }
};

// Lifecycle
onMounted(async () => {
  if (activeTab.value === 'cards') {
    await loadCreditCards();
  }
});

// Watch for tab changes
const switchToCardsTab = async () => {
  activeTab.value = 'cards';
  if (creditCards.value.length === 0) {
    await loadCreditCards();
  }
};
</script>

<style scoped>
.customer-profile {
  padding: var(--spacing-xl) 0;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-text);
  margin-bottom: var(--spacing-xl);
  text-align: center;
}

/* Tabs */
.tabs {
  display: flex;
  border-bottom: 2px solid var(--color-border);
  margin-bottom: var(--spacing-xl);
}

.tab-btn {
  padding: var(--spacing-md) var(--spacing-lg);
  background: none;
  border: none;
  font-size: 1rem;
  font-weight: 500;
  color: var(--color-text-muted);
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.2s ease;
}

.tab-btn:hover {
  color: var(--color-primary);
}

.tab-btn.active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
}

/* Personal Info */
.info-section {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  padding: var(--spacing-xl);
}

.info-section h2 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-lg);
}

.info-grid {
  display: grid;
  gap: var(--spacing-lg);
}

.info-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.info-item label {
  font-weight: 600;
  color: var(--color-text);
  min-width: 100px;
}

.info-item span {
  color: var(--color-text-muted);
}

/* Section header */
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.section-header h2 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
}

/* Credit Cards Grid */
.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: var(--spacing-lg);
}

.credit-card {
  background: linear-gradient(135deg, var(--color-primary), var(--color-primary-hover));
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  color: white;
  box-shadow: var(--shadow-md);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
}

.card-type {
  font-size: 0.875rem;
  font-weight: 500;
  opacity: 0.9;
}

.card-actions {
  display: flex;
  gap: var(--spacing-sm);
}

.card-actions .btn {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
}

.card-number {
  font-size: 1.25rem;
  font-weight: 600;
  letter-spacing: 0.1em;
  margin-bottom: var(--spacing-md);
}

.card-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-name {
  font-size: 0.875rem;
  font-weight: 500;
  text-transform: uppercase;
}

.card-expiry {
  font-size: 0.875rem;
  opacity: 0.9;
}

.empty-state {
  text-align: center;
  padding: var(--spacing-2xl);
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
}

.empty-state p {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
  font-size: 1.125rem;
}

/* Modal */
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

.modal {
  background: white;
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: var(--shadow-lg);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
}

.modal-header h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--color-text);
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: var(--color-text-muted);
  padding: 0;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  color: var(--color-text);
}

.edit-notice {
  background-color: #e8f4fd;
  border: 1px solid #b3d9f2;
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
  margin: var(--spacing-lg);
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  color: #1e40af;
  font-size: 0.875rem;
}

.edit-notice .icon-info::before {
  content: "ℹ️";
  font-style: normal;
}

.modal-body {
  padding: var(--spacing-lg);
}

.form-group {
  margin-bottom: var(--spacing-lg);
}

.form-group.half-width {
  flex: 1;
}

.form-row {
  display: flex;
  gap: var(--spacing-md);
}

.form-group label {
  display: block;
  font-weight: 500;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.form-control {
  width: 100%;
  padding: var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: 1rem;
  transition: border-color 0.2s ease;
}

.form-control:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-control.invalid {
  border-color: #ef4444;
  box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: var(--spacing-md);
  padding-top: var(--spacing-lg);
  margin-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

/* Responsive */
@media (max-width: 768px) {
  .section-header {
    flex-direction: column;
    align-items: stretch;
    gap: var(--spacing-md);
  }
  
  .cards-grid {
    grid-template-columns: 1fr;
  }
  
  .modal {
    width: 95%;
    margin: var(--spacing-md);
  }
  
  .form-row {
    flex-direction: column;
  }
  
  .card-actions {
    flex-direction: column;
  }
}
</style>
