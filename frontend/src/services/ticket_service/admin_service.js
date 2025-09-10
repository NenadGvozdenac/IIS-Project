import axios from 'axios';

const API_BASE_URL = 'https://localhost:5005/api';

export class AdminService {
    static async getSeasons() {
        try {
            const response = await axios.get(`${API_BASE_URL}/Seasons`, {
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
            const response = await axios.get(`${API_BASE_URL}/Matches`, {
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
            const response = await axios.get(`${API_BASE_URL}/Competitions`, {
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
