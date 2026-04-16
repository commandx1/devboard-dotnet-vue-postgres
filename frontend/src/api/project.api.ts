import { http } from './http'
import type { CreateProjectRequest, Project } from '@/types/project.types'

export const projectApi = {
  getMine: () =>
    http.get<Project[]>('/projects').then((res) => res.data),

  create: (payload: CreateProjectRequest) =>
    http.post<Project>('/projects', payload).then((res) => res.data)
}
