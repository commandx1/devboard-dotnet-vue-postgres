import { computed, ref } from 'vue'
import { taskApi } from '@/api/task.api'
import type { CreateTaskRequest, Task } from '@/types/task.types'

export function useTasks(projectId: number) {
  const tasks = ref<Task[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const todoTasks = computed(() => tasks.value.filter((task) => task.status === 'TODO'))

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

  return {
    tasks,
    loading,
    error,
    todoTasks,
    fetchTasks,
    createTask
  }
}
