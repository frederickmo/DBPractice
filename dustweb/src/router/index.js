import {createRouter, createWebHashHistory} from 'vue-router'
import Login from '@/views/Login'
import Carrier from '@/views/Carrier'
import Watcher from '@/views/Watcher'
import GarbageMan from "@/views/GarbageMan";
import StationStaff from "@/views/StationStaff"
//GarbageMan Components
import Information from "@/components/GarbageMan/GetInfo"

//Watcher Components


//Carrier Components


//StationStaff Components


//Administrator Components



const routes = [
    {
        path: '/',
        name: Login,
        component: Login
    }, {
        path: '/GarbageMan',
        name: GarbageMan,
        component: GarbageMan,Information
    }, {
        path: '/Carrier',
        name: Carrier,
        component: Carrier
    }, {
        path: '/StationStaff',
        name: StationStaff,
        component: StationStaff
    }, {
        path: '/Watcher',
        name: Watcher,
        component: Watcher
    }
]

const router = createRouter({
    history: createWebHashHistory(),
    routes
})

export default router
