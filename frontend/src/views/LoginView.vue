<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { extractErrorMessage } from '@/utils/error.utils'

const router = useRouter()
const auth = useAuthStore()

const form = reactive({
  email: '',
  password: ''
})

const loading = ref(false)
const error = ref<string | null>(null)

async function submit() {
  loading.value = true
  error.value = null
  try {
    await auth.login(form)
    router.push('/')
  } catch (err) {
    error.value = extractErrorMessage(err, 'Login failed. Please check your credentials.')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-wrap">
    <form class="card stack auth-card" @submit.prevent="submit">
      <span class="badge">Welcome back</span>
      <h1>Sign in to DevBoard</h1>
      <p class="muted">Track your projects and manage tasks with a cleaner workflow.</p>
      <input v-model="form.email" class="input" type="email" placeholder="Email" required />
      <input v-model="form.password" class="input" type="password" placeholder="Password" required />
      <p v-if="error" class="error">{{ error }}</p>
      <button class="button" :disabled="loading" type="submit">
        {{ loading ? 'Signing in...' : 'Sign in' }}
      </button>
      <p class="switch-link">
        No account?
        <RouterLink to="/register">Create one</RouterLink>
      </p>
    </form>
  </div>
</template>

<style scoped>
.auth-wrap {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 24px;
}

.auth-card {
  width: min(460px, 92vw);
}

.error {
  color: #b91c1c;
  font-size: 14px;
}

.switch-link {
  margin: 0;
  color: #334155;
  font-size: 14px;
}

.switch-link a {
  color: #1d4ed8;
  font-weight: 600;
  text-decoration: none;
}

.switch-link a:hover {
  text-decoration: underline;
}
</style>
