import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Landing from './pages/Landing'
import RegisterCandidate from './pages/RegisterCandidate'
import RegisterRecruiter from './pages/RegisterRecruiter'
import { ProtectedRoute } from './routes/ProtectedRoute'
import CandidateDashboard from './pages/CandidateDashboard'
import RecruiterDashboard from './pages/RecruiterDashboard'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register/candidate" element={<RegisterCandidate />} />
        <Route path="/register/recruiter" element={<RegisterRecruiter />} />

        <Route 
          path="/candidate/dashboard"
            element= 
              {
                <ProtectedRoute allowedRole="Candidate">
                  <CandidateDashboard />
                </ProtectedRoute>
              }
        />

        <Route
          path="/recruiter/dashboard"
              element= 
                {
                  <ProtectedRoute allowedRole="Recruiter">
                    <RecruiterDashboard />
                  </ProtectedRoute>
                }
        />

      </Routes>
    </BrowserRouter>
  )
}

export default App