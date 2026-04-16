# Agent: Frontend - Vue 3 + Composition API + Pinia

## MERN -> Vue Shift

| React | Vue 3 |
|------|-------|
| `useState` | `ref()` / `reactive()` |
| `useEffect` | `onMounted`, `watch`, `watchEffect` |
| `useMemo` | `computed()` |
| Context/Redux | Pinia |
| React Router | Vue Router |
| Custom hooks | Composables (`useX`) |

## Composable Pattern

```ts
// src/composables/useTasks.ts
import { computed, ref } from 'vue'
import { taskApi } from '@/api/task.api'
import type { CreateTaskRequest, Task } from '@/types/task.types'

export function useTasks(projectId: number) {
  const tasks = ref<Task[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const todoCount = computed(() => tasks.value.filter(t => t.status === 'TODO').length)

  async function fetchTasks() {
    loading.value = true
    error.value = null
    try {
      tasks.value = await taskApi.getByProject(projectId)
    } catch {
      error.value = 'Tasks could not be loaded.'
    } finally {
      loading.value = false
    }
  }

  async function createTask(payload: CreateTaskRequest) {
    const created = await taskApi.create(projectId, payload)
    tasks.value.unshift(created)
    return created
  }

  return { tasks, loading, error, todoCount, fetchTasks, createTask }
}
```

## Pinia Auth Store

```ts
export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('devboard_token'))
  const user = ref<User | null>(null)
  const isAuthenticated = computed(() => Boolean(token.value))

  function setSession(nextToken: string, nextUser: User) {
    token.value = nextToken
    user.value = nextUser
    localStorage.setItem('devboard_token', nextToken)
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('devboard_token')
  }

  return { token, user, isAuthenticated, setSession, logout }
})
```

## Axios API Layer
- Tek bir `http` instance kullan.
- Request interceptor ile JWT ekle.
- Response interceptor ile `401` durumunda logout + `/login` yonlendirmesi yap.

## Router
- `meta.requiresAuth` ile route guard kullan.
- Giris yapmayan kullanici private route'a girerse `/login` sayfasina gitsin.

## Component Kurallari
- Her zaman `<script setup lang="ts">`
- Props typed: `defineProps<T>()`
- Event typed: `defineEmits<T>()`
- Component adlari PascalCase
- Composable adlari `useX`

## TypeScript Types
- API response modelleri `src/types/*` altinda tutulur.
- Enum benzeri alanlari union type ile tanimla:
```ts
export type TaskStatus = 'TODO' | 'IN_PROGRESS' | 'DONE' | 'ARCHIVED'
```
