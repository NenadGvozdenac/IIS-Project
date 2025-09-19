import axios from 'axios';

const API_BASE_URL = 'https://localhost:5005/api/neo4j/Reports';

export class ReportsService {
    static async getMatchTicketSalesReport() {
        try {
            const response = await axios.get(`${API_BASE_URL}/match-ticket-sales`, {
                headers: {
                    'accept': '*/*'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching match ticket sales report:', error);
            throw error;
        }
    }

    static async getCustomerSpendingReport() {
        try {
            const response = await axios.get(`${API_BASE_URL}/customer-spending`, {
                headers: {
                    'accept': '*/*'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching customer spending report:', error);
            throw error;
        }
    }

    static async getSectorAnalysisReport() {
        try {
            const response = await axios.get(`${API_BASE_URL}/sector-analysis`, {
                headers: {
                    'accept': '*/*'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching sector analysis report:', error);
            throw error;
        }
    }

    static async getAllReports() {
        try {
            const [matchSales, customerSpending, sectorAnalysis] = await Promise.all([
                this.getMatchTicketSalesReport(),
                this.getCustomerSpendingReport(),
                this.getSectorAnalysisReport()
            ]);

            return {
                matchSales,
                customerSpending, 
                sectorAnalysis
            };
        } catch (error) {
            console.error('Error fetching all reports:', error);
            throw error;
        }
    }
}