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
            <h2>Cart #{{ cart.idCart }}</h2>
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
            <div class="item-price">
              <span class="price">{{ formatPrice(item.price) }} RSD</span>
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
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { CartService } from '../../services/cart_service.js';

const router = useRouter();

// Reactive data
const cart = ref(null);
const loading = ref(false);

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
  router.push('/customer-dashboard');
};

const proceedToCheckout = () => {
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
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
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
</style>
