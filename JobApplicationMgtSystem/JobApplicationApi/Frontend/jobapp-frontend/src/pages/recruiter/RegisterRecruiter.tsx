import { getAuthErrorMessages } from '../../api/authErrors'
import { useState } from 'react'
import { useAuth } from '../../context/useAuth'
import { useNavigate } from 'react-router-dom'
import axiosClient from '../../api/axiosClient'

interface RegisterRecruiterRequest
{
  username: string
  email: string
  password: string
  companyName: string
}

function RegisterRecruiter()
{
  const {login} = useAuth()
  const navigate = useNavigate()
const [errors, setErrors] = useState<string[]>([])
const [isSubmitting, setIsSubmitting] = useState(false)
  
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

  async function handleSubmit(e: React.FormEvent)
  {
    e.preventDefault()
    
    await handleRegisterRecruiter(
      {
        userName: formData.username,
        email: formData.email,
        password: formData.password,
        companyName: formData.companyName,
      }
    )

  }

  const handleRegisterRecruiter = async (data: 
    {
      userName: string
      email: string
      password: string
      companyName: string
    }
  ) =>
  {
    setErrors([])
    setIsSubmitting(true)
    try {
      const response = await axiosClient.post("/auth/register/recruiter", data)
      const {token, userId, userName, role} = response.data

      login ({userId, userName, role}, token)

      navigate("/recruiter/dashboard")
    }
    catch(error)
    {
      setErrors(getAuthErrorMessages(error))
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="page page-narrow">
      <h1>Register as Recruiter</h1>
      <form onSubmit={handleSubmit} className="auth-form">
{errors.length > 0 && (
  <div className="form-note is-error" role="alert">
    {errors.map(message => <p key={message}>{message}</p>)}
  </div>
)}
        <div className="field">
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
        <div className="field">
          <label htmlFor="email">Email</label>
          <input
            type="email"
            id="email"
            name="email"
            required
            value={formData.email}
            onChange={handleChange}
          />
        </div>
        <div className="field">
          <label htmlFor="password">Password</label>
          <input
            type="password"
            id="password"
            name="password"
            minLength={6}
            value={formData.password}
            onChange={handleChange}
            required
          />
        </div>
        <div className="field">
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
        <button className="btn btn-primary" type="submit" disabled={isSubmitting}>{isSubmitting ? "Registering…" : "Register"}</button>
      </form>
    </div>
  )
}

export default RegisterRecruiter