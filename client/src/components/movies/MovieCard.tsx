import { Link } from 'react-router-dom';
import type { MovieInLibrary } from '../../types';
import { StarIcon, ClockIcon, HeartIcon } from '@heroicons/react/24/outline';
import { StarIcon as StarIconSolid, ClockIcon as ClockIconSolid, HeartIcon as HeartIconSolid } from '@heroicons/react/24/solid';

interface MovieCardProps {
  movie: MovieInLibrary;
  onToggleWatched?: (movieId: number) => void;
  onToggleWatchLater?: (movieId: number) => void;
  onToggleFavorite?: (movieId: number) => void;
}

export default function MovieCard({ 
  movie, 
  onToggleWatched,
  onToggleWatchLater,
  onToggleFavorite 
}: MovieCardProps) {
  if (!movie) {
    return null;
  }

  const releaseYear = new Date(movie.releaseDate).getFullYear();

  return (
    <div className="bg-white rounded-lg shadow overflow-hidden">
      <Link to={`/movies/${movie.id}`}>
        <div className="aspect-[2/3] relative">
          <div className="w-full h-full bg-gray-200 flex items-center justify-center">
            <span className="text-gray-400">No poster</span>
          </div>
        </div>
      </Link>

      <div className="p-4">
        <Link to={`/movies/${movie.id}`}>
          <h3 className="text-lg font-medium text-gray-900 truncate">
            {movie.title}
          </h3>
        </Link>
        <div className="mt-1 flex items-center text-sm text-gray-500">
          <span>{releaseYear}</span>
          <span className="mx-2">•</span>
          <span>{movie.genre.title}</span>
        </div>

        <div className="mt-4 flex items-center justify-between">
          <div className="flex space-x-2">
            {onToggleWatched && (
              <button
                onClick={() => onToggleWatched(movie.id)}
                className={`p-1.5 rounded-full ${
                  movie.isWatched
                    ? 'bg-green-100 text-green-600'
                    : 'bg-gray-100 text-gray-400 hover:text-gray-500'
                }`}
                title={movie.isWatched ? "Watched" : "Mark as watched"}
              >
                {movie.isWatched ? (
                  <StarIconSolid className="w-5 h-5" />
                ) : (
                  <StarIcon className="w-5 h-5" />
                )}
              </button>
            )}

            {onToggleWatchLater && (
              <button
                onClick={() => onToggleWatchLater(movie.id)}
                className={`p-1.5 rounded-full ${
                  movie.isWatchLater
                    ? 'bg-blue-100 text-blue-600'
                    : 'bg-gray-100 text-gray-400 hover:text-gray-500'
                }`}
                title={movie.isWatchLater ? "In watch later" : "Add to watch later"}
              >
                {movie.isWatchLater ? (
                  <ClockIconSolid className="w-5 h-5" />
                ) : (
                  <ClockIcon className="w-5 h-5" />
                )}
              </button>
            )}

            {onToggleFavorite && (
              <button
                onClick={() => onToggleFavorite(movie.id)}
                className={`p-1.5 rounded-full ${
                  movie.isFavorite
                    ? 'bg-red-100 text-red-600'
                    : 'bg-gray-100 text-gray-400 hover:text-gray-500'
                }`}
                title={movie.isFavorite ? "Favorited" : "Add to favorites"}
              >
                {movie.isFavorite ? (
                  <HeartIconSolid className="w-5 h-5" />
                ) : (
                  <HeartIcon className="w-5 h-5" />
                )}
              </button>
            )}
          </div>

          {movie.userRating && (
            <div className="flex items-center">
              <StarIconSolid className="w-4 h-4 text-yellow-400" />
              <span className="ml-1 text-sm text-gray-600">{movie.userRating}/5</span>
            </div>
          )}
        </div>
      </div>
    </div>
  );
} 