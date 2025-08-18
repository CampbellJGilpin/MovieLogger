import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { ArrowLeftIcon, PencilIcon, TrashIcon, PlusIcon } from '@heroicons/react/24/outline';
import { FilmIcon } from '@heroicons/react/24/solid';
import listService, { type List } from '../../services/listService';
import { useAuth } from '../../contexts/useAuth';
import MovieCard from '../movies/MovieCard';
import CreateListModal from './CreateListModal';

export default function ListDetail() {
  const { listId } = useParams<{ listId: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  
  const [list, setList] = useState<List | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  useEffect(() => {
    loadList();
  }, [listId, user]);

  const loadList = async () => {
    if (!user || !listId) return;

    try {
      setLoading(true);
      const listData = await listService.getList(user.id, parseInt(listId));
      setList(listData);
      setError(null);
    } catch (err: any) {
      console.error('Failed to load list:', err);
      if (err.response?.status === 404) {
        setError('List not found');
      } else {
        setError('Failed to load list');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateList = async (request: { name: string; description?: string }) => {
    if (!user || !list) return;

    try {
      const updatedList = await listService.updateList(user.id, list.id, request);
      setList(prev => prev ? { ...prev, ...updatedList } : null);
      setIsEditModalOpen(false);
    } catch (err) {
      console.error('Failed to update list:', err);
      throw err;
    }
  };

  const handleDeleteList = async () => {
    if (!user || !list) return;

    if (!window.confirm(`Are you sure you want to delete "${list.name}"? This action cannot be undone.`)) {
      return;
    }

    try {
      setIsDeleting(true);
      await listService.deleteList(user.id, list.id);
      navigate('/lists');
    } catch (err) {
      console.error('Failed to delete list:', err);
      setIsDeleting(false);
    }
  };

  const handleRemoveMovie = async (movieId: number) => {
    if (!user || !list) return;

    try {
      await listService.removeMovieFromList(user.id, list.id, movieId);
      setList(prev => prev ? {
        ...prev,
        movies: prev.movies?.filter(movie => movie.id !== movieId) || [],
        movieCount: prev.movieCount - 1
      } : null);
    } catch (err) {
      console.error('Failed to remove movie from list:', err);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error || !list) {
    return (
      <div className="text-center py-12">
        <p className="text-red-600 mb-4">{error || 'List not found'}</p>
        <Link
          to="/lists"
          className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          <ArrowLeftIcon className="h-4 w-4 mr-2" />
          Back to Lists
        </Link>
      </div>
    );
  }

  return (
    <>
      <div className="mb-6">
        <div className="flex items-center justify-between mb-4">
          <Link
            to="/lists"
            className="inline-flex items-center text-gray-600 hover:text-gray-900 transition-colors"
          >
            <ArrowLeftIcon className="h-4 w-4 mr-1" />
            Back to Lists
          </Link>
          
          <div className="flex items-center space-x-2">
            <button
              onClick={() => setIsEditModalOpen(true)}
              className="p-2 text-gray-400 hover:text-blue-600 transition-colors"
              title="Edit list"
            >
              <PencilIcon className="h-5 w-5" />
            </button>
            <button
              onClick={handleDeleteList}
              disabled={isDeleting}
              className="p-2 text-gray-400 hover:text-red-600 transition-colors disabled:opacity-50"
              title="Delete list"
            >
              <TrashIcon className="h-5 w-5" />
            </button>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-6">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">{list.name}</h1>
          
          {list.description && (
            <p className="text-gray-600 mb-4">{list.description}</p>
          )}

          <div className="flex items-center justify-between text-sm text-gray-500">
            <div className="flex items-center space-x-4">
              <div className="flex items-center">
                <FilmIcon className="h-4 w-4 mr-1" />
                <span>{list.movieCount} {list.movieCount === 1 ? 'movie' : 'movies'}</span>
              </div>
              <div>Created {formatDate(list.createdDate)}</div>
              {list.updatedDate !== list.createdDate && (
                <div>Updated {formatDate(list.updatedDate)}</div>
              )}
            </div>
          </div>
        </div>
      </div>

      <div className="bg-white rounded-lg shadow-sm">
        <div className="p-6 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-xl font-semibold text-gray-900">Movies in this List</h2>
            <Link
              to={`/movies?addToList=${list.id}`}
              className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <PlusIcon className="h-4 w-4 mr-2" />
              Add Movies
            </Link>
          </div>
        </div>

        <div className="p-6">
          {!list.movies || list.movies.length === 0 ? (
            <div className="text-center py-12">
              <FilmIcon className="mx-auto h-12 w-12 text-gray-400" />
              <h3 className="mt-2 text-sm font-medium text-gray-900">No movies yet</h3>
              <p className="mt-1 text-sm text-gray-500">
                Add some movies to get started with this list.
              </p>
              <div className="mt-6">
                <Link
                  to={`/movies?addToList=${list.id}`}
                  className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                  <PlusIcon className="h-4 w-4 mr-2" />
                  Browse Movies
                </Link>
              </div>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {list.movies.map(movie => (
                <MovieCard
                  key={movie.id}
                  movie={movie}
                  onRemoveFromList={() => handleRemoveMovie(movie.id)}
                  showRemoveFromList={true}
                />
              ))}
            </div>
          )}
        </div>
      </div>

      <CreateListModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSubmit={handleUpdateList}
        title="Edit List"
        initialValues={{
          name: list.name,
          description: list.description || ''
        }}
      />
    </>
  );
}