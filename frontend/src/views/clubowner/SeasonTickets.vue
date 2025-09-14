<template>
    <div class="season-tickets">
        <h1>Season Tickets Management</h1>

        <div v-if="loading" class="loading">
            Loading seasons...
        </div>

        <div v-else-if="error" class="error">
            {{ error }}
        </div>

        <div v-else-if="seasons.length === 0" class="no-seasons">
            No seasons found.
        </div>

        <div v-else class="seasons-list">
            <div v-for="season in seasons" :key="season.idSeason" class="season-card">
                <div class="season-info">
                    <h3>{{ season.name }}</h3>
                    <p class="season-period">{{ formatDateRange(season.startedAt, season.endedAt) }}</p>
                    <p class="season-status" :class="getSeasonStatusClass(season)">
                        {{ getSeasonStatusText(season) }}
                    </p>
                </div>

                <div class="season-actions">
                    <button v-if="!season.ticketsForSale" 
                            :disabled="seasonTicketsEnabled(season)"
                            @click="openPricingModal(season)" 
                            class="enable-tickets-btn">
                        <span v-if="enableLoading === season.idSeason">Enabling...</span>
                        <span v-else>Enable Season Tickets</span>
                    </button>

                    <div v-else class="tickets-enabled">
                        <span class="enabled-badge">Season Tickets Enabled</span>
                        <p class="enabled-date">{{ formatDate(season.ticketsWentOnSale) }}</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Zone Pricing Modal -->
        <div v-if="showPricingModal" class="modal-overlay" @click="closePricingModal">
            <div class="modal-content" @click.stop>
                <div class="modal-header">
                    <h3>Set Season Ticket Prices</h3>
                    <div class="header-actions">
                        <button @click="setDefaultPrices" class="default-btn" :disabled="!canEnableTickets || enableLoading">Set Default Prices</button>
                        <button @click="closePricingModal" class="close-btn">&times;</button>
                    </div>
                </div>

                <div class="modal-body">
                    <p class="season-title">{{ selectedSeason?.name }}</p>
                    <p class="season-period">{{ formatDateRange(selectedSeason?.startedAt, selectedSeason?.endedAt) }}</p>

                    <div v-if="zonesLoading" class="loading">
                        Loading zones...
                    </div>

                    <div v-else-if="zones.length > 0" class="zones-list">
                        <div v-for="zone in zones" :key="zone.idZone" class="zone-pricing">
                            <h4>{{ zone.name }}</h4>
                            <div class="pricing-input">
                                <label>Season Ticket Price:</label>
                                <input type="number" 
                                       step="100" 
                                       min="0" 
                                       v-model.number="zonePrices[zone.idZone]"
                                       placeholder="e.g., 15000" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button @click="closePricingModal" class="cancel-btn">Cancel</button>
                    <button @click="enableSeasonTickets" 
                            :disabled="!canEnableTickets || enableLoading"
                            class="enable-btn">
                        <span v-if="enableLoading">Processing...</span>
                        <span v-else>Enable Season Tickets</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue'
import { SeasonService } from '../../services/ticket_service/season_service'
import { ZoneService } from '../../services/ticket_service/zone_service'

export default {
    name: 'SeasonTickets',
    setup() {
        const seasons = ref([])
        const zones = ref([])
        const loading = ref(true)
        const zonesLoading = ref(false)
        const enableLoading = ref(null)
        const error = ref('')
        const showPricingModal = ref(false)
        const selectedSeason = ref(null)

        // Zone prices (in cents)
        const zonePrices = ref({})

        const formatDate = (dateString) => {
            if (!dateString) return ''
            const date = new Date(dateString)
            return date.toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'long',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            })
        }

        const formatDateRange = (startDate, endDate) => {
            const start = new Date(startDate).toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            })
            
            if (!endDate) {
                return `Start: ${start}`
            }
            
            const end = new Date(endDate).toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            })
            
            return `${start} - ${end}`
        }

        const getSeasonStatusClass = (season) => {
            const today = new Date()
            const startDate = new Date(season.startedAt)
            const endDate = season.endedAt ? new Date(season.endedAt) : null

            if (endDate && today > endDate) {
                return 'finished'
            } else if (today >= startDate) {
                return 'ongoing'
            } else {
                return 'upcoming'
            }
        }

        const seasonTicketsEnabled = (season) => {
            return season.ticketsForSale || enableLoading.value === season.idSeason;
        }

        const getSeasonStatusText = (season) => {
            const today = new Date()
            const startDate = new Date(season.startedAt)
            const endDate = season.endedAt ? new Date(season.endedAt) : null

            if (endDate && today > endDate) {
                return 'Finished'
            } else if (today >= startDate) {
                return 'Ongoing'
            } else {
                return 'Upcoming'
            }
        }

        const loadSeasons = async () => {
            try {
                loading.value = true
                error.value = ''
                const response = await SeasonService.getAllSeasons()

                if (response.isSuccess && response.value) {
                    // Sort seasons by start date (newest first)
                    seasons.value = response.value.sort((a, b) => {
                        return new Date(b.startedAt) - new Date(a.startedAt)
                    })
                } else {
                    seasons.value = []
                    error.value = response.error || 'Failed to load seasons'
                }
            } catch (err) {
                error.value = 'Failed to load seasons: ' + (err.message || err)
            } finally {
                loading.value = false
            }
        }

        const loadZones = async () => {
            try {
                zonesLoading.value = true
                const response = await ZoneService.getAllZones()
                zones.value = response.value.zones

                // Initialize zone prices
                zonePrices.value = {}
                zones.value.forEach(zone => {
                    zonePrices.value[zone.idZone] = null
                })

            } catch (err) {
                error.value = 'Failed to load zones: ' + (err.message || err)
                zones.value = []
                zonePrices.value = {}
            } finally {
                zonesLoading.value = false
            }
        }

        const openPricingModal = async (season) => {
            selectedSeason.value = season
            showPricingModal.value = true

            // Clear pricing parameters immediately
            zonePrices.value = {}

            await loadZones()
        }

        const closePricingModal = () => {
            showPricingModal.value = false
            selectedSeason.value = null
            zonePrices.value = {}
        }

        const setDefaultPrices = () => {
            zones.value.forEach(zone => {
                // Set different default prices based on zone rank or name
                let defaultPrice = 15000 // 150.00 EUR default
                
                if (zone.name.toLowerCase().includes('vip') || zone.rank === 1) {
                    defaultPrice = 25000 // 250.00 EUR for VIP
                } else if (zone.name.toLowerCase().includes('premium') || zone.rank === 2) {
                    defaultPrice = 18000 // 180.00 EUR for Premium
                } else if (zone.rank >= 3) {
                    defaultPrice = 10000 // 100.00 EUR for standard zones
                }
                
                zonePrices.value[zone.idZone] = defaultPrice
            })
        }

        const canEnableTickets = computed(() => {
            if (!zones.value.length) return false

            return zones.value.every(zone => {
                const price = zonePrices.value[zone.idZone]
                return typeof price === 'number' && price > 0
            })
        })

        const enableSeasonTickets = async () => {
            if (!selectedSeason.value || !canEnableTickets.value) return

            try {
                enableLoading.value = selectedSeason.value.idSeason

                // Prepare zone prices in the format expected by the API
                const zonePricesArray = zones.value.map(zone => ({
                    zoneId: zone.idZone,
                    price: zonePrices.value[zone.idZone]
                }))

                // Enable season tickets
                await SeasonService.enableSeasonTickets(selectedSeason.value.idSeason, zonePricesArray)

                // Update the season in our local data
                const seasonIndex = seasons.value.findIndex(s => s.idSeason === selectedSeason.value.idSeason)
                if (seasonIndex !== -1) {
                    seasons.value[seasonIndex].ticketsForSale = true
                    seasons.value[seasonIndex].ticketsWentOnSale = new Date().toISOString()
                }

                // Close modal and show success
                closePricingModal()
                alert('Season tickets have been successfully enabled!')

            } catch (err) {
                error.value = 'Failed to enable season tickets: ' + (err.message || err)
            } finally {
                enableLoading.value = null
            }
        }

        onMounted(() => {
            loadSeasons()
        })

        return {
            seasons,
            zones,
            loading,
            zonesLoading,
            enableLoading,
            error,
            showPricingModal,
            selectedSeason,
            zonePrices,
            canEnableTickets,
            formatDate,
            formatDateRange,
            getSeasonStatusClass,
            getSeasonStatusText,
            openPricingModal,
            closePricingModal,
            setDefaultPrices,
            seasonTicketsEnabled,
            enableSeasonTickets
        }
    }
}
</script>

<style scoped>
.season-tickets {
    padding: 20px;
    max-width: 1200px;
    margin: 0 auto;
}

h1 {
    color: #333;
    margin-bottom: 30px;
    text-align: center;
}

.loading {
    text-align: center;
    padding: 40px;
    color: #666;
}

.error {
    color: #dc3545;
    text-align: center;
    padding: 20px;
    background-color: #f8d7da;
    border: 1px solid #f5c6cb;
    border-radius: 5px;
    margin-bottom: 20px;
}

.no-seasons {
    text-align: center;
    padding: 40px;
    color: #666;
    font-style: italic;
}

.seasons-list {
    display: grid;
    gap: 20px;
}

.season-card {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px;
    border: 1px solid #ddd;
    border-radius: 8px;
    background-color: #fff;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    transition: box-shadow 0.3s ease;
}

.season-card:hover {
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.season-info h3 {
    margin: 0 0 10px 0;
    color: #333;
    font-size: 1.3em;
}

.season-info p {
    margin: 5px 0;
    color: #666;
}

.season-period {
    font-weight: bold;
    color: #007bff;
}

.season-status {
    font-weight: bold;
    padding: 4px 8px;
    border-radius: 4px;
    display: inline-block;
}

.season-status.upcoming {
    background-color: #fff3cd;
    color: #856404;
}

.season-status.ongoing {
    background-color: #d4edda;
    color: #155724;
}

.season-status.finished {
    background-color: #d1ecf1;
    color: #0c5460;
}

.season-actions {
    display: flex;
    align-items: center;
    gap: 15px;
}

.enable-tickets-btn {
    padding: 10px 20px;
    background-color: #28a745;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-weight: bold;
    transition: background-color 0.3s ease;
}

.enable-tickets-btn:hover:not(:disabled) {
    background-color: #218838;
}

.enable-tickets-btn:disabled {
    background-color: #6c757d;
    cursor: not-allowed;
}

.tickets-enabled {
    text-align: right;
}

.enabled-badge {
    background-color: #17a2b8;
    color: white;
    padding: 8px 16px;
    border-radius: 4px;
    font-weight: bold;
    display: block;
    margin-bottom: 5px;
}

.enabled-date {
    font-size: 0.9em;
    color: #666;
    margin: 0;
}

/* Modal Styles */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

.modal-content {
    background-color: white;
    padding: 0;
    border-radius: 8px;
    width: 90%;
    max-width: 700px;
    max-height: 90%;
    overflow-y: auto;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px;
    border-bottom: 1px solid #ddd;
    background-color: #f8f9fa;
}

.modal-header h3 {
    margin: 0;
    color: #333;
}

.header-actions {
    display: flex;
    align-items: center;
    gap: 10px;
}

.default-btn {
    padding: 8px 16px;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    font-weight: bold;
    transition: background-color 0.3s ease;
}

.default-btn:hover {
    background-color: #0056b3;
}

.close-btn {
    background: none;
    border: none;
    font-size: 24px;
    cursor: pointer;
    color: #666;
    width: 30px;
    height: 30px;
    display: flex;
    align-items: center;
    justify-content: center;
}

.close-btn:hover {
    color: #333;
}

.modal-body {
    padding: 20px;
}

.season-title {
    font-size: 1.2em;
    font-weight: bold;
    color: #333;
    margin-bottom: 5px;
}

.zones-list {
    margin-top: 20px;
}

.zone-pricing {
    margin-bottom: 20px;
    padding: 15px;
    border: 1px solid #ddd;
    border-radius: 5px;
    background-color: #f8f9fa;
}

.zone-pricing h4 {
    margin: 0 0 15px 0;
    color: #333;
    border-bottom: 1px solid #ddd;
    padding-bottom: 8px;
}

.pricing-input {
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.pricing-input label {
    font-weight: bold;
    color: #555;
}

.pricing-input input {
    padding: 10px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 16px;
}

.pricing-input input:focus {
    outline: none;
    border-color: #007bff;
    box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

.modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    padding: 20px;
    border-top: 1px solid #ddd;
    background-color: #f8f9fa;
}

.cancel-btn,
.enable-btn {
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-weight: bold;
    transition: background-color 0.3s ease;
}

.cancel-btn {
    background-color: #6c757d;
    color: white;
}

.cancel-btn:hover {
    background-color: #545b62;
}

.enable-btn {
    background-color: #007bff;
    color: white;
}

.enable-btn:hover:not(:disabled) {
    background-color: #0056b3;
}

.enable-btn:disabled {
    background-color: #6c757d;
    cursor: not-allowed;
}

@media (max-width: 768px) {
    .season-card {
        flex-direction: column;
        align-items: stretch;
        gap: 15px;
    }

    .season-actions {
        justify-content: center;
    }

    .tickets-enabled {
        text-align: center;
    }
}
</style>