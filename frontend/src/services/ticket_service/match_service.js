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
