import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import Dashboard from '../views/Dashboard.vue'
import CustomerDashboard from '../views/customer/Dashboard.vue'
import Profile from '../views/customer/Profile.vue'
import MyTickets from '../views/customer/MyTickets.vue'
import Cart from '../views/customer/Cart.vue'
import SeatSelection from '../views/customer/SeatSelection.vue'
import DataAnalysis from '../views/analyst/DataAnalysis.vue'
import TeamDetail from '../views/analyst/TeamDetail.vue'
import TeamEdit from '../views/analyst/TeamEdit.vue'
import TeamAdd from '../views/analyst/TeamAdd.vue'
import MatchesOverview from '../views/analyst/MatchesOverview.vue'
import MatchDetail from '../views/analyst/MatchDetail.vue'
import ZonesSeats from '../views/administrator/ZonesSeats.vue'
import { getUserData } from '../services/auth_service.js'
import Matches from '../views/teammanager/Matches.vue'
import MatchesView from '../views/clubmanager/MatchesView.vue'
import MatchDetails from '../views/teammanager/MatchDetails.vue'
import MatchDetailsClubManager from '../views/clubmanager/MatchDetailsClubManager.vue'
import Players from '../views/teammanager/Players.vue'
import TravelInfos from '../views/clubmanager/TravelInfos.vue'
import ScoutDashboard from '../views/scout/ScoutDashboard.vue'
import CreatePlayer from '../views/scout/CreatePlayer.vue'
import Metrics from '../views/scout/Metrics.vue'
import Sessions from '../views/scout/Sessions.vue'
import PlayerAnalysis from '../views/scout/PlayerAnalysis.vue'
import PlayerRecommendations from '../views/scout/PlayerRecommendations.vue'
import PlayerProfile from '../views/scout/PlayerProfile.vue'
import EditPlayer from '../views/scout/EditPlayer.vue'

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
    path: '/seat-selection/:matchId',
    name: 'SeatSelection',
    component: SeatSelection,
    meta: { requiresAuth: true, requiresRole: 'customer' }
  },
  {
    path: '/analyst',
    name: 'DataAnalysis',
    component: DataAnalysis,
    meta: { requiresAuth: true, requiresRole: 'analyst' }
  },
  {
    path: '/analyst/team/add',
    name: 'TeamAdd',
    component: TeamAdd,
    meta: { requiresAuth: true, requiresRole: 'analyst' }
  },
  {
    path: '/analyst/team/:id',
    name: 'TeamDetail',
    component: TeamDetail,
    meta: { requiresAuth: true, requiresRole: 'analyst' }
  },
  {
    path: '/analyst/team/:id/edit',
    name: 'TeamEdit',
    component: TeamEdit,
    meta: { requiresAuth: true, requiresRole: 'analyst' }
  },
  {
    path: '/analyst/matches',
    name: 'Matches',
    component: MatchesOverview,
    meta: { requiresAuth: true, requiresRole: 'analyst' }
  },
  {
    path: '/analyst/matches/:id',
    name: 'MatchDetail',
    component: MatchDetail,
    meta: { requiresAuth: true, requiresRole: 'analyst' }
  },
  {
    path: '/admin/zones-seats',
    name: 'ZonesSeats',
    component: ZonesSeats,
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/team-manager/matches',
    name: 'TeamManagerMatches',
    component: Matches,
    meta: { requiresAuth: true, requiresRole: 'team manager' }
  },
  {
    path: '/club-manager/matches',
    name: 'ClubManagerMatches',
    component: MatchesView,
    meta: { requiresAuth: true, requiresRole: 'club manager' }
  },
  {
    path: '/team-manager/matches/:id',
    name: 'MatchDetails',
    component: MatchDetails,
    meta: { requiresAuth: true, requiresRole: 'team manager' }
  },
  {
    path: '/club-manager/matches/:id',
    name: 'MatchDetailsClubManager',
    component: MatchDetailsClubManager,
    meta: { requiresAuth: true, requiresRole: 'club manager' }
  },
  {
    path: '/team-manager/players',
    name: 'TeamManagerPlayers',
    component: Players,
    meta: { requiresAuth: true, requiresRole: 'team manager' }
  },
  {
    path: '/club-manager/travelinfo',
    name: 'ClubManagerTravel',
    component: TravelInfos,
    meta: { requiresAuth: true, requiresRole: 'club manager' }
  },
  {
    path: '/scout',
    name: 'ScoutDashboard',
    component: ScoutDashboard,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/create-player',
    name: 'CreatePlayer',
    component: CreatePlayer,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/metrics',
    name: 'Metrics',
    component: Metrics,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/sessions',
    name: 'Sessions',
    component: Sessions,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/analysis',
    name: 'PlayerAnalysis',
    component: PlayerAnalysis,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/recommendations',
    name: 'PlayerRecommendations',
    component: PlayerRecommendations,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/player/:id',
    name: 'PlayerProfile',
    component: PlayerProfile,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/player/:id/edit',
    name: 'EditPlayer',
    component: EditPlayer,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/player/:id',
    name: 'PlayerProfile',
    component: PlayerProfile,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  },
  {
    path: '/scout/player/:id/edit',
    name: 'EditPlayer',
    component: EditPlayer,
    meta: { requiresAuth: true, requiresRole: 'scouting manager' }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard to check authentication
router.beforeEach((to, _, next) => {
  const token = localStorage.getItem('token')
  const userData = getUserData()
  
  // If user is logged in and trying to access root or login/register, redirect to their dashboard
  if ((to.path === '/' || to.path === '/login' || to.path === '/register') && token && userData) {
    if (userData.userRole === 'scouting manager') {
      next('/scout')
      return
    } else if (userData.userRole === 'customer') {
      next('/customer-dashboard')
      return
    } else if (userData.userRole === 'team manager') {
      next('/team-manager/matches')
      return
    } else if (userData.userRole === 'club manager') {
      next('/club-manager/matches')
      return
    } else {
      next('/dashboard')
      return
    }
  }

  // If scout tries to access general dashboard, redirect to scout dashboard
  if (to.path === '/dashboard' && token && userData && userData.userRole === 'scouting manager') {
    next('/scout')
    return
  }
  
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else if (to.meta.requiresAdmin) {
    if (!userData || userData.userRole !== 'admin') {
      next('/dashboard') // Redirect to dashboard if not admin
    } else {
      next()
    }
  } 
  else if (to.meta.requiresRole) {
    if (!userData || userData.userRole !== to.meta.requiresRole) {
      // Redirect to appropriate dashboard based on user role
      if (userData && userData.userRole === 'scouting manager') {
        next('/scout')
      } else if (userData && userData.userRole === 'customer') {
        next('/customer-dashboard')
      } else if (userData && userData.userRole === 'team manager') {
        next('/team-manager/matches')
      } else if (userData && userData.userRole === 'club manager') {
        next('/club-manager/matches')
      } else {
        next('/dashboard')
      }
    } else {
      next()
    }
  }
  else {
    next()
  }
})

export default router
