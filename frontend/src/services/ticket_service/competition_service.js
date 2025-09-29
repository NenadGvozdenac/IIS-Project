import axios from 'axios';
import { TICKETS_URL } from '../const_service.js';

export class CompetitionService {
    static async getAllCompetitions() {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/Competitions`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getCompetitionById(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/Competitions/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async createCompetition(competitionData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${TICKETS_URL}/Competitions`, competitionData, {
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

    static async updateCompetition(id, competitionData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.put(`${TICKETS_URL}/Competitions/${id}`, competitionData, {
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

    static async deleteCompetition(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.delete(`${TICKETS_URL}/Competitions/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }
}
