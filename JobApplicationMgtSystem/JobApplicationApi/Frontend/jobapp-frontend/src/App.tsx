import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Landing from './pages/Landing'
import RegisterCandidate from './pages/candidate/RegisterCandidate'
import RegisterRecruiter from './pages/recruiter/RegisterRecruiter'
import { ProtectedRoute } from './routes/ProtectedRoute'
import CandidateDashboard from './pages/candidate/CandidateDashboard'
import RecruiterDashboard from './pages/recruiter/RecruiterDashboard'
import BrowsePostings from './pages/candidate/BrowsePostings'
import CandidateProfile from './pages/candidate/CandidateProfile'
import MyApplications from './pages/candidate/MyApplications'
import MyPostings from './pages/recruiter/MyPostings'
import CreatePostings from './pages/recruiter/CreatePostings'
import PostingApplications from './pages/recruiter/PostingApplications'
import RecruiterProfile from './pages/recruiter/RecruiterProfile'
import DashboardLayout from './layouts/DashboardLayout'


function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register/candidate" element={<RegisterCandidate />} />
        <Route path="/register/recruiter" element={<RegisterRecruiter />} />

        <Route element={<DashboardLayout />}>
          <Route 
            path="/candidate/dashboard"
              element= 
                {
                  <ProtectedRoute allowedRole="Candidate"><CandidateDashboard /></ProtectedRoute>
                }
          />

          <Route
            path="/recruiter/dashboard"
                element= 
                  {
                    <ProtectedRoute allowedRole="Recruiter"><RecruiterDashboard /></ProtectedRoute>
                  }
          />

          <Route 
            path="/candidate/postings" 
                element=
                {
                  <ProtectedRoute allowedRole="Candidate"><BrowsePostings /></ProtectedRoute>
                } 
          />

          <Route path="/candidate/applications" 
            element=
            {
              <ProtectedRoute allowedRole="Candidate"><MyApplications /></ProtectedRoute>
            } 
          />
          <Route path="/candidate/profile" 
            element=
            {
              <ProtectedRoute allowedRole="Candidate"><CandidateProfile /></ProtectedRoute>
            } 
          />

          <Route path="/recruiter/postings" 
            element=
            {
              <ProtectedRoute allowedRole="Recruiter"><MyPostings /></ProtectedRoute>
            } 
          />

          <Route path="/recruiter/postings/new" 
            element=
            {
              <ProtectedRoute allowedRole="Recruiter"><CreatePostings /></ProtectedRoute>
            } 
          />

          <Route path="/recruiter/postings/:id/applications" 
            element=
            {
              <ProtectedRoute allowedRole="Recruiter"><PostingApplications /></ProtectedRoute>
            } 
          />

          <Route path="/recruiter/profile" 
            element=
            {
              <ProtectedRoute allowedRole="Recruiter"><RecruiterProfile /></ProtectedRoute>
            } 
          />
        </Route>

      </Routes>
    </BrowserRouter>
  )
}

export default App