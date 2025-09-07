<template>
  <div class="customer-cart">
    <div class="container">
      <h1 class="page-title">My Cart</h1>

      <!-- Loading State -->
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Loading your cart...</p>
      </div>

      <!-- Cart Content -->
      <div v-else-if="cart" class="cart-content">
        <!-- Cart Header -->
        <div class="cart-header">
          <div class="cart-info">
            <h2>Cart</h2>
            <p class="cart-meta">
              Created: {{ formatDate(cart.createdAt) }} |
              Items: {{ cart.itemsNumber }} |
              Status: <span class="status-badge" :class="cart.status">{{ cart.status }}</span>
            </p>
          </div>
          <div class="cart-total">
            <div class="total-amount">{{ formatPrice(cart.totalAmount) }} RSD</div>
          </div>
        </div>

        <!-- Cart Items -->
        <div v-if="cart.cartItems && cart.cartItems.length > 0" class="cart-items">
          <div v-for="item in cart.cartItems" :key="item.idPurchaseOffer" class="cart-item">
            <div class="item-details">
              <h3 class="item-name">{{ item.purchaseOfferName }}</h3>
              <p class="item-description">{{ item.purchaseOfferDescription }}</p>
              <p class="item-added">Added: {{ formatDate(item.addedAt) }}</p>
            </div>
            <div class="item-actions">
              <div class="item-price">
                <span class="price">{{ formatPrice(item.price) }} RSD</span>
              </div>
              <button @click="removeFromCart(item.idPurchaseOffer)" class="btn btn-sm btn-danger">
                Remove
              </button>
            </div>
          </div>
        </div>

        <!-- Empty Cart -->
        <div v-else class="empty-cart">
          <div class="empty-icon">🛒</div>
          <h3>Your cart is empty</h3>
          <p>Add some tickets to get started!</p>
        </div>

        <!-- Cart Actions -->
        <div v-if="cart.cartItems && cart.cartItems.length > 0" class="cart-actions">
          <button @click="goToDashboard" class="btn btn-secondary">
            Continue Shopping
          </button>
          <button @click="proceedToCheckout" class="btn btn-primary">
            Proceed to Checkout
          </button>
        </div>
      </div>

      <!-- Error State -->
      <div v-else class="error-state">
        <div class="error-icon">❌</div>
        <h3>Unable to load cart</h3>
        <p>There was an error loading your cart. Please try again.</p>
        <button @click="loadCart" class="btn btn-primary">
          Retry
        </button>
      </div>
    </div>

    <!-- Checkout Modal -->
    <div v-if="showCheckoutModal" class="modal-overlay" @click="closeCheckoutModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h3>Checkout</h3>
          <button @click="closeCheckoutModal" class="close-btn">&times;</button>
        </div>

        <div class="modal-body">
          <div class="checkout-summary">
            <h4>Order Summary</h4>
            <div class="summary-items">
              <div v-for="item in cart.cartItems" :key="item.idPurchaseOffer" class="summary-item">
                <span class="item-name">{{ item.purchaseOfferName }}</span>
                <span class="item-price">{{ formatPrice(item.price) }} RSD</span>
              </div>
            </div>
            <div class="summary-total">
              <strong>Total: {{ formatPrice(cart.totalAmount) }} RSD</strong>
            </div>
          </div>

          <div class="payment-section">
            <h4>Select Payment Method</h4>

            <!-- Loading credit cards -->
            <div v-if="loadingCards" class="loading-cards">
              <div class="spinner-sm"></div>
              <span>Loading credit cards...</span>
            </div>

            <!-- Credit cards list -->
            <div v-else-if="creditCards.length > 0" class="credit-cards-list">
              <div v-for="card in creditCards" :key="card.idCreditCard" class="credit-card-option">
                <label class="card-radio">
                  <input type="radio" :value="card.idCreditCard" v-model="selectedCardId" name="creditCard" />
                  <div class="card-info">
                    <div class="card-number">**** **** **** {{ card.number.slice(-4) }}</div>
                    <div class="card-name">{{ card.name }}</div>
                  </div>
                </label>
              </div>
            </div>

            <!-- Add new card option -->
            <div class="add-new-card">
              <label class="card-radio">
                <input type="radio" value="new" v-model="selectedCardId" name="creditCard" />
                <div class="card-info">
                  <div class="card-number">Add New Credit Card</div>
                  <div class="card-name">Enter new payment details</div>
                </div>
              </label>
            </div>

            <!-- New card form -->
            <div v-if="selectedCardId === 'new'" class="new-card-form">
              <div class="form-group">
                <label>Card Number *</label>
                <input v-model="newCard.number" type="text" class="form-control" placeholder="1234 5678 9012 3456"
                  maxlength="19" @input="formatCardNumber" />
              </div>

              <div class="form-group">
                <label>Cardholder Name *</label>
                <input v-model="newCard.name" type="text" class="form-control" placeholder="John Doe" />
              </div>

              <div class="form-row">
                <div class="form-group half-width">
                  <label>Expiry Date *</label>
                  <input v-model="newCard.expirationDate" type="date" class="form-control" :min="minExpiryDate" />
                </div>

                <div class="form-group half-width">
                  <label>CVV *</label>
                  <input v-model="newCard.cvv" type="text" class="form-control" placeholder="123" maxlength="4"
                    @input="formatCVV" />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closeCheckoutModal" class="btn btn-secondary">
            Cancel
          </button>
          <button @click="completeCheckout" class="btn btn-primary" :disabled="checkoutLoading || !canCheckout">
            {{ checkoutLoading ? 'Processing...' : 'Complete Purchase' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { CartService } from '../../services/cart_service.js';
import { CreditCardService } from '../../services/credit_card_service.js';

const router = useRouter();

// Reactive data
const cart = ref(null);
const loading = ref(false);
const showCheckoutModal = ref(false);
const creditCards = ref([]);
const loadingCards = ref(false);
const selectedCardId = ref(null);
const checkoutLoading = ref(false);

// New card form
const newCard = ref({
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

const canCheckout = computed(() => {
  if (!selectedCardId.value) return false;

  if (selectedCardId.value === 'new') {
    const cardNumber = (newCard.value.number || '').replace(/\s/g, '');
    return (
      cardNumber.length === 16 &&
      (newCard.value.name || '').trim().length > 0 &&
      (newCard.value.cvv || '').length >= 3 &&
      (newCard.value.expirationDate || '').length > 0
    );
  }

  return true;
});

// Methods
const loadCart = async () => {
  try {
    loading.value = true;
    const response = await CartService.getCurrentCart();
    cart.value = response.value || response;
  } catch (error) {
    console.error('Error loading cart:', error);
    cart.value = null;
  } finally {
    loading.value = false;
  }
};

const formatDate = (dateString) => {
  if (!dateString) return 'N/A';
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
};

const formatPrice = (price) => {
  if (!price && price !== 0) return '0.00';
  return parseFloat(price).toLocaleString('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
};

const goToDashboard = () => {
  router.push('/customer/dashboard');
};

const removeFromCart = async (purchaseOfferId) => {
  if (!confirm('Are you sure you want to remove this item from your cart?')) {
    return;
  }

  try {
    await CartService.removeFromCart(purchaseOfferId);
    await loadCart(); // Reload cart after removal
    alert('Item removed from cart successfully!');
  } catch (error) {
    console.error('Error removing item from cart:', error);
    alert('Error removing item from cart');
  }
};

const proceedToCheckout = async () => {
  showCheckoutModal.value = true;
  await loadCreditCards();
};

const loadCreditCards = async () => {
  try {
    loadingCards.value = true;
    const response = await CreditCardService.getCreditCardsByUser();
    creditCards.value = response.value || response || [];
  } catch (error) {
    console.error('Error loading credit cards:', error);
    creditCards.value = [];
  } finally {
    loadingCards.value = false;
  }
};

const closeCheckoutModal = () => {
  showCheckoutModal.value = false;
  selectedCardId.value = null;
  newCard.value = {
    number: '',
    name: '',
    cvv: '',
    expirationDate: ''
  };
};

const formatCardNumber = (event) => {
  let value = event.target.value.replace(/\s/g, '').replace(/[^0-9]/gi, '');

  if (value.length > 16) {
    value = value.slice(0, 16);
  }

  const formattedValue = value.match(/.{1,4}/g)?.join(' ') || value;
  newCard.value.number = formattedValue;
};

const formatCVV = (event) => {
  const value = event.target.value.replace(/[^0-9]/gi, '');
  newCard.value.cvv = value;
};

const completeCheckout = async () => {
  try {
    checkoutLoading.value = true;

    let cardToUse = selectedCardId.value;

    // If new card selected, create it first
    if (selectedCardId.value === 'new') {
      const cardData = {
        number: newCard.value.number.replace(/\s/g, ''),
        name: newCard.value.name,
        cvv: newCard.value.cvv,
        expirationDate: newCard.value.expirationDate
      };

      const newCardResponse = await CreditCardService.createCreditCard(cardData);
      cardToUse = newCardResponse.value?.idCreditCard || newCardResponse.idCreditCard;
    }

    await CartService.checkout(cart.value.idCart, cardToUse);

    alert('Checkout completed successfully!');
    closeCheckoutModal();
    await loadCart(); // Reload cart

  } catch (error) {
    console.error('Error during checkout:', error);
    alert('Error during checkout. Please try again.');
  } finally {
    checkoutLoading.value = false;
  }
};

const proceedToCheckout_old = () => {
  // TODO: Implement checkout process
  alert('Checkout functionality will be implemented soon!');
};

// Lifecycle
onMounted(() => {
  loadCart();
});
</script>

<style scoped>
.customer-cart {
  padding: var(--spacing-xl) 0;
  min-height: calc(100vh - 200px);
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-text);
  margin-bottom: var(--spacing-xl);
  text-align: center;
}

/* Loading State */
.loading-state {
  text-align: center;
  padding: var(--spacing-2xl);
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--color-border);
  border-top: 4px solid var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto var(--spacing-md);
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

/* Cart Content */
.cart-content {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  overflow: hidden;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: var(--spacing-xl);
  border-bottom: 1px solid var(--color-border);
  background: var(--color-surface);
}

.cart-info h2 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.cart-meta {
  color: var(--color-text-muted);
  font-size: 0.875rem;
  margin: 0;
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: var(--radius-sm);
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-badge.created {
  background-color: #e0f2fe;
  color: #0277bd;
}

.status-badge.pending {
  background-color: #fff3e0;
  color: #f57c00;
}

.status-badge.completed {
  background-color: #e8f5e8;
  color: #2e7d32;
}

.total-amount {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-primary);
}

/* Cart Items */
.cart-items {
  border-top: 1px solid var(--color-border);
}

.cart-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
  transition: background-color 0.2s ease;
}

.cart-item:last-child {
  border-bottom: none;
}

.cart-item:hover {
  background-color: var(--color-surface);
}

.item-details {
  flex: 1;
  margin-right: var(--spacing-md);
}

.item-actions {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: var(--spacing-sm);
}

.item-name {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.item-description {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-sm);
  line-height: 1.5;
}

.item-added {
  color: var(--color-text-muted);
  font-size: 0.875rem;
  margin: 0;
}

.item-price .price {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--color-text);
}

.btn-sm {
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
  border: 1px solid #dc3545;
}

.btn-danger:hover {
  background-color: #c82333;
  border-color: #bd2130;
}

/* Empty Cart */
.empty-cart {
  text-align: center;
  padding: var(--spacing-2xl);
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: var(--spacing-lg);
}

.empty-cart h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.empty-cart p {
  color: var(--color-text-muted);
  margin: 0;
}

/* Error State */
.error-state {
  text-align: center;
  padding: var(--spacing-2xl);
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
}

.error-icon {
  font-size: 4rem;
  margin-bottom: var(--spacing-lg);
}

.error-state h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.error-state p {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
}

/* Cart Actions */
.cart-actions {
  display: flex;
  justify-content: space-between;
  gap: var(--spacing-md);
  padding: var(--spacing-xl);
  border-top: 1px solid var(--color-border);
  background: var(--color-surface);
}

.cart-actions .btn {
  flex: 1;
  max-width: 200px;
}

/* Responsive */
@media (max-width: 768px) {
  .cart-header {
    flex-direction: column;
    gap: var(--spacing-md);
  }

  .cart-item {
    flex-direction: column;
    gap: var(--spacing-md);
  }

  .cart-actions {
    flex-direction: column;
  }

  .cart-actions .btn {
    max-width: none;
  }
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

.modal {
  background: white;
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 800px;
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

.modal-body {
  padding: var(--spacing-lg);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: var(--spacing-md);
  padding: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

/* Checkout Styles */
.checkout-summary {
  margin-bottom: var(--spacing-xl);
  padding: var(--spacing-lg);
  background: var(--color-surface);
  border-radius: var(--radius-md);
}

.checkout-summary h4 {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.summary-items {
  margin-bottom: var(--spacing-md);
}

.summary-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-sm) 0;
  border-bottom: 1px solid var(--color-border);
}

.summary-item:last-child {
  border-bottom: none;
}

.summary-total {
  padding-top: var(--spacing-md);
  border-top: 2px solid var(--color-border);
  font-size: 1.125rem;
}

.payment-section h4 {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.loading-cards {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md);
  color: var(--color-text-muted);
}

.spinner-sm {
  width: 16px;
  height: 16px;
  border: 2px solid var(--color-border);
  border-top: 2px solid var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.credit-cards-list {
  margin-bottom: var(--spacing-md);
}

.credit-card-option,
.add-new-card {
  margin-bottom: var(--spacing-sm);
}

.card-radio {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: border-color 0.2s ease;
}

.card-radio:hover {
  border-color: var(--color-primary);
}

.card-radio input[type="radio"] {
  margin: 0;
}

.card-info {
  flex: 1;
}

.card-number {
  font-weight: 500;
  color: var(--color-text);
}

.card-name {
  font-size: 0.875rem;
  color: var(--color-text-muted);
}

.new-card-form {
  margin-top: var(--spacing-md);
  padding: var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  background: var(--color-surface);
}

.form-group {
  margin-bottom: var(--spacing-md);
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
</style>
