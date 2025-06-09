import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { MovieInLibrary, Review } from '../../types';
import { StarIcon, ClockIcon, HeartIcon, PencilIcon } from '@heroicons/react/24/outline';
import { StarIcon as StarIconSolid, ClockIcon as ClockIconSolid, HeartIcon as HeartIconSolid } from '@heroicons/react/24/solid';
import AddReviewModal from '../reviews/AddReviewModal';

interface MovieDetailProps {
  movie: MovieInLibrary;
  reviews: Review[];
  onToggleWatched?: (movieId: string) => void;
  onToggleWatchLater?: (movieId: string) => void;
  onToggleFavorite?: (movieId: string) => void;
  onAddReview?: (movieId: string, rating: number, comment: string) => void;
  onEditMovie?: (movieId: string) => void;
}

export default function MovieDetail({
  movie,
  reviews,
  onToggleWatched,
  onToggleWatchLater,
  onToggleFavorite,
  onAddReview,
  onEditMovie,
}: MovieDetailProps) {
  const [isReviewModalOpen, setIsReviewModalOpen] = useState(false);
  const navigate = useNavigate();

  const averageRating = reviews.length > 0
    ? reviews.reduce((acc, review) => acc + review.rating, 0) / reviews.length
    : null;

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="lg:grid lg:grid-cols-3 lg:gap-8">
        {/* Movie Poster */}
        <div className="lg:col-span-1">
          <div className="aspect-[2/3] relative rounded-lg overflow-hidden shadow-lg">
            {movie.posterUrl ? (
              <img
                src={movie.posterUrl}
                alt={movie.title}
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="w-full h-full bg-gray-200 flex items-center justify-center">
                <span className="text-gray-400">No poster</span>
              </div>
            )}
          </div>
        </div>

        {/* Movie Info */}
        <div className="lg:col-span-2 mt-8 lg:mt-0">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">{movie.title}</h1>
              <div className="mt-2 flex items-center text-sm text-gray-500">
                <span>{movie.releaseYear}</span>
                <span className="mx-2">•</span>
                <span>{movie.genre}</span>
                {averageRating && (
                  <>
                    <span className="mx-2">•</span>
                    <div className="flex items-center">
                      <StarIconSolid className="w-4 h-4 text-yellow-400" />
                      <span className="ml-1">{averageRating.toFixed(1)}/5</span>
                    </div>
                  </>
                )}
              </div>
            </div>

            {onEditMovie && (
              <button
                onClick={() => onEditMovie(movie.id)}
                className="btn btn-secondary flex items-center"
              >
                <PencilIcon className="w-4 h-4 mr-2" />
                Edit
              </button>
            )}
          </div>

          <p className="mt-4 text-gray-600">{movie.description}</p>

          {/* Action Buttons */}
          <div className="mt-6 flex items-center space-x-4">
            {onToggleWatched && (
              <button
                onClick={() => onToggleWatched(movie.id)}
                className={`btn ${
                  movie.isWatched
                    ? 'bg-green-100 text-green-700 hover:bg-green-200'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                } flex items-center`}
              >
                {movie.isWatched ? (
                  <StarIconSolid className="w-5 h-5 mr-2" />
                ) : (
                  <StarIcon className="w-5 h-5 mr-2" />
                )}
                {movie.isWatched ? 'Watched' : 'Mark as Watched'}
              </button>
            )}

            {onToggleWatchLater && (
              <button
                onClick={() => onToggleWatchLater(movie.id)}
                className={`btn ${
                  movie.isWatchLater
                    ? 'bg-blue-100 text-blue-700 hover:bg-blue-200'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                } flex items-center`}
              >
                {movie.isWatchLater ? (
                  <ClockIconSolid className="w-5 h-5 mr-2" />
                ) : (
                  <ClockIcon className="w-5 h-5 mr-2" />
                )}
                {movie.isWatchLater ? 'In Watch Later' : 'Add to Watch Later'}
              </button>
            )}

            {onToggleFavorite && (
              <button
                onClick={() => onToggleFavorite(movie.id)}
                className={`btn ${
                  movie.isFavorite
                    ? 'bg-red-100 text-red-700 hover:bg-red-200'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                } flex items-center`}
              >
                {movie.isFavorite ? (
                  <HeartIconSolid className="w-5 h-5 mr-2" />
                ) : (
                  <HeartIcon className="w-5 h-5 mr-2" />
                )}
                {movie.isFavorite ? 'Favorited' : 'Add to Favorites'}
              </button>
            )}
          </div>

          {/* Reviews Section */}
          <div className="mt-8">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-900">Reviews</h2>
              {onAddReview && (
                <button
                  onClick={() => setIsReviewModalOpen(true)}
                  className="btn btn-primary"
                >
                  Add Review
                </button>
              )}
            </div>

            {reviews.length > 0 ? (
              <div className="space-y-4">
                {reviews.map((review) => (
                  <div
                    key={review.id}
                    className="bg-white p-4 rounded-lg shadow"
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center">
                        <StarIconSolid className="w-4 h-4 text-yellow-400" />
                        <span className="ml-1 text-sm text-gray-600">
                          {review.rating}/5
                        </span>
                      </div>
                      <span className="text-sm text-gray-500">
                        {new Date(review.viewDate).toLocaleDateString()}
                      </span>
                    </div>
                    <p className="mt-2 text-gray-600">{review.comment}</p>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500">No reviews yet</p>
            )}
          </div>
        </div>
      </div>

      {onAddReview && (
        <AddReviewModal
          isOpen={isReviewModalOpen}
          onClose={() => setIsReviewModalOpen(false)}
          onSubmit={(rating, comment) => {
            onAddReview(movie.id, rating, comment);
            setIsReviewModalOpen(false);
          }}
        />
      )}
    </div>
  );
} 