<template>
    <div class="admin-zones-seats">
        <div class="container">
            <h1 class="page-title">Zones and Seats Management</h1>

            <!-- Navigation tabs -->
            <div class="tabs">
                <button class="tab-btn" :class="{ active: activeTab === 'zones' }" @click="activeTab = 'zones'">
                    Zones
                </button>
                <button class="tab-btn" :class="{ active: activeTab === 'seats' }" @click="activeTab = 'seats'">
                    Seats
                </button>
            </div>

            <!-- Zones Tab -->
            <div v-if="activeTab === 'zones'" class="tab-content">
                <div class="section-header">
                    <h2>Zone Management</h2>
                    <button @click="openCreateZoneModal" class="btn btn-primary">
                        <i class="icon-plus"></i>
                        New Zone
                    </button>
                </div>

                <!-- Zones Table -->
                <div class="table-container">
                    <table class="data-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Rank</th>
                                <th>Maximum Capacity</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="zone in zones" :key="zone.idZone">
                                <td>{{ zone.name }}</td>
                                <td>{{ zone.rank }}</td>
                                <td>{{ zone.maximumCapacity }}</td>
                                <td>
                                    <span class="status-badge" :class="zone.status?.toLowerCase()">
                                        {{ zone.status }}
                                    </span>
                                </td>
                                <td>
                                    <div class="action-buttons">
                                        <button @click="editZone(zone)" class="btn btn-sm btn-secondary">
                                            Edit
                                        </button>
                                        <button @click="deleteZone(zone.idZone)" class="btn btn-sm btn-danger">
                                            Delete
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div v-if="zones.length === 0" class="empty-state">
                        <p>No zones to display. Total zones: {{ zones.length }}</p>
                        <p>Debug: {{ JSON.stringify(zones) }}</p>
                    </div>
                </div>
            </div>

            <!-- Seats Tab -->
            <div v-if="activeTab === 'seats'" class="tab-content">
                <div class="section-header">
                    <h2>Seat Management</h2>
                    <button @click="openCreateSeatModal" class="btn btn-primary">
                        <i class="icon-plus"></i>
                        New Seat
                    </button>
                </div>

                <!-- Seats Table -->
                <div class="table-container">
                    <table class="data-table">
                        <thead>
                            <tr>
                                <th>Row</th>
                                <th>Number</th>
                                <th>Type</th>
                                <th>Direction</th>
                                <th>Status</th>
                                <th>Zone</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="seat in seats" :key="seat.idSeat">
                                <td>{{ seat.row }}</td>
                                <td>{{ seat.number }}</td>
                                <td>{{ seat.type }}</td>
                                <td>{{ seat.direction }}</td>
                                <td>
                                    <span class="status-badge" :class="seat.status?.toLowerCase()">
                                        {{ seat.status }}
                                    </span>
                                </td>
                                <td>{{ getZoneName(seat.idZone) }}</td>
                                <td>
                                    <div class="action-buttons">
                                        <button @click="editSeat(seat)" class="btn btn-sm btn-secondary">
                                            Edit
                                        </button>
                                        <button @click="deleteSeat(seat.idSeat)" class="btn btn-sm btn-danger">
                                            Delete
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div v-if="seats.length === 0" class="empty-state">
                        <p>No seats to display.</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Zone Modal -->
        <div v-if="showZoneModal" class="modal-overlay" @click="closeZoneModal">
            <div class="modal" @click.stop>
                <div class="modal-header">
                    <h3>{{ isEditingZone ? 'Edit Zone' : 'New Zone' }}</h3>
                    <button @click="closeZoneModal" class="close-btn">&times;</button>
                </div>

                <form @submit.prevent="saveZone" class="modal-body">
                    <div class="form-group">
                        <label for="zoneName">Zone Name *</label>
                        <input id="zoneName" v-model="zoneForm.name" type="text" class="form-control" required
                            placeholder="Enter zone name" />
                    </div>

                    <div class="form-group">
                        <label for="zoneRank">Rank *</label>
                        <input id="zoneRank" v-model.number="zoneForm.rank" type="number" class="form-control" required
                            min="1" placeholder="Enter zone rank" />
                    </div>

                    <div class="form-group">
                        <label for="zoneCapacity">Maximum Capacity *</label>
                        <input id="zoneCapacity" v-model.number="zoneForm.maximumCapacity" type="number"
                            class="form-control" required min="1" placeholder="Enter maximum capacity" />
                    </div>

                    <div class="form-group">
                        <label for="zoneStatus">Status *</label>
                        <select id="zoneStatus" v-model="zoneForm.status" class="form-control" required>
                            <option value="">Select status</option>
                            <option value="enabled">Enabled</option>
                            <option value="disabled">Disabled</option>
                        </select>
                    </div>
                    <div class="modal-footer">
                        <button type="button" @click="closeZoneModal" class="btn btn-secondary">
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary" :disabled="loading">
                            {{ loading ? 'Saving...' : (isEditingZone ? 'Save' : 'Create') }}
                        </button>
                    </div>
                </form>
            </div>
        </div>

        <!-- Seat Modal -->
        <div v-if="showSeatModal" class="modal-overlay" @click="closeSeatModal">
            <div class="modal" @click.stop>
                <div class="modal-header">
                    <h3>{{ isEditingSeat ? 'Edit Seat' : 'New Seat' }}</h3>
                    <button @click="closeSeatModal" class="close-btn">&times;</button>
                </div>

                <form @submit.prevent="saveSeat" class="modal-body">
                    <div class="form-group">
                        <label for="seatRow">Row *</label>
                        <input id="seatRow" v-model.number="seatForm.row" type="number" class="form-control" required
                            min="1" placeholder="Enter row number" />
                    </div>

                    <div class="form-group">
                        <label for="seatNumber">Seat Number *</label>
                        <input id="seatNumber" v-model.number="seatForm.number" type="number" class="form-control"
                            required min="1" placeholder="Enter seat number" />
                    </div>

                    <div class="form-group">
                        <label for="seatType">Type</label>
                        <input id="seatType" v-model="seatForm.type" type="text" class="form-control" readonly
                            value="regular" />
                    </div>
                    <div class="form-group">
                        <label for="seatDirection">Direction *</label>
                        <select id="seatDirection" v-model="seatForm.direction" class="form-control" required>
                            <option value="">Select direction</option>
                            <option value="North">North</option>
                            <option value="South">South</option>
                            <option value="East">East</option>
                            <option value="West">West</option>
                        </select>
                    </div>

                    <div class="form-group">
                        <label for="seatStatus">Status *</label>
                        <select id="seatStatus" v-model="seatForm.status" class="form-control" required>
                            <option value="">Select status</option>
                            <option value="enabled">Enabled</option>
                            <option value="disabled">Disabled</option>
                            <option value="empty">Empty</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="seatZone">Zone *</label>
                        <select id="seatZone" v-model="seatForm.idZone" class="form-control" required>
                            <option value="">Select zone</option>
                            <option v-for="zone in zones" :key="zone.idZone" :value="zone.idZone">
                                {{ zone.name }}
                            </option>
                        </select>
                    </div>

                    <div class="modal-footer">
                        <button type="button" @click="closeSeatModal" class="btn btn-secondary">
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary" :disabled="loading">
                            {{ loading ? 'Saving...' : (isEditingSeat ? 'Save' : 'Create') }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { ZoneService } from '../../services/zone_service.js';
import { SeatService } from '../../services/seat_service.js';

// Reactive data
const activeTab = ref('zones');
const zones = ref([]);
const seats = ref([]);
const loading = ref(false);

// Zone modal
const showZoneModal = ref(false);
const isEditingZone = ref(false);
const zoneForm = ref({
    name: '',
    rank: null,
    maximumCapacity: null,
    status: ''
});

// Seat modal
const showSeatModal = ref(false);
const isEditingSeat = ref(false);
const seatForm = ref({
    row: null,
    number: null,
    type: 'regular',
    direction: '',
    status: '',
    idZone: null
});// Methods
const loadZones = async () => {
    try {
        loading.value = true;
        const response = await ZoneService.getAllZones();
        zones.value = response.value.zones || [];
    } catch (error) {
        console.error('Error loading zones:', error);
        alert('Error loading zones');
    } finally {
        loading.value = false;
    }
};

const loadSeats = async () => {
    try {
        loading.value = true;
        const response = await SeatService.getAllSeats();
        seats.value = response.value || [];
    } catch (error) {
        console.error('Error loading seats:', error);
        alert('Error loading seats');
    } finally {
        loading.value = false;
    }
};

const getZoneName = (zoneId) => {
    const zone = zones.value.find(z => z.idZone === zoneId);
    return zone ? zone.name : 'Unknown zone';
};

// Zone methods
const openCreateZoneModal = () => {
    isEditingZone.value = false;
    zoneForm.value = {
        name: '',
        rank: null,
        maximumCapacity: null,
        status: ''
    };
    showZoneModal.value = true;
};

const editZone = (zone) => {
    isEditingZone.value = true;
    zoneForm.value = {
        idZone: zone.idZone,
        name: zone.name,
        rank: zone.rank,
        maximumCapacity: zone.maximumCapacity,
        status: zone.status
    };
    showZoneModal.value = true;
};

const closeZoneModal = () => {
    showZoneModal.value = false;
    zoneForm.value = {
        name: '',
        rank: null,
        maximumCapacity: null,
        status: ''
    };
};

const saveZone = async () => {
    try {
        loading.value = true;

        if (isEditingZone.value) {
            await ZoneService.updateZone(zoneForm.value.idZone, {
                name: zoneForm.value.name,
                rank: zoneForm.value.rank,
                maximumCapacity: zoneForm.value.maximumCapacity,
                status: zoneForm.value.status
            });
            alert('Zone updated successfully!');
        } else {
            await ZoneService.createZone(zoneForm.value);
            alert('Zone created successfully!');
        }

        closeZoneModal();
        await loadZones();
    } catch (error) {
        console.error('Error saving zone:', error);
        alert('Error saving zone');
    } finally {
        loading.value = false;
    }
};

const deleteZone = async (zoneId) => {
  // First check if zone has any seats
  const zoneSeats = seats.value.filter(seat => seat.idZone === zoneId);
  
  if (zoneSeats.length > 0) {
    alert(`Cannot delete this zone. It has ${zoneSeats.length} seat(s) assigned to it. Please delete all seats from this zone first.`);
    return;
  }
  
  if (!confirm('Are you sure you want to delete this zone?')) {
    return;
  }

  try {
    await ZoneService.deleteZone(zoneId);
    alert('Zone deleted successfully!');
    await loadZones();
    await loadSeats(); // Reload seats as they might be affected
  } catch (error) {
    console.error('Error deleting zone:', error);
    alert('Error deleting zone');
  }
};

// Seat methods
const openCreateSeatModal = () => {
    isEditingSeat.value = false;
    seatForm.value = {
        row: null,
        number: null,
        type: 'regular',
        direction: '',
        status: '',
        idZone: null
    };
    showSeatModal.value = true;
}; const editSeat = (seat) => {
    isEditingSeat.value = true;
    seatForm.value = {
        idSeat: seat.idSeat,
        row: seat.row,
        number: seat.number,
        type: seat.type,
        direction: seat.direction,
        status: seat.status,
        idZone: seat.idZone
    };
    showSeatModal.value = true;
};

const closeSeatModal = () => {
    showSeatModal.value = false;
    seatForm.value = {
        row: null,
        number: null,
        type: 'regular',
        direction: '',
        status: '',
        idZone: null
    };
}; const saveSeat = async () => {
    try {
        loading.value = true;

        if (isEditingSeat.value) {
            await SeatService.updateSeat(seatForm.value.idSeat, {
                row: seatForm.value.row,
                number: seatForm.value.number,
                type: seatForm.value.type,
                direction: seatForm.value.direction,
                status: seatForm.value.status,
                idZone: seatForm.value.idZone
            });
            alert('Seat updated successfully!');
        } else {
            await SeatService.createSeat(seatForm.value);
            alert('Seat created successfully!');
        }

        closeSeatModal();
        await loadSeats();
    } catch (error) {
        console.error('Error saving seat:', error);
        alert('Error saving seat');
    } finally {
        loading.value = false;
    }
};

const deleteSeat = async (seatId) => {
    if (!confirm('Are you sure you want to delete this seat?')) {
        return;
    }

    try {
        await SeatService.deleteSeat(seatId);
        alert('Seat deleted successfully!');
        await loadSeats();
    } catch (error) {
        console.error('Error deleting seat:', error);
        alert('Error deleting seat');
    }
};

// Lifecycle
onMounted(async () => {
    await loadZones();
    await loadSeats();
});
</script>

<style scoped>
.admin-zones-seats {
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

/* Table */
.table-container {
    background: white;
    border-radius: var(--radius-lg);
    box-shadow: var(--shadow-sm);
    overflow: hidden;
}

.data-table {
    width: 100%;
    border-collapse: collapse;
}

.data-table th {
    background-color: var(--color-surface);
    padding: var(--spacing-md);
    text-align: left;
    font-weight: 600;
    color: var(--color-text);
    border-bottom: 1px solid var(--color-border);
}

.data-table td {
    padding: var(--spacing-md);
    border-bottom: 1px solid var(--color-border);
}

.data-table tbody tr:hover {
    background-color: var(--color-surface);
}

.status-badge {
    padding: 0.25rem 0.5rem;
    border-radius: var(--radius-md);
    font-size: 0.75rem;
    font-weight: 500;
    text-transform: uppercase;
}

.status-badge.enabled {
    background-color: #dcfce7;
    color: #166534;
}

.status-badge.disabled {
    background-color: #fef2f2;
    color: #991b1b;
}

.status-badge.empty {
    background-color: #f3f4f6;
    color: #374151;
}

.action-buttons {
    display: flex;
    gap: var(--spacing-sm);
}

.empty-state {
    padding: var(--spacing-xl);
    text-align: center;
    color: var(--color-text-muted);
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

.modal-body {
    padding: var(--spacing-lg);
}

.form-group {
    margin-bottom: var(--spacing-lg);
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

    .table-container {
        overflow-x: auto;
    }

    .data-table {
        min-width: 600px;
    }

    .modal {
        width: 95%;
        margin: var(--spacing-md);
    }
}
</style>
