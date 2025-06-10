import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import MovieDetail from '../components/movies/MovieDetail';
import EditMovieModal from '../components/movies/EditMovieModal';
import AddReviewModal from '../components/reviews/AddReviewModal';
import type { MovieInLibrary } from '../types/index';
import type { MovieCreateRequest } from '../types';
import * as movieService from '../services/movieService';

export default function MovieDetails() {
  const { id } = useParams<{ id: string }>();
  const [movie, setMovie] = useState<MovieInLibrary | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isReviewModalOpen, setIsReviewModalOpen] = useState(false);

  useEffect(() => {
    if (id) {
      loadMovie(parseInt(id, 10));
    }
  }, [id]);

  const loadMovie = async (movieId: number) => {
    try {
      setIsLoading(true);
      setError(null);
      const movieData = await movieService.getMovie(movieId);
      console.log('Movie Data:', movieData);
      const reviews = await movieService.getMovieReviews(movieId);
      console.log('Reviews:', reviews);
      setMovie({ ...movieData, reviews });
      console.log('Combined Movie with Reviews:', { ...movieData, reviews });
    } catch (err) {
      setError('Failed to load movie details. Please try again later.');
      console.error('Error loading movie details:', err);
      setMovie(null);
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

  const handleUpdateMovie = async (movieId: number, movieData: MovieCreateRequest) => {
    if (!movie) return;
    try {
      await movieService.updateMovie(movieId, movieData);
      await loadMovie(movieId);
      setIsEditModalOpen(false);
    } catch (err: any) {
      if (err?.response?.status !== 401) {
        console.error('Error updating movie:', err);
      }
    }
  };

  const handleAddReview = async (review: { score: number; reviewText: string }) => {
    if (!movie?.id) return;
    try {
      await movieService.addMovieReview(movie.id, review);
      await loadMovie(movie.id);
      setIsReviewModalOpen(false);
    } catch (err) {
      console.error('Error adding review:', err);
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
        onToggleWatched={handleToggleWatched}
        onToggleWatchLater={handleToggleWatchLater}
        onToggleFavorite={handleToggleFavorite}
        onEditMovie={() => setIsEditModalOpen(true)}
        onAddReview={() => setIsReviewModalOpen(true)}
      />

      {isEditModalOpen && (
        <EditMovieModal
          movie={movie}
          isOpen={isEditModalOpen}
          onClose={() => setIsEditModalOpen(false)}
          onSubmit={handleUpdateMovie}
        />
      )}

      {isReviewModalOpen && (
        <AddReviewModal
          isOpen={isReviewModalOpen}
          onClose={() => setIsReviewModalOpen(false)}
          onSubmit={handleAddReview}
          movieTitle={movie.title}
        />
      )}
    </>
  );
} 