<script setup lang="ts">
import { computed, onMounted, reactive } from 'vue'
import { useRoute } from 'vue-router'
import { useTasks } from '@/composables/useTasks'
import type { TaskPriority } from '@/types/task.types'

const route = useRoute()
const projectId = computed(() => Number(route.params.id))

const { tasks, loading, error, fetchTasks, createTask } = useTasks(projectId.value)

const form = reactive({
  title: '',
  description: '',
  priority: 'MEDIUM' as TaskPriority
})

async function submit() {
  if (!form.title.trim()) {
    return
  }

  await createTask({
    title: form.title,
    description: form.description,
    priority: form.priority
  })

  form.title = ''
  form.description = ''
  form.priority = 'MEDIUM'
}

onMounted(fetchTasks)

function priorityClass(priority: TaskPriority) {
  if (priority === 'URGENT') return 'priority urgent'
  if (priority === 'HIGH') return 'priority high'
  if (priority === 'LOW') return 'priority low'
  return 'priority medium'
}
</script>

<template>
  <section class="project stack">
    <div class="card project-hero">
      <span class="badge">Project</span>
      <h1>#{{ projectId }} Task Board</h1>
      <p class="muted">Create and prioritize your tasks with a clean kanban-like list.</p>
    </div>

    <form class="card stack create-task" @submit.prevent="submit">
      <h2>Create a task</h2>
      <input v-model="form.title" class="input" placeholder="Task title" />
      <input v-model="form.description" class="input" placeholder="Description" />
      <select v-model="form.priority" class="input">
        <option value="LOW">LOW</option>
        <option value="MEDIUM">MEDIUM</option>
        <option value="HIGH">HIGH</option>
        <option value="URGENT">URGENT</option>
      </select>
      <button class="button" type="submit">Add task</button>
    </form>

    <p v-if="loading" class="muted">Loading tasks...</p>
    <p v-if="error" class="error">{{ error }}</p>

    <div v-if="tasks.length" class="task-grid">
      <article v-for="task in tasks" :key="task.id" class="card task-card">
        <div class="task-top">
          <strong>{{ task.title }}</strong>
          <span :class="priorityClass(task.priority)">{{ task.priority }}</span>
        </div>
        <p class="muted">{{ task.description || 'No description' }}</p>
        <small class="muted">Status: {{ task.status }}</small>
      </article>
    </div>

    <div v-else-if="!loading" class="card empty-state">
      <h3>No tasks yet</h3>
      <p class="muted">Use the form above to add your first task.</p>
    </div>
  </section>
</template>

<style scoped>
.project {
  gap: 18px;
}

.project-hero {
  display: grid;
  gap: 9px;
}

.project-hero h1 {
  font-size: clamp(26px, 3.5vw, 34px);
}

.create-task h2 {
  font-size: 20px;
}

.task-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 14px;
}

.task-card {
  display: grid;
  gap: 8px;
}

.task-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
}

.priority {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 76px;
  padding: 4px 8px;
  border-radius: 999px;
  font-family: 'Manrope', sans-serif;
  font-size: 11px;
  font-weight: 800;
}

.priority.low {
  color: #0f766e;
  background: rgba(20, 184, 166, 0.16);
}

.priority.medium {
  color: #1e40af;
  background: rgba(59, 130, 246, 0.16);
}

.priority.high {
  color: #9a3412;
  background: rgba(249, 115, 22, 0.18);
}

.priority.urgent {
  color: #991b1b;
  background: rgba(239, 68, 68, 0.2);
}

.empty-state {
  text-align: center;
  display: grid;
  gap: 8px;
}

.error {
  color: var(--danger);
}
</style>
