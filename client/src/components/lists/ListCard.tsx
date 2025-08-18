import { useState } from 'react';
import { Link } from 'react-router-dom';
import { PencilIcon, TrashIcon, FilmIcon } from '@heroicons/react/24/outline';
import type { ListSummary } from '../../services/listService';

interface ListCardProps {
  list: ListSummary;
  onEdit: (list: ListSummary) => void;
  onDelete: (listId: number) => void;
}

export default function ListCard({ list, onEdit, onDelete }: ListCardProps) {
  const [isDeleting, setIsDeleting] = useState(false);

  const handleDelete = async () => {
    if (window.confirm(`Are you sure you want to delete "${list.name}"? This action cannot be undone.`)) {
      setIsDeleting(true);
      try {
        await onDelete(list.id);
      } catch (error) {
        console.error('Failed to delete list:', error);
        setIsDeleting(false);
      }
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  return (
    <div className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200 overflow-hidden">
      <div className="p-6">
        <div className="flex items-start justify-between">
          <div className="flex-1 min-w-0">
            <Link
              to={`/lists/${list.id}`}
              className="text-lg font-semibold text-gray-900 hover:text-blue-600 transition-colors"
            >
              {list.name}
            </Link>
            {list.description && (
              <p className="mt-1 text-sm text-gray-600 line-clamp-2">
                {list.description}
              </p>
            )}
          </div>
          <div className="flex items-center space-x-2 ml-4">
            <button
              onClick={() => onEdit(list)}
              className="p-1 text-gray-400 hover:text-blue-600 transition-colors"
              title="Edit list"
            >
              <PencilIcon className="h-4 w-4" />
            </button>
            <button
              onClick={handleDelete}
              disabled={isDeleting}
              className="p-1 text-gray-400 hover:text-red-600 transition-colors disabled:opacity-50"
              title="Delete list"
            >
              <TrashIcon className="h-4 w-4" />
            </button>
          </div>
        </div>

        <div className="mt-4 flex items-center justify-between text-sm text-gray-500">
          <div className="flex items-center">
            <FilmIcon className="h-4 w-4 mr-1" />
            <span>
              {list.movieCount} {list.movieCount === 1 ? 'movie' : 'movies'}
            </span>
          </div>
          <div>
            Updated {formatDate(list.updatedDate)}
          </div>
        </div>
      </div>
    </div>
  );
}