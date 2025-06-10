import { useState, useEffect } from 'react';
import MovieList from '../components/movies/MovieList';
import type { MovieInLibrary } from '../types';
import * as movieService from '../services/movieService';

export default function MyLibrary() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'watched' | 'watchLater' | 'favorites'>('watched');

  useEffect(() => {
    loadMovies();
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

  const handleToggleInLibrary = async (movieId: number) => {
    try {
      await movieService.toggleWatched(movieId);
      setMovies(movies.map(movie =>
        movie.id === movieId
          ? { ...movie, isWatched: !movie.isWatched }
          : movie
      ));
    } catch (err) {
      console.error('Error toggling library status:', err);
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

  const filteredMovies = movies.filter(movie => {
    switch (activeTab) {
      case 'watched':
        return movie.isWatched;
      case 'watchLater':
        return movie.isWatchLater;
      case 'favorites':
        return movie.isFavorite;
      default:
        return false;
    }
  });

  if (isLoading) {
    return (
      <div className="text-center py-12">
        <div className="text-gray-500">Loading your library...</div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 className="text-2xl font-bold text-gray-900 mb-8">My Library</h1>

      <div className="border-b border-gray-200 mb-8">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('watched')}
            className={`
              pb-4 px-1 border-b-2 font-medium text-sm
              ${activeTab === 'watched'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }
            `}
          >
            Watched
          </button>
          <button
            onClick={() => setActiveTab('watchLater')}
            className={`
              pb-4 px-1 border-b-2 font-medium text-sm
              ${activeTab === 'watchLater'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }
            `}
          >
            Watch Later
          </button>
          <button
            onClick={() => setActiveTab('favorites')}
            className={`
              pb-4 px-1 border-b-2 font-medium text-sm
              ${activeTab === 'favorites'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }
            `}
          >
            Favorites
          </button>
        </nav>
      </div>

      {error ? (
        <div className="rounded-md bg-red-50 p-4">
          <div className="text-sm text-red-700">{error}</div>
        </div>
      ) : (
        <MovieList
          movies={filteredMovies}
          onToggleWatched={handleToggleInLibrary}
          onToggleFavorite={handleToggleFavorite}
          onDelete={handleDeleteMovie}
          emptyMessage={
            isLoading
              ? 'Loading your library...'
              : error
              ? error
              : 'No movies in your library'
          }
        />
      )}
    </div>
  );
} 