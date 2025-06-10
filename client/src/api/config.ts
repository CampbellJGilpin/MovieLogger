import axios from 'axios';

// Create axios instance with default config
const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL || 'http://localhost:5049/api'}`,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to add auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

// Add response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Log the error for debugging
    console.error('API Error:', error.response?.status, error.response?.data);
    
    // Only remove token if it's explicitly an authentication error
    if (error.response?.status === 401 && error.response?.data?.message?.includes('invalid token')) {
      localStorage.removeItem('token');
    }
    return Promise.reject(error);
  }
);

export default api; 