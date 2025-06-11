import { useState, useEffect } from 'react';
import MovieList from '../components/movies/MovieList';
import type { MovieInLibrary } from '../types';
import * as movieService from '../services/movieService';

export default function MyLibrary() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'all' | 'favorites'>('all');

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

  const handleToggleFavorite = async (movieId: number) => {
    try {
      const updatedMovie = await movieService.toggleFavorite(movieId);
      setMovies(movies.map(movie =>
        movie.id === movieId
          ? updatedMovie
          : movie
      ));
    } catch (err) {
      console.error('Error toggling favorite status:', err);
    }
  };

  const filteredMovies = activeTab === 'favorites' 
    ? movies.filter(m => m.isFavorite)
    : movies;

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-2xl font-bold text-gray-900">My Library</h1>
      </div>

      <div className="border-b border-gray-200 mb-8">
        <nav className="-mb-px flex space-x-8" aria-label="Tabs">
          <button
            onClick={() => setActiveTab('all')}
            className={`
              whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm
              ${activeTab === 'all'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }
            `}
          >
            All Movies
          </button>
          <button
            onClick={() => setActiveTab('favorites')}
            className={`
              whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm
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
          onToggleFavorite={handleToggleFavorite}
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