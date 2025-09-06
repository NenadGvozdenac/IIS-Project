<template>
    <div class="seat-selection">
        <div class="container">
            <!-- Header -->
            <div class="header-section">
                <button @click="goBack" class="back-btn">
                    < Back to Matches
                </button>
                <div class="match-info" v-if="selectedMatch">
                    <h1>{{ selectedMatch.name }}</h1>
                    <div class="match-details">
                        <span class="match-date">{{ formatMatchDate(selectedMatch.scheduledAt) }}</span>
                        <span class="match-venue">{{ selectedMatch.hall }}, {{ selectedMatch.city }}</span>
                    </div>
                </div>
            </div>

            <!-- Zone Selection -->
            <div class="zone-selection-section">
                <h2>Select a Zone</h2>
                <div v-if="loadingZones" class="loading-state">
                    <div class="spinner"></div>
                    <p>Loading zones...</p>
                </div>
                <div v-else class="zones-grid">
                    <div v-for="zone in zones" :key="zone.idZone" @click="selectZone(zone)"
                        :class="['zone-card', { 'selected': selectedZone?.idZone === zone.idZone }]">
                        <div class="zone-header">
                            <h3>{{ zone.name }}</h3>
                            <div class="zone-rank">Rank {{ zone.rank }}</div>
                        </div>
                        <div class="zone-details">
                            <div class="zone-capacity">
                                {{ zone.maximumCapacity }} seats
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Side Selection -->
            <div v-if="selectedZone" class="side-selection-section">
                <h2>Select a Side in {{ selectedZone.name }}</h2>
                <div class="sides-grid">
                    <div v-for="side in availableSides" :key="side" @click="selectSide(side)"
                        :class="['side-card', { 'selected': selectedSide === side }]">
                        <div class="side-header">
                            <h3>{{ side.charAt(0).toUpperCase() + side.slice(1) }} Side</h3>
                            <div class="side-icon">{{ getSideIcon(side) }}</div>
                        </div>
                        <div class="side-details">
                            <div class="side-info">{{ getSideSeatsCount(side) }} seats</div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Seat Selection -->
            <div v-if="selectedZone && selectedSide" class="seat-selection-section">
                <h2>Select a Seat in {{ selectedZone.name }} - {{ selectedSide.charAt(0).toUpperCase() + selectedSide.slice(1) }} Side</h2>

                <div v-if="loadingSeats" class="loading-state">
                    <div class="spinner"></div>
                    <p>Loading seats...</p>
                </div>

                <div v-else-if="groupedSeats && Object.keys(groupedSeats).length > 0" class="seats-container">
                    <div class="seats-legend">
                        <div class="legend-item">
                            <div class="seat-icon available"></div>
                            <span>Available</span>
                        </div>
                        <div class="legend-item">
                            <div class="seat-icon selected"></div>
                            <span>Selected</span>
                        </div>
                        <div class="legend-item">
                            <div class="seat-icon unavailable"></div>
                            <span>Unavailable</span>
                        </div>
                    </div>

                    <div class="stadium-view">
                        <div class="stage">COURT</div>
                        <div class="rows-container">
                            <div v-for="row in sortedRows" :key="row" class="seat-row">
                                <div class="row-label">Row {{ row }}</div>
                                <div class="seats-in-row">
                                    <div v-for="seat in groupedSeats[row]" :key="seat.idSeat" @click="selectSeat(seat)"
                                        :class="['seat', getSeatStatus(seat)]"
                                        :title="`Row ${seat.seatRow}, Seat ${seat.seatNumber}`">
                                        {{ seat.seatNumber }}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div v-else class="no-seats">
                    <p>No seats available for this zone.</p>
                </div>
            </div>

            <!-- Purchase Summary -->
            <div v-if="selectedSeat && currentOffer" class="purchase-summary">
                <div class="summary-card">
                    <h3>Purchase Summary</h3>
                    <div class="offer-details">
                        <div class="detail-row">
                            <span class="label">Match:</span>
                            <span class="value">{{ currentOffer.matchName }}</span>
                        </div>
                        <div class="detail-row">
                            <span class="label">Zone:</span>
                            <span class="value">{{ currentOffer.zoneName }}</span>
                        </div>
                        <div class="detail-row">
                            <span class="label">Seat:</span>
                            <span class="value">Row {{ currentOffer.seatRow }}, Seat {{ currentOffer.seatNumber
                                }}</span>
                        </div>
                        <div class="detail-row">
                            <span class="label">Type:</span>
                            <span class="value">{{ currentOffer.seatType }}</span>
                        </div>
                        <div class="detail-row total">
                            <span class="label">Total Price:</span>
                            <span class="value price">{{ currentOffer.price.toFixed(2) }} RSD</span>
                        </div>
                    </div>

                    <div class="purchase-actions">
                        <button @click="addToCart" :disabled="addingToCart" class="btn btn-primary btn-large">
                            <span v-if="addingToCart">Adding to Cart...</span>
                            <span v-else>Add to Cart</span>
                        </button>
                    </div>
                </div>
            </div>

            <!-- Error Messages -->
            <div v-if="error" class="error-message">
                <div class="error-content">
                    <i class="icon-error"></i>
                    <span>{{ error }}</span>
                    <button @click="clearError" class="error-close">&times;</button>
                </div>
            </div>

            <!-- Success Messages -->
            <div v-if="successMessage" class="success-message">
                <div class="success-content">
                    <i class="icon-success"></i>
                    <span>{{ successMessage }}</span>
                    <button @click="clearSuccess" class="success-close">&times;</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { ZoneService } from '../../services/zone_service.js';
import { SeatService } from '../../services/seat_service.js';
import { PurchaseOfferService } from '../../services/purchase_offer_service.js';
import { MatchService } from '../../services/match_service.js';
import { getUserData } from '../../services/auth_service.js';

export default {
    name: 'SeatSelection',
    data() {
        return {
            matchId: null,
            selectedMatch: null,
            zones: [],
            selectedZone: null,
            selectedSide: null,
            availableSides: ['north', 'east', 'south', 'west'],
            seats: [],
            groupedSeats: {},
            seatOffers: {}, // New: store offers for each seat
            selectedSeat: null,
            currentOffer: null,
            loadingZones: false,
            loadingSeats: false,
            addingToCart: false,
            error: null,
            successMessage: null
        };
    },
    computed: {
        sortedRows() {
            return Object.keys(this.groupedSeats).sort((a, b) => parseInt(a) - parseInt(b));
        }
    },
    async mounted() {
        this.matchId = this.$route.params.matchId;
        if (this.matchId) {
            await this.loadMatchDetails();
            // Only load zones if we have a valid match that hasn't passed
            if (this.selectedMatch && !this.error) {
                await this.loadZones();
            }
        }
    },
    methods: {
        async loadMatchDetails() {
            try {
                const matches = await MatchService.getMatchesInOurHall();
                if (matches.isSuccess) {
                    this.selectedMatch = matches.value.find(m => m.idMatch == this.matchId);
                    
                    // Check if match has already passed
                    if (this.selectedMatch && this.selectedMatch.scheduledAt) {
                        const matchDate = new Date(this.selectedMatch.scheduledAt);
                        const now = new Date();
                        
                        if (matchDate <= now) {
                            this.error = 'This match has already taken place. Ticket sales are no longer available.';
                            // Redirect back to dashboard after a delay
                            setTimeout(() => {
                                this.$router.push('/customer-dashboard');
                            }, 3000);
                            return;
                        }
                    }
                    
                    if (!this.selectedMatch) {
                        this.error = 'Match not found.';
                        setTimeout(() => {
                            this.$router.push('/customer-dashboard');
                        }, 2000);
                    }
                }
            } catch (error) {
                this.error = 'Failed to load match details';
            }
        },

        async loadZones() {
            this.loadingZones = true;
            try {
                const response = await ZoneService.getAllZones();
                debugger
                if (response.isSuccess) {
                    this.zones = response.value.zones;
                }
            } catch (error) {
                this.error = 'Failed to load zones';
            } finally {
                this.loadingZones = false;
            }
        },

        async selectZone(zone) {
            this.selectedZone = zone;
            this.selectedSide = null; // Reset side selection
            this.selectedSeat = null;
            this.currentOffer = null;
            this.seatOffers = {}; // Reset seat offers
            // Note: Don't load seats yet, wait for side selection
        },

        selectSide(side) {
            this.selectedSide = side;
            this.selectedSeat = null;
            this.currentOffer = null;
            this.seatOffers = {}; // Reset seat offers
            this.loadSeats(); // Load seats for the selected side
        },

        async loadSeats() {
            if (!this.selectedZone || !this.selectedSide) return;
            
            this.loadingSeats = true;
            try {
                const response = await SeatService.getSeatsByZone(this.selectedZone.idZone);
                if (response.isSuccess) {
                    // Filter seats by the selected side using the correct field name
                    console.log('Selected side:', this.selectedSide);
                    console.log('Total seats from API:', response.value.length);
                    console.log('Sample seat data:', response.value[0]);
                    
                    this.seats = response.value.filter(seat => {
                        console.log(`Seat ${seat.seatRow}-${seat.seatNumber}: direction="${seat.seatDirection}", selectedSide="${this.selectedSide}", match=${seat.seatDirection === this.selectedSide}`);
                        return seat.seatDirection === this.selectedSide;
                    });
                    
                    console.log('Filtered seats count:', this.seats.length);
                    this.groupSeats();
                    await this.checkSeatOffers();
                }
            } catch (error) {
                this.error = 'Failed to load seats';
            } finally {
                this.loadingSeats = false;
            }
        },

        async checkSeatOffers() {
            // Reset seat offers
            this.seatOffers = {};
            
            // Check offers for each seat
            const offerPromises = this.seats.map(async (seat) => {
                try {
                    const response = await PurchaseOfferService.getIndividualTicketOffer(
                        this.selectedZone.idZone,
                        seat.seatRow,
                        seat.seatNumber,
                        this.selectedSide,
                        this.matchId
                    );
                    
                    if (response.isSuccess) {
                        this.seatOffers[seat.idSeat] = response.value;
                    }
                } catch (error) {
                    // If there's no offer for this seat, it will remain unavailable
                    console.log(`No offer available for seat ${seat.seatRow}-${seat.seatNumber} on ${this.selectedSide} side`);
                }
            });
            
            await Promise.all(offerPromises);
        },

        groupSeats() {
            this.groupedSeats = {};
            this.seats.forEach(seat => {
                const row = seat.seatRow;
                if (!this.groupedSeats[row]) {
                    this.groupedSeats[row] = [];
                }
                this.groupedSeats[row].push(seat);
            });

            // Sort seats in each row by seat number
            Object.keys(this.groupedSeats).forEach(row => {
                this.groupedSeats[row].sort((a, b) => parseInt(a.seatNumber) - parseInt(b.seatNumber));
            });
        },

        async selectSeat(seat) {
            // Check if seat has an available offer
            if (!this.seatOffers[seat.idSeat]) {
                return; // No offer available, can't select
            }

            this.selectedSeat = seat;
            this.currentOffer = this.seatOffers[seat.idSeat];
        },

        async addToCart() {
            if (!this.currentOffer) return;

            this.addingToCart = true;
            try {
                const userData = getUserData();
                const response = await PurchaseOfferService.addToCart(
                    this.currentOffer.idPurchaseOffer,
                    userData.userID
                );

                if (response.isSuccess || response.code === 200) {
                    this.successMessage = 'Ticket added to cart successfully!';
                    // Reset selection after adding to cart
                    this.selectedSeat = null;
                    this.currentOffer = null;

                    // Optionally redirect to cart after a delay
                    setTimeout(() => {
                        this.$router.push('/cart');
                    }, 500);
                }
            } catch (error) {
                this.error = error.error || 'Failed to add ticket to cart';
            } finally {
                this.addingToCart = false;
            }
        },

        getSeatStatus(seat) {
            if (this.selectedSeat && this.selectedSeat.idSeat === seat.idSeat) {
                return 'selected';
            }
            
            // Check if there's an available offer for this seat
            const hasOffer = this.seatOffers[seat.idSeat];
            if (hasOffer && seat.seatStatus === 'enabled') {
                return 'available';
            }
            
            return 'unavailable';
        },

        formatMatchDate(dateString) {
            const date = new Date(dateString);
            return date.toLocaleDateString('en-US', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
        },

        goBack() {
            this.$router.push('/customer-dashboard');
        },

        clearError() {
            this.error = null;
        },

        clearSuccess() {
            this.successMessage = null;
        },

        getSideIcon(side) {
            switch (side) {
                case 'north': return '⬆️';
                case 'east': return '➡️';
                case 'south': return '⬇️';
                case 'west': return '⬅️';
                default: return '📍';
            }
        },

        getSideSeatsCount(side) {
            if (!this.selectedZone) return 0;
            
            // Calculate expected seats per side based on zone capacity
            const totalCapacity = this.selectedZone.maximumCapacity;
            const seatsPerSide = totalCapacity / 4;
            return Math.floor(seatsPerSide);
        }
    }
};
</script>

<style scoped>
.seat-selection {
    padding: var(--spacing-xl) 0;
    min-height: calc(100vh - 200px);
    background-color: var(--color-surface);
}

/* Header Section */
.header-section {
    background: white;
    border-radius: var(--radius-lg);
    padding: var(--spacing-xl);
    margin-bottom: var(--spacing-xl);
    box-shadow: var(--shadow-sm);
    border: 1px solid var(--color-border);
}

.back-btn {
    background: var(--color-secondary);
    color: white;
    border: none;
    padding: var(--spacing-sm) var(--spacing-md);
    border-radius: var(--radius-md);
    cursor: pointer;
    margin-bottom: var(--spacing-md);
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    font-size: 0.875rem;
    font-weight: 500;
    transition: all 0.2s ease;
}

.back-btn:hover {
    background: var(--color-text);
}

.match-info h1 {
    color: var(--color-text);
    margin-bottom: var(--spacing-sm);
    font-size: 2rem;
    font-weight: 600;
}

.match-details {
    display: flex;
    gap: var(--spacing-xl);
    color: var(--color-text-light);
    font-size: 0.875rem;
}

/* Zone and Seat Selection Sections */
.zone-selection-section,
.seat-selection-section {
    background: white;
    border-radius: var(--radius-lg);
    padding: var(--spacing-xl);
    margin-bottom: var(--spacing-xl);
    box-shadow: var(--shadow-sm);
    border: 1px solid var(--color-border);
}

.zone-selection-section h2,
.seat-selection-section h2 {
    color: var(--color-text);
    margin-bottom: var(--spacing-lg);
    font-size: 1.5rem;
    font-weight: 600;
}

/* Zones Grid */
.zones-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: var(--spacing-md);
    margin-top: var(--spacing-md);
}

.zone-card {
    border: 1px solid var(--color-border);
    border-radius: var(--radius-md);
    padding: var(--spacing-lg);
    cursor: pointer;
    transition: all 0.2s ease;
    background: white;
}

.zone-card:hover {
    border-color: var(--color-primary);
    box-shadow: var(--shadow-md);
    transform: translateY(-1px);
}

.zone-card.selected {
    border-color: var(--color-primary);
    background: #f8faff;
    box-shadow: var(--shadow-md);
}

.zone-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: var(--spacing-md);
}

.zone-header h3 {
    margin: 0;
    color: var(--color-text);
    font-size: 1.125rem;
    font-weight: 600;
}

.zone-rank {
    background: var(--color-primary);
    color: white;
    padding: 0.25rem 0.5rem;
    border-radius: var(--radius-sm);
    font-size: 0.75rem;
    font-weight: 600;
}

.zone-capacity {
    color: var(--color-text-light);
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    font-size: 0.875rem;
}

/* Side Selection Styles */
.side-selection-section {
    background: white;
    border-radius: var(--radius-lg);
    padding: var(--spacing-xl);
    margin-bottom: var(--spacing-xl);
    box-shadow: var(--shadow-sm);
    border: 1px solid var(--color-border);
}

.side-selection-section h2 {
    color: var(--color-text);
    margin-bottom: var(--spacing-lg);
    font-size: 1.5rem;
    font-weight: 600;
}

.sides-grid {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: var(--spacing-md);
    max-width: 1000px;
    margin: 0 auto;
}

.side-card {
    border: 1px solid var(--color-border);
    border-radius: var(--radius-md);
    padding: var(--spacing-lg);
    cursor: pointer;
    transition: all 0.2s ease;
    background: white;
    text-align: center;
}

.side-card:hover {
    border-color: var(--color-primary);
    box-shadow: var(--shadow-md);
    transform: translateY(-1px);
}

.side-card.selected {
    border-color: var(--color-primary);
    background: #f8faff;
    box-shadow: var(--shadow-md);
}

.side-header {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: var(--spacing-sm);
    margin-bottom: var(--spacing-md);
}

.side-header h3 {
    margin: 0;
    color: var(--color-text);
    font-size: 1.125rem;
    font-weight: 600;
}

.side-icon {
    font-size: 2rem;
    margin-bottom: var(--spacing-sm);
}

.side-details {
    color: var(--color-text-light);
    font-size: 0.875rem;
}

.side-info {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: var(--spacing-sm);
}

/* Seats Legend */
.seats-legend {
    display: flex;
    gap: var(--spacing-xl);
    margin-bottom: var(--spacing-xl);
    justify-content: center;
    padding: var(--spacing-md);
    background: var(--color-surface);
    border-radius: var(--radius-md);
    border: 1px solid var(--color-border);
}

.legend-item {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    font-size: 0.875rem;
    color: var(--color-text);
}

.seat-icon {
    width: 20px;
    height: 20px;
    border-radius: var(--radius-sm);
    border: 1px solid var(--color-border);
}

.seat-icon.available {
    background: var(--color-success);
    border-color: var(--color-success);
}

.seat-icon.selected {
    background: var(--color-primary);
    border-color: var(--color-primary);
}

.seat-icon.unavailable {
    background: var(--color-error);
    border-color: var(--color-error);
}

/* Stadium View */
.stadium-view {
    max-width: 800px;
    margin: 0 auto;
}

.stage {
    background: var(--color-text);
    color: white;
    text-align: center;
    padding: var(--spacing-md);
    margin-bottom: var(--spacing-xl);
    border-radius: var(--radius-md);
    font-weight: 600;
    font-size: 1.125rem;
}

.seat-row {
    display: flex;
    align-items: center;
    margin-bottom: var(--spacing-sm);
}

.row-label {
    width: 60px;
    text-align: right;
    margin-right: var(--spacing-md);
    font-weight: 600;
    color: var(--color-text-light);
    font-size: 0.875rem;
}

.seats-in-row {
    display: flex;
    gap: 0.25rem;
    flex: 1;
    justify-content: center;
}

.seat {
    width: 30px;
    height: 30px;
    border: 1px solid var(--color-border);
    border-radius: var(--radius-sm);
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    font-size: 0.75rem;
    font-weight: 600;
    transition: all 0.2s ease;
    color: white;
}

.seat.available {
    background: var(--color-success);
    border-color: var(--color-success);
}

.seat.available:hover {
    background: #059669;
    border-color: #059669;
    transform: scale(1.1);
}

.seat.selected {
    background: var(--color-primary);
    border-color: var(--color-primary);
    transform: scale(1.1);
    box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.3);
}

.seat.unavailable {
    background: var(--color-error);
    border-color: var(--color-error);
    cursor: not-allowed;
    opacity: 0.6;
}

/* Purchase Summary */
.purchase-summary {
    position: fixed;
    bottom: var(--spacing-xl);
    right: var(--spacing-xl);
    z-index: 1000;
}

.summary-card {
    background: white;
    border-radius: var(--radius-lg);
    padding: var(--spacing-xl);
    box-shadow: var(--shadow-lg);
    border: 1px solid var(--color-border);
    min-width: 320px;
}

.summary-card h3 {
    margin-top: 0;
    color: var(--color-text);
    font-size: 1.25rem;
    font-weight: 600;
    margin-bottom: var(--spacing-md);
}

.detail-row {
    display: flex;
    justify-content: space-between;
    margin-bottom: var(--spacing-sm);
    font-size: 0.875rem;
}

.detail-row .label {
    color: var(--color-text-light);
}

.detail-row .value {
    color: var(--color-text);
    font-weight: 500;
}

.detail-row.total {
    border-top: 1px solid var(--color-border);
    padding-top: var(--spacing-sm);
    margin-top: var(--spacing-md);
    font-weight: 600;
}

.detail-row.total .value.price {
    color: var(--color-success);
    font-size: 1.125rem;
}

/* Loading State */
.loading-state {
    text-align: center;
    padding: var(--spacing-2xl);
    color: var(--color-text-light);
}

.spinner {
    border: 3px solid var(--color-border);
    border-top: 3px solid var(--color-primary);
    border-radius: 50%;
    width: 40px;
    height: 40px;
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

/* Messages */
.error-message,
.success-message {
    position: fixed;
    top: var(--spacing-xl);
    right: var(--spacing-xl);
    z-index: 1001;
    max-width: 400px;
}

.error-content,
.success-content {
    padding: var(--spacing-md);
    border-radius: var(--radius-md);
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    box-shadow: var(--shadow-lg);
    font-size: 0.875rem;
}

.error-content {
    background: #fef2f2;
    color: #991b1b;
    border: 1px solid #fecaca;
}

.success-content {
    background: #f0fdf4;
    color: #166534;
    border: 1px solid #bbf7d0;
}

.error-close,
.success-close {
    background: none;
    border: none;
    font-size: 1.25rem;
    cursor: pointer;
    margin-left: auto;
    color: inherit;
    opacity: 0.7;
}

.error-close:hover,
.success-close:hover {
    opacity: 1;
}

.no-seats {
    text-align: center;
    padding: var(--spacing-2xl);
    color: var(--color-text-light);
}

/* Responsive Design */
@media (max-width: 768px) {
    .seat-selection {
        padding: var(--spacing-md) 0;
    }

    .container {
        padding: 0 var(--spacing-sm);
    }

    .purchase-summary {
        position: static;
        margin-top: var(--spacing-xl);
    }

    .summary-card {
        min-width: auto;
    }

    .match-details {
        flex-direction: column;
        gap: var(--spacing-sm);
    }

    .zones-grid {
        grid-template-columns: 1fr;
    }

    .sides-grid {
        grid-template-columns: repeat(2, 1fr);
    }

    .seats-legend {
        flex-direction: column;
        gap: var(--spacing-md);
        align-items: center;
    }

    .stadium-view {
        max-width: 100%;
        overflow-x: auto;
    }

    .header-section h1 {
        font-size: 1.5rem;
    }
}

@media (max-width: 480px) {
    .seat {
        width: 24px;
        height: 24px;
        font-size: 0.625rem;
    }

    .row-label {
        width: 50px;
        font-size: 0.75rem;
    }

    .zone-card {
        padding: var(--spacing-md);
    }

    .sides-grid {
        grid-template-columns: 1fr;
        gap: var(--spacing-sm);
    }

    .summary-card {
        padding: var(--spacing-md);
    }
}
</style>
