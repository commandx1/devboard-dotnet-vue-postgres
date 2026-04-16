<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { extractErrorMessage } from '@/utils/error.utils'

const router = useRouter()
const auth = useAuthStore()

const form = reactive({
  email: '',
  username: '',
  password: ''
})

const loading = ref(false)
const error = ref<string | null>(null)

async function submit() {
  loading.value = true
  error.value = null
  try {
    await auth.register(form)
    router.push('/')
  } catch (err) {
    error.value = extractErrorMessage(err, 'Registration failed. Please try again.')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-wrap">
    <form class="card stack auth-card" @submit.prevent="submit">
      <span class="badge">New workspace</span>
      <h1>Create account</h1>
      <p class="muted">Start your first project board in less than a minute.</p>
      <input v-model="form.email" class="input" type="email" placeholder="Email" required />
      <input
        v-model="form.username"
        class="input"
        type="text"
        placeholder="Username"
        minlength="3"
        maxlength="50"
        required
      />
      <input
        v-model="form.password"
        class="input"
        type="password"
        placeholder="Password"
        minlength="8"
        required
      />
      <p v-if="error" class="error">{{ error }}</p>
      <button class="button" :disabled="loading" type="submit">
        {{ loading ? 'Creating...' : 'Create account' }}
      </button>
      <p class="switch-link">
        Already have an account?
        <RouterLink to="/login">Sign in</RouterLink>
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
  width: min(480px, 92vw);
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
