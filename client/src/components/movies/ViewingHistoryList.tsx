import { Link } from 'react-router-dom';
import type { Viewing } from '../../types/index';

interface ViewingHistoryListProps {
  viewings: Viewing[];
  onToggleFavorite?: (movieId: number) => void;
  emptyMessage?: string;
}

export default function ViewingHistoryList({ 
  viewings, 
  onToggleFavorite, 
  emptyMessage = 'No viewing history found' 
}: ViewingHistoryListProps) {
  if (viewings.length === 0) {
    return (
      <div className="text-center py-12">
        <div className="text-gray-500">{emptyMessage}</div>
      </div>
    );
  }

  return (
    <div className="mt-8 flow-root">
      <div className="-mx-4 -my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
        <div className="inline-block min-w-full py-2 align-middle sm:px-6 lg:px-8">
          <table className="min-w-full divide-y divide-gray-300">
            <thead>
              <tr>
                <th scope="col" className="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900 sm:pl-6">Title</th>
                <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">View Date</th>
                <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Score</th>
                <th scope="col" className="px-3 py-3.5 text-center text-sm font-semibold text-gray-900">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {viewings
                .filter((viewing) => viewing.movie) // Filter out viewings with null movies
                .map((viewing) => (
                <tr key={viewing.id}>
                  {/* Title */}
                  <td className="whitespace-nowrap py-4 pl-4 pr-3 sm:pl-6">
                    <div className="flex items-center">
                      <div>
                        <Link 
                          to={`/movies/${viewing.movie.id}`} 
                          className="text-sm font-medium text-gray-900 hover:text-indigo-600"
                        >
                          {viewing.movie.title}
                        </Link>
                        <div className="text-sm text-gray-500">
                          {new Date(viewing.movie.releaseDate).getFullYear()} • {viewing.movie.genre?.title || 'Unknown Genre'}
                        </div>
                      </div>
                    </div>
                  </td>

                  {/* View Date */}
                  <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-900">
                    {new Date(viewing.dateViewed).toLocaleDateString()}
                  </td>

                  {/* Score */}
                  <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                    {viewing.review && viewing.review.score ? (
                      <div className="flex items-center">
                        {[1, 2, 3, 4, 5].map((star) => (
                          <svg
                            key={star}
                            className={`h-4 w-4 ${
                              star <= viewing.review!.score
                                ? 'text-yellow-400'
                                : 'text-gray-300'
                            }`}
                            fill="currentColor"
                            viewBox="0 0 20 20"
                          >
                            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                          </svg>
                        ))}
                        <span className="ml-2 text-sm text-gray-600">
                          {viewing.review.score}/5
                        </span>
                      </div>
                    ) : (
                      <span className="text-sm text-gray-400">No rating</span>
                    )}
                  </td>

                  {/* Actions */}
                  <td className="relative whitespace-nowrap py-4 pl-3 pr-4 text-center text-sm font-medium sm:pr-6">
                    {onToggleFavorite && (
                      <button
                        onClick={() => onToggleFavorite(viewing.movie.id)}
                        className={`p-1 rounded ${
                          viewing.favourite
                            ? 'text-red-600 bg-red-100'
                            : 'text-gray-400 hover:text-gray-500'
                        }`}
                        title={viewing.favourite ? 'Remove from favorites' : 'Add to favorites'}
                      >
                        <svg className="h-5 w-5" fill={viewing.favourite ? 'currentColor' : 'none'} viewBox="0 0 24 24" stroke="currentColor">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                        </svg>
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}