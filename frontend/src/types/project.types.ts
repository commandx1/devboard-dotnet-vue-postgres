export interface Project {
  id: number
  name: string
  description?: string
  color: string
  createdAt: string
  updatedAt: string
}

export interface CreateProjectRequest {
  name: string
  description?: string
  color?: string
}
