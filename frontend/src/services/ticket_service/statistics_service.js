import axios from 'axios';
import { MatchService } from './match_service.js';

import { TICKETS_URL } from '../const_service.js';

export class StatisticsService {
    static async getMatchStatistics(matchId) {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${TICKETS_URL}/Statistics/${matchId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'accept': '*/*'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching match statistics:', error);
            throw error;
        }
    }

    static async getFinishedMatches() {
        try {
            // Use MatchService to get matches in our hall
            const response = await MatchService.getMatchesInOurHall();
            
            if (response.isSuccess) {
                // Filter for finished matches only
                const finishedMatches = response.value.filter(match => 
                    new Date(match.scheduledAt) < new Date()
                );
                
                return {
                    ...response,
                    value: finishedMatches
                };
            }
            
            return response;
        } catch (error) {
            console.error('Error fetching finished matches:', error);
            throw error;
        }
    }
}