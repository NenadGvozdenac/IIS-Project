<template>
  <div class="my-tickets">
    <div class="container">
      <div class="page-header">
        <h1 class="page-title">My Tickets</h1>
        <div class="breadcrumb">
          <router-link to="/customer-dashboard" class="breadcrumb-link">Dashboard</router-link>
          <span class="breadcrumb-separator">›</span>
          <span class="breadcrumb-current">My Tickets</span>
        </div>
      </div>

      <div class="tickets-section">
        <div class="section-header">
          <h2>Purchase History</h2>
          <div class="history-stats" v-if="purchaseHistory.length > 0">
            {{ purchaseHistory.length }} purchase{{ purchaseHistory.length !== 1 ? 's' : '' }}
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loadingHistory" class="loading-state">
          <div class="spinner"></div>
          <p>Loading your tickets...</p>
        </div>

        <!-- Purchase History List -->
        <div v-else-if="purchaseHistory.length > 0" class="history-list">
          <div v-for="purchase in purchaseHistory" :key="purchase.idPurchaseOffer" class="history-item">
            <div class="history-header">
              <div class="ticket-type-badge" :class="getTicketTypeClass(purchase.type)">
                {{ purchase.type }}
              </div>
              <div class="purchase-date">
                {{ formatDate(purchase.purchaseDate) }}
              </div>
            </div>
            
            <div class="history-content">
              <div class="ticket-info">
                <h3 class="ticket-name">{{ purchase.name }}</h3>
                <p class="ticket-description">{{ purchase.description }}</p>
                
                <div class="ticket-details">
                  <div class="detail-row">
                    <span class="label">Seat:</span>
                    <span class="value">{{ purchase.zoneName }} - Row {{ purchase.seatRow }}, Seat {{ purchase.seatNumber }}</span>
                  </div>
                  <div class="detail-row" v-if="purchase.matchName">
                    <span class="label">Match:</span>
                    <span class="value">{{ purchase.matchName }}</span>
                  </div>
                  <div class="detail-row" v-if="purchase.seasonName">
                    <span class="label">Season:</span>
                    <span class="value">{{ purchase.seasonName }}</span>
                  </div>
                  <div class="detail-row">
                    <span class="label">Valid until:</span>
                    <span class="value">{{ formatDate(purchase.expiresAt) }}</span>
                  </div>
                </div>
              </div>
              
              <div class="ticket-actions">
                <div class="ticket-price">
                  {{ formatPrice(purchase.price) }} RSD
                </div>
                <button @click="printTicket(purchase)" class="btn btn-primary btn-sm">
                  Print Ticket
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty History -->
        <div v-else class="empty-state">
          <div class="empty-icon">🎫</div>
          <h3>No Tickets Found</h3>
          <p>You haven't purchased any tickets yet.</p>
          <router-link to="/customer-dashboard" class="btn btn-primary">
            Browse Matches
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { MatchService } from '../../services/match_service.js';

// Reactive data
const purchaseHistory = ref([]);
const loadingHistory = ref(false);

// Methods
const loadPurchaseHistory = async () => {
  try {
    loadingHistory.value = true;
    const response = await MatchService.getPurchaseHistory();
    purchaseHistory.value = response.value || response || [];
  } catch (error) {
    console.error('Error loading purchase history:', error);
    purchaseHistory.value = [];
  } finally {
    loadingHistory.value = false;
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

const getTicketTypeClass = (type) => {
  switch (type) {
    case 'season ticket':
      return 'season-ticket';
    case 'individual ticket':
      return 'individual-ticket';
    default:
      return 'default-ticket';
  }
};

const printTicket = (ticket) => {
  // TODO: Implement ticket printing functionality
  alert(`Printing ticket: ${ticket.name}`);
};

// Lifecycle
onMounted(() => {
  loadPurchaseHistory();
});
</script>

<style scoped>
.my-tickets {
  padding: var(--spacing-xl) 0;
  min-height: calc(100vh - 200px);
}

.page-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.breadcrumb {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
  font-size: 0.875rem;
  color: var(--color-text-muted);
}

.breadcrumb-link {
  color: var(--color-primary);
  text-decoration: none;
  font-weight: 500;
}

.breadcrumb-link:hover {
  text-decoration: underline;
}

.breadcrumb-separator {
  color: var(--color-text-muted);
}

.breadcrumb-current {
  font-weight: 500;
}

.tickets-section {
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  padding: var(--spacing-2xl);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
  padding-bottom: var(--spacing-lg);
  border-bottom: 2px solid var(--color-border);
}

.section-header h2 {
  font-size: 1.875rem;
  font-weight: 600;
  color: var(--color-text);
  margin: 0;
}

.history-stats {
  color: var(--color-text-muted);
  font-size: 0.875rem;
  font-weight: 500;
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--color-background);
  border-radius: var(--radius-md);
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

/* Purchase History Styles */
.history-list {
  display: grid;
  gap: var(--spacing-xl);
}

.history-item {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
  border-radius: var(--radius-lg);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07), 0 1px 3px rgba(0, 0, 0, 0.06);
  padding: var(--spacing-xl);
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 0, 0, 0.05);
  position: relative;
  overflow: hidden;
}

.history-item::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--color-primary), var(--color-primary-hover));
}

.history-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1), 0 4px 10px rgba(0, 0, 0, 0.06);
}

.history-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-lg);
  padding-bottom: var(--spacing-md);
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
}

.ticket-type-badge {
  padding: 0.5rem 1rem;
  border-radius: 50px;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.ticket-type-badge.season-ticket {
  background: linear-gradient(135deg, #e8f5e8, #c8e6c9);
  color: #1b5e20;
  border: 1px solid #4caf50;
}

.ticket-type-badge.individual-ticket {
  background: linear-gradient(135deg, #e3f2fd, #bbdefb);
  color: #0d47a1;
  border: 1px solid #2196f3;
}

.ticket-type-badge.default-ticket {
  background: linear-gradient(135deg, #f5f5f5, #e0e0e0);
  color: #616161;
  border: 1px solid #9e9e9e;
}

.purchase-date {
  font-weight: 600;
  color: var(--color-text-muted);
  font-size: 0.9rem;
}

.history-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--spacing-lg);
}

.ticket-info {
  flex: 1;
}

.ticket-name {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
  line-height: 1.3;
}

.ticket-description {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
  line-height: 1.5;
  font-size: 0.95rem;
}

.ticket-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-md);
}

.detail-row {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
  padding: var(--spacing-md);
  background: rgba(59, 130, 246, 0.05);
  border-radius: var(--radius-md);
  border-left: 3px solid var(--color-primary);
}

.detail-row .label {
  font-size: 0.8rem;
  font-weight: 600;
  color: var(--color-text-muted);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.detail-row .value {
  font-weight: 600;
  color: var(--color-text);
  font-size: 0.95rem;
}

.ticket-actions {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: var(--spacing-md);
  min-width: 150px;
}

.ticket-price {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-primary);
  text-align: right;
  padding: var(--spacing-md);
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.1), rgba(59, 130, 246, 0.05));
  border-radius: var(--radius-lg);
  border: 2px solid rgba(59, 130, 246, 0.2);
  min-width: 120px;
}

.ticket-actions .btn {
  background: linear-gradient(135deg, var(--color-primary), var(--color-primary-hover));
  color: white;
  border: none;
  padding: var(--spacing-md) var(--spacing-lg);
  border-radius: 50px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3);
  font-size: 0.85rem;
}

.ticket-actions .btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(59, 130, 246, 0.4);
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: var(--spacing-2xl);
  background: linear-gradient(135deg, #f8f9fa, #e9ecef);
  border-radius: var(--radius-lg);
  border: 2px dashed var(--color-border);
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: var(--spacing-lg);
}

.empty-state h3 {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text);
  margin-bottom: var(--spacing-md);
}

.empty-state p {
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-lg);
  font-size: 1.125rem;
}

.empty-state .btn {
  padding: var(--spacing-md) var(--spacing-xl);
  font-weight: 600;
  text-decoration: none;
  display: inline-block;
}

/* Responsive */
@media (max-width: 768px) {
  .page-title {
    font-size: 2rem;
  }
  
  .section-header {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--spacing-md);
  }
  
  .history-header {
    flex-direction: column;
    gap: var(--spacing-sm);
  }
  
  .history-content {
    flex-direction: column;
    gap: var(--spacing-lg);
  }
  
  .ticket-actions {
    align-items: stretch;
    width: 100%;
  }
  
  .ticket-price {
    text-align: center;
    min-width: auto;
  }
  
  .ticket-details {
    grid-template-columns: 1fr;
  }
  
  .breadcrumb {
    flex-wrap: wrap;
  }
}
</style>
