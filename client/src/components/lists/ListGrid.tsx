import { useState, useEffect, useCallback } from 'react';
import { PlusIcon } from '@heroicons/react/24/outline';
import ListCard from './ListCard';
import CreateListModal from './CreateListModal';
import listService, { type ListSummary, type CreateListRequest } from '../../services/listService';
import { useAuth } from '../../contexts/useAuth';

interface ListGridProps {
  onListCreated?: (list: ListSummary) => void;
  onListUpdated?: (list: ListSummary) => void;
  onListDeleted?: (listId: number) => void;
}

export default function ListGrid({ onListCreated, onListUpdated, onListDeleted }: ListGridProps) {
  const { user } = useAuth();
  const [lists, setLists] = useState<ListSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editingList, setEditingList] = useState<ListSummary | null>(null);

  const loadLists = useCallback(async () => {
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
  }, [user]);

  useEffect(() => {
    loadLists();
  }, [loadLists]);

  const handleCreateList = async (request: CreateListRequest) => {
    if (!user) return;

    try {
      const newList = await listService.createList(user.id, request);
      setLists(prev => [newList, ...prev]);
      setIsCreateModalOpen(false);
      onListCreated?.(newList);
    } catch (err) {
      console.error('Failed to create list:', err);
      throw err;
    }
  };

  const handleEditList = async (request: CreateListRequest) => {
    if (!user || !editingList) return;

    try {
      const updatedList = await listService.updateList(user.id, editingList.id, request);
      setLists(prev => prev.map(list => list.id === updatedList.id ? updatedList : list));
      setEditingList(null);
      onListUpdated?.(updatedList);
    } catch (err) {
      console.error('Failed to update list:', err);
      throw err;
    }
  };

  const handleDeleteList = async (listId: number) => {
    if (!user) return;

    try {
      await listService.deleteList(user.id, listId);
      setLists(prev => prev.filter(list => list.id !== listId));
      onListDeleted?.(listId);
    } catch (err) {
      console.error('Failed to delete list:', err);
      throw err;
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-12">
        <p className="text-red-600">{error}</p>
        <button
          onClick={loadLists}
          className="mt-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          Try Again
        </button>
      </div>
    );
  }

  return (
    <>
      <div className="mb-6">
        <div className="flex items-center justify-between">
          <h1 className="text-2xl font-bold text-gray-900">My Lists</h1>
          <button
            onClick={() => setIsCreateModalOpen(true)}
            className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            <PlusIcon className="h-5 w-5 mr-2" />
            Create List
          </button>
        </div>
        <p className="mt-1 text-gray-600">
          Organize your movies into custom collections
        </p>
      </div>

      {lists.length === 0 ? (
        <div className="text-center py-12 bg-gray-50 rounded-lg">
          <div className="mx-auto max-w-md">
            <PlusIcon className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No lists yet</h3>
            <p className="mt-1 text-sm text-gray-500">
              Get started by creating your first movie list.
            </p>
            <div className="mt-6">
              <button
                onClick={() => setIsCreateModalOpen(true)}
                className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                <PlusIcon className="h-5 w-5 mr-2" />
                Create Your First List
              </button>
            </div>
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {lists.map(list => (
            <ListCard
              key={list.id}
              list={list}
              onEdit={setEditingList}
              onDelete={handleDeleteList}
            />
          ))}
        </div>
      )}

      <CreateListModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSubmit={handleCreateList}
        title="Create New List"
      />

      {editingList && (
        <CreateListModal
          isOpen={true}
          onClose={() => setEditingList(null)}
          onSubmit={handleEditList}
          title="Edit List"
          initialValues={{
            name: editingList.name,
            description: editingList.description || ''
          }}
        />
      )}
    </>
  );
}