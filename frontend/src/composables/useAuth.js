import { ref, computed } from 'vue'
import { authAPI } from '../services/api'

// Global authentication state
const authToken = ref(localStorage.getItem('authToken'))
const userInfo = ref(JSON.parse(localStorage.getItem('userInfo') || 'null'))

export function useAuth() {
  const isAuthenticated = computed(() => !!authToken.value)
  const userName = computed(() => userInfo.value?.name || '')

  const login = async (credentials) => {
    try {
      const result = await authAPI.login(credentials)
      
      if (result.success) {
        authToken.value = result.data.data.token
        localStorage.setItem('authToken', result.data.data.token)
        
        // Decode basic user info from token payload
        try {
          const payload = JSON.parse(atob(result.data.data.token.split('.')[1]))
          const user = {
            id: payload.sub,
            name: payload.name,
            email: payload.email,
            userType: payload.user_type
          }
          userInfo.value = user
          localStorage.setItem('userInfo', JSON.stringify(user))
        } catch (decodeError) {
          // Fallback if token decode fails
          const user = {
            email: credentials.email
          }
          userInfo.value = user
          localStorage.setItem('userInfo', JSON.stringify(user))
        }
        
        return { success: true }
      } else {
        return { success: false, message: result.message }
      }
    } catch (error) {
      return { success: false, message: error.response?.data?.message || 'Login failed' }
    }
  }

  const register = async (userData) => {
    try {
      const result = await authAPI.register(userData)
      
      if (result.success) {
        authToken.value = result.data.data.token
        localStorage.setItem('authToken', result.data.data.token)
        
        // Decode basic user info from token payload
        try {
          const payload = JSON.parse(atob(result.data.data.token.split('.')[1]))
          const user = {
            id: payload.sub,
            name: payload.name,
            email: payload.email,
            userType: payload.user_type
          }
          userInfo.value = user
          localStorage.setItem('userInfo', JSON.stringify(user))
        } catch (decodeError) {
          // Fallback if token decode fails
          const user = {
            email: userData.email,
            name: userData.name,
            surname: userData.surname
          }
          userInfo.value = user
          localStorage.setItem('userInfo', JSON.stringify(user))
        }
        
        return { success: true }
      } else {
        return { success: false, message: result.message }
      }
    } catch (error) {
      return { success: false, message: error.response?.data?.message || 'Registration failed' }
    }
  }

  const logout = () => {
    authToken.value = null
    userInfo.value = null
    localStorage.removeItem('authToken')
    localStorage.removeItem('userInfo')
  }

  return {
    isAuthenticated,
    userName,
    userInfo: computed(() => userInfo.value),
    token: computed(() => authToken.value),
    login,
    register,
    logout
  }
}
