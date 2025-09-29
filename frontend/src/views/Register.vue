<template>
  <div class="register-page">
    <div class="container">
      <div class="register-container">
        <div class="register-card">
          <div class="register-header">
            <h1>Create Account</h1>
            <p>Join Sports Hub and start your journey</p>
          </div>

          <form @submit.prevent="handleRegister" class="register-form">
            <div v-if="errorMessage" class="alert alert-error">
              {{ errorMessage }}
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="name" class="form-label">First Name</label>
                <input
                  id="name"
                  v-model="form.name"
                  type="text"
                  class="form-input"
                  :class="{ 'error': errors.name }"
                  placeholder="Enter your first name"
                  required
                />
                <span v-if="errors.name" class="error-text">{{ errors.name }}</span>
              </div>

              <div class="form-group">
                <label for="surname" class="form-label">Last Name</label>
                <input
                  id="surname"
                  v-model="form.surname"
                  type="text"
                  class="form-input"
                  :class="{ 'error': errors.surname }"
                  placeholder="Enter your last name"
                  required
                />
                <span v-if="errors.surname" class="error-text">{{ errors.surname }}</span>
              </div>
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
              <label for="phone" class="form-label">Phone Number</label>
              <input
                id="phone"
                v-model="form.phone"
                type="tel"
                class="form-input"
                :class="{ 'error': errors.phone }"
                placeholder="Enter your phone number"
                required
              />
              <span v-if="errors.phone" class="error-text">{{ errors.phone }}</span>
            </div>

            <div class="form-group">
              <label for="password" class="form-label">Password</label>
              <input
                id="password"
                v-model="form.password"
                type="password"
                class="form-input"
                :class="{ 'error': errors.password }"
                placeholder="Create a password"
                required
              />
              <span v-if="errors.password" class="error-text">{{ errors.password }}</span>
            </div>

            <div class="form-group">
              <label for="confirmPassword" class="form-label">Confirm Password</label>
              <input
                id="confirmPassword"
                v-model="form.confirm_password"
                type="password"
                class="form-input"
                :class="{ 'error': errors.confirmPassword }"
                placeholder="Confirm your password"
                required
              />
              <span v-if="errors.confirmPassword" class="error-text">{{ errors.confirmPassword }}</span>
            </div>

            <button 
              type="submit" 
              class="btn btn-primary w-full"
              :disabled="loading"
            >
              {{ loading ? 'Creating Account...' : 'Create Account' }}
            </button>
          </form>

          <div class="register-footer">
            <p>
              Already have an account? 
              <router-link to="/login" class="link-primary">Sign in here</router-link>
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
import { AuthService } from '../services/auth_service.js'

const router = useRouter()

const loading = ref(false)
const errorMessage = ref('')

const form = reactive({
  name: '',
  surname: '',
  email: '',
  phone: '',
  password: '',
  confirm_password: ''
})

const errors = reactive({
  name: '',
  surname: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: ''
})

const validateForm = () => {
  // Reset errors
  Object.keys(errors).forEach(key => errors[key] = '')
  
  let isValid = true
  
  if (!form.name.trim()) {
    errors.name = 'First name is required'
    isValid = false
  }
  
  if (!form.surname.trim()) {
    errors.surname = 'Last name is required'
    isValid = false
  }
  
  if (!form.email.trim()) {
    errors.email = 'Email is required'
    isValid = false
  } else if (!/\S+@\S+\.\S+/.test(form.email)) {
    errors.email = 'Please enter a valid email'
    isValid = false
  }
  
  if (!form.phone.trim()) {
    errors.phone = 'Phone number is required'
    isValid = false
  }
  
  if (!form.password) {
    errors.password = 'Password is required'
    isValid = false
  } else if (form.password.length < 6) {
    errors.password = 'Password must be at least 6 characters'
    isValid = false
  }
  
  if (!form.confirm_password) {
    errors.confirmPassword = 'Please confirm your password'
    isValid = false
  } else if (form.password !== form.confirm_password) {
    errors.confirmPassword = 'Passwords do not match'
    isValid = false
  }
  
  return isValid
}

const handleRegister = async () => {
  if (!validateForm()) return
  
  loading.value = true
  errorMessage.value = ''
  
  try {
    const result = await AuthService.register(form.name, form.surname, form.email, form.password, form.phone, form.confirm_password, 'customer')

    if (result.code === 201) {
      router.push('/dashboard')
    } else {
      errorMessage.value = result.message || 'Registration failed. Please try again.'
    }
  } catch (error) {
    errorMessage.value = error.message || 'An unexpected error occurred. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.register-page {
  min-height: calc(100vh - 4rem);
  display: flex;
  align-items: center;
  background-color: var(--color-surface);
  padding: var(--spacing-xl) 0;
}

.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
}

.register-card {
  background: white;
  padding: var(--spacing-2xl);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-lg);
  width: 100%;
  max-width: 500px;
}

.register-header {
  text-align: center;
  margin-bottom: var(--spacing-xl);
}

.register-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-text);
  margin-bottom: var(--spacing-sm);
}

.register-header p {
  color: var(--color-text-light);
  font-size: 0.875rem;
}

.register-form {
  margin-bottom: var(--spacing-xl);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.form-input.error,
.form-select.error {
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

.register-footer {
  text-align: center;
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

.register-footer p {
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
  .register-page {
    padding: var(--spacing-md) 0;
  }
  
  .register-card {
    margin: 0 var(--spacing-md);
    padding: var(--spacing-xl);
  }
  
  .register-header h1 {
    font-size: 1.75rem;
  }
  
  .form-row {
    grid-template-columns: 1fr;
  }
}
</style>
