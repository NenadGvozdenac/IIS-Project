import axios from 'axios';
import { TICKETS_URL } from './const_service.js';
import { getUserData } from './auth_service.js';

export class CreditCardService {
    static async getCreditCardsByUser() {
        try {
            const token = localStorage.getItem('token');
            const userData = getUserData();
            const response = await axios.get(`${TICKETS_URL}/creditcards/user/${userData.userID}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getCreditCardById(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/creditcards/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async createCreditCard(cardData) {
        try {
            const token = localStorage.getItem('token');
            const userData = getUserData();
            
            const requestData = {
                ...cardData,
                idUser: userData.userID,
                cvv: parseInt(cardData.cvv, 10)
            };
            
            const response = await axios.post(`${TICKETS_URL}/creditcards`, requestData, {
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

    static async updateCreditCard(id, cardData) {
        try {
            const token = localStorage.getItem('token');
            
            const requestData = {
                ...cardData,
                cvv: parseInt(cardData.cvv, 10)
            };
            
            const response = await axios.put(`${TICKETS_URL}/creditcards/${id}`, requestData, {
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

    static async deleteCreditCard(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.delete(`${TICKETS_URL}/creditcards/${id}`, {
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
