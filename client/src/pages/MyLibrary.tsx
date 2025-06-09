import { useState, useEffect } from 'react';
import MovieList from '../components/movies/MovieList';
import type { MovieInLibrary } from '../types';
import * as movieService from '../services/movieService';

export default function MyLibrary() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [activeTab, setActiveTab] = useState<'watched' | 'watchLater' | 'favorites'>('watched');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadLibrary();
  }, []);

  const loadLibrary = async () => {
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

  const handleToggleWatched = async (movieId: string) => {
    try {
      await movieService.toggleWatched(movieId);
      setMovies(movies.map(movie =>
        movie.id === movieId
          ? { ...movie, isWatched: !movie.isWatched }
          : movie
      ));
    } catch (err) {
      console.error('Error toggling watched status:', err);
    }
  };

  const handleToggleWatchLater = async (movieId: string) => {
    try {
      await movieService.toggleWatchLater(movieId);
      setMovies(movies.map(movie =>
        movie.id === movieId
          ? { ...movie, isWatchLater: !movie.isWatchLater }
          : movie
      ));
    } catch (err) {
      console.error('Error toggling watch later status:', err);
    }
  };

  const handleToggleFavorite = async (movieId: string) => {
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

  const tabs = [
    { id: 'watched', name: 'Watched', count: movies.filter(m => m.isWatched).length },
    { id: 'watchLater', name: 'Watch Later', count: movies.filter(m => m.isWatchLater).length },
    { id: 'favorites', name: 'Favorites', count: movies.filter(m => m.isFavorite).length },
  ];

  return (
    <div className="py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 className="text-2xl font-bold text-gray-900 mb-6">My Library</h1>

        <div className="border-b border-gray-200">
          <nav className="-mb-px flex space-x-8" aria-label="Tabs">
            {tabs.map((tab) => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id as typeof activeTab)}
                className={`
                  whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm
                  ${
                    activeTab === tab.id
                      ? 'border-blue-500 text-blue-600'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }
                `}
              >
                {tab.name}
                <span
                  className={`ml-2 py-0.5 px-2.5 rounded-full text-xs font-medium
                    ${
                      activeTab === tab.id
                        ? 'bg-blue-100 text-blue-600'
                        : 'bg-gray-100 text-gray-900'
                    }
                  `}
                >
                  {tab.count}
                </span>
              </button>
            ))}
          </nav>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 p-4 my-6">
            <div className="text-sm text-red-700">{error}</div>
          </div>
        )}

        {isLoading ? (
          <div className="text-center py-12">
            <div className="text-gray-500">Loading your library...</div>
          </div>
        ) : (
          <div className="mt-6">
            <MovieList
              movies={filteredMovies}
              onToggleWatched={handleToggleWatched}
              onToggleWatchLater={handleToggleWatchLater}
              onToggleFavorite={handleToggleFavorite}
              emptyMessage={`No ${activeTab} movies yet`}
            />
          </div>
        )}
      </div>
    </div>
  );
} 