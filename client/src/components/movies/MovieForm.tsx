import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import type { Movie, MovieCreateRequest, Genre } from '../../types';
import * as movieService from '../../services/movieService';

interface MovieFormProps {
  movie?: Movie;
  onSubmit: (movieData: MovieCreateRequest) => void;
  onCancel: () => void;
}

export default function MovieForm({ movie, onSubmit, onCancel }: MovieFormProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [releaseDate, setReleaseDate] = useState(new Date().toISOString().split('T')[0]);
  const [genreId, setGenreId] = useState(1);
  const [genres, setGenres] = useState<Genre[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadGenres();
  }, []);

  useEffect(() => {
    if (movie) {
      setTitle(movie.title);
      setDescription(movie.description);
      setReleaseDate(new Date(movie.releaseDate).toISOString().split('T')[0]);
      setGenreId(movie.genre.id);
    }
  }, [movie]);

  const loadGenres = async () => {
    try {
      const genresData = await movieService.getGenres();
      setGenres(genresData);
      if (!movie && genresData.length > 0) {
        setGenreId(genresData[0].id);
      }
      setIsLoading(false);
    } catch (err) {
      console.error('Error loading genres:', err);
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    e.stopPropagation();

    const movieData: MovieCreateRequest = {
      title,
      description,
      releaseDate: new Date(releaseDate).toISOString(),
      genreId,
      isDeleted: false
    };

    try {
      onSubmit(movieData);
    } catch (err) {
      console.error('Error submitting form:', err);
    }
  };

  if (isLoading) {
    return <div className="text-center py-4">Loading...</div>;
  }

  return (
    <form onSubmit={handleSubmit} className="mt-4">
      <div className="space-y-4">
        <div>
          <label htmlFor="title" className="block text-sm font-medium text-gray-700">
            Title
          </label>
          <input
            type="text"
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
            required
          />
        </div>

        <div>
          <label htmlFor="description" className="block text-sm font-medium text-gray-700">
            Description
          </label>
          <textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={3}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
            required
          />
        </div>

        <div>
          <label htmlFor="releaseDate" className="block text-sm font-medium text-gray-700">
            Release Date
          </label>
          <input
            type="date"
            id="releaseDate"
            value={releaseDate}
            onChange={(e) => setReleaseDate(e.target.value)}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
            required
          />
        </div>

        <div>
          <label htmlFor="genreId" className="block text-sm font-medium text-gray-700">
            Genre
          </label>
          <select
            id="genreId"
            value={genreId}
            onChange={(e) => setGenreId(Number(e.target.value))}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
            required
          >
            {genres.map(genre => (
              <option key={genre.id} value={genre.id}>
                {genre.title}
              </option>
            ))}
          </select>
        </div>

        <div className="flex justify-end space-x-3">
          <button
            type="button"
            onClick={(e) => {
              e.preventDefault();
              e.stopPropagation();
              onCancel();
            }}
            className="btn btn-secondary"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="btn btn-primary"
          >
            Save
          </button>
        </div>
      </div>
    </form>
  );
} 