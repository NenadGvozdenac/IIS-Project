import axios from 'axios';
import { TICKETS_URL } from './const_service.js';

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
}
