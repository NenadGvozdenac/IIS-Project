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

                <!-- Zone Filters -->
                <div class="filters-container">
                    <div class="filter-row">
                        <div class="filter-group">
                            <label for="zoneNameFilter">Filter by Name:</label>
                            <input 
                                id="zoneNameFilter" 
                                v-model="zoneFilters.name" 
                                type="text" 
                                class="filter-input" 
                                placeholder="Search zone name..."
                            />
                        </div>
                        <div class="filter-group">
                            <label for="zoneStatusFilter">Filter by Status:</label>
                            <select id="zoneStatusFilter" v-model="zoneFilters.status" class="filter-select">
                                <option value="">All Statuses</option>
                                <option value="enabled">Enabled</option>
                                <option value="disabled">Disabled</option>
                            </select>
                        </div>
                        <div class="filter-group">
                            <label for="zoneRankFilter">Filter by Rank:</label>
                            <select id="zoneRankFilter" v-model="zoneFilters.rank" class="filter-select">
                                <option value="">All Ranks</option>
                                <option v-for="rank in uniqueZoneRanks" :key="rank" :value="rank">{{ rank }}</option>
                            </select>
                        </div>
                        <div class="filter-group">
                            <button @click="clearZoneFilters" class="btn btn-secondary btn-sm">Clear Filters</button>
                        </div>
                    </div>
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
                            <tr v-for="zone in paginatedZones" :key="zone.idZone">
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

                    <!-- Zone Pagination -->
                    <div class="pagination-container">
                        <div class="pagination-info">
                            <span>Showing {{ (zonePagination.currentPage - 1) * zonePagination.itemsPerPage + 1 }} to 
                                {{ Math.min(zonePagination.currentPage * zonePagination.itemsPerPage, filteredZones.length) }} 
                                of {{ filteredZones.length }} zones</span>
                            <select v-model="zonePagination.itemsPerPage" @change="changeZoneItemsPerPage(zonePagination.itemsPerPage)" class="items-per-page-select">
                                <option v-for="option in itemsPerPageOptions" :key="option" :value="option">{{ option }} per page</option>
                            </select>
                        </div>
                        <div class="pagination-controls">
                            <button 
                                @click="changeZonePage(zonePagination.currentPage - 1)" 
                                :disabled="zonePagination.currentPage <= 1"
                                class="btn btn-sm btn-secondary"
                            >
                                Previous
                            </button>
                            
                            <template v-for="page in zonePageNumbers" :key="page">
                                <button 
                                    v-if="typeof page === 'number'"
                                    @click="changeZonePage(page)" 
                                    :class="['btn', 'btn-sm', page === zonePagination.currentPage ? 'btn-primary' : 'btn-secondary']"
                                >
                                    {{ page }}
                                </button>
                                <span v-else class="pagination-ellipsis">{{ page }}</span>
                            </template>
                            
                            <button 
                                @click="changeZonePage(zonePagination.currentPage + 1)" 
                                :disabled="zonePagination.currentPage >= zoneTotalPages"
                                class="btn btn-sm btn-secondary"
                            >
                                Next
                            </button>
                        </div>
                    </div>

                    <div v-if="filteredZones.length === 0" class="empty-state">
                        <p v-if="hasActiveZoneFilters">No zones match the current filters.</p>
                        <p v-else>No zones to display. Total zones: {{ zones.length }}</p>
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

                <!-- Seat Filters -->
                <div class="filters-container">
                    <div class="filter-row">
                        <div class="filter-group">
                            <label for="seatRowFilter">Filter by Row:</label>
                            <input 
                                id="seatRowFilter" 
                                v-model="seatFilters.row" 
                                type="text" 
                                class="filter-input" 
                                placeholder="Search row..."
                            />
                        </div>
                        <div class="filter-group">
                            <label for="seatNumberFilter">Filter by Number:</label>
                            <input 
                                id="seatNumberFilter" 
                                v-model="seatFilters.number" 
                                type="text" 
                                class="filter-input" 
                                placeholder="Search number..."
                            />
                        </div>
                        <div class="filter-group">
                            <label for="seatTypeFilter">Filter by Type:</label>
                            <select id="seatTypeFilter" v-model="seatFilters.type" class="filter-select">
                                <option value="">All Types</option>
                                <option v-for="type in uniqueSeatTypes" :key="type" :value="type">{{ type }}</option>
                            </select>
                        </div>
                        <div class="filter-group">
                            <label for="seatZoneFilter">Filter by Zone:</label>
                            <select id="seatZoneFilter" v-model="seatFilters.zone" class="filter-select">
                                <option value="">All Zones</option>
                                <option v-for="zone in zones" :key="zone.idZone" :value="zone.idZone">{{ zone.name }}</option>
                            </select>
                        </div>
                        <div class="filter-group">
                            <label for="seatStatusFilter">Filter by Status:</label>
                            <select id="seatStatusFilter" v-model="seatFilters.status" class="filter-select">
                                <option value="">All Statuses</option>
                                <option value="empty">Empty/No Status</option>
                                <option v-for="status in uniqueSeatStatuses" :key="status" :value="status">{{ status }}</option>
                            </select>
                        </div>
                        <div class="filter-group">
                            <button @click="clearSeatFilters" class="btn btn-secondary btn-sm">Clear Filters</button>
                        </div>
                    </div>
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
                            <tr v-for="seat in paginatedSeats" :key="seat.idSeat">
                                <td>{{ seat.row }}</td>
                                <td>{{ seat.number }}</td>
                                <td>{{ seat.type }}</td>
                                <td>{{ seat.direction }}</td>
                                <td>
                                    <span class="status-badge" :class="seat.status?.toLowerCase()">
                                        {{ seat.status || 'No status' }}
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

                    <!-- Seat Pagination -->
                    <div class="pagination-container">
                        <div class="pagination-info">
                            <span>Showing {{ (seatPagination.currentPage - 1) * seatPagination.itemsPerPage + 1 }} to 
                                {{ Math.min(seatPagination.currentPage * seatPagination.itemsPerPage, filteredSeats.length) }} 
                                of {{ filteredSeats.length }} seats</span>
                            <select v-model="seatPagination.itemsPerPage" @change="changeSeatItemsPerPage(seatPagination.itemsPerPage)" class="items-per-page-select">
                                <option v-for="option in itemsPerPageOptions" :key="option" :value="option">{{ option }} per page</option>
                            </select>
                        </div>
                        <div class="pagination-controls">
                            <button 
                                @click="changeSeatPage(seatPagination.currentPage - 1)" 
                                :disabled="seatPagination.currentPage <= 1"
                                class="btn btn-sm btn-secondary"
                            >
                                Previous
                            </button>
                            
                            <template v-for="page in seatPageNumbers" :key="page">
                                <button 
                                    v-if="typeof page === 'number'"
                                    @click="changeSeatPage(page)" 
                                    :class="['btn', 'btn-sm', page === seatPagination.currentPage ? 'btn-primary' : 'btn-secondary']"
                                >
                                    {{ page }}
                                </button>
                                <span v-else class="pagination-ellipsis">{{ page }}</span>
                            </template>
                            
                            <button 
                                @click="changeSeatPage(seatPagination.currentPage + 1)" 
                                :disabled="seatPagination.currentPage >= seatTotalPages"
                                class="btn btn-sm btn-secondary"
                            >
                                Next
                            </button>
                        </div>
                    </div>

                    <div v-if="filteredSeats.length === 0" class="empty-state">
                        <p v-if="hasActiveSeatFilters">No seats match the current filters.</p>
                        <p v-else>No seats to display.</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Zone Modal -->
        <div v-if="showZoneModal" class="modal-overlay" @click="closeZoneModal">
            <div class="modal" @click.stop>
                <div class="modal-header">
                    <h3>{{ isEditingZone ? 'Edit Zone' : 'Create New Zone' }}</h3>
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
                        <button type="submit" class="btn btn-primary" :disabled="loading || !isZoneFormValid">
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
                    <h3>{{ isEditingSeat ? 'Edit Seat' : 'Create New Seat' }}</h3>
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
                            value="standard" />
                    </div>
                    <div class="form-group">
                        <label for="seatDirection">Direction *</label>
                        <select id="seatDirection" v-model="seatForm.direction" class="form-control" required>
                            <option value="">Select direction</option>
                            <option value="north">north</option>
                            <option value="south">south</option>
                            <option value="east">east</option>
                            <option value="west">west</option>
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
                        <button type="submit" class="btn btn-primary" :disabled="loading || !isSeatFormValid">
                            {{ loading ? 'Saving...' : (isEditingSeat ? 'Save' : 'Create') }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { ZoneService } from '../../services/ticket_service/zone_service.js';
import { SeatService } from '../../services/ticket_service/seat_service.js';

// Reactive data
const activeTab = ref('zones');
const zones = ref([]);
const seats = ref([]);
const loading = ref(false);

// Filter data
const zoneFilters = ref({
    name: '',
    status: '',
    rank: ''
});

const seatFilters = ref({
    row: '',
    number: '',
    type: '',
    zone: '',
    status: ''
});

// Pagination data
const zonePagination = ref({
    currentPage: 1,
    itemsPerPage: 25,
    totalItems: 0
});

const seatPagination = ref({
    currentPage: 1,
    itemsPerPage: 25,
    totalItems: 0
});

const itemsPerPageOptions = [10, 25, 50, 100];

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
    type: 'standard',
    direction: '',
    status: '',
    idZone: ''
});

// Computed properties for filtering
const filteredZones = computed(() => {
    const filtered = zones.value.filter(zone => {
        const nameMatch = !zoneFilters.value.name || 
            zone.name.toLowerCase().includes(zoneFilters.value.name.toLowerCase());
        const statusMatch = !zoneFilters.value.status || 
            zone.status?.toLowerCase() === zoneFilters.value.status.toLowerCase();
        const rankMatch = !zoneFilters.value.rank || 
            zone.rank.toString() === zoneFilters.value.rank.toString();
        
        return nameMatch && statusMatch && rankMatch;
    });
    
    zonePagination.value.totalItems = filtered.length;
    return filtered;
});

const paginatedZones = computed(() => {
    const start = (zonePagination.value.currentPage - 1) * zonePagination.value.itemsPerPage;
    const end = start + zonePagination.value.itemsPerPage;
    return filteredZones.value.slice(start, end);
});

const zoneTotalPages = computed(() => {
    return Math.ceil(filteredZones.value.length / zonePagination.value.itemsPerPage);
});

const paginatedSeats = computed(() => {
    const start = (seatPagination.value.currentPage - 1) * seatPagination.value.itemsPerPage;
    const end = start + seatPagination.value.itemsPerPage;
    return filteredSeats.value.slice(start, end);
});

const seatTotalPages = computed(() => {
    return Math.ceil(filteredSeats.value.length / seatPagination.value.itemsPerPage);
});

const filteredSeats = computed(() => {
    const filtered = seats.value.filter(seat => {
        const rowMatch = !seatFilters.value.row || 
            seat.row.toString().toLowerCase().includes(seatFilters.value.row.toLowerCase());
        const numberMatch = !seatFilters.value.number || 
            seat.number.toString().toLowerCase().includes(seatFilters.value.number.toLowerCase());
        const typeMatch = !seatFilters.value.type || 
            seat.type?.toLowerCase() === seatFilters.value.type.toLowerCase();
        const zoneMatch = !seatFilters.value.zone || 
            seat.idZone.toString() === seatFilters.value.zone.toString();
        
        // Handle status filtering including empty status
        let statusMatch = true;
        if (seatFilters.value.status) {
            if (seatFilters.value.status === 'empty') {
                statusMatch = !seat.status || seat.status.trim() === '';
            } else {
                statusMatch = seat.status?.toLowerCase() === seatFilters.value.status.toLowerCase();
            }
        }
        
        return rowMatch && numberMatch && typeMatch && zoneMatch && statusMatch;
    });
    
    seatPagination.value.totalItems = filtered.length;
    return filtered;
});

// Computed properties for pagination display
const zonePageNumbers = computed(() => {
    const current = zonePagination.value.currentPage;
    const total = zoneTotalPages.value;
    const pages = [];
    
    if (total <= 7) {
        // Show all pages if 7 or fewer
        for (let i = 1; i <= total; i++) {
            pages.push(i);
        }
    } else {
        // Always show first page
        pages.push(1);
        
        if (current <= 4) {
            // Current page is near beginning
            for (let i = 2; i <= 5; i++) {
                pages.push(i);
            }
            pages.push('...');
            pages.push(total);
        } else if (current >= total - 3) {
            // Current page is near end
            pages.push('...');
            for (let i = total - 4; i <= total; i++) {
                pages.push(i);
            }
        } else {
            // Current page is in middle
            pages.push('...');
            for (let i = current - 1; i <= current + 1; i++) {
                pages.push(i);
            }
            pages.push('...');
            pages.push(total);
        }
    }
    
    return pages;
});

const seatPageNumbers = computed(() => {
    const current = seatPagination.value.currentPage;
    const total = seatTotalPages.value;
    const pages = [];
    
    if (total <= 7) {
        // Show all pages if 7 or fewer
        for (let i = 1; i <= total; i++) {
            pages.push(i);
        }
    } else {
        // Always show first page
        pages.push(1);
        
        if (current <= 4) {
            // Current page is near beginning
            for (let i = 2; i <= 5; i++) {
                pages.push(i);
            }
            pages.push('...');
            pages.push(total);
        } else if (current >= total - 3) {
            // Current page is near end
            pages.push('...');
            for (let i = total - 4; i <= total; i++) {
                pages.push(i);
            }
        } else {
            // Current page is in middle
            pages.push('...');
            for (let i = current - 1; i <= current + 1; i++) {
                pages.push(i);
            }
            pages.push('...');
            pages.push(total);
        }
    }
    
    return pages;
});

// Unique values for filter dropdowns
const uniqueZoneRanks = computed(() => {
    const ranks = zones.value.map(zone => zone.rank).filter(rank => rank != null);
    return [...new Set(ranks)].sort((a, b) => a - b);
});

const uniqueSeatTypes = computed(() => {
    const types = seats.value.map(seat => seat.type).filter(type => type);
    return [...new Set(types)].sort();
});

const uniqueSeatStatuses = computed(() => {
    const statuses = seats.value.map(seat => seat.status).filter(status => status && status.trim() !== '');
    return [...new Set(statuses)].sort();
});

// Check if filters are active
const hasActiveZoneFilters = computed(() => {
    return zoneFilters.value.name || zoneFilters.value.status || zoneFilters.value.rank;
});

const hasActiveSeatFilters = computed(() => {
    return seatFilters.value.row || seatFilters.value.number || seatFilters.value.type || 
           seatFilters.value.zone || seatFilters.value.status;
});

// Form validation
const isZoneFormValid = computed(() => {
    return zoneForm.value.name && 
           zoneForm.value.rank && 
           zoneForm.value.maximumCapacity && 
           zoneForm.value.status;
});

const isSeatFormValid = computed(() => {
    return seatForm.value.row && 
           seatForm.value.number && 
           seatForm.value.direction && 
           seatForm.value.status && 
           seatForm.value.idZone;
});

// Filter methods
const clearZoneFilters = () => {
    zoneFilters.value = {
        name: '',
        status: '',
        rank: ''
    };
    zonePagination.value.currentPage = 1;
};

const clearSeatFilters = () => {
    seatFilters.value = {
        row: '',
        number: '',
        type: '',
        zone: '',
        status: ''
    };
    seatPagination.value.currentPage = 1;
};

// Pagination methods
const changeZonePage = (page) => {
    if (page >= 1 && page <= zoneTotalPages.value) {
        zonePagination.value.currentPage = page;
    }
};

const changeSeatPage = (page) => {
    if (page >= 1 && page <= seatTotalPages.value) {
        seatPagination.value.currentPage = page;
    }
};

const changeZoneItemsPerPage = (items) => {
    zonePagination.value.itemsPerPage = items;
    zonePagination.value.currentPage = 1;
};

const changeSeatItemsPerPage = (items) => {
    seatPagination.value.itemsPerPage = items;
    seatPagination.value.currentPage = 1;
};

// Methods
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
        type: 'standard',
        direction: '',
        status: '',
        idZone: ''
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
        type: 'standard',
        direction: '',
        status: '',
        idZone: ''
    };
}; const saveSeat = async () => {
    try {
        loading.value = true;

        // Check for duplicate seat when creating a new seat
        if (!isEditingSeat.value) {
            const existingSeat = seats.value.find(seat => 
                seat.row === seatForm.value.row && 
                seat.number === seatForm.value.number && 
                seat.direction === seatForm.value.direction &&
                seat.idZone === seatForm.value.idZone
            );
            
            if (existingSeat) {
                const zoneName = getZoneName(seatForm.value.idZone);
                alert(`A seat with row ${seatForm.value.row}, number ${seatForm.value.number}, direction ${seatForm.value.direction}, and zone ${zoneName} already exists.`);
                loading.value = false;
                return;
            }
        }

        // Check for duplicate seat when editing (exclude current seat from check)
        if (isEditingSeat.value) {
            const existingSeat = seats.value.find(seat => 
                seat.idSeat !== seatForm.value.idSeat && // Exclude current seat
                seat.row === seatForm.value.row && 
                seat.number === seatForm.value.number && 
                seat.direction === seatForm.value.direction &&
                seat.idZone === seatForm.value.idZone
            );
            
            if (existingSeat) {
                const zoneName = getZoneName(seatForm.value.idZone);
                alert(`Cannot update seat: A seat with row ${seatForm.value.row}, number ${seatForm.value.number}, direction ${seatForm.value.direction}, and zone ${zoneName} already exists.`);
                loading.value = false;
                return;
            }
        }

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

/* Filters */
.filters-container {
    background: var(--color-surface);
    border: 1px solid var(--color-border);
    border-radius: 8px;
    padding: var(--spacing-lg);
    margin-bottom: var(--spacing-lg);
}

.filter-row {
    display: flex;
    flex-wrap: wrap;
    gap: var(--spacing-md);
    align-items: end;
}

.filter-group {
    display: flex;
    flex-direction: column;
    min-width: 150px;
    flex: 1;
}

.filter-group label {
    font-size: 0.875rem;
    font-weight: 500;
    color: var(--color-text);
    margin-bottom: var(--spacing-xs);
}

.filter-input,
.filter-select {
    padding: var(--spacing-sm);
    border: 1px solid var(--color-border);
    border-radius: 4px;
    font-size: 0.875rem;
    background: white;
    transition: border-color 0.2s ease;
}

.filter-input:focus,
.filter-select:focus {
    outline: none;
    border-color: var(--color-primary);
    box-shadow: 0 0 0 2px rgba(99, 102, 241, 0.1);
}

.filter-input::placeholder {
    color: var(--color-text-muted);
}

.filter-group button {
    align-self: flex-end;
}

/* Pagination */
.pagination-container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: var(--spacing-lg);
    border-top: 1px solid var(--color-border);
    background: var(--color-surface);
}

.pagination-info {
    display: flex;
    align-items: center;
    gap: var(--spacing-md);
    font-size: 0.875rem;
    color: var(--color-text-muted);
}

.items-per-page-select {
    padding: var(--spacing-xs) var(--spacing-sm);
    border: 1px solid var(--color-border);
    border-radius: 4px;
    font-size: 0.875rem;
    background: white;
}

.pagination-controls {
    display: flex;
    align-items: center;
    gap: var(--spacing-xs);
}

.pagination-controls .btn {
    min-width: 40px;
    height: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 0.875rem;
}

.pagination-controls .btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
    pointer-events: none;
}

.pagination-controls span {
    padding: 0 var(--spacing-sm);
    color: var(--color-text-muted);
    font-size: 0.875rem;
}

.pagination-ellipsis {
    padding: 0.25rem 0.5rem;
    margin: 0 0.25rem;
    color: var(--color-text-muted);
    font-weight: bold;
    user-select: none;
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
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
    backdrop-filter: blur(4px);
}

.modal {
    background: white;
    border-radius: var(--radius-lg);
    max-width: 500px;
    width: 90%;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
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
    transition: color 0.2s ease;
}

.close-btn:hover {
    color: var(--color-text);
}

.modal-body {
    padding: var(--spacing-lg);
}

.form-group {
    margin-bottom: var(--spacing-md);
}

.form-group label {
    display: block;
    margin-bottom: var(--spacing-xs);
    font-weight: 500;
    color: var(--color-text);
    font-size: 0.875rem;
}

.form-control {
    width: 100%;
    padding: var(--spacing-sm) var(--spacing-md);
    border: 2px solid var(--color-border);
    border-radius: var(--radius-md);
    font-size: 0.875rem;
    transition: all 0.2s ease;
}

.form-control:focus {
    outline: none;
    border-color: var(--color-primary);
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-control:disabled {
    background-color: #f9fafb;
    color: #6b7280;
    cursor: not-allowed;
    opacity: 0.7;
}

.form-text {
    font-size: 0.75rem;
    margin-top: 0.25rem;
    display: block;
}

.text-muted {
    color: #6b7280;
}

.modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: var(--spacing-sm);
    padding-top: var(--spacing-md);
    border-top: 1px solid var(--color-border);
    margin-top: var(--spacing-md);
}

/* Responsive */
@media (max-width: 768px) {
    .section-header {
        flex-direction: column;
        align-items: stretch;
        gap: var(--spacing-md);
    }

    .filter-row {
        flex-direction: column;
        align-items: stretch;
    }

    .filter-group {
        min-width: auto;
    }

    .filter-group button {
        align-self: stretch;
        margin-top: var(--spacing-sm);
    }

    .pagination-container {
        flex-direction: column;
        gap: var(--spacing-md);
        align-items: stretch;
    }

    .pagination-info {
        justify-content: space-between;
    }

    .pagination-controls {
        justify-content: center;
        flex-wrap: wrap;
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
