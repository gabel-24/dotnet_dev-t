import {useState, useEffect, type ReactNode} from "react"
import { AuthContext, type AuthUser } from "./AuthContext"
import {clearSession, sessionExpiredEvent, tokenExpiresAt} from "./session"

export function AuthProvider({children}: {children: ReactNode})
{
    const [session] = useState(() => {
        try {
            const storedToken = localStorage.getItem("token");
            const storedUser = localStorage.getItem("user");
            const parsed = storedUser ? JSON.parse(storedUser) : null;
            if (storedToken && tokenExpiresAt(storedToken) > Date.now() && parsed && typeof parsed.userId === "string" && typeof parsed.role === "string")
                return { user: parsed as AuthUser, token: storedToken };
        } catch {
            // Ignore invalid persisted session data.
        }
        clearSession();
        return { user: null, token: null };
    });
    const [user, setUser] = useState<AuthUser | null>(session.user);
    const [token, setToken] = useState<string | null>(session.token);

    useEffect(() => {
        const expire = () => {
            clearSession();
            setUser(null);
            setToken(null);
        };
        window.addEventListener(sessionExpiredEvent, expire);
        const timer = token ? window.setTimeout(expire, Math.max(0, tokenExpiresAt(token) - Date.now())) : undefined;
        return () => {
            window.removeEventListener(sessionExpiredEvent, expire);
            if (timer !== undefined) window.clearTimeout(timer);
        };
    }, [token]);

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