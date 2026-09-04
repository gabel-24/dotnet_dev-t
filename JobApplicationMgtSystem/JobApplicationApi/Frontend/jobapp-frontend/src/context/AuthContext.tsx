import { createContext } from "react"

export interface AuthUser
{
    userId: string
    userName: string
    role: string
}

export interface AuthContextType
{
    user: AuthUser | null
    token: string | null
    login: (user: AuthUser, token: string ) => void
    logout: () => void
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined)