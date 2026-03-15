import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  { path: '/',        redirect: '/movies' },
  { path: '/auth',    component: () => import('@/views/AuthView.vue'),         meta: { public: true } },
  { path: '/movies',  component: () => import('@/views/MoviesView.vue'),       meta: { auth: true } },
  { path: '/books',   component: () => import('@/views/BooksView.vue'),        meta: { auth: true } },
  { path: '/foods',   component: () => import('@/views/FoodsView.vue'),        meta: { auth: true } },
  { path: '/pick',    component: () => import('@/views/RandomPickerView.vue'), meta: { auth: true } },
  { path: '/item/:type/:id', component: () => import('@/views/ItemDetailView.vue'), meta: { auth: true } },
  { path: '/admin',   component: () => import('@/views/AdminView.vue'),        meta: { auth: true, admin: true } },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior: () => ({ top: 0 }),
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.auth && !auth.isAuthenticated) return '/auth'
  if (to.meta.admin && !auth.isAdmin)        return '/movies'
  if (to.path === '/auth' && auth.isAuthenticated) return '/movies'
})

export default router
