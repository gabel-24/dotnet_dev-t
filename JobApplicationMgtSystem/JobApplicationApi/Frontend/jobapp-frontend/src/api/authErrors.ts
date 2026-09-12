import axios from 'axios'

export function getAuthErrorMessages(error: unknown): string[] {
  if (!axios.isAxiosError(error)) return ['Something went wrong. Please try again.']
  if (!error.response) return ['Unable to connect to the server. Please check your connection and try again.']
  if (error.response.status >= 500) return ['The server could not complete your request. Please try again later.']

  const data = error.response.data
  if (data && typeof data === 'object') {
    if (data.errors && typeof data.errors === 'object') {
      const messages = Object.values(data.errors).flat().filter(
        (message): message is string => typeof message === 'string' && message.trim().length > 0
      )
      if (messages.length) return [...new Set(messages)]
    }
    if (typeof data.message === 'string' && data.message.trim()) return [data.message]
  }
  if (error.response.status === 401) return ['Invalid email or password.']
  if (error.response.status === 409) return ['An account with these details already exists. Please log in or use different details.']
  if (typeof data === 'string' && data.trim() && !data.includes('<')) return [data]
  return ['Please check your details and try again.']
}
