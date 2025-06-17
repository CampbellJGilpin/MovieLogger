import type { MovieInLibrary } from '../../types/index';
import MovieListRow from './MovieListRow';

interface MovieListProps {
  movies: MovieInLibrary[];
  onToggleLibrary?: (movieId: number) => void;
  onToggleFavorite?: (movieId: number) => void;
  onDelete?: (movieId: number) => void;
  emptyMessage?: string;
}

export default function MovieList({ 
  movies, 
  onToggleLibrary,
  onToggleFavorite,
  onDelete,
  emptyMessage = 'No movies found'
}: MovieListProps) {
  if (!movies.length) {
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
                <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Release Date</th>
                <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Genre</th>
                <th scope="col" className="px-3 py-3.5 text-center text-sm font-semibold text-gray-900">Actions</th>
                <th scope="col" className="relative py-3.5 pl-3 pr-4 sm:pr-6">
                  <span className="sr-only">Actions</span>
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {movies.map((movie) => (
                <MovieListRow
                  key={movie.id}
                  movie={movie}
                  onToggleLibrary={onToggleLibrary}
                  onToggleFavorite={onToggleFavorite}
                  onDelete={onDelete}
                />
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
} 