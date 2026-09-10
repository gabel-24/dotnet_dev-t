import {Link} from 'react-router-dom'

function Landing()
{
return (
<div className="landing">
<h1>Job Application Management System</h1>
<p>Get started</p>
<div className="landing-actions">
<Link to="/register/candidate">I'm a Candidate</Link>
<Link to="/register/recruiter">I'm a Recruiter</Link>
<Link to="/login" className="secondary">Already have an account? Login</Link>
</div>
</div>
  )
}

export default Landing
