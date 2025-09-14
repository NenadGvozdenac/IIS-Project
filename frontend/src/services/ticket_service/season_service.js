import axios from 'axios';
import { TICKETS_URL } from '../const_service.js';

export class SeasonService {
    static async getAllSeasons() {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/Seasons`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getSeasonById(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/Seasons/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async createSeason(seasonData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${TICKETS_URL}/Seasons`, seasonData, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async updateSeason(id, seasonData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.put(`${TICKETS_URL}/seasons/${id}`, seasonData, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async deleteSeason(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.delete(`${TICKETS_URL}/seasons/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async enableSeasonTickets(seasonId, zonePrices) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${TICKETS_URL}/Seasons/${seasonId}/enable-tickets`, 
                { zonePrices }, 
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }
}
