import { useState, useEffect, useCallback } from 'react';
import { PlusIcon } from '@heroicons/react/24/outline';
import MovieList from '../components/movies/MovieList';
import AddMovieModal from '../components/movies/AddMovieModal';
import type { Movie, MovieInLibrary } from '../types';
import * as movieService from '../services/movieService';

interface MovieCreateRequest {
  title: string;
  description: string;
  releaseDate: string;
  genreId: number;
}

export default function AllMovies() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isAddMovieModalOpen, setIsAddMovieModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearchQuery, setDebouncedSearchQuery] = useState('');

  useEffect(() => {
    loadMovies();
  }, []);

  // Debounce search query
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchQuery(searchQuery);
    }, 500); // Wait for 500ms after the user stops typing

    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Perform search when debounced query changes
  useEffect(() => {
    if (debouncedSearchQuery) {
      handleSearch(debouncedSearchQuery);
    } else {
      loadMovies();
    }
  }, [debouncedSearchQuery]);

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

  const handleSearch = async (query: string) => {
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

  const handleAddMovie = async (movieData: MovieCreateRequest) => {
    try {
      await movieService.createMovie(movieData);
      await loadMovies();
      setIsAddMovieModalOpen(false);
    } catch (err) {
      console.error('Error adding movie:', err);
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
        <MovieList
          movies={movies}
          onToggleInLibrary={handleToggleInLibrary}
          onToggleFavorite={handleToggleFavorite}
          onDelete={handleDeleteMovie}
          emptyMessage={
            searchQuery
              ? 'No movies found matching your search'
              : 'No movies added yet'
          }
        />
      )}

      <AddMovieModal
        isOpen={isAddMovieModalOpen}
        onClose={() => setIsAddMovieModalOpen(false)}
        onSubmit={handleAddMovie}
      />
    </div>
  );
} 