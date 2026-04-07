import { createRouter, createWebHistory } from 'vue-router'
import Login from './views/Login.vue'
import HurdaList from './views/HurdaList.vue'
import HurdaAdd from './views/HurdaAdd.vue'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  { path: '/hurda', component: HurdaList },
  {
    path: '/hurda/yeni',
    component: HurdaAdd,
    meta: { requiresAdmin: true },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.path !== '/login' && !token) {
    next('/login')
    return
  }
  if (to.meta.requiresAdmin && localStorage.getItem('role') !== 'Admin') {
    next('/hurda')
    return
  }
  next()
})

export default router
