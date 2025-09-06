import axios from 'axios';
import { TICKETS_URL } from './const_service.js';

export class SeatService {
    static async getAllSeats() {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/seats`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getSeatById(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/seats/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async createSeat(seatData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post(`${TICKETS_URL}/seats`, seatData, {
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

    static async updateSeat(id, seatData) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.put(`${TICKETS_URL}/seats/${id}`, seatData, {
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

    static async deleteSeat(id) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.delete(`${TICKETS_URL}/seats/${id}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            return response.data;
        } catch (error) {
            throw error.response?.data || error;
        }
    }

    static async getSeatsByZone(zoneId) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/seats/zone/${zoneId}`, {
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
