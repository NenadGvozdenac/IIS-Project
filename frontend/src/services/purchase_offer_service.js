import axios from 'axios';
import { TICKETS_URL } from './const_service.js';

export class PurchaseOfferService {
    static async getIndividualTicketOffer(zone, row, seat, match) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(
                `${TICKETS_URL}/PurchaseOffers/individual-ticket/zone/${zone}/row/${row}/seat/${seat}/match/${match}`,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            );
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async addToCart(purchaseOfferId, userId) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(
                `${TICKETS_URL}/PurchaseOffers/${purchaseOfferId}/add-to-cart`,
                { userId: userId },
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
