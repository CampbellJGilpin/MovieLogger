import { useState, useEffect } from 'react';
import { XMarkIcon, CheckIcon } from '@heroicons/react/24/outline';
import { useAuth } from '../../contexts/useAuth';
import listService, { type ListSummary } from '../../services/listService';
import type { Movie } from '../../types/index';

interface AddToListModalProps {
  isOpen: boolean;
  onClose: () => void;
  movie: Movie | null;
  onSuccess?: () => void;
}

export default function AddToListModal({ isOpen, onClose, movie, onSuccess }: AddToListModalProps) {
  const { user } = useAuth();
  const [lists, setLists] = useState<ListSummary[]>([]);
  const [loading, setLoading] = useState(false);
  const [adding, setAdding] = useState<Record<number, boolean>>({});
  const [movieInLists, setMovieInLists] = useState<Record<number, boolean>>({});
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen && user && movie) {
      loadUserLists();
      checkMovieInLists();
    }
  }, [isOpen, user, movie]);

  const loadUserLists = async () => {
    if (!user) return;
    
    try {
      setLoading(true);
      const userLists = await listService.getUserLists(user.id);
      setLists(userLists);
      setError(null);
    } catch (err) {
      console.error('Failed to load lists:', err);
      setError('Failed to load lists');
    } finally {
      setLoading(false);
    }
  };

  const checkMovieInLists = async () => {
    if (!user || !movie) return;
    
    try {
      const userLists = await listService.getUserLists(user.id);
      const movieStatus: Record<number, boolean> = {};
      
      for (const list of userLists) {
        try {
          const inList = await listService.isMovieInList(user.id, list.id, movie.id);
          movieStatus[list.id] = inList;
        } catch (err) {
          movieStatus[list.id] = false;
        }
      }
      
      setMovieInLists(movieStatus);
    } catch (err) {
      console.error('Failed to check movie status in lists:', err);
    }
  };

  const handleAddToList = async (listId: number) => {
    if (!user || !movie) return;

    setAdding(prev => ({ ...prev, [listId]: true }));
    setError(null);

    try {
      await listService.addMovieToList(user.id, listId, movie.id);
      setMovieInLists(prev => ({ ...prev, [listId]: true }));
      onSuccess?.();
    } catch (err: any) {
      console.error('Failed to add movie to list:', err);
      setError(err.response?.data?.message || 'Failed to add movie to list');
    } finally {
      setAdding(prev => ({ ...prev, [listId]: false }));
    }
  };

  const handleRemoveFromList = async (listId: number) => {
    if (!user || !movie) return;

    setAdding(prev => ({ ...prev, [listId]: true }));
    setError(null);

    try {
      await listService.removeMovieFromList(user.id, listId, movie.id);
      setMovieInLists(prev => ({ ...prev, [listId]: false }));
      onSuccess?.();
    } catch (err: any) {
      console.error('Failed to remove movie from list:', err);
      setError(err.response?.data?.message || 'Failed to remove movie from list');
    } finally {
      setAdding(prev => ({ ...prev, [listId]: false }));
    }
  };

  const handleClose = () => {
    if (!Object.values(adding).some(Boolean)) {
      onClose();
      setError(null);
    }
  };

  if (!isOpen || !movie) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={handleClose} />

        <div className="inline-block w-full max-w-md p-6 my-8 overflow-hidden text-left align-middle transition-all transform bg-white shadow-xl rounded-lg">
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-medium text-gray-900">
              Add "{movie.title}" to Lists
            </h3>
            <button
              onClick={handleClose}
              className="text-gray-400 hover:text-gray-600"
            >
              <XMarkIcon className="h-6 w-6" />
            </button>
          </div>

          {error && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
              <p className="text-sm text-red-600">{error}</p>
            </div>
          )}

          <div className="max-h-64 overflow-y-auto">
            {loading ? (
              <div className="text-center py-4">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
                <p className="mt-2 text-sm text-gray-500">Loading your lists...</p>
              </div>
            ) : lists.length === 0 ? (
              <div className="text-center py-8">
                <p className="text-gray-500 mb-4">You don't have any lists yet.</p>
                <button
                  onClick={() => {
                    onClose();
                    window.location.href = '/lists';
                  }}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Create Your First List
                </button>
              </div>
            ) : (
              <div className="space-y-2">
                {lists.map(list => {
                  const inList = movieInLists[list.id];
                  const isWorking = adding[list.id];

                  return (
                    <div key={list.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                      <div className="flex-1 min-w-0">
                        <h4 className="text-sm font-medium text-gray-900">{list.name}</h4>
                        {list.description && (
                          <p className="text-xs text-gray-500 truncate">{list.description}</p>
                        )}
                        <p className="text-xs text-gray-400">{list.movieCount} movies</p>
                      </div>
                      
                      <button
                        onClick={() => inList ? handleRemoveFromList(list.id) : handleAddToList(list.id)}
                        disabled={isWorking}
                        className={`ml-3 px-3 py-1 text-xs font-medium rounded-full transition-colors ${
                          inList
                            ? 'bg-green-100 text-green-800 hover:bg-green-200'
                            : 'bg-blue-100 text-blue-800 hover:bg-blue-200'
                        } disabled:opacity-50 disabled:cursor-not-allowed`}
                      >
                        {isWorking ? (
                          <div className="animate-spin rounded-full h-3 w-3 border border-current border-t-transparent"></div>
                        ) : inList ? (
                          <>
                            <CheckIcon className="w-3 h-3 inline mr-1" />
                            In List
                          </>
                        ) : (
                          'Add to List'
                        )}
                      </button>
                    </div>
                  );
                })}
              </div>
            )}
          </div>

          <div className="mt-6 flex justify-end">
            <button
              onClick={handleClose}
              disabled={Object.values(adding).some(Boolean)}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200 disabled:opacity-50"
            >
              Done
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}