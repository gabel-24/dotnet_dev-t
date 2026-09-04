import { useState } from 'react'
import axiosClient from '../api/axiosClient'
import { useAuth } from '../context/useAuth'
import { useNavigate } from 'react-router-dom'

interface RegisterCandidateRequest
{
  username: string
  email: string
  password: string
}

function RegisterCandidate()
{
  const {login } = useAuth()
  const navigate = useNavigate()

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

  async function handleSubmit(e: React.FormEvent) {
  e.preventDefault()
  await handleRegisterCandidate({
    userName: formData.username,
    email: formData.email,
    password: formData.password,
  })
}

  const handleRegisterCandidate = async (data: {
  userName: string;
  email: string;
  password: string;
  headline?: string;
  resumeUrl?: string;
  skills?: string[];
}) => {
  try {
    const response = await axiosClient.post("/auth/register/candidate", data);

    const { token, userId, userName, role } = response.data;

    login({ userId, userName, role }, token)

    
    navigate("/candidate/dashboard")

  } catch (error) {
    console.error("Registration failed:", error);
    // next step: show an error message to the user
  }
};

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