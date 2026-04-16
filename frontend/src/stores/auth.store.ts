import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import type { LoginRequest, RegisterRequest, User } from '@/types/auth.types'
import { authApi } from '@/api/auth.api'

const TOKEN_KEY = 'devboard_token'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(TOKEN_KEY))
  const user = ref<User | null>(null)
  const isAuthenticated = computed(() => Boolean(token.value))

  function setSession(nextToken: string, nextUser: User) {
    token.value = nextToken
    user.value = nextUser
    localStorage.setItem(TOKEN_KEY, nextToken)
  }

  async function login(payload: LoginRequest) {
    const response = await authApi.login(payload)
    setSession(response.token, response.user)
  }

  async function register(payload: RegisterRequest) {
    const response = await authApi.register(payload)
    setSession(response.token, response.user)
  }

  async function loadMe() {
    if (!token.value) {
      return
    }

    user.value = await authApi.me()
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
  }

  return {
    token,
    user,
    isAuthenticated,
    login,
    register,
    loadMe,
    logout
  }
})
