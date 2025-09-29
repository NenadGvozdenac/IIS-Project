import axios from 'axios';
import { TICKETS_URL } from '../const_service.js';
import { getUserData } from '../auth_service.js';

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

    static async addToCart(purchaseOfferId) {
        try {
            const token = localStorage.getItem('token');
            const userData = getUserData();
            const response = await axios.post(`${TICKETS_URL}/purchaseoffers/${purchaseOfferId}/add-to-cart`, 
                { userId: userData.userID }, 
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

    static async removeFromCart(purchaseOfferId) {
        try {
            const token = localStorage.getItem('token');
            const userData = getUserData();
            console.log('Removing from cart:', purchaseOfferId, 'for user:', userData.userID);
            const response = await axios.delete(`${TICKETS_URL}/purchaseoffers/${purchaseOfferId}/remove-from-cart`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                data: {
                    userId: userData.userID
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async checkout(cartId, idCreditCard) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${TICKETS_URL}/carts/${cartId}/purchase`, 
                { idCreditCard }, 
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
