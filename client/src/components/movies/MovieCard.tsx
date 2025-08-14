import type { MovieInLibrary } from '../../types/index';
import { HeartIcon } from '@heroicons/react/24/outline';
import { HeartIcon as HeartIconSolid } from '@heroicons/react/24/solid';

interface MovieCardProps {
  movie: MovieInLibrary;
  onToggleFavorite?: (movieId: number) => void;
}

export default function MovieCard({ 
  movie, 
  onToggleFavorite 
}: MovieCardProps) {
  if (!movie) {
    return null;
  }

  // Helper to get full poster URL
  const getPosterUrl = () => {
    if (!movie.posterPath) return undefined;
    if (movie.posterPath.startsWith('http')) return movie.posterPath;
    // Static files are served from the backend root, not the /api endpoint
    const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5049';
    const baseUrl = apiBaseUrl.replace('/api', ''); // Remove /api suffix for static files
    return `${baseUrl}${movie.posterPath}`;
  };

  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden">
      {/* Poster Image */}
      {getPosterUrl() && (
        <div className="aspect-[2/3] bg-gray-100 overflow-hidden">
          <img
            src={getPosterUrl()}
            alt={movie.title + ' poster'}
            className="object-cover w-full h-full"
          />
        </div>
      )}
      <div className="p-4">
        <div className="flex justify-between items-start mb-2">
          <h3 className="text-lg font-semibold text-gray-900">{movie.title}</h3>
          <div className="flex space-x-2">
            {onToggleFavorite && (
              <button
                onClick={() => onToggleFavorite(movie.id)}
                className={`p-1.5 rounded-full ${
                  movie.isFavorite
                    ? 'bg-red-100 text-red-600'
                    : 'bg-gray-100 text-gray-400 hover:text-gray-500'
                }`}
                title={movie.isFavorite ? "Favourited" : "Add to favourites"}
              >
                {movie.isFavorite ? (
                  <HeartIconSolid className="w-5 h-5" />
                ) : (
                  <HeartIcon className="w-5 h-5" />
                )}
              </button>
            )}
          </div>
        </div>

        <div className="text-sm text-gray-600 mb-2">
          {movie.releaseDate && (
            <span>Released: {new Date(movie.releaseDate).getFullYear()}</span>
          )}
          {movie.genre && (
            <span className="ml-2">• {movie.genre.title}</span>
          )}
        </div>

        {movie.description && (
          <p className="text-gray-700 text-sm mb-4">{movie.description}</p>
        )}

        {movie.userRating && (
          <div className="flex items-center">
            <span className="text-sm text-gray-600">{movie.userRating}/5</span>
          </div>
        )}
      </div>
    </div>
  );
} 