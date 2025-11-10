import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '../views/Dashboard.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import MyLinks from '../views/MyLinks.vue'
const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: Dashboard,
  },
  {
    path: '/login',
    name: 'Login',
    component: Login,
  },
  {
    path: '/register',
    name: 'Register',
    component: Register,
  },
  {

     path: '/links',
     name: 'MyLinks',
     component: MyLinks,

  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
