import { Link } from 'react-router-dom';
import { PlusIcon, FilmIcon } from '@heroicons/react/24/outline';

export default function Dashboard() {
  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-semibold text-gray-900">Dashboard</h1>
        <p className="mt-2 text-sm text-gray-700">
          Welcome to MovieLogger. Start by adding movies to your library or browse our collection.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Link
          to="/movies/new"
          className="relative flex items-center space-x-3 rounded-lg border border-gray-300 bg-white px-6 py-5 shadow-sm hover:border-blue-500 hover:ring-1 hover:ring-blue-500"
        >
          <div className="flex-shrink-0">
            <PlusIcon className="h-10 w-10 text-gray-400" aria-hidden="true" />
          </div>
          <div className="min-w-0 flex-1">
            <span className="absolute inset-0" aria-hidden="true" />
            <p className="text-sm font-medium text-gray-900">Add New Movie</p>
            <p className="truncate text-sm text-gray-500">Add a new movie to the database</p>
          </div>
        </Link>

        <Link
          to="/movies"
          className="relative flex items-center space-x-3 rounded-lg border border-gray-300 bg-white px-6 py-5 shadow-sm hover:border-blue-500 hover:ring-1 hover:ring-blue-500"
        >
          <div className="flex-shrink-0">
            <FilmIcon className="h-10 w-10 text-gray-400" aria-hidden="true" />
          </div>
          <div className="min-w-0 flex-1">
            <span className="absolute inset-0" aria-hidden="true" />
            <p className="text-sm font-medium text-gray-900">View All Movies</p>
            <p className="truncate text-sm text-gray-500">Browse and discover movies</p>
          </div>
        </Link>
      </div>

      {/* Recent Activity section can be added here later */}
    </div>
  );
} 