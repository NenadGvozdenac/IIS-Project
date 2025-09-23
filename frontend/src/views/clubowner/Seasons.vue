<template>
    <div class="admin-zones-seats">
        <div class="container">
            <h1 class="page-title">Seasons Management</h1>

            <div class="section-header">
                <h2>Season Management</h2>
                <button @click="openCreateSeasonModal" class="btn btn-primary">
                    <i class="icon-plus"></i>
                    New Season
                </button>
            </div>

            <!-- Season Filters -->
            <div class="filters-container">
                <div class="filter-row">
                    <div class="filter-group">
                        <label for="seasonNameFilter">Filter by Name:</label>
                        <input 
                            id="seasonNameFilter" 
                            v-model="seasonFilters.name" 
                            type="text" 
                            class="filter-input" 
                            placeholder="Search season name..."
                        />
                    </div>
                    <div class="filter-group">
                        <button @click="clearSeasonFilters" class="btn btn-secondary btn-sm">Clear Filters</button>
                    </div>
                </div>
            </div>

            <!-- Seasons Table -->
            <div class="table-container">
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="season in paginatedSeasons" :key="season.idSeason">
                            <td>{{ season.name }}</td>
                            <td>{{ formatDate(season.startedAt) }}</td>
                            <td>{{ formatDate(season.endedAt) }}</td>
                            <td>
                                <span class="status-badge" :class="getStatusClass(season.isActive ? 'active' : 'inactive')">
                                    {{ season.isActive ? 'Active' : 'Inactive' }}
                                </span>
                            </td>
                            <td>
                                <div class="action-buttons">
                                    <button @click="editSeason(season)" class="btn btn-sm btn-secondary">
                                        Edit
                                    </button>
                                    <button 
                                        @click="deleteSeason(season.idSeason)" 
                                        class="btn btn-sm btn-danger"
                                        :disabled="season.isActive"
                                        :title="season.isActive ? 'Cannot delete active season' : 'Delete season'"
                                    >
                                        Delete
                                    </button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>

                <!-- Season Pagination -->
                <div class="pagination-container">
                    <div class="pagination-info">
                        <span>Showing {{ (seasonPagination.currentPage - 1) * seasonPagination.itemsPerPage + 1 }} to 
                            {{ Math.min(seasonPagination.currentPage * seasonPagination.itemsPerPage, filteredSeasons.length) }} 
                            of {{ filteredSeasons.length }} seasons</span>
                        <select v-model="seasonPagination.itemsPerPage" @change="changeSeasonItemsPerPage(seasonPagination.itemsPerPage)" class="items-per-page-select">
                            <option v-for="option in itemsPerPageOptions" :key="option" :value="option">{{ option }} per page</option>
                        </select>
                    </div>
                    <div class="pagination-controls">
                        <button 
                            @click="changeSeasonPage(seasonPagination.currentPage - 1)" 
                            :disabled="seasonPagination.currentPage <= 1"
                            class="btn btn-sm btn-secondary"
                        >
                            Previous
                        </button>
                        
                        <template v-for="page in seasonPageNumbers" :key="page">
                            <button 
                                v-if="typeof page === 'number'"
                                @click="changeSeasonPage(page)" 
                                :class="['btn', 'btn-sm', page === seasonPagination.currentPage ? 'btn-primary' : 'btn-secondary']"
                            >
                                {{ page }}
                            </button>
                            <span v-else class="pagination-ellipsis">{{ page }}</span>
                        </template>
                        
                        <button 
                            @click="changeSeasonPage(seasonPagination.currentPage + 1)" 
                            :disabled="seasonPagination.currentPage >= seasonTotalPages"
                            class="btn btn-sm btn-secondary"
                        >
                            Next
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Add/Edit Season Modal -->
        <div v-if="showSeasonModal" class="modal-overlay" @click="closeSeasonModal">
            <div class="modal" @click.stop>
                <div class="modal-header">
                    <h3>{{ isEditingSeason ? 'Edit Season' : 'Create New Season' }}</h3>
                    <button @click="closeSeasonModal" class="close-btn">&times;</button>
                </div>
                <form @submit.prevent="saveSeason" class="modal-body">
                    <div class="form-group">
                        <label for="seasonName">Season Name *</label>
                        <input 
                            id="seasonName" 
                            v-model="seasonForm.name" 
                            type="text" 
                            class="form-control" 
                            required 
                            placeholder="e.g., Premier League 2024"
                        />
                    </div>

                    <div class="form-group">
                        <label for="seasonStartDate">Start Date *</label>
                        <input 
                            id="seasonStartDate" 
                            v-model="seasonForm.startDate" 
                            type="date" 
                            class="form-control" 
                            required 
                            :disabled="isEditingSeason && seasonForm.isActive"
                        />
                        <small v-if="isEditingSeason && seasonForm.isActive" class="form-text text-muted">
                            Cannot change start date for active seasons
                        </small>
                    </div>

                    <div class="form-group">
                        <label for="seasonEndDate">End Date</label>
                        <input 
                            id="seasonEndDate" 
                            v-model="seasonForm.endDate" 
                            type="date" 
                            class="form-control" 
                            :min="seasonForm.startDate"
                        />
                    </div>

                    <div class="modal-footer">
                        <button type="button" @click="closeSeasonModal" class="btn btn-secondary">
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary" :disabled="loading">
                            {{ loading ? 'Saving...' : (isEditingSeason ? 'Save' : 'Create') }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { SeasonService } from '../../services/ticket_service/season_service.js';

// Reactive data
const seasons = ref([]);
const loading = ref(false);

// Filter data
const seasonFilters = ref({
    name: ''
});

// Pagination data
const seasonPagination = ref({
    currentPage: 1,
    itemsPerPage: 25,
    totalItems: 0
});

const itemsPerPageOptions = [10, 25, 50, 100];

// Modal data
const showSeasonModal = ref(false);
const isEditingSeason = ref(false);
const seasonForm = ref({
    idSeason: null,
    name: '',
    year: new Date().getFullYear(),
    startDate: '',
    endDate: '',
    isActive: false
});

// Computed properties
const filteredSeasons = computed(() => {
    const filtered = seasons.value.filter(season => {
        const nameMatch = !seasonFilters.value.name || 
            season.name?.toLowerCase().includes(seasonFilters.value.name.toLowerCase());
        
        return nameMatch;
    });
    
    seasonPagination.value.totalItems = filtered.length;
    return filtered;
});

const paginatedSeasons = computed(() => {
    const start = (seasonPagination.value.currentPage - 1) * seasonPagination.value.itemsPerPage;
    const end = start + seasonPagination.value.itemsPerPage;
    return filteredSeasons.value.slice(start, end);
});

const seasonTotalPages = computed(() => {
    return Math.ceil(filteredSeasons.value.length / seasonPagination.value.itemsPerPage);
});

// Computed properties for pagination display
const seasonPageNumbers = computed(() => {
    const current = seasonPagination.value.currentPage;
    const total = seasonTotalPages.value;
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

// Methods
const loadSeasons = async () => {
    try {
        loading.value = true;
        const response = await SeasonService.getAllSeasons();
        seasons.value = response.value;
    } catch (error) {
        console.error('Error loading seasons:', error);
        alert('Failed to load seasons. Please try again.');
    } finally {
        loading.value = false;
    }
};

const openCreateSeasonModal = () => {
    isEditingSeason.value = false;
    seasonForm.value = {
        idSeason: null,
        name: '',
        startDate: '',
        endDate: '',
        isActive: false
    };
    showSeasonModal.value = true;
};

const editSeason = (season) => {
    isEditingSeason.value = true;
    seasonForm.value = {
        idSeason: season.idSeason,
        name: season.name || '',
        startDate: season.startedAt ? season.startedAt.split('T')[0] : '',
        endDate: season.endedAt ? season.endedAt.split('T')[0] : '',
        isActive: season.isActive
    };
    showSeasonModal.value = true;
};

const closeSeasonModal = () => {
    showSeasonModal.value = false;
    isEditingSeason.value = false;
    seasonForm.value = {
        idSeason: null,
        name: '',
        startDate: '',
        endDate: '',
        isActive: false
    };
};

const saveSeason = async () => {
    try {
        loading.value = true;
        
        const seasonData = {
            name: seasonForm.value.name,
            startedAt: seasonForm.value.startDate,
            endedAt: seasonForm.value.endDate || null
        };

        if (isEditingSeason.value) {
            await SeasonService.updateSeason(seasonForm.value.idSeason, seasonData);
        } else {
            await SeasonService.createSeason(seasonData);
        }

        await loadSeasons();
        closeSeasonModal();
        alert(isEditingSeason.value ? 'Season updated successfully!' : 'Season created successfully!');
    } catch (error) {
        console.error('Error saving season:', error);
        alert('Failed to save season. Please try again.');
    } finally {
        loading.value = false;
    }
};

const deleteSeason = async (seasonId) => {
    // Find the season to check if it's active
    const season = seasons.value.find(s => s.idSeason === seasonId);
    
    if (season && season.isActive) {
        alert('Cannot delete an active season. Please deactivate it first.');
        return;
    }

    if (!confirm('Are you sure you want to delete this season? This action cannot be undone.')) {
        return;
    }

    try {
        loading.value = true;
        await SeasonService.deleteSeason(seasonId);
        await loadSeasons();
        alert('Season deleted successfully!');
    } catch (error) {
        console.error('Error deleting season:', error);
        alert('Failed to delete season. Please try again.');
    } finally {
        loading.value = false;
    }
};

const clearSeasonFilters = () => {
    seasonFilters.value = {
        name: ''
    };
    seasonPagination.value.currentPage = 1;
};

// Pagination methods
const changeSeasonPage = (page) => {
    if (page >= 1 && page <= seasonTotalPages.value) {
        seasonPagination.value.currentPage = page;
    }
};

const changeSeasonItemsPerPage = (itemsPerPage) => {
    seasonPagination.value.itemsPerPage = itemsPerPage;
    seasonPagination.value.currentPage = 1;
};

// Utility methods
const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
};

const extractYear = (dateString) => {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.getFullYear();
};

const getStatusClass = (status) => {
    switch (status?.toLowerCase()) {
        case 'active':
            return 'active';
        case 'inactive':
            return 'inactive';
        case 'completed':
            return 'completed';
        default:
            return 'inactive';
    }
};

// Lifecycle
onMounted(() => {
    loadSeasons();
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

.status-badge.active {
    background-color: #dcfce7;
    color: #166534;
}

.status-badge.inactive {
    background-color: #fef2f2;
    color: #991b1b;
}

.status-badge.completed {
    background-color: #f3f4f6;
    color: #374151;
}

.action-buttons {
    display: flex;
    gap: var(--spacing-sm);
}

.btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
    pointer-events: none;
}

.empty-state {
    padding: var(--spacing-xl);
    text-align: center;
    color: var(--color-text-muted);
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
    .filter-row {
        grid-template-columns: 1fr;
    }
    
    .pagination-container {
        flex-direction: column;
        align-items: stretch;
    }
    
    .pagination-controls {
        justify-content: center;
    }
    
    .section-header {
        flex-direction: column;
        gap: var(--spacing-md);
        align-items: stretch;
    }
    
    .data-table {
        font-size: 0.75rem;
    }
    
    .data-table th,
    .data-table td {
        padding: var(--spacing-sm);
    }
}
</style>
