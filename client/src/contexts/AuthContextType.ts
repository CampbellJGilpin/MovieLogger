import { createContext } from 'react';

export interface User {
  id: number;
  email: string;
  userName: string;
}

export interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, userName: string) => Promise<void>;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined); 