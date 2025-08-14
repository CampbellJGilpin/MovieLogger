import { useState, useEffect, useCallback } from 'react';
import { PlusIcon } from '@heroicons/react/24/outline';
import MovieList from '../components/movies/MovieList';
import AddMovieModal from '../components/movies/AddMovieModal';
import type { MovieInLibrary } from '../types/index';
import * as movieService from '../services/movieService';
import api from '../api/config';

export default function AllMovies() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isAddMovieModalOpen, setIsAddMovieModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearchQuery, setDebouncedSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize] = useState(10);

  const loadMovies = useCallback(async (query = debouncedSearchQuery, page = currentPage) => {
    try {
      setIsLoading(true);
      setError(null);
      const userResponse = await api.get('/accounts/me');
      const userId = userResponse.data.id;
      const { items, totalPages } = await movieService.getAllMoviesForUser(userId, query, page, pageSize);
      setMovies(items);
      setTotalPages(totalPages);
    } catch (err) {
      setError('Failed to load movies. Please try again later.');
      console.error('Error loading movies:', err);
    } finally {
      setIsLoading(false);
    }
  }, [debouncedSearchQuery, currentPage, pageSize]);

  useEffect(() => {
    loadMovies();
  }, [loadMovies]);

  // Debounce search query
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchQuery(searchQuery);
    }, 500);
    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Perform search when debounced query changes
  useEffect(() => {
    loadMovies(debouncedSearchQuery, 1);
    setCurrentPage(1);
  }, [debouncedSearchQuery, loadMovies]);

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
    loadMovies(debouncedSearchQuery, newPage);
    window.scrollTo(0, 0);
  };

  const handleAddMovie = async (movieData: FormData, onUploadProgress?: (progressEvent: { loaded: number; total?: number }) => void) => {
    try {
      await movieService.createMovie(movieData, onUploadProgress);
      await loadMovies();
      setIsAddMovieModalOpen(false);
    } catch (err) {
      console.error('Error adding movie:', err);
    }
  };

  const handleToggleLibrary = async (movieId: number) => {
    const movie = movies.find(m => m.id === movieId);
    if (!movie) return;
    try {
      await movieService.toggleLibrary(movieId, movie.inLibrary);
      setMovies(prev => prev.map(m => m.id === movieId ? { ...m, inLibrary: !m.inLibrary } : m));
    } catch (error) {
      console.error('Error toggling library:', error);
    }
  };

  const handleToggleFavorite = async (movieId: number) => {
    try {
      await movieService.toggleFavorite(movieId);
      setMovies(movies.map(movie =>
        movie.id === movieId
          ? { ...movie, isFavorite: !movie.isFavorite }
          : movie
      ));
    } catch (err) {
      console.error('Error toggling favorite status:', err);
    }
  };

  const handleDeleteMovie = async (movieId: number) => {
    try {
      await movieService.deleteMovie(movieId);
      setMovies(movies.filter(movie => movie.id !== movieId));
    } catch (err) {
      console.error('Error deleting movie:', err);
    }
  };

  if (isLoading) {
    return (
      <div className="text-center py-12">
        <div className="text-gray-500">Loading movies...</div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-8">
        <div className="flex-1 min-w-0">
          <h1 className="text-2xl font-bold text-gray-900">All Movies</h1>
        </div>
        <div className="mt-4 sm:mt-0 sm:ml-16 sm:flex-none">
          <button
            type="button"
            onClick={() => setIsAddMovieModalOpen(true)}
            className="btn btn-primary flex items-center"
          >
            <PlusIcon className="w-5 h-5 mr-2" />
            Add Movie
          </button>
        </div>
      </div>

      <div className="mb-8">
        <input
          type="text"
          placeholder="Search movies..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="input w-full max-w-md"
        />
      </div>

      {error ? (
        <div className="rounded-md bg-red-50 p-4">
          <div className="text-sm text-red-700">{error}</div>
        </div>
      ) : (
        <>
          <MovieList
            movies={movies}
            onToggleLibrary={handleToggleLibrary}
            onToggleFavorite={handleToggleFavorite}
            onDelete={handleDeleteMovie}
            emptyMessage={
              isLoading
                ? 'Loading movies'
                : error
                ? 'Error loading movies'
                : 'No movies found'
            }
          />
          {/* Pagination */}
          <div className="mt-8 flex justify-center">
            <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
              <button
                onClick={() => handlePageChange(currentPage - 1)}
                disabled={currentPage === 1}
                className={`relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium ${
                  currentPage === 1
                    ? 'text-gray-300 cursor-not-allowed'
                    : 'text-gray-500 hover:bg-gray-50'
                }`}
              >
                Previous
              </button>
              
              {[...Array(totalPages)].map((_, i) => (
                <button
                  key={i + 1}
                  onClick={() => handlePageChange(i + 1)}
                  className={`relative inline-flex items-center px-4 py-2 border text-sm font-medium ${
                    currentPage === i + 1
                      ? 'z-10 bg-indigo-50 border-indigo-500 text-indigo-600'
                      : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                  }`}
                >
                  {i + 1}
                </button>
              ))}
              
              <button
                onClick={() => handlePageChange(currentPage + 1)}
                disabled={currentPage === totalPages}
                className={`relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium ${
                  currentPage === totalPages
                    ? 'text-gray-300 cursor-not-allowed'
                    : 'text-gray-500 hover:bg-gray-50'
                }`}
              >
                Next
              </button>
            </nav>
          </div>
        </>
      )}

      <AddMovieModal
        isOpen={isAddMovieModalOpen}
        onClose={() => setIsAddMovieModalOpen(false)}
        onSubmit={handleAddMovie}
      />
    </div>
  );
} 