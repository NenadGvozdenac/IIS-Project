import axios from 'axios';
import { TICKETS_URL } from './const_service.js';
import { getUserData } from './auth_service.js';

export class CartService {
    static async getCurrentCart() {
        try {
            const token = localStorage.getItem('token');
            const userData = getUserData();
            const response = await axios.get(`${TICKETS_URL}/carts/user/${userData.userID}/current`, {
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
