import axios from 'axios'; 

import { AUTH_URL } from './const_service.js';

export class AuthService {
    static async login(email, password) {
        try {
            const response = await axios.post(`${AUTH_URL}/login`, { email, password });
            localStorage.setItem('token', response.data.data.token);
            return response.data;
        } catch (error) {
            throw error.response.data;
        }
    }

    static async register(name, surname, email, password, phone, confirm_password, user_type) {
        try {
            const response = await axios.post(`${AUTH_URL}/register`, {
                name,
                surname,
                email,
                phone,
                password,
                confirm_password,
                user_type
            });
            localStorage.setItem('token', response.data.data.token);
            return response.data;
        } catch (error) {
            throw error.response.data;
        }
    }

    static decode(jwt) {
        if (!jwt) return null;
        const payload = jwt.split('.')[1];
        if (!payload) return null;
        try {
            // Base64 decode (handle padding)
            const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));
            const decoded = JSON.parse(jsonPayload);
            return {
                userRole: decoded.role || decoded.userRole,
                userEmail: decoded.email || decoded.userEmail,
                userName: decoded.name || decoded.userName,
                userID: decoded.id || decoded.userID
            };
        } catch (e) {
            return null;
        }
    }
}

export function getUserData() {
    const token = localStorage.getItem('token');
    if (!token) return null;

    const userData = AuthService.decode(token);
    return userData;
}