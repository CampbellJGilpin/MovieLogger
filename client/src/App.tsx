import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import AllMovies from './pages/AllMovies';
import MovieDetails from './pages/MovieDetails';
import MyLibrary from './pages/MyLibrary';
import Login from './components/auth/Login';
import Register from './components/auth/Register';
import { AuthProvider, useAuth } from './contexts/AuthContext';

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { user } = useAuth();
  return user ? <>{children}</> : <Navigate to="/login" />;
}

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route element={<Layout />}>
            <Route index element={
              <PrivateRoute>
                <Dashboard />
              </PrivateRoute>
            } />
            <Route path="movies" element={
              <PrivateRoute>
                <AllMovies />
              </PrivateRoute>
            } />
            <Route path="movies/:id" element={
              <PrivateRoute>
                <MovieDetails />
              </PrivateRoute>
            } />
            <Route path="library" element={
              <PrivateRoute>
                <MyLibrary />
              </PrivateRoute>
            } />
            <Route path="profile" element={
              <PrivateRoute>
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                  <h1 className="text-2xl font-bold text-gray-900">Profile</h1>
                  <p className="mt-4 text-gray-500">Coming soon...</p>
                </div>
              </PrivateRoute>
            } />
          </Route>
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
