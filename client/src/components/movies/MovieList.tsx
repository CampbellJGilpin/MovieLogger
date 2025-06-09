import type { MovieInLibrary } from '../../types';
import MovieCard from './MovieCard';

interface MovieListProps {
  movies: MovieInLibrary[];
  onToggleWatched?: (movieId: string) => void;
  onToggleWatchLater?: (movieId: string) => void;
  onToggleFavorite?: (movieId: string) => void;
  emptyMessage?: string;
}

export default function MovieList({
  movies,
  onToggleWatched,
  onToggleWatchLater,
  onToggleFavorite,
  emptyMessage = 'No movies found'
}: MovieListProps) {
  if (movies.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">{emptyMessage}</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
      {movies.map((movie) => (
        <MovieCard
          key={movie.id}
          movie={movie}
          onToggleWatched={onToggleWatched}
          onToggleWatchLater={onToggleWatchLater}
          onToggleFavorite={onToggleFavorite}
        />
      ))}
    </div>
  );
} 