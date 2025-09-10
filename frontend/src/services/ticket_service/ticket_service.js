import axios from 'axios';
import { TICKETS_URL } from '../const_service.js';

export class TicketService {
    static async printTicket(ticketId) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(
                `${TICKETS_URL}/tickets/${ticketId}/print`,
                {},
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

    static async getTicketPrintTemplate(ticketData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(
                `${TICKETS_URL}/tickets/print-template`,
                ticketData,
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
