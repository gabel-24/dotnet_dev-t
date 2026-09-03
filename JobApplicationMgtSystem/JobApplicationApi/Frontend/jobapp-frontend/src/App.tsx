import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Landing from './pages/Landing'
import RegisterCandidate from './pages/RegisterCandidate'
import RegisterRecruiter from './pages/RegisterRecruiter'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register/candidate" element={<RegisterCandidate />} />
        <Route path="/register/recruiter" element={<RegisterRecruiter />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App