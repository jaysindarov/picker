import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null)
  const user  = ref(null)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin         = computed(() => user.value?.role === 'Admin')

  async function login(email, password) {
    const { data } = await authApi.login({ email, password })
    _applyAuth(data)
  }

  async function register(email, password, displayName) {
    const { data } = await authApi.register({ email, password, displayName })
    _applyAuth(data)
  }

  async function fetchMe() {
    if (!token.value) return
    try {
      const { data } = await authApi.me()
      user.value = { userId: data.userId, email: data.email, displayName: data.name, role: data.role }
    } catch {
      logout()
    }
  }

  function logout() {
    token.value = null
    user.value  = null
    localStorage.removeItem('token')
  }

  function _applyAuth(data) {
    token.value = data.token
    user.value  = { userId: data.userId, email: data.email, displayName: data.displayName, role: data.role }
    localStorage.setItem('token', data.token)
  }

  return { token, user, isAuthenticated, isAdmin, login, register, fetchMe, logout }
})
