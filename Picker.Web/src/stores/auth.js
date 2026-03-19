import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null)
  const user  = ref(null)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin         = computed(() => user.value?.role === 'Admin')

  async function login(username, password) {
    const { data } = await authApi.login({ username, password })
    _applyAuth(data)
  }

  async function register(email, password, username, firstName, lastName) {
    const { data } = await authApi.register({ email, password, username, firstName, lastName })
    _applyAuth(data)
  }

  async function fetchMe() {
    if (!token.value) return
    try {
      const { data } = await authApi.me()
      user.value = { userId: data.userId, email: data.email, username: data.username, firstName: data.firstName, lastName: data.lastName, role: data.role }
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
    user.value  = { userId: data.userId, email: data.email, username: data.username, firstName: data.firstName, lastName: data.lastName, role: data.role }
    localStorage.setItem('token', data.token)
  }

  return { token, user, isAuthenticated, isAdmin, login, register, fetchMe, logout }
})
