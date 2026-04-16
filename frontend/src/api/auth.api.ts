import { http } from './http'
import type { AuthResponse, LoginRequest, RegisterRequest, User } from '@/types/auth.types'

export const authApi = {
  login: (payload: LoginRequest) =>
    http.post<AuthResponse>('/auth/login', payload).then((res) => res.data),

  register: (payload: RegisterRequest) =>
    http.post<AuthResponse>('/auth/register', payload).then((res) => res.data),

  me: () =>
    http.get<User>('/auth/me').then((res) => res.data)
}
