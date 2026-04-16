<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { projectApi } from '@/api/project.api'
import type { Project } from '@/types/project.types'

const projects = ref<Project[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

const form = reactive({
  name: '',
  description: '',
  color: '#2563eb'
})

async function fetchProjects() {
  loading.value = true
  error.value = null
  try {
    projects.value = await projectApi.getMine()
  } catch {
    error.value = 'Projects could not be loaded.'
  } finally {
    loading.value = false
  }
}

async function createProject() {
  if (!form.name.trim()) {
    return
  }

  const created = await projectApi.create({
    name: form.name,
    description: form.description,
    color: form.color
  })

  projects.value.unshift(created)
  form.name = ''
  form.description = ''
}

onMounted(fetchProjects)

function formatDate(value: string) {
  return new Date(value).toLocaleDateString('tr-TR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  })
}
</script>

<template>
  <section class="dashboard stack">
    <div class="card hero">
      <div class="stack hero-content">
        <span class="badge">Workspace</span>
        <h1>My Projects</h1>
        <p class="muted">Create, track and organize your work in one place.</p>
      </div>
      <div class="hero-meta">
        <span class="meta-count">{{ projects.length }}</span>
        <span class="muted">active projects</span>
      </div>
    </div>

    <form class="card stack create-form" @submit.prevent="createProject">
      <h2>Create a new project</h2>
      <input v-model="form.name" class="input" placeholder="Project name" />
      <input v-model="form.description" class="input" placeholder="Description" />
      <div class="color-row">
        <label for="project-color" class="muted">Color</label>
        <input id="project-color" v-model="form.color" class="color-input" type="color" />
      </div>
      <button class="button" type="submit">Create project</button>
    </form>

    <p v-if="loading" class="muted">Loading projects...</p>
    <p v-if="error" class="error">{{ error }}</p>

    <div v-if="projects.length" class="project-grid">
      <RouterLink
        v-for="project in projects"
        :key="project.id"
        :to="`/projects/${project.id}`"
        class="card project-card"
      >
        <div class="project-top">
          <strong>{{ project.name }}</strong>
          <span class="project-dot" :style="{ backgroundColor: project.color }" />
        </div>
        <p class="muted">{{ project.description || 'No description' }}</p>
        <small class="muted">Updated {{ formatDate(project.updatedAt) }}</small>
      </RouterLink>
    </div>

    <div v-else-if="!loading" class="card empty-state">
      <h3>No projects yet</h3>
      <p class="muted">Create your first project to start adding tasks.</p>
    </div>
  </section>
</template>

<style scoped>
.dashboard {
  gap: 18px;
}

.hero {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 16px;
}

.hero-content {
  gap: 8px;
}

.hero-meta {
  text-align: right;
}

.meta-count {
  display: block;
  font-family: 'Manrope', sans-serif;
  font-size: 32px;
  font-weight: 800;
  line-height: 1;
}

.create-form h2 {
  font-size: 20px;
}

.color-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.color-input {
  width: 52px;
  height: 34px;
  border: 1px solid #cbd5e1;
  border-radius: 10px;
  padding: 2px;
  background: white;
}

.project-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(230px, 1fr));
  gap: 14px;
}

.project-card {
  text-decoration: none;
  display: grid;
  gap: 8px;
  transition: transform 0.2s ease, border-color 0.2s ease;
}

.project-card:hover {
  transform: translateY(-3px);
  border-color: rgba(59, 130, 246, 0.4);
}

.project-top {
  display: flex;
  justify-content: space-between;
  gap: 10px;
}

.project-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  margin-top: 6px;
  box-shadow: 0 0 0 4px rgba(15, 23, 42, 0.06);
}

.empty-state {
  text-align: center;
  gap: 8px;
  display: grid;
}

.error {
  color: var(--danger);
}

@media (max-width: 720px) {
  .hero {
    flex-direction: column;
    align-items: flex-start;
  }

  .hero-meta {
    text-align: left;
  }
}
</style>
