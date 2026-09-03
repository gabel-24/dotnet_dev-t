import { useState } from "react";

interface RegisterRequest 
{
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: "Candidate" | "Recruiter";
}

function Register()
{
  const[formData, setFormData] = useState<RegisterRequest>(
  {
    userName: '',
    email: '',
    password: '',
    confirmPassword: '',
    role: 'Candidate',
  })

  function handleChange(e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>)
  {
    const {name, value} = e.target
    setFormData((prev) => ({...prev, [name]: value}))
  }

  function handleSubmit(e: React.FormEvent)
  {
    e.preventDefault()

    if(formData.password !== formData.confirmPassword)
    {
      console.log('Passwords do not match')
      return
    }

    console.log('Form submitted:', formData)
  }

  return (
    <div>
      <h1>Register</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="userName">Name</label>
          <input
            type="text"
            id="userName"
            name="userName"
            value={formData.userName}
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
            required
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
          <label htmlFor="confirmPassword">Confirm Password</label>
          <input
            type="password"
            id="confirmPassword"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleChange}
            required
          />
        </div>
        <div>
          <label htmlFor="role">I am a</label>
          <select
            id="role"
            name="role"
            value={formData.role}
            onChange={handleChange}
          >
            <option value="Candidate">Candidate</option>
            <option value="Recruiter">Recruiter</option>
          </select>
        </div>
        <button type="submit">Register</button>
      </form>
    </div>
  )

}

export default Register
