import { useState, useEffect, useCallback } from 'react';
import MovieList from '../components/movies/MovieList';
import type { MovieInLibrary } from '../types/index';
import * as movieService from '../services/movieService';

export default function MyLibrary() {
  const [movies, setMovies] = useState<MovieInLibrary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'all' | 'favorites'>('all');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize] = useState(10);

  const loadMovies = useCallback(async (tab = activeTab, page = currentPage) => {
    try {
      setIsLoading(true);
      setError(null);
      
      let result;
      if (tab === 'favorites') {
        result = await movieService.getMyFavoritesPaginated(page, pageSize);
      } else {
        result = await movieService.getMyLibraryPaginated(page, pageSize);
      }
      
      setMovies(result.items);
      setTotalPages(result.totalPages);
    } catch (err) {
      setError('Failed to load your library. Please try again later.');
      console.error('Error loading library:', err);
    } finally {
      setIsLoading(false);
    }
  }, [activeTab, currentPage, pageSize]);

  useEffect(() => {
    loadMovies();
  }, [loadMovies]);

  // Handle tab changes
  useEffect(() => {
    setCurrentPage(1);
    loadMovies(activeTab, 1);
  }, [activeTab, loadMovies]);

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
    loadMovies(activeTab, newPage);
    window.scrollTo(0, 0);
  };

  const handleToggleFavorite = async (movieId: number) => {
    try {
      await movieService.toggleFavorite(movieId);
      await loadMovies();
    } catch (err) {
      console.error('Error toggling favorite status:', err);
    }
  };

  if (isLoading) {
    return (
      <div className="text-center py-12">
        <div className="text-gray-500">Loading your library...</div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-2xl font-bold text-gray-900">My Library</h1>
      </div>

      <div className="border-b border-gray-200 mb-8">
        <nav className="-mb-px flex space-x-8" aria-label="Tabs">
          <button
            onClick={() => setActiveTab('all')}
            className={`
              whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm
              ${activeTab === 'all'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }
            `}
          >
            All Movies
          </button>
          <button
            onClick={() => setActiveTab('favorites')}
            className={`
              whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm
              ${activeTab === 'favorites'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }
            `}
          >
            Favorites
          </button>
        </nav>
      </div>

      {error ? (
        <div className="rounded-md bg-red-50 p-4">
          <div className="text-sm text-red-700">{error}</div>
        </div>
      ) : (
        <>
          <MovieList
            movies={movies}
            onToggleFavorite={handleToggleFavorite}
            emptyMessage={
              isLoading
                ? 'Loading your library...'
                : error
                ? error
                : activeTab === 'favorites'
                ? 'No favorite movies yet'
                : 'No movies in your library'
            }
          />
          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-8 flex justify-center">
              <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                <button
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={currentPage === 1}
                  className={`relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium ${
                    currentPage === 1
                      ? 'text-gray-300 cursor-not-allowed'
                      : 'text-gray-500 hover:bg-gray-50'
                  }`}
                >
                  Previous
                </button>
                
                {[...Array(totalPages)].map((_, i) => (
                  <button
                    key={i + 1}
                    onClick={() => handlePageChange(i + 1)}
                    className={`relative inline-flex items-center px-4 py-2 border text-sm font-medium ${
                      currentPage === i + 1
                        ? 'z-10 bg-indigo-50 border-indigo-500 text-indigo-600'
                        : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                    }`}
                  >
                    {i + 1}
                  </button>
                ))}
                
                <button
                  onClick={() => handlePageChange(currentPage + 1)}
                  disabled={currentPage === totalPages}
                  className={`relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium ${
                    currentPage === totalPages
                      ? 'text-gray-300 cursor-not-allowed'
                      : 'text-gray-500 hover:bg-gray-50'
                  }`}
                >
                  Next
                </button>
              </nav>
            </div>
          )}
        </>
      )}
    </div>
  );
} 