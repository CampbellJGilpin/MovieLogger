import { useState, useEffect } from 'react';
import { ChartBarIcon, StarIcon, EyeIcon } from '@heroicons/react/24/outline';
import type { GenrePreferencesSummary } from '../../types/index';
import * as genrePreferencesService from '../../services/genrePreferencesService';

interface GenrePreferencesSectionProps {
  className?: string;
}

export default function GenrePreferencesSection({ className = '' }: GenrePreferencesSectionProps) {
  const [preferences, setPreferences] = useState<GenrePreferencesSummary | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadGenrePreferences();
  }, []);

  const loadGenrePreferences = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await genrePreferencesService.getUserGenrePreferences();
      setPreferences(data);
    } catch (err) {
      setError('Failed to load genre preferences');
      console.error('Error loading genre preferences:', err);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className={`bg-white rounded-lg shadow p-6 ${className}`}>
        <div className="animate-pulse">
          <div className="h-6 bg-gray-200 rounded w-1/3 mb-4"></div>
          <div className="space-y-3">
            <div className="h-4 bg-gray-200 rounded"></div>
            <div className="h-4 bg-gray-200 rounded w-5/6"></div>
            <div className="h-4 bg-gray-200 rounded w-4/6"></div>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={`bg-white rounded-lg shadow p-6 ${className}`}>
        <div className="text-center text-gray-500">
          <p className="text-sm">{error}</p>
        </div>
      </div>
    );
  }

  if (!preferences || preferences.totalMoviesWatched === 0) {
    return (
      <div className={`bg-white rounded-lg shadow p-6 ${className}`}>
        <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
          <ChartBarIcon className="h-5 w-5 mr-2 text-indigo-600" />
          Genre Preferences
        </h2>
        <p className="text-gray-500 text-sm">
          Start watching movies to see your genre preferences!
        </p>
      </div>
    );
  }

  const topGenres = preferences.genrePreferences.slice(0, 5);

  return (
    <div className={`bg-white rounded-lg shadow p-6 ${className}`}>
      <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
        <ChartBarIcon className="h-5 w-5 mr-2 text-indigo-600" />
        Genre Preferences
      </h2>

      {/* Summary Stats */}
      <div className="grid grid-cols-3 gap-4 mb-6">
        <div className="text-center">
          <div className="text-2xl font-bold text-indigo-600">{preferences.totalMoviesWatched}</div>
          <div className="text-xs text-gray-500">Total Watched</div>
        </div>
        <div className="text-center">
          <div className="text-2xl font-bold text-green-600">{preferences.totalUniqueGenres}</div>
          <div className="text-xs text-gray-500">Genres Explored</div>
        </div>
        <div className="text-center">
          <div className="text-2xl font-bold text-purple-600">
            {preferences.mostWatchedGenre?.genreTitle || 'N/A'}
          </div>
          <div className="text-xs text-gray-500">Top Genre</div>
        </div>
      </div>

      {/* Top Genres Chart */}
      <div className="mb-6">
        <h3 className="text-sm font-medium text-gray-900 mb-3">Most Watched Genres</h3>
        <div className="space-y-3">
          {topGenres.map((genre, index) => (
            <div key={genre.genreId} className="flex items-center">
              <div className="flex-shrink-0 w-8 h-8 bg-indigo-100 rounded-full flex items-center justify-center">
                <span className="text-xs font-medium text-indigo-600">{index + 1}</span>
              </div>
              <div className="flex-1 ml-3">
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium text-gray-900">{genre.genreTitle}</span>
                  <span className="text-sm text-gray-500">{genre.watchCount} movies</span>
                </div>
                <div className="mt-1">
                  <div className="flex items-center">
                    <div className="flex-1 bg-gray-200 rounded-full h-2">
                      <div
                        className="bg-indigo-600 h-2 rounded-full"
                        style={{ width: `${genre.percentageOfTotalWatches}%` }}
                      ></div>
                    </div>
                    <span className="ml-2 text-xs text-gray-500">
                      {genre.percentageOfTotalWatches}%
                    </span>
                  </div>
                </div>
                {genre.averageRating > 0 && (
                  <div className="flex items-center mt-1">
                    <StarIcon className="h-3 w-3 text-yellow-400 mr-1" />
                    <span className="text-xs text-gray-500">
                      {genre.averageRating.toFixed(1)} avg rating
                    </span>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Highlights */}
      <div className="grid grid-cols-1 gap-3">
        {preferences.highestRatedGenre && preferences.highestRatedGenre.averageRating > 0 && (
          <div className="bg-green-50 rounded-lg p-3">
            <div className="flex items-center">
              <StarIcon className="h-4 w-4 text-green-600 mr-2" />
              <div>
                <div className="text-sm font-medium text-green-900">
                  Highest Rated: {preferences.highestRatedGenre.genreTitle}
                </div>
                <div className="text-xs text-green-700">
                  {preferences.highestRatedGenre.averageRating.toFixed(1)} average rating
                </div>
              </div>
            </div>
          </div>
        )}

        {preferences.leastWatchedGenre && preferences.leastWatchedGenre.watchCount > 0 && (
          <div className="bg-orange-50 rounded-lg p-3">
            <div className="flex items-center">
              <EyeIcon className="h-4 w-4 text-orange-600 mr-2" />
              <div>
                <div className="text-sm font-medium text-orange-900">
                  Least Watched: {preferences.leastWatchedGenre.genreTitle}
                </div>
                <div className="text-xs text-orange-700">
                  Only {preferences.leastWatchedGenre.watchCount} movie(s)
                </div>
              </div>
            </div>
          </div>
        )}
      </div>

    </div>
  );
} 