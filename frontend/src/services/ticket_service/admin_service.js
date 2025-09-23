import axios from 'axios';

import { TICKETS_URL } from '../const_service.js';

export class AdminService {
    static async getSeasons() {
        try {
            const response = await axios.get(`${TICKETS_URL}/Seasons`, {
                headers: {
                    'accept': '*/*'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching seasons:', error);
            throw error;
        }
    }

    static async getMatches() {
        try {
            const response = await axios.get(`${TICKETS_URL}/Matches`, {
                headers: {
                    'accept': '*/*'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching matches:', error);
            throw error;
        }
    }

    static async getCompetitions() {
        try {
            const response = await axios.get(`${TICKETS_URL}/Competitions`, {
                headers: {
                    'accept': '*/*'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching competitions:', error);
            throw error;
        }
    }
}
