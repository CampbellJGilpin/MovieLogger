import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import MovieDetail from '../components/movies/MovieDetail';
import EditMovieModal from '../components/movies/EditMovieModal';
import type { Movie, MovieInLibrary, Review } from '../types';
import * as movieService from '../services/movieService';

export default function MovieDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [movie, setMovie] = useState<MovieInLibrary | null>(null);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  useEffect(() => {
    if (id) {
      loadMovie(parseInt(id, 10));
    }
  }, [id]);

  const loadMovie = async (movieId: number) => {
    try {
      setIsLoading(true);
      setError(null);
      const [movieData, reviewsData] = await Promise.all([
        movieService.getMovie(movieId),
        movieService.getMovieReviews(movieId),
      ]);
      setMovie(movieData);
      setReviews(reviewsData);
    } catch (err) {
      setError('Failed to load movie details. Please try again later.');
      console.error('Error loading movie details:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleToggleWatched = async (movieId: number) => {
    if (!movie) return;
    try {
      await movieService.toggleWatched(movieId);
      setMovie({ ...movie, isWatched: !movie.isWatched });
    } catch (err) {
      console.error('Error toggling watched status:', err);
    }
  };

  const handleToggleWatchLater = async (movieId: number) => {
    if (!movie) return;
    try {
      await movieService.toggleWatchLater(movieId);
      setMovie({ ...movie, isWatchLater: !movie.isWatchLater });
    } catch (err) {
      console.error('Error toggling watch later status:', err);
    }
  };

  const handleToggleFavorite = async (movieId: number) => {
    if (!movie) return;
    try {
      await movieService.toggleFavorite(movieId);
      setMovie({ ...movie, isFavorite: !movie.isFavorite });
    } catch (err) {
      console.error('Error toggling favorite status:', err);
    }
  };

  const handleAddReview = async (movieId: number, rating: number, comment: string) => {
    try {
      const newReview = await movieService.addMovieReview(movieId, { rating, comment });
      setReviews([...reviews, newReview]);
    } catch (err) {
      console.error('Error adding review:', err);
    }
  };

  const handleUpdateMovie = async (movieId: number, movieData: Omit<Movie, 'id'>) => {
    try {
      const updatedMovie = await movieService.updateMovie(movieId, movieData);
      setMovie({ ...movie!, ...updatedMovie });
      setIsEditModalOpen(false);
    } catch (err) {
      console.error('Error updating movie:', err);
    }
  };

  if (isLoading) {
    return (
      <div className="text-center py-12">
        <div className="text-gray-500">Loading movie details...</div>
      </div>
    );
  }

  if (error || !movie) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="rounded-md bg-red-50 p-4">
          <div className="text-sm text-red-700">
            {error || 'Movie not found'}
          </div>
        </div>
      </div>
    );
  }

  return (
    <>
      <MovieDetail
        movie={movie}
        reviews={reviews}
        onToggleWatched={handleToggleWatched}
        onToggleWatchLater={handleToggleWatchLater}
        onToggleFavorite={handleToggleFavorite}
        onAddReview={handleAddReview}
        onEditMovie={() => setIsEditModalOpen(true)}
      />

      <EditMovieModal
        movie={movie}
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSubmit={handleUpdateMovie}
      />
    </>
  );
} 