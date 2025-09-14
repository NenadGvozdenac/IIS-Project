<template>
    <div class="upcoming-matches">
        <h1>Upcoming Matches</h1>

        <div v-if="loading" class="loading">
            Loading matches...
        </div>

        <div v-else-if="error" class="error">
            {{ error }}
        </div>

        <div v-else-if="matches.length === 0" class="no-matches">
            No upcoming matches found in our hall.
        </div>

        <div v-else class="matches-list">
            <div v-for="match in matches" :key="match.idMatch" class="match-card">
                <div class="match-info">
                    <h3>{{ match.name }}</h3>
                    <p class="match-date">{{ formatDate(match.scheduledAt) }}</p>
                    <p class="competition">{{ match.competitionName }}</p>
                    <p class="hall">{{ match.hall }}</p>
                    <p class="season">{{ match.seasonName }}</p>
                </div>

                <div class="match-actions">
                    <span v-if="getDaysUntilMatch(match.scheduledAt) > 5" class="days-remaining">
                        {{ getDaysUntilMatch(match.scheduledAt) }} days remaining
                    </span>

                    <button v-else :disabled="match.ticketsForSale || enableLoading === match.idMatch"
                        @click="openPricingModal(match)" class="enable-tickets-btn"
                        :class="{ 'enabled': match.ticketsForSale }">
                        <span v-if="enableLoading === match.idMatch">Enabling...</span>
                        <span v-else-if="match.ticketsForSale">Tickets Enabled</span>
                        <span v-else>Enable Tickets</span>
                    </button>
                </div>
            </div>
        </div>

        <!-- Pricing Parameters Modal -->
        <div v-if="showPricingModal" class="modal-overlay" @click="closePricingModal">
            <div class="modal-content" @click.stop>
                <div class="modal-header">
                    <h3>Set Pricing Parameters</h3>
                    <div class="header-actions">
                        <button @click="setDefaultValues" class="default-btn">Set Default Values</button>
                        <button @click="closePricingModal" class="close-btn">&times;</button>
                    </div>
                </div>

                <div class="modal-body">
                    <p class="match-title">{{ selectedMatch?.name }}</p>
                    <p class="match-date">{{ formatDate(selectedMatch?.scheduledAt) }}</p>

                    <div v-if="zonesLoading" class="loading">
                        Loading zones...
                    </div>

                    <div v-else-if="zones.length > 0" class="zones-list">
                        <div v-for="zone in zones" :key="zone.idZone" class="zone-parameter">
                            <h4>{{ zone.name }}</h4>
                            <div class="parameter-inputs">
                                <div class="input-group">
                                    <label>Price Factor:</label>
                                    <input type="number" step="1" min="0" v-model.number="priceFactors[zone.idZone]"
                                        placeholder="e.g., 800" />
                                </div>

                                <div class="input-group">
                                    <label>Time Factor:</label>
                                    <input type="number" step="1" min="0" v-model.number="timeFactors[zone.idZone]"
                                        placeholder="e.g., 70" />
                                </div>

                                <div class="input-group">
                                    <label>Minimum Seat Price:</label>
                                    <input type="number" step="1" min="0"
                                        v-model.number="minimumSeatPrices[zone.idZone]" placeholder="e.g., 1000" />
                                </div>

                                <div class="input-group">
                                    <label>Maximum Seat Price:</label>
                                    <input type="number" step="1" min="0"
                                        v-model.number="maximumSeatPrices[zone.idZone]" placeholder="e.g., 10000" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button @click="closePricingModal" class="cancel-btn">Cancel</button>
                    <button @click="createParametersAndEnableTickets" :disabled="!canCreateParameters || createLoading"
                        class="create-btn">
                        <span v-if="createLoading">Processing...</span>
                        <span v-else>Create Parameters & Enable Tickets</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, onMounted, computed, reactive } from 'vue'
import { MatchService } from '../../services/ticket_service/match_service'
import { ZoneService } from '../../services/ticket_service/zone_service'
import { getUserData } from '../../services/auth_service'

export default {
    name: 'UpcomingMatches',
    setup() {
        const matches = ref([])
        const zones = ref([])
        const loading = ref(true)
        const zonesLoading = ref(false)
        const enableLoading = ref(null)
        const createLoading = ref(false)
        const error = ref('')
        const showPricingModal = ref(false)
        const selectedMatch = ref(null)

        // Create individual reactive refs for each parameter type
        const priceFactors = ref({})
        const timeFactors = ref({})
        const minimumSeatPrices = ref({})
        const maximumSeatPrices = ref({})

        // Helper functions to manage individual parameters
        const initializeZoneParameters = (zoneId) => {
            console.log('Initializing parameters for zone:', zoneId)
            // Just ensure the keys exist without setting default values
            if (!(zoneId in priceFactors.value)) priceFactors.value[zoneId] = null
            if (!(zoneId in timeFactors.value)) timeFactors.value[zoneId] = null
            if (!(zoneId in minimumSeatPrices.value)) minimumSeatPrices.value[zoneId] = null
            if (!(zoneId in maximumSeatPrices.value)) maximumSeatPrices.value[zoneId] = null
        }

        const clearAllParameters = () => {
            priceFactors.value = {}
            timeFactors.value = {}
            minimumSeatPrices.value = {}
            maximumSeatPrices.value = {}
        }

        const setDefaultValues = () => {
            zones.value.forEach(zone => {
                priceFactors.value[zone.idZone] = 800
                timeFactors.value[zone.idZone] = 70
                minimumSeatPrices.value[zone.idZone] = 1000
                maximumSeatPrices.value[zone.idZone] = 10000
            })
        }

        const formatDate = (dateString) => {
            const date = new Date(dateString)
            return date.toLocaleDateString('en-US', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            })
        }

        const getDaysUntilMatch = (dateString) => {
            const matchDate = new Date(dateString)
            const today = new Date()
            const timeDiff = matchDate.getTime() - today.getTime()
            return Math.ceil(timeDiff / (1000 * 3600 * 24))
        }

        const loadMatches = async () => {
            try {
                loading.value = true
                error.value = ''
                const response = await MatchService.getMatchesInOurHall()

                // Handle the new API response format
                if (response.isSuccess && response.value) {
                    // Filter out matches that are before today
                    const today = new Date()
                    today.setHours(0, 0, 0, 0) // Reset time to start of day for comparison

                    const upcomingMatches = response.value.filter(match => {
                        const matchDate = new Date(match.scheduledAt)
                        matchDate.setHours(0, 0, 0, 0) // Reset time to start of day for comparison
                        return matchDate >= today
                    })

                    // Sort matches by date (earliest first)
                    matches.value = upcomingMatches.sort((a, b) => {
                        return new Date(a.scheduledAt) - new Date(b.scheduledAt)
                    })
                } else {
                    matches.value = []
                    error.value = response.error || 'Failed to load matches'
                }
            } catch (err) {
                error.value = 'Failed to load matches: ' + (err.message || err)
            } finally {
                loading.value = false
            }
        }

        const loadZones = async () => {
            try {
                zonesLoading.value = true
                const response = await ZoneService.getAllZones()
                zones.value = response.value.zones

                clearAllParameters()

                // Just initialize the structure for each zone
                zones.value.forEach(zone => {
                    initializeZoneParameters(zone.idZone)
                })

            } catch (err) {
                error.value = 'Failed to load zones: ' + (err.message || err)
                zones.value = []
                // Clear pricing parameters on error
                clearAllParameters()
            } finally {
                zonesLoading.value = false
            }
        }

        const openPricingModal = async (match) => {
            selectedMatch.value = match
            showPricingModal.value = true

            // Initialize empty pricing parameters immediately to prevent template errors
            clearAllParameters()

            await loadZones()
        }

        const closePricingModal = () => {
            showPricingModal.value = false
            selectedMatch.value = null
            // Clear pricing parameters
            clearAllParameters()
        }

        const canCreateParameters = computed(() => {
            if (!zones.value.length) return false

            return zones.value.every(zone => {
                const priceFactor = priceFactors.value[zone.idZone]
                const timeFactor = timeFactors.value[zone.idZone]
                const minPrice = minimumSeatPrices.value[zone.idZone]
                const maxPrice = maximumSeatPrices.value[zone.idZone]

                return typeof priceFactor === 'number' && priceFactor > 0 &&
                    typeof timeFactor === 'number' && timeFactor > 0 &&
                    typeof minPrice === 'number' && minPrice > 0 &&
                    typeof maxPrice === 'number' && maxPrice > 0 &&
                    minPrice <= maxPrice
            })
        })

        const createParametersAndEnableTickets = async () => {
            if (!selectedMatch.value || !canCreateParameters.value) return

            try {
                createLoading.value = true

                // Create pricing parameters for each zone
                for (const zone of zones.value) {
                    const userData = getUserData()

                    await MatchService.createTicketPriceParameter(selectedMatch.value.idMatch, {
                        zoneId: zone.idZone,
                        priceFactor: priceFactors.value[zone.idZone],
                        timeFactor: timeFactors.value[zone.idZone],
                        minimumSeatPrice: minimumSeatPrices.value[zone.idZone],
                        maximumSeatPrice: maximumSeatPrices.value[zone.idZone],
                        userId: userData.userID
                    })
                }

                // Enable tickets for the match
                enableLoading.value = selectedMatch.value.idMatch
                await MatchService.enableTicketsForMatch(selectedMatch.value.idMatch)

                // Update the match in our local data
                const matchIndex = matches.value.findIndex(m => m.idMatch === selectedMatch.value.idMatch)
                if (matchIndex !== -1) {
                    matches.value[matchIndex].ticketsForSale = true
                }

                // Close modal and show success
                closePricingModal()
                alert('Pricing parameters created and tickets enabled successfully!')

            } catch (err) {
                error.value = 'Failed to create parameters or enable tickets: ' + (err.message || err)
            } finally {
                createLoading.value = false
                enableLoading.value = null
            }
        }

        onMounted(() => {
            loadMatches()
        })

        return {
            matches,
            zones,
            loading,
            zonesLoading,
            enableLoading,
            createLoading,
            error,
            showPricingModal,
            selectedMatch,
            priceFactors,
            timeFactors,
            minimumSeatPrices,
            maximumSeatPrices,
            canCreateParameters,
            setDefaultValues,
            formatDate,
            getDaysUntilMatch,
            openPricingModal,
            closePricingModal,
            createParametersAndEnableTickets
        }
    }
}
</script>

<style scoped>
.upcoming-matches {
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

.no-matches {
    text-align: center;
    padding: 40px;
    color: #666;
    font-style: italic;
}

.matches-list {
    display: grid;
    gap: 20px;
}

.match-card {
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

.match-card:hover {
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.match-info h3 {
    margin: 0 0 10px 0;
    color: #333;
    font-size: 1.3em;
}

.match-info p {
    margin: 5px 0;
    color: #666;
}

.match-date {
    font-weight: bold;
    color: #007bff;
}

.competition {
    font-style: italic;
}

.season {
    color: #28a745;
    font-weight: bold;
}

.match-actions {
    display: flex;
    align-items: center;
    gap: 15px;
}

.days-remaining {
    color: #ffc107;
    font-weight: bold;
    padding: 8px 16px;
    background-color: #fff3cd;
    border: 1px solid #ffeaa7;
    border-radius: 4px;
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

.enable-tickets-btn.enabled {
    background-color: #17a2b8;
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
    max-width: 800px;
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

.match-title {
    font-size: 1.2em;
    font-weight: bold;
    color: #333;
    margin-bottom: 5px;
}

.zones-list {
    margin-top: 20px;
}

.zone-parameter {
    margin-bottom: 25px;
    padding: 15px;
    border: 1px solid #ddd;
    border-radius: 5px;
    background-color: #f8f9fa;
}

.zone-parameter h4 {
    margin: 0 0 15px 0;
    color: #333;
    border-bottom: 1px solid #ddd;
    padding-bottom: 8px;
}

.parameter-inputs {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-template-rows: 1fr 1fr;
    gap: 15px;
}

.input-group {
    display: flex;
    flex-direction: column;
}

.input-group label {
    font-weight: bold;
    margin-bottom: 5px;
    color: #555;
}

.input-group input {
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
}

.input-group input:focus {
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
.create-btn {
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

.create-btn {
    background-color: #007bff;
    color: white;
}

.create-btn:hover:not(:disabled) {
    background-color: #0056b3;
}

.create-btn:disabled {
    background-color: #6c757d;
    cursor: not-allowed;
}

@media (max-width: 768px) {
    .match-card {
        flex-direction: column;
        align-items: stretch;
        gap: 15px;
    }

    .match-actions {
        justify-content: center;
    }

    .parameter-inputs {
        grid-template-columns: 1fr;
    }
}
</style>