import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import Dashboard from '../views/Dashboard.vue'
import DataAnalysis from '../views/analyst/DataAnalysis.vue'
import TeamDetail from '../views/analyst/TeamDetail.vue'
import TeamEdit from '../views/analyst/TeamEdit.vue'
import TeamAdd from '../views/analyst/TeamAdd.vue'

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
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard to check authentication
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('authToken')
  
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else {
    next()
  }
})

export default router
