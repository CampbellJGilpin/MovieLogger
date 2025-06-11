import { useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import api from '../api/config';
import { AuthContext, type User } from './AuthContextType';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    checkAuthStatus();
  }, []);

  const checkAuthStatus = async () => {
    try {
      const response = await api.get('/accounts/me');
      setUser(response.data);
      setIsAuthenticated(true);
    } catch {
      setUser(null);
      setIsAuthenticated(false);
    }
  };

  const login = async (email: string, password: string) => {
    const response = await api.post('/accounts/login', { email, password });
    setUser(response.data);
    setIsAuthenticated(true);
  };

  const register = async (email: string, password: string, userName: string) => {
    const response = await api.post('/accounts/register', { email, password, userName });
    setUser(response.data);
    setIsAuthenticated(true);
  };

  const logout = async () => {
    await api.post('/accounts/logout');
    setUser(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// Create a separate file for this hook
export { useAuth } from './useAuth'; 