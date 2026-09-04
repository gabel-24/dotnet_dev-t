import {useState, useEffect, type ReactNode} from "react"
import { AuthContext, type AuthUser } from "./AuthContext"

export function AuthProvider({children}: {children: ReactNode})
{
    const [user, setUser] = useState<AuthUser | null>(null)
    const [token, setToken] = useState<string | null>(null)

    useEffect(() =>
    {
        const storedToken = localStorage.getItem("token")
        const storedUser = localStorage.getItem("user")

        if(storedToken && storedUser)
        {
            setToken(storedToken)
            setUser(JSON.parse(storedUser))
        }
    }, [])

    function login(userData: AuthUser, newToken: string)
    {
        setUser(userData)
        setToken(newToken)
        localStorage.setItem("token", newToken)
        localStorage.setItem("user", JSON.stringify(userData))
    }

    function logout()
    {
        setUser(null)
        setToken(null)
        localStorage.removeItem("token")
        localStorage.removeItem("user")
    }

    return (
        <AuthContext.Provider value={{user, token, login, logout}} > 
            {children}
        </AuthContext.Provider>
    )
}