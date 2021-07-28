import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login'
import GarbageMan from '../views/GarbageMan'
import Watcher from '../views/Watcher'
import Carrier from '../views/Carrier'
import StationStaff from '../views/StationStaff'
import Admin from '../views/Admin'
import Information from '../components/GarbageMan/GetInfo'

const routes = [
  {
    path: '/',
    name: 'Login',
    component: Login
  },
  {
    path: '/GarbageMan',
    name: 'GarbageMan',
    component: GarbageMan, Information
  },
  {
    path:'/Watcher',
    name:'Watcher',
    component:Watcher,
  },
  {
    path:'/Carrier',
    name:'Carrier',
    component:Carrier
  },
  {
    path:'/StationStaff',
    name:'StationStaff',
    component:StationStaff
  },
  {
    path:'/Admin',
    name:'Admin',
    component:Admin
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
