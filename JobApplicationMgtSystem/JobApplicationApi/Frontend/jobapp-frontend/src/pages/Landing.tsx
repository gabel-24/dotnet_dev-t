import {Link} from 'react-router-dom'

function Landing()
{
    return (
    <div>
      <h1>Job Application Management System</h1>
      <p>Get started</p>
      <div>
        <Link to="/register/candidate">I'm a Candidate</Link>
      </div>
      <div>
        <Link to="/register/recruiter">I'm a Recruiter</Link>
      </div>
      <div>
        <Link to="/login">Already have an account? Login</Link>
      </div>
    </div>
  )
}

export default Landing