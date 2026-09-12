import { getAuthErrorMessages } from '../api/authErrors'
import { useState } from 'react'
import axiosClient from '../api/axiosClient'
import { useAuth } from '../context/useAuth'
import { useNavigate } from 'react-router-dom'

interface LoginRequest {
email: string
password: string
}

function Login() 
{
const {login} = useAuth()
const navigate = useNavigate()
const [errors, setErrors] = useState<string[]>([])
const [isSubmitting, setIsSubmitting] = useState(false)

const [formData, setFormData] = useState<LoginRequest>({
email: '',
password: '',
  })

function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
const { name, value } = e.target
setFormData((prev) => ({ ...prev, [name]: value }))
  }



const handleLogin = async (data: { email: string; password: string }) => {
setErrors([])
    setIsSubmitting(true)
    try {
const response = await axiosClient.post("/auth/login", data)

const { token, userId, userName, role } = response.data

login({ userId, userName, role }, token)

if(role === "Candidate")
      {
navigate("/candidate/dashboard")
      }
else if(role === "Recruiter")
      {
navigate("/recruiter/dashboard")
      }


    } catch (error) {
setErrors(getAuthErrorMessages(error))
    } finally {
      setIsSubmitting(false)
    }
  }


async function handleSubmit(e: React.FormEvent) {
e.preventDefault()
await handleLogin({
email: formData.email,
password: formData.password,
    })
  }


return (
<div className="page page-narrow">
<h1>Login</h1>
<form onSubmit={handleSubmit} className="auth-form">
{errors.length > 0 && (
  <div className="form-note is-error" role="alert">
    {errors.map(message => <p key={message}>{message}</p>)}
  </div>
)}
<div className="field">
<label htmlFor='email'>Email</label>
<input
type='email'
id='email'
name='email'
value={formData.email}
onChange={handleChange}
required
/>
</div>
<div className="field">
<label htmlFor='password'>Password</label>
<input
type='password'
id='password'
name='password'
value={formData.password}
onChange={handleChange}
required
/>
</div>
<button type="submit" className="btn btn-primary" disabled={isSubmitting}>{isSubmitting ? "Logging in…" : "Login"}</button>
</form>
</div>
  )
}
export default Login
