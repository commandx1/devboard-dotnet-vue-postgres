import { http } from './http'
import type { CreateTaskRequest, Task, UpdateTaskRequest } from '@/types/task.types'

export const taskApi = {
  getByProject: (projectId: number) =>
    http.get<Task[]>(`/projects/${projectId}/tasks`).then((res) => res.data),

  create: (projectId: number, payload: CreateTaskRequest) =>
    http.post<Task>(`/projects/${projectId}/tasks`, payload).then((res) => res.data),

  update: (taskId: number, payload: UpdateTaskRequest) =>
    http.patch<Task>(`/tasks/${taskId}`, payload).then((res) => res.data),

  delete: (taskId: number) =>
    http.delete(`/tasks/${taskId}`)
}
