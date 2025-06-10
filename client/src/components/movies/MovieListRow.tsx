import { useState } from 'react';
import { Link } from 'react-router-dom';
import { TrashIcon } from '@heroicons/react/24/outline';
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
        <td className="px-3 py-4 text-sm text-gray-500">{movie.genre.title}</td>
        <td className="px-3 py-4 text-sm text-center">
          <input
            type="checkbox"
            checked={movie.isWatched}
            onChange={() => onToggleWatched?.(movie.id)}
            className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-600"
            id={`watched-${movie.id}`}
            title="In Library"
          />
        </td>
        <td className="px-3 py-4 text-sm text-center">
          <input
            type="checkbox"
            checked={movie.isFavorite}
            onChange={() => onToggleFavorite?.(movie.id)}
            className="h-4 w-4 rounded border-gray-300 text-red-600 focus:ring-red-600"
            id={`favorite-${movie.id}`}
            title="Favourite"
          />
        </td>
        <td className="px-3 py-4 text-sm text-right">
          {onDelete && (
            <button
              onClick={() => setIsDeleteModalOpen(true)}
              className="text-red-600 hover:text-red-900"
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