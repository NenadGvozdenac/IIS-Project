import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import Dashboard from '../views/Dashboard.vue'
import CustomerDashboard from '../views/customer/Dashboard.vue'
import Profile from '../views/customer/Profile.vue'
import MyTickets from '../views/customer/MyTickets.vue'
import Cart from '../views/customer/Cart.vue'
import DataAnalysis from '../views/analyst/DataAnalysis.vue'
import TeamDetail from '../views/analyst/TeamDetail.vue'
import TeamEdit from '../views/analyst/TeamEdit.vue'
import TeamAdd from '../views/analyst/TeamAdd.vue'
import ZonesSeats from '../views/administrator/ZonesSeats.vue'
import { getUserData } from '../services/auth_service.js'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/register',
    name: 'Register',
    component: Register
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: Dashboard,
    meta: { requiresAuth: true }
  },
  {
    path: '/customer-dashboard',
    name: 'CustomerDashboard',
    component: CustomerDashboard,
    meta: { requiresAuth: true, requiresRole: 'customer' }
  },
  {
    path: '/profile',
    name: 'Profile',
    component: Profile,
    meta: { requiresAuth: true }
  },
  {
    path: '/my-tickets',
    name: 'MyTickets',
    component: MyTickets,
    meta: { requiresAuth: true, requiresRole: 'customer' }
  },
  {
    path: '/cart',
    name: 'Cart',
    component: Cart,
    meta: { requiresAuth: true, requiresRole: 'customer' }
  },
  {
    path: '/analyst',
    name: 'DataAnalysis',
    component: DataAnalysis,
    meta: { requiresAuth: true }
  },
  {
    path: '/analyst/team/add',
    name: 'TeamAdd',
    component: TeamAdd,
    meta: { requiresAuth: true }
  },
  {
    path: '/analyst/team/:id',
    name: 'TeamDetail',
    component: TeamDetail,
    meta: { requiresAuth: true }
  },
  {
    path: '/analyst/team/:id/edit',
    name: 'TeamEdit',
    component: TeamEdit,
    meta: { requiresAuth: true }
  },
  {
    path: '/admin/zones-seats',
    name: 'ZonesSeats',
    component: ZonesSeats,
    meta: { requiresAuth: true, requiresAdmin: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard to check authentication
router.beforeEach((to, _, next) => {
  const token = localStorage.getItem('token')
  
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else if (to.meta.requiresAdmin) {
    const userData = getUserData()
    if (!userData || userData.userRole !== 'admin') {
      next('/dashboard') // Redirect to dashboard if not admin
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router
