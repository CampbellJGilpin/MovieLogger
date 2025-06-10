import { useState } from 'react';
import { Link } from 'react-router-dom';
import { TrashIcon } from '@heroicons/react/24/outline';
import type { MovieInLibrary } from '../../types';
import DeleteConfirmationModal from './DeleteConfirmationModal';

interface MovieListRowProps {
  movie: MovieInLibrary;
  onToggleWatched?: (movieId: number) => void;
  onToggleWatchLater?: (movieId: number) => void;
  onToggleFavorite?: (movieId: number) => void;
  onDelete?: (movieId: number) => void;
}

export default function MovieListRow({
  movie,
  onToggleWatched,
  onToggleWatchLater,
  onToggleFavorite,
  onDelete
}: MovieListRowProps) {
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const releaseDate = new Date(movie.releaseDate).toLocaleDateString();

  return (
    <>
      <tr className="border-b border-gray-200">
        <td className="py-4 pl-4 pr-3 text-sm sm:pl-6">
          <Link to={`/movies/${movie.id}`} className="font-medium text-gray-900 hover:text-indigo-600">
            {movie.title}
          </Link>
        </td>
        <td className="px-3 py-4 text-sm text-gray-500">{releaseDate}</td>
        <td className="px-3 py-4 text-sm text-center">
          <div className="flex items-center space-x-4">
            <input
              type="checkbox"
              checked={movie.isWatched}
              onChange={() => onToggleWatched?.(movie.id)}
              className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-600"
            />
            <input
              type="checkbox"
              checked={movie.isWatchLater}
              onChange={() => onToggleWatchLater?.(movie.id)}
              className="h-4 w-4 rounded border-gray-300 text-yellow-600 focus:ring-yellow-600"
            />
            <button
              onClick={() => onToggleFavorite?.(movie.id)}
              className={`text-${movie.isFavorite ? 'red' : 'gray'}-500 hover:text-red-700`}
            >
              <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9.653 16.915l-.005-.003-.019-.01a20.759 20.759 0 01-1.162-.682 22.045 22.045 0 01-2.582-1.9C4.045 12.733 2 10.352 2 7.5a4.5 4.5 0 018-2.828A4.5 4.5 0 0118 7.5c0 2.852-2.044 5.233-3.885 6.82a22.049 22.049 0 01-3.744 2.582l-.019.01-.005.003h-.002a.739.739 0 01-.69.001l-.002-.001z" />
              </svg>
            </button>
            {onDelete && (
              <button
                onClick={() => setIsDeleteModalOpen(true)}
                className="text-red-600 hover:text-red-900"
              >
                <TrashIcon className="h-5 w-5" />
                <span className="sr-only">Delete {movie.title}</span>
              </button>
            )}
          </div>
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