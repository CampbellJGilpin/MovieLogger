import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { MovieInLibrary } from '../../types';
import { StarIcon, ClockIcon, HeartIcon, PencilIcon } from '@heroicons/react/24/outline';
import { StarIcon as StarIconSolid, ClockIcon as ClockIconSolid, HeartIcon as HeartIconSolid } from '@heroicons/react/24/solid';

interface MovieDetailProps {
  movie: MovieInLibrary;
  onToggleWatched?: (movieId: number) => void;
  onToggleWatchLater?: (movieId: number) => void;
  onToggleFavorite?: (movieId: number) => void;
  onEditMovie?: (movieId: number) => void;
}

export default function MovieDetail({
  movie,
  onToggleWatched,
  onToggleWatchLater,
  onToggleFavorite,
  onEditMovie,
}: MovieDetailProps) {
  // Safety check for null/undefined movie
  if (!movie?.id || !movie?.title) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center text-gray-500">
          Loading movie details...
        </div>
      </div>
    );
  }

  // Safely get the release year
  const releaseYear = movie.releaseDate ? new Date(movie.releaseDate).getFullYear() : null;

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="lg:grid lg:grid-cols-3 lg:gap-8">
        {/* Movie Poster */}
        <div className="lg:col-span-1">
          <div className="aspect-[2/3] relative rounded-lg overflow-hidden shadow-lg">
            <div className="w-full h-full bg-gray-200 flex items-center justify-center">
              <span className="text-gray-400">No poster</span>
            </div>
          </div>
        </div>

        {/* Movie Info */}
        <div className="lg:col-span-2 mt-8 lg:mt-0">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">{movie.title}</h1>
              <div className="mt-2 flex items-center text-sm text-gray-500">
                {releaseYear && <span>{releaseYear}</span>}
                {releaseYear && movie.genre?.title && <span className="mx-2">•</span>}
                {movie.genre?.title && <span>{movie.genre.title}</span>}
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
        </div>
      </div>
    </div>
  );
} 