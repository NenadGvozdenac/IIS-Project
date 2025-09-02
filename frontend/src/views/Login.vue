<template>
  <div class="login-page">
    <div class="container">
      <div class="login-container">
        <div class="login-card">
          <div class="login-header">
            <h1>Welcome Back</h1>
            <p>Sign in to your Sports Hub account</p>
          </div>

          <form @submit.prevent="handleLogin" class="login-form">
            <div v-if="errorMessage" class="alert alert-error">
              {{ errorMessage }}
            </div>

            <div class="form-group">
              <label for="email" class="form-label">Email Address</label>
              <input
                id="email"
                v-model="form.email"
                type="email"
                class="form-input"
                :class="{ 'error': errors.email }"
                placeholder="Enter your email"
                required
              />
              <span v-if="errors.email" class="error-text">{{ errors.email }}</span>
            </div>

            <div class="form-group">
              <label for="password" class="form-label">Password</label>
              <input
                id="password"
                v-model="form.password"
                type="password"
                class="form-input"
                :class="{ 'error': errors.password }"
                placeholder="Enter your password"
                required
              />
              <span v-if="errors.password" class="error-text">{{ errors.password }}</span>
            </div>

            <button 
              type="submit" 
              class="btn btn-primary w-full"
              :disabled="loading"
            >
              {{ loading ? 'Signing in...' : 'Sign In' }}
            </button>
          </form>

          <div class="login-footer">
            <p>
              Don't have an account? 
              <router-link to="/register" class="link-primary">Sign up here</router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'

const router = useRouter()
const { login } = useAuth()

const loading = ref(false)
const errorMessage = ref('')

const form = reactive({
  email: '',
  password: ''
})

const errors = reactive({
  email: '',
  password: ''
})

const validateForm = () => {
  errors.email = ''
  errors.password = ''
  
  if (!form.email) {
    errors.email = 'Email is required'
    return false
  }
  
  if (!form.password) {
    errors.password = 'Password is required'
    return false
  }
  
  if (form.password.length < 6) {
    errors.password = 'Password must be at least 6 characters'
    return false
  }
  
  return true
}

const handleLogin = async () => {
  if (!validateForm()) return
  
  loading.value = true
  errorMessage.value = ''
  
  try {
    const result = await login(form)
    
    if (result.success) {
      router.push('/dashboard')
    } else {
      errorMessage.value = result.message || 'Login failed. Please try again.'
    }
  } catch (error) {
    errorMessage.value = 'An unexpected error occurred. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page {
  min-height: calc(100vh - 4rem);
  display: flex;
  align-items: center;
  background-color: var(--color-surface);
  padding: var(--spacing-xl) 0;
}

.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
}

.login-card {
  background: white;
  padding: var(--spacing-2xl);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-lg);
  width: 100%;
  max-width: 400px;
}

.login-header {
  text-align: center;
  margin-bottom: var(--spacing-xl);
}

.login-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.login-header p {
  color: var(--color-text-light);
  font-size: 0.875rem;
}

.login-form {
  margin-bottom: var(--spacing-xl);
}

.form-input.error {
  border-color: var(--color-error);
}

.error-text {
  color: var(--color-error);
  font-size: 0.75rem;
  margin-top: var(--spacing-xs);
  display: block;
}

.w-full {
  width: 100%;
}

.login-footer {
  text-align: center;
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

.login-footer p {
  color: var(--color-text-light);
  font-size: 0.875rem;
}

.link-primary {
  color: var(--color-primary);
  font-weight: 500;
  text-decoration: none;
}

.link-primary:hover {
  color: var(--color-primary-hover);
  text-decoration: underline;
}

/* Responsive */
@media (max-width: 768px) {
  .login-page {
    padding: var(--spacing-md) 0;
  }
  
  .login-card {
    margin: 0 var(--spacing-md);
    padding: var(--spacing-xl);
  }
  
  .login-header h1 {
    font-size: 1.75rem;
  }
}
</style>
