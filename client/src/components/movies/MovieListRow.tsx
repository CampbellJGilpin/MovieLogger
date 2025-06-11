import { useState } from 'react';
import { Link } from 'react-router-dom';
import { HeartIcon as HeartIconSolid } from '@heroicons/react/24/solid';
import { HeartIcon, TrashIcon, CheckIcon } from '@heroicons/react/24/outline';
import type { MovieInLibrary } from '../../types';
import DeleteConfirmationModal from './DeleteConfirmationModal';

interface MovieListRowProps {
  movie: MovieInLibrary;
  onToggleWatched?: (movieId: number) => void;
  onToggleFavorite?: (movieId: number) => void;
  onDelete?: (movieId: number) => void;
}

export default function MovieListRow({ 
  movie, 
  onToggleWatched,
  onToggleFavorite,
  onDelete 
}: MovieListRowProps) {
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const releaseYear = movie.releaseDate ? new Date(movie.releaseDate).getFullYear() : 'Unknown';

  return (
    <>
      <tr>
        <td className="whitespace-nowrap py-4 pl-4 pr-3 text-sm sm:pl-6">
          <Link to={`/movies/${movie.id}`} className="text-indigo-600 hover:text-indigo-900">
            {movie.title}
          </Link>
        </td>
        <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{releaseYear}</td>
        <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{movie.genre.title}</td>
        <td className="whitespace-nowrap px-3 py-4 text-sm text-center">
          {onToggleWatched && (
            <button
              onClick={() => onToggleWatched(movie.id)}
              className={`p-1.5 rounded-full mr-2 ${
                movie.isWatched
                  ? 'bg-green-100 text-green-600'
                  : 'bg-gray-100 text-gray-400 hover:text-gray-500'
              }`}
              title={movie.isWatched ? "Watched" : "Mark as watched"}
            >
              <CheckIcon className="w-5 h-5" />
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
              title={movie.isFavorite ? "Favourited" : "Add to favourites"}
            >
              {movie.isFavorite ? (
                <HeartIconSolid className="w-5 h-5" />
              ) : (
                <HeartIcon className="w-5 h-5" />
              )}
            </button>
          )}
        </td>
        <td className="relative whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium sm:pr-6">
          {onDelete && (
            <button
              onClick={() => setIsDeleteModalOpen(true)}
              className="text-red-600 hover:text-red-900"
              title={`Delete ${movie.title}`}
            >
              <TrashIcon className="h-5 w-5" />
              <span className="sr-only">Delete {movie.title}</span>
            </button>
          )}
        </td>
      </tr>

      <DeleteConfirmationModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={() => onDelete?.(movie.id)}
        movieTitle={movie.title}
      />
    </>
  );
} 