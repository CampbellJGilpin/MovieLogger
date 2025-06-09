import { useState, useEffect } from 'react';
import { PlusIcon } from '@heroicons/react/24/outline';
import MovieList from '../components/movies/MovieList';
import AddMovieModal from '../components/movies/AddMovieModal';
import type { Movie, MovieInLibrary } from '../types';
import * as movieService from '../services/movieService';

export default function AllMovies() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isAddMovieModalOpen, setIsAddMovieModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    loadMovies();
  }, []);

  const loadMovies = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await movieService.getAllMovies();
      setMovies(data);
    } catch (err) {
      setError('Failed to load movies. Please try again later.');
      console.error('Error loading movies:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearch = async () => {
    if (!searchQuery.trim()) {
      loadMovies();
      return;
    }

    try {
      setIsLoading(true);
      setError(null);
      const results = await movieService.searchMovies(searchQuery);
      setMovies(results);
    } catch (err) {
      setError('Failed to search movies. Please try again later.');
      console.error('Error searching movies:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleAddMovie = async (movieData: Omit<Movie, 'id'>) => {
    try {
      await movieService.createMovie(movieData);
      loadMovies();
    } catch (err) {
      setError('Failed to add movie. Please try again later.');
      console.error('Error adding movie:', err);
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

  return (
    <div className="py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">All Movies</h1>
          <button
            onClick={() => setIsAddMovieModalOpen(true)}
            className="btn btn-primary flex items-center"
          >
            <PlusIcon className="w-5 h-5 mr-2" />
            Add Movie
          </button>
        </div>

        <div className="mb-6">
          <div className="max-w-lg">
            <div className="mt-1 flex rounded-md shadow-sm">
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                placeholder="Search movies..."
                className="input flex-1"
              />
              <button
                onClick={handleSearch}
                className="ml-3 btn btn-secondary"
              >
                Search
              </button>
            </div>
          </div>
        </div>

        {error && (
          <div className="rounded-md bg-red-50 p-4 mb-6">
            <div className="text-sm text-red-700">{error}</div>
          </div>
        )}

        {isLoading ? (
          <div className="text-center py-12">
            <div className="text-gray-500">Loading movies...</div>
          </div>
        ) : (
          <MovieList
            movies={movies}
            onToggleWatched={handleToggleWatched}
            onToggleWatchLater={handleToggleWatchLater}
            onToggleFavorite={handleToggleFavorite}
            emptyMessage="No movies found. Try adding some!"
          />
        )}
      </div>

      <AddMovieModal
        isOpen={isAddMovieModalOpen}
        onClose={() => setIsAddMovieModalOpen(false)}
        onSubmit={handleAddMovie}
      />
    </div>
  );
} 