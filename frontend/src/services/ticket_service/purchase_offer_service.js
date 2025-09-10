import axios from 'axios';
import { TICKETS_URL } from '../const_service.js';

export class PurchaseOfferService {
    static async getIndividualTicketOffer(zone, row, seat, direction, match) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(
                `${TICKETS_URL}/PurchaseOffers/individual-ticket/zone/${zone}/row/${row}/seat/${seat}/direction/${direction}/match/${match}`,
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

    static async checkSeasonTicketConflict(seatId, matchDate) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(
                `${TICKETS_URL}/PurchaseOffers/season-ticket-conflict/seat/${seatId}/match-date/${matchDate}`,
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

    static async getSeasonTickets() {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(
                `${TICKETS_URL}/PurchaseOffers/season-tickets`,
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

    static async getSeasonTicketBySeat(zoneId, row, number, direction, seasonId) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(
                `${TICKETS_URL}/PurchaseOffers/season-ticket/zone/${zoneId}/row/${row}/seat/${number}/direction/${direction}/season/${seasonId}`,
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

    static async getExistingSeasonTickets(zoneId, seasonId) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(
                `${TICKETS_URL}/PurchaseOffers/existing-season-tickets/zone/${zoneId}/season/${seasonId}`,
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
}
