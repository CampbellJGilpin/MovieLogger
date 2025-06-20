import React, { useState, useEffect } from 'react';
import api from '../api/config';
import type { User } from '../types/user';
import { AuthContext } from './AuthContextDef';

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const checkAuthStatus = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        setLoading(false);
        setIsAuthenticated(false);
        return;
      }

      const response = await api.get('/api/accounts/me');
      setUser(response.data);
      setIsAuthenticated(true);
    } catch (error) {
      console.error('Auth check failed:', error);
      localStorage.removeItem('token');
      setUser(null);
      setIsAuthenticated(false);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    checkAuthStatus();
  }, []);

  const login = async (email: string, password: string) => {
    try {
      const response = await api.post('/api/accounts/login', { email, password });
      const { token, user } = response.data;
      localStorage.setItem('token', token);
      await checkAuthStatus();
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };

  const register = async (email: string, password: string, userName: string) => {
    try {
      const response = await api.post('/api/accounts/register', { email, password, userName });
      const { token, user } = response.data;
      localStorage.setItem('token', token);
      await checkAuthStatus();
    } catch (error) {
      console.error('Registration failed:', error);
      throw error;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider value={{
      user,
      loading,
      isAuthenticated,
      login,
      register,
      logout,
      checkAuthStatus
    }}>
      {children}
    </AuthContext.Provider>
  );
} 