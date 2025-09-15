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
import SeasonalTickets from '../views/customer/SeasonalTickets.vue'
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
import AdminDashboard from '../views/administrator/Dashboard.vue'
import ClubOwnerDashboard from '../views/clubowner/Dashboard.vue'
import ClubOwnerSeasons from '../views/clubowner/Seasons.vue'
import ClubOwnerCompetitions from '../views/clubowner/Competitions.vue'
import UpcomingMatches from '../views/clubowner/UpcomingMatches.vue'
import SeasonTickets from '../views/clubowner/SeasonTickets.vue'
import PricingStatistics from '../views/clubowner/PricingStatistics.vue'

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
    path: '/admin/dashboard',
    name: 'AdminDashboard',
    component: AdminDashboard,
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  {
    path: '/club-owner/dashboard',
    name: 'ClubOwnerDashboard',
    component: ClubOwnerDashboard,
    meta: { requiresAuth: true, requiresRole: 'club owner' }
  },
  {
    path: '/club-owner/seasons',
    name: 'ClubOwnerSeasons',
    component: ClubOwnerSeasons,
    meta: { requiresAuth: true, requiresRole: 'club owner' }
  },
  {
    path: '/club-owner/competitions',
    name: 'ClubOwnerCompetitions',
    component: ClubOwnerCompetitions,
    meta: { requiresAuth: true, requiresRole: 'club owner' }
  },
  {
    path: '/club-owner/upcoming-matches',
    name: 'UpcomingMatches',
    component: UpcomingMatches,
    meta: { requiresAuth: true, requiresRole: 'club owner' }
  },
  {
    path: '/club-owner/season-tickets',
    name: 'SeasonTickets',
    component: SeasonTickets,
    meta: { requiresAuth: true, requiresRole: 'club owner' }
  },
  {
    path: '/club-owner/pricing-statistics',
    name: 'PricingStatistics',
    component: PricingStatistics,
    meta: { requiresAuth: true, requiresRole: 'club owner' }
  },
  {
    path: '/customer/dashboard',
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
    path: '/seasonal-tickets',
    name: 'SeasonalTickets',
    component: SeasonalTickets,
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
  } 
  else if (to.meta.requiresRole) {
  const userData = getUserData()
  if (!userData || userData.userRole !== to.meta.requiresRole) {
    next('/dashboard')
  } else {
    next()
  }
  }
  else {
    next()
  }
})

export default router
