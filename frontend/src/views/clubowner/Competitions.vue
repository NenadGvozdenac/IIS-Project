<template>
    <div class="admin-zones-seats">
        <div class="container">
            <h1 class="page-title">Competitions Management</h1>

            <div class="section-header">
                <h2>Competition Management</h2>
                <button @click="openCreateCompetitionModal" class="btn btn-primary">
                    <i class="icon-plus"></i>
                    New Competition
                </button>
            </div>

            <!-- Competition Filters -->
            <div class="filters-container">
                <div class="filter-row">
                    <div class="filter-group">
                        <label for="competitionNameFilter">Filter by Name:</label>
                        <input 
                            id="competitionNameFilter" 
                            v-model="competitionFilters.name" 
                            type="text" 
                            class="filter-input" 
                            placeholder="Search competition name..."
                        />
                    </div>
                    <div class="filter-group">
                        <button @click="clearCompetitionFilters" class="btn btn-secondary btn-sm">Clear Filters</button>
                    </div>
                </div>
            </div>

            <!-- Competitions Table -->
            <div class="table-container">
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                            <th>Planned Matches</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="competition in paginatedCompetitions" :key="competition.idCompetition">
                            <td>{{ competition.name }}</td>
                            <td>{{ formatDate(competition.startedAt) }}</td>
                            <td>{{ formatDate(competition.endedAt) }}</td>
                            <td>{{ competition.numberOfMatches || 'N/A' }}</td>
                            <td>
                                <span class="status-badge" :class="getStatusClass(competition.isActive ? 'active' : 'inactive')">
                                    {{ competition.isActive ? 'Active' : 'Inactive' }}
                                </span>
                            </td>
                            <td>
                                <div class="action-buttons">
                                    <button @click="editCompetition(competition)" class="btn btn-sm btn-secondary">
                                        Edit
                                    </button>
                                    <button 
                                        @click="deleteCompetition(competition.idCompetition)" 
                                        class="btn btn-sm btn-danger"
                                        :disabled="competition.isActive"
                                        :title="competition.isActive ? 'Cannot delete active competition' : 'Delete competition'"
                                    >
                                        Delete
                                    </button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>

                <!-- Competition Pagination -->
                <div class="pagination-container">
                    <div class="pagination-info">
                        <span>Showing {{ (competitionPagination.currentPage - 1) * competitionPagination.itemsPerPage + 1 }} to 
                            {{ Math.min(competitionPagination.currentPage * competitionPagination.itemsPerPage, filteredCompetitions.length) }} 
                            of {{ filteredCompetitions.length }} competitions</span>
                        <select v-model="competitionPagination.itemsPerPage" @change="changeCompetitionItemsPerPage(competitionPagination.itemsPerPage)" class="items-per-page-select">
                            <option v-for="option in itemsPerPageOptions" :key="option" :value="option">{{ option }} per page</option>
                        </select>
                    </div>
                    <div class="pagination-controls">
                        <button 
                            @click="changeCompetitionPage(competitionPagination.currentPage - 1)" 
                            :disabled="competitionPagination.currentPage <= 1"
                            class="btn btn-sm btn-secondary"
                        >
                            Previous
                        </button>
                        
                        <template v-for="page in competitionPageNumbers" :key="page">
                            <button 
                                v-if="typeof page === 'number'"
                                @click="changeCompetitionPage(page)" 
                                :class="['btn', 'btn-sm', page === competitionPagination.currentPage ? 'btn-primary' : 'btn-secondary']"
                            >
                                {{ page }}
                            </button>
                            <span v-else class="pagination-ellipsis">{{ page }}</span>
                        </template>
                        
                        <button 
                            @click="changeCompetitionPage(competitionPagination.currentPage + 1)" 
                            :disabled="competitionPagination.currentPage >= competitionTotalPages"
                            class="btn btn-sm btn-secondary"
                        >
                            Next
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Add/Edit Competition Modal -->
        <div v-if="showCompetitionModal" class="modal-overlay" @click="closeCompetitionModal">
            <div class="modal" @click.stop>
                <div class="modal-header">
                    <h3>{{ isEditingCompetition ? 'Edit Competition' : 'Create New Competition' }}</h3>
                    <button @click="closeCompetitionModal" class="close-btn">&times;</button>
                </div>
                <form @submit.prevent="saveCompetition" class="modal-body">
                    <div class="form-group">
                        <label for="competitionName">Competition Name *</label>
                        <input 
                            id="competitionName" 
                            v-model="competitionForm.name" 
                            type="text" 
                            class="form-control" 
                            required 
                            placeholder="e.g., UEFA Champions League"
                        />
                    </div>

                    <div class="form-group">
                        <label for="competitionStartDate">Start Date</label>
                        <input 
                            id="competitionStartDate" 
                            v-model="competitionForm.startDate" 
                            type="date" 
                            class="form-control" 
                            :disabled="isEditingCompetition && competitionForm.isActive"
                        />
                        <small v-if="isEditingCompetition && competitionForm.isActive" class="form-text text-muted">
                            Cannot change start date for active competitions
                        </small>
                    </div>

                    <div class="form-group">
                        <label for="competitionEndDate">End Date</label>
                        <input 
                            id="competitionEndDate" 
                            v-model="competitionForm.endDate" 
                            type="date" 
                            class="form-control" 
                            :min="competitionForm.startDate"
                        />
                    </div>

                    <div class="form-group">
                        <label for="competitionNumberOfMatches">Number of Planned Matches</label>
                        <input 
                            id="competitionNumberOfMatches" 
                            v-model.number="competitionForm.numberOfMatches" 
                            type="number" 
                            class="form-control" 
                            required 
                            min="1"
                            placeholder="e.g., 38"
                        />
                    </div>

                    <div class="modal-footer">
                        <button type="button" @click="closeCompetitionModal" class="btn btn-secondary">
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary" :disabled="loading">
                            {{ loading ? 'Saving...' : (isEditingCompetition ? 'Save' : 'Create') }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { CompetitionService } from '../../services/ticket_service/competition_service.js';

// Reactive data
const competitions = ref([]);
const loading = ref(false);

// Filter data
const competitionFilters = ref({
    name: ''
});

// Pagination data
const competitionPagination = ref({
    currentPage: 1,
    itemsPerPage: 25,
    totalItems: 0
});

const itemsPerPageOptions = [10, 25, 50, 100];

// Modal data
const showCompetitionModal = ref(false);
const isEditingCompetition = ref(false);
const competitionForm = ref({
    idCompetition: null,
    name: '',
    startDate: '',
    endDate: '',
    numberOfMatches: 0,
    isActive: false
});

// Computed properties
const filteredCompetitions = computed(() => {
    const filtered = competitions.value.filter(competition => {
        const nameMatch = !competitionFilters.value.name || 
            competition.name?.toLowerCase().includes(competitionFilters.value.name.toLowerCase());
        
        return nameMatch;
    });
    
    competitionPagination.value.totalItems = filtered.length;
    return filtered;
});

const paginatedCompetitions = computed(() => {
    const start = (competitionPagination.value.currentPage - 1) * competitionPagination.value.itemsPerPage;
    const end = start + competitionPagination.value.itemsPerPage;
    return filteredCompetitions.value.slice(start, end);
});

const competitionTotalPages = computed(() => {
    return Math.ceil(filteredCompetitions.value.length / competitionPagination.value.itemsPerPage);
});

// Computed properties for pagination display
const competitionPageNumbers = computed(() => {
    const current = competitionPagination.value.currentPage;
    const total = competitionTotalPages.value;
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
const loadCompetitions = async () => {
    try {
        loading.value = true;
        const response = await CompetitionService.getAllCompetitions();
        competitions.value = response.value;
    } catch (error) {
        console.error('Error loading competitions:', error);
        alert('Failed to load competitions. Please try again.');
    } finally {
        loading.value = false;
    }
};

const openCreateCompetitionModal = () => {
    isEditingCompetition.value = false;
    competitionForm.value = {
        idCompetition: null,
        name: '',
        startDate: '',
        endDate: '',
        numberOfMatches: 0,
        isActive: false
    };
    showCompetitionModal.value = true;
};

const editCompetition = (competition) => {
    isEditingCompetition.value = true;
    competitionForm.value = {
        idCompetition: competition.idCompetition,
        name: competition.name || '',
        startDate: competition.startedAt ? competition.startedAt.split('T')[0] : '',
        endDate: competition.endedAt ? competition.endedAt.split('T')[0] : '',
        numberOfMatches: competition.numberOfMatches || 0,
        isActive: competition.isActive
    };
    showCompetitionModal.value = true;
};

const closeCompetitionModal = () => {
    showCompetitionModal.value = false;
    isEditingCompetition.value = false;
    competitionForm.value = {
        idCompetition: null,
        name: '',
        startDate: '',
        endDate: '',
        numberOfMatches: 0,
        isActive: false
    };
};

const saveCompetition = async () => {
    try {
        loading.value = true;
        
        const competitionData = {
            name: competitionForm.value.name,
            startedAt: competitionForm.value.startDate || null,
            endedAt: competitionForm.value.endDate || null,
            numberOfMatches: competitionForm.value.numberOfMatches
        };

        if (isEditingCompetition.value) {
            await CompetitionService.updateCompetition(competitionForm.value.idCompetition, competitionData);
        } else {
            await CompetitionService.createCompetition(competitionData);
        }

        await loadCompetitions();
        closeCompetitionModal();
        alert(isEditingCompetition.value ? 'Competition updated successfully!' : 'Competition created successfully!');
    } catch (error) {
        console.error('Error saving competition:', error);
        alert('Failed to save competition. Please try again.');
    } finally {
        loading.value = false;
    }
};

const deleteCompetition = async (competitionId) => {
    // Find the competition to check if it's active
    const competition = competitions.value.find(c => c.idCompetition === competitionId);
    
    if (competition && competition.isActive) {
        alert('Cannot delete an active competition. Please deactivate it first.');
        return;
    }

    if (!confirm('Are you sure you want to delete this competition? This action cannot be undone.')) {
        return;
    }

    try {
        loading.value = true;
        await CompetitionService.deleteCompetition(competitionId);
        await loadCompetitions();
        alert('Competition deleted successfully!');
    } catch (error) {
        console.error('Error deleting competition:', error);
        alert('Failed to delete competition. Please try again.');
    } finally {
        loading.value = false;
    }
};

const clearCompetitionFilters = () => {
    competitionFilters.value = {
        name: ''
    };
    competitionPagination.value.currentPage = 1;
};

// Pagination methods
const changeCompetitionPage = (page) => {
    if (page >= 1 && page <= competitionTotalPages.value) {
        competitionPagination.value.currentPage = page;
    }
};

const changeCompetitionItemsPerPage = (itemsPerPage) => {
    competitionPagination.value.itemsPerPage = itemsPerPage;
    competitionPagination.value.currentPage = 1;
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

const getStatusClass = (status) => {
    switch (status?.toLowerCase()) {
        case 'active':
            return 'active';
        case 'inactive':
            return 'inactive';
        case 'completed':
            return 'completed';
        case 'upcoming':
            return 'upcoming';
        default:
            return 'inactive';
    }
};

// Lifecycle
onMounted(() => {
    loadCompetitions();
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

.status-badge.upcoming {
    background-color: #dcfce7;
    color: #166534;
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
