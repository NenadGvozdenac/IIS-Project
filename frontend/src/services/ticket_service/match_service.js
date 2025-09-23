import axios from 'axios';
import { TICKETS_URL } from '../const_service.js';
import { getUserData } from '../auth_service.js';

export class MatchService {
    static async getMatchesInOurHall() {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/matches/in-our-hall`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getAllMatches() {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/matches`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getMatchById(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/matches/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async createTicketPriceParameter(matchId, parameterData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${TICKETS_URL}/matches/${matchId}/create-price-parameters`, parameterData, {
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

    static async enableTicketsForMatch(matchId) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${TICKETS_URL}/matches/${matchId}/enable-tickets`, {}, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getPurchaseHistory() {
        try {
            const token = localStorage.getItem('token');
            const userData = getUserData();
            const response = await axios.get(`${TICKETS_URL}/PurchaseOffers/purchase-history/${userData.userID}`, {
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
