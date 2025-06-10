import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import AllMovies from './pages/AllMovies';
import MovieDetails from './pages/MovieDetails';
import MovieSearch from './pages/MovieSearch';
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
            <Route path="search" element={
              <PrivateRoute>
                <MovieSearch />
              </PrivateRoute>
            } />
            <Route path="library" element={
              <PrivateRoute>
                <MyLibrary />
              </PrivateRoute>
            } />
          </Route>
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
