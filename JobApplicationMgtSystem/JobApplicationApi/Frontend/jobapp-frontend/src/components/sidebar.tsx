import {NavLink, useNavigate} from "react-router-dom"
import {useAuth} from "../context/useAuth"

const CandidateLinks =
[
    {to: "/candidate/dashboard", label: "Dashboard"},
    {to: "/candidate/postings", label: "Browse Postings"},
    {to: "/candidate/applications", label: "My Applications"},
    {to: "/candidate/profile", label: "My Profile"},
]

const RecruiterLinks =
[
    {to: "/recruiter/dashboard", label: "Dashboard"},
    {to: "/recruiter/postings", label: "My Postings"},
    {to: "/recruiter/profile", label: "My Profile"},
]

const Sidebar = () =>
{
    const {user, logout} = useAuth();
    const navigate = useNavigate();
    const links = user?.role === "Candidate" ? CandidateLinks : RecruiterLinks;

    const handleLogout =() =>
    {
        logout();
        navigate("/login");
    };

    return(
        <aside className="sidebar">
            <nav>
                <ul>
                    {links.map((link) =>(
                        <li key={link.to}>
                            <NavLink
                                to={link.to}
                                className={({isActive}) => (isActive ? 'active' : '')}
                                >
                                    {link.label}
                                </NavLink>
                        </li>
                    ))}
                </ul>
            </nav>
            <button onClick={handleLogout}>Logout</button>
        </aside>
    );
};

export default Sidebar