import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import type { Movie } from '../../types';

interface MovieFormProps {
  movie?: Movie;
  onSubmit: (movieData: Omit<Movie, 'id'>) => void;
  onCancel: () => void;
}

export default function MovieForm({ movie, onSubmit, onCancel }: MovieFormProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [releaseYear, setReleaseYear] = useState('');
  const [genre, setGenre] = useState('');
  const [posterUrl, setPosterUrl] = useState('');

  useEffect(() => {
    if (movie) {
      setTitle(movie.title);
      setDescription(movie.description);
      setReleaseYear(movie.releaseYear.toString());
      setGenre(movie.genre);
      setPosterUrl(movie.posterUrl || '');
    }
  }, [movie]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    onSubmit({
      title,
      description,
      releaseYear: parseInt(releaseYear, 10),
      genre,
      posterUrl: posterUrl || undefined,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div>
        <label htmlFor="title" className="block text-sm font-medium text-gray-700">
          Title
        </label>
        <input
          type="text"
          id="title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="input mt-1"
          required
        />
      </div>

      <div>
        <label htmlFor="description" className="block text-sm font-medium text-gray-700">
          Description
        </label>
        <textarea
          id="description"
          rows={4}
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="input mt-1"
          required
        />
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
        <div>
          <label htmlFor="releaseYear" className="block text-sm font-medium text-gray-700">
            Release Year
          </label>
          <input
            type="number"
            id="releaseYear"
            min="1888"
            max={new Date().getFullYear()}
            value={releaseYear}
            onChange={(e) => setReleaseYear(e.target.value)}
            className="input mt-1"
            required
          />
        </div>

        <div>
          <label htmlFor="genre" className="block text-sm font-medium text-gray-700">
            Genre
          </label>
          <select
            id="genre"
            value={genre}
            onChange={(e) => setGenre(e.target.value)}
            className="input mt-1"
            required
          >
            <option value="">Select a genre</option>
            <option value="Action">Action</option>
            <option value="Adventure">Adventure</option>
            <option value="Animation">Animation</option>
            <option value="Comedy">Comedy</option>
            <option value="Crime">Crime</option>
            <option value="Documentary">Documentary</option>
            <option value="Drama">Drama</option>
            <option value="Family">Family</option>
            <option value="Fantasy">Fantasy</option>
            <option value="Horror">Horror</option>
            <option value="Mystery">Mystery</option>
            <option value="Romance">Romance</option>
            <option value="Science Fiction">Science Fiction</option>
            <option value="Thriller">Thriller</option>
            <option value="Western">Western</option>
          </select>
        </div>
      </div>

      <div>
        <label htmlFor="posterUrl" className="block text-sm font-medium text-gray-700">
          Poster URL
        </label>
        <input
          type="url"
          id="posterUrl"
          value={posterUrl}
          onChange={(e) => setPosterUrl(e.target.value)}
          className="input mt-1"
          placeholder="https://example.com/movie-poster.jpg"
        />
      </div>

      <div className="flex justify-end space-x-3">
        <button
          type="button"
          onClick={onCancel}
          className="btn btn-secondary"
        >
          Cancel
        </button>
        <button
          type="submit"
          className="btn btn-primary"
        >
          {movie ? 'Update Movie' : 'Add Movie'}
        </button>
      </div>
    </form>
  );
} 