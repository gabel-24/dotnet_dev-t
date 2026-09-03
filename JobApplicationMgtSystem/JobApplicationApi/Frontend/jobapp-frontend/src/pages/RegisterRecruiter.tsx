import { useState } from 'react'

interface RegisterRecruiterRequest
{
  username: string
  email: string
  password: string
  companyName: string
}

function RegisterRecruiter()
{
  const [formData, setFormData] = useState<RegisterRecruiterRequest>(
  {
    username: '',
    email: '',
    password: '',
    companyName: '',
  })

  function handleChange(e: React.ChangeEvent<HTMLInputElement>)
  {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  function handleSubmit(e: React.FormEvent)
  {
    e.preventDefault()
    console.log('Recruiter registration submitted:', formData)
  }

  return (
    <div>
      <h1>Register as Recruiter</h1>
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
        <div>
          <label htmlFor="companyName">Company Name</label>
          <input
            type="text"
            id="companyName"
            name="companyName"
            value={formData.companyName}
            onChange={handleChange}
            required
          />
        </div>
        <button type="submit">Register</button>
      </form>
    </div>
  )
}

export default RegisterRecruiter