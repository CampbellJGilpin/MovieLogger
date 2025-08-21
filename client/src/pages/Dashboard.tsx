import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { PlusIcon } from '@heroicons/react/24/outline';
import type { MovieInLibrary, Viewing } from '../types/index';
import * as movieService from '../services/movieService';
import AddMovieModal from '../components/movies/AddMovieModal';
import GenrePreferencesSection from '../components/dashboard/GenrePreferencesSection';

interface MovieListSectionProps {
  title: string;
  movies: MovieInLibrary[];
  onToggleFavorite?: (movieId: number) => void;
  emptyMessage?: string;
}

function RecentlyWatchedSection({ viewings }: { viewings: Viewing[] }) {
  // Helper to get full poster URL
  const getPosterUrl = (posterPath?: string) => {
    if (!posterPath) return undefined;
    if (posterPath.startsWith('http')) return posterPath;
    // Static files are served from the backend root, not the /api endpoint
    const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5049';
    const baseUrl = apiBaseUrl.replace('/api', ''); // Remove /api suffix for static files
    return `${baseUrl}${posterPath}`;
  };

  return (
    <div className="bg-white rounded-lg shadow p-6">
      <h2 className="text-lg font-semibold text-gray-900 mb-4">Recently Watched</h2>
      {viewings.length === 0 ? (
        <p className="text-gray-500 text-sm">No recently watched movies</p>
      ) : (
        <div className="flex space-x-4 overflow-x-auto pb-2">
          {viewings.slice(0, 8).map(viewing => {
            const posterUrl = getPosterUrl(viewing.movie.posterPath);
            return (
              <div key={viewing.id} className="flex-shrink-0">
                <Link to={`/movies/${viewing.movie.id}`} className="block">
                  <div className="w-32">
                    <div className="relative aspect-[2/3] mb-2">
                      {posterUrl ? (
                        <img
                          src={posterUrl}
                          alt={viewing.movie.title}
                          className="w-full h-full object-cover rounded-lg shadow-sm"
                        />
                      ) : (
                        <div className="w-full h-full bg-gray-200 rounded-lg flex items-center justify-center">
                          <span className="text-gray-400 text-xs text-center px-2">
                            No poster
                          </span>
                        </div>
                      )}
                    </div>
                    <div className="text-xs">
                      <h3 className="font-medium text-gray-900 hover:text-indigo-600 line-clamp-2">
                        {viewing.movie.title}
                      </h3>
                      <p className="text-gray-500 mt-1">
                        {new Date(viewing.movie.releaseDate).getFullYear()}
                      </p>
                      <p className="text-gray-400">
                        Watched {new Date(viewing.dateViewed).toLocaleDateString()}
                      </p>
                      {viewing.review && viewing.review.score > 0 && (
                        <div className="flex items-center mt-1">
                          {[1, 2, 3, 4, 5].map((star) => (
                            <svg
                              key={star}
                              className={`h-3 w-3 ${
                                star <= viewing.review!.score
                                  ? 'text-yellow-400'
                                  : 'text-gray-300'
                              }`}
                              fill="currentColor"
                              viewBox="0 0 20 20"
                            >
                              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                            </svg>
                          ))}
                        </div>
                      )}
                    </div>
                  </div>
                </Link>
              </div>
            );
          })}
        </div>
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

  const handleAddMovie = async (movieData: FormData) => {
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
        <>
          <div className="mb-8">
            <RecentlyWatchedSection viewings={recentlyWatched} />
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
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
            <GenrePreferencesSection />
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