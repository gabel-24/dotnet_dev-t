import { useState } from 'react'

interface RegisterCandidateRequest
{
  username: string
  email: string
  password: string
}

function RegisterCandidate()
{
  const [formData, setFormData] = useState<RegisterCandidateRequest>(
  {
    username: '',
    email: '',
    password: '',
  })

  function handleChange(e: React.ChangeEvent<HTMLInputElement>)
  {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  function handleSubmit(e: React.FormEvent)
  {
    e.preventDefault()
    console.log('Candidate registration submitted:', formData)
  }

  return (
    <div>
      <h1>Register as Candidate</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Username</label>
          <input
            type="text"
            id="username"
            name="username"
            value={formData.username}
            onChange={handleChange}
            required
          />
        </div>
        <div>
          <label htmlFor="email">Email</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
          />
        </div>
        <div>
          <label htmlFor="password">Password</label>
          <input
            type="password"
            id="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            required
          />
        </div>
        <button type="submit">Register</button>
      </form>
    </div>
  )
}

export default RegisterCandidate