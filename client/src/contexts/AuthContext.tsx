import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import api from '../api/config';
import type { User } from '../types';
import type { AxiosError } from 'axios';

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, userName: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  const getCurrentUser = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        setLoading(false);
        return;
      }

      const response = await api.get('/accounts/me');
      setUser(response.data);
    } catch (error) {
      console.error('Failed to get current user:', error);
      // Only clear auth if it's an authentication error
      if ((error as AxiosError)?.response?.status === 401) {
        localStorage.removeItem('token');
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getCurrentUser();
  }, []);

  const login = async (email: string, password: string) => {
    try {
      const response = await api.post('/accounts/login', { email, password });
      const { token, user: userData } = response.data;
      localStorage.setItem('token', token);
      setUser(userData);
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };

  const register = async (email: string, password: string, userName: string) => {
    try {
      await api.post('/accounts/register', {
        email,
        password,
        userName,
      });
      const loginResponse = await api.post('/accounts/login', {
        email,
        password,
      });
      const { token, user: userData } = loginResponse.data;
      localStorage.setItem('token', token);
      setUser(userData);
    } catch (error) {
      console.error('Registration failed:', error);
      throw error;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const value = {
    user,
    loading,
    login,
    register,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
} 