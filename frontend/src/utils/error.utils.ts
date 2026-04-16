import axios from 'axios'

interface ApiErrorPayload {
  message?: string
}

export function extractErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ApiErrorPayload | undefined
    if (data?.message) {
      return data.message
    }
  }

  return fallback
}
