<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const auth = useAuthStore()
const router = useRouter()

onMounted(async () => {
  if (auth.isAuthenticated && !auth.user) {
    try {
      await auth.loadMe()
    } catch {
      auth.logout()
      router.push('/login')
    }
  }
})

function logout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="layout-shell">
    <header class="header">
      <div class="container nav">
        <RouterLink to="/" class="brand">
          <span class="brand-mark">DB</span>
          <span>DevBoard</span>
        </RouterLink>
        <div class="right">
          <span v-if="auth.user" class="user-pill">{{ auth.user.username }}</span>
          <button class="button secondary" @click="logout">Logout</button>
        </div>
      </div>
    </header>

    <main class="container page">
      <RouterView v-slot="{ Component, route }">
        <Transition name="content-fade" mode="out-in">
          <component :is="Component" :key="route.fullPath" />
        </Transition>
      </RouterView>
    </main>
  </div>
</template>

<style scoped>
.layout-shell {
  min-height: 100vh;
}

.header {
  position: sticky;
  top: 0;
  z-index: 15;
  border-bottom: 1px solid rgba(148, 163, 184, 0.28);
  background: rgba(248, 251, 255, 0.8);
  backdrop-filter: blur(10px);
}

.nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  min-height: 72px;
}

.brand {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  text-decoration: none;
  font-family: 'Manrope', sans-serif;
  font-size: 20px;
  font-weight: 800;
}

.brand-mark {
  width: 32px;
  height: 32px;
  border-radius: 10px;
  display: inline-grid;
  place-items: center;
  color: white;
  background: linear-gradient(140deg, #1d4ed8, #0f766e);
  box-shadow: 0 8px 20px rgba(29, 78, 216, 0.24);
}

.user-pill {
  display: inline-flex;
  align-items: center;
  padding: 6px 12px;
  border-radius: 999px;
  font-family: 'Manrope', sans-serif;
  font-size: 13px;
  font-weight: 700;
  background: rgba(15, 23, 42, 0.08);
  color: #0f172a;
}

.right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.page {
  padding: 26px 0 34px;
}

@media (max-width: 640px) {
  .nav {
    min-height: 64px;
  }

  .user-pill {
    display: none;
  }
}
</style>
