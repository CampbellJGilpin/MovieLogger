import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { PlusIcon } from '@heroicons/react/24/outline';
import type { MovieInLibrary, MovieCreateRequest, Viewing } from '../types/index';
import * as movieService from '../services/movieService';
import AddMovieModal from '../components/movies/AddMovieModal';

interface MovieListSectionProps {
  title: string;
  movies: MovieInLibrary[];
  onToggleFavorite?: (movieId: number) => void;
  emptyMessage?: string;
}

function RecentlyWatchedSection({ viewings }: { viewings: Viewing[] }) {
  return (
    <div>
      <h2 className="text-lg font-semibold text-gray-900 mb-4">Recently Watched</h2>
      {viewings.length === 0 ? (
        <p className="text-gray-500 text-sm">No recently watched movies</p>
      ) : (
        <ul className="space-y-3">
          {viewings.map(viewing => (
            <li key={viewing.id} className="flex items-center justify-between bg-white rounded-lg shadow p-4">
              <div>
                <Link to={`/movies/${viewing.movie.id}`} className="text-sm font-medium text-gray-900 hover:text-indigo-600">
                  {viewing.movie.title}
                </Link>
                <p className="text-xs text-gray-500 mt-1">
                  {new Date(viewing.movie.releaseDate).getFullYear()} • {viewing.movie.genre.title}
                </p>
                <p className="text-xs text-gray-400 mt-1">
                  Watched on {new Date(viewing.dateViewed).toLocaleDateString()}
                </p>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

function MovieListSection({ title, movies, onToggleFavorite, emptyMessage = 'No movies found' }: MovieListSectionProps) {
  return (
    <div>
      <h2 className="text-lg font-semibold text-gray-900 mb-4">{title}</h2>
      {movies.length === 0 ? (
        <p className="text-gray-500 text-sm">{emptyMessage}</p>
      ) : (
        <ul className="space-y-3">
          {movies.map(movie => (
            <li key={movie.id} className="flex items-center justify-between bg-white rounded-lg shadow p-4">
              <div>
                <Link to={`/movies/${movie.id}`} className="text-sm font-medium text-gray-900 hover:text-indigo-600">
                  {movie.title}
                </Link>
                <p className="text-xs text-gray-500 mt-1">
                  {new Date(movie.releaseDate).getFullYear()} • {movie.genre.title}
                </p>
              </div>
              <div className="flex space-x-2">
                {onToggleFavorite && (
                  <button
                    onClick={() => onToggleFavorite(movie.id)}
                    className={`p-1 rounded ${
                      movie.isFavorite
                        ? 'text-red-600 bg-red-100'
                        : 'text-gray-400 hover:text-gray-500'
                    }`}
                  >
                    <svg className="h-5 w-5" fill={movie.isFavorite ? 'currentColor' : 'none'} viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                    </svg>
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default function Dashboard() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [recentlyWatched, setRecentlyWatched] = useState<Viewing[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isAddMovieModalOpen, setIsAddMovieModalOpen] = useState(false);

  useEffect(() => {
    loadMovies();
    loadRecentlyWatched();
  }, []);

  const loadMovies = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await movieService.getMyLibrary();
      setMovies(data);
    } catch (err) {
      setError('Failed to load your library. Please try again later.');
      console.error('Error loading library:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const loadRecentlyWatched = async () => {
    try {
      const data = await movieService.getRecentlyWatchedMovies();
      setRecentlyWatched(data);
    } catch (err) {
      setError('Failed to load recently watched movies. Please try again later.');
      console.error('Error loading recently watched:', err);
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

  const handleAddMovie = async (movieData: MovieCreateRequest) => {
    try {
      await movieService.createMovie(movieData);
      await loadMovies();
      setIsAddMovieModalOpen(false);
    } catch (err) {
      console.error('Error adding movie:', err);
    }
  };

  if (isLoading) {
    return (
      <div className="text-center py-12">
        <div className="text-gray-500">Loading your dashboard...</div>
      </div>
    );
  }

  const favoriteMovies = movies.filter(m => m.isFavorite);
  const libraryMovies = movies.filter(m => m.inLibrary);

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-8">
        <div className="flex-1 min-w-0">
          <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
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

      {error ? (
        <div className="rounded-md bg-red-50 p-4">
          <div className="text-sm text-red-700">{error}</div>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <RecentlyWatchedSection viewings={recentlyWatched} />
          <MovieListSection
            title="Personal Library"
            movies={libraryMovies.slice(0, 5)}
            onToggleFavorite={handleToggleFavorite}
            emptyMessage="Your personal library is empty"
          />
          <MovieListSection
            title="Favourites"
            movies={favoriteMovies.slice(0, 5)}
            onToggleFavorite={handleToggleFavorite}
            emptyMessage="No favorite movies yet"
          />
        </div>
      )}

      <AddMovieModal
        isOpen={isAddMovieModalOpen}
        onClose={() => setIsAddMovieModalOpen(false)}
        onSubmit={handleAddMovie}
      />
    </div>
  );
} 