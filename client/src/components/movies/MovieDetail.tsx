import type { MovieInLibrary } from '../../types/index';
import { PencilIcon, ClockIcon } from '@heroicons/react/24/outline';
import Toggle from '../common/Toggle';
import StarRating from '../common/StarRating';

interface MovieDetailProps {
  movie: MovieInLibrary;
  onToggleLibrary?: (movieId: number) => void;
  onToggleWatchLater?: (movieId: number) => void;
  onToggleFavorite?: (movieId: number) => void;
  onEditMovie?: (movieId: number) => void;
  onAddReview?: () => void;
}

export default function MovieDetail({
  movie,
  onToggleLibrary,
  onToggleWatchLater,
  onToggleFavorite,
  onEditMovie,
  onAddReview,
}: MovieDetailProps) {
  if (!movie?.id || !movie?.title) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-gray-500">
          <p>Movie not found</p>
        </div>
      </div>
    );
  }

  // Get the 5 most recent reviews
  const recentReviews = movie.reviews?.slice(0, 5) || [];

  // Helper to get full poster URL
  const getPosterUrl = () => {
    if (!movie.posterPath) return undefined;
    if (movie.posterPath.startsWith('http')) return movie.posterPath;
    // Static files are served from the backend root, not the /api endpoint
    const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5049';
    const baseUrl = apiBaseUrl.replace('/api', ''); // Remove /api suffix for static files
    return `${baseUrl}${movie.posterPath}`;
  };

  // Helper to format runtime in minutes to hours and minutes
  const formatRuntime = (runtimeMinutes?: number) => {
    if (!runtimeMinutes) return null;
    const hours = Math.floor(runtimeMinutes / 60);
    const minutes = runtimeMinutes % 60;
    if (hours === 0) return `${minutes}m`;
    if (minutes === 0) return `${hours}h`;
    return `${hours}h ${minutes}m`;
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="grid grid-cols-12 gap-8">
        {/* Left Column - Movie Details */}
        <div className="col-span-5">
          {/* Title Section */}
          <div className="mb-6">
            <h1 className="text-4xl font-bold text-gray-900 mb-2">{movie.title}</h1>
            <div className="flex items-center gap-3">
              <span className="text-xl text-gray-600">{new Date(movie.releaseDate).getFullYear()}</span>
              {formatRuntime(movie.runtimeMinutes) && (
                <span className="text-xl text-gray-600">•</span>
              )}
              {formatRuntime(movie.runtimeMinutes) && (
                <span className="text-xl text-gray-600">{formatRuntime(movie.runtimeMinutes)}</span>
              )}
              <span className="px-3 py-1 rounded-full bg-blue-100 text-blue-800 text-sm font-medium">
                {movie.genre.title}
              </span>
            </div>
          </div>

          {/* Description */}
          <p className="text-gray-600 mb-6">{movie.description}</p>

          {/* Actions Row */}
          <div className="flex flex-wrap items-center gap-4">
            <div className="flex gap-3">
              <button
                onClick={() => onEditMovie?.(movie.id)}
                className="inline-flex items-center px-3 py-1.5 rounded-md text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
              >
                <PencilIcon className="h-4 w-4 mr-1.5" />
                Edit Movie
              </button>
              <button
                onClick={onAddReview}
                className="inline-flex items-center px-3 py-1.5 rounded-md text-sm font-medium text-white bg-green-600 hover:bg-green-700"
              >
                <ClockIcon className="h-4 w-4 mr-1.5" />
                Log Viewing
              </button>
            </div>

            <div className="flex items-center gap-4">
              <Toggle
                enabled={movie.inLibrary}
                onChange={() => onToggleLibrary?.(movie.id)}
                label="In Library"
              />
              <Toggle
                enabled={!!movie.isWatchLater}
                onChange={() => onToggleWatchLater?.(movie.id)}
                label="Watch Later"
              />
              <Toggle
                enabled={movie.isFavorite}
                onChange={() => onToggleFavorite?.(movie.id)}
                label="Favourite"
              />
            </div>
          </div>
        </div>

        {/* Middle Column - Poster */}
        <div className="col-span-3 -mx-4">
          <div className="aspect-[2/3] bg-gray-100 rounded-lg overflow-hidden shadow-lg flex items-center justify-center">
            {getPosterUrl() ? (
              <img
                src={getPosterUrl()}
                alt={movie.title + ' poster'}
                className="object-cover w-full h-full"
              />
            ) : (
              <div className="w-full h-full flex items-center justify-center text-gray-400">
                No poster
              </div>
            )}
          </div>
        </div>

        {/* Right Column - View History */}
        <div className="col-span-4">
          <h2 className="text-xl font-semibold mb-4">View History</h2>
          <div className="space-y-3 max-h-[400px] overflow-y-auto pr-2">
            {recentReviews.map((review) => (
              <div key={review.id} className="border-b pb-2.5">
                <div className="flex items-center justify-between mb-1">
                  <div className="text-sm text-gray-500">
                    {new Date(review.dateViewed).toLocaleDateString()}
                  </div>
                  <StarRating rating={review.score} className="ml-2" />
                </div>
                <div className="text-sm">{review.reviewText}</div>
              </div>
            ))}
            {recentReviews.length === 0 && (
              <div className="text-gray-500 text-sm">No viewing history</div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
} 