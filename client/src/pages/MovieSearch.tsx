import { useState } from 'react';
import MovieList from '../components/movies/MovieList';
import type { MovieInLibrary } from '../types/index';
import * as movieService from '../services/movieService';

export default function MovieSearch() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');

  const handleSearch = async (query: string) => {
    setSearchQuery(query);
    if (!query.trim()) {
      setMovies([]);
      return;
    }

    try {
      setIsLoading(true);
      setError(null);
      const results = await movieService.searchMovies(query);
      setMovies(results);
    } catch (err) {
      setError('Failed to search movies. Please try again later.');
      console.error('Error searching movies:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleToggleLibrary = async (movieId: number) => {
    try {
      await movieService.toggleLibrary(movieId);
      setMovies(movies.map(movie =>
        movie.id === movieId
          ? { ...movie, inLibrary: !movie.inLibrary }
          : movie
      ));
    } catch (error) {
      console.error('Error toggling library status:', error);
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
    } catch (error) {
      console.error('Error toggling favorite status:', error);
    }
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 className="text-2xl font-bold text-gray-900 mb-8">Search Movies</h1>

      <div className="mb-8">
        <input
          type="text"
          placeholder="Search movies..."
          value={searchQuery}
          onChange={(e) => handleSearch(e.target.value)}
          className="input w-full max-w-md"
        />
      </div>

      {error ? (
        <div className="rounded-md bg-red-50 p-4">
          <div className="text-sm text-red-700">{error}</div>
        </div>
      ) : isLoading ? (
        <div className="text-center py-12">
          <div className="text-gray-500">Searching movies...</div>
        </div>
      ) : (
        <MovieList
          movies={movies}
          onToggleLibrary={handleToggleLibrary}
          onToggleFavorite={handleToggleFavorite}
          emptyMessage="No movies found matching your search"
        />
      )}
    </div>
  );
} 