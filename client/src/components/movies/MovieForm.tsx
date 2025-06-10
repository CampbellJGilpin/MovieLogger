import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import type { Movie, MovieCreateRequest } from '../../types';

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

  useEffect(() => {
    if (movie) {
      setTitle(movie.title);
      setDescription(movie.description);
      // Convert the date to local date string for the input
      const date = new Date(movie.releaseDate);
      setReleaseDate(date.toISOString().split('T')[0]);
      setGenreId(movie.genre.id);
    }
  }, [movie]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    
    // Convert the date to UTC
    const date = new Date(releaseDate);
    const utcDate = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
    
    onSubmit({
      title,
      description,
      releaseDate: utcDate.toISOString(),
      genreId,
      isDeleted: false
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
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          rows={4}
          className="input mt-1"
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
          value={genreId}
          onChange={(e) => setGenreId(parseInt(e.target.value))}
          className="input mt-1"
          required
        >
          <option value={1}>Action</option>
          <option value={2}>Horror</option>
          <option value={3}>Drama</option>
          <option value={4}>Comedy</option>
          <option value={5}>Thriller</option>
          <option value={6}>Romance</option>
          <option value={7}>Science Fiction</option>
          <option value={8}>Western</option>
          <option value={9}>Documentary</option>
          <option value={10}>Family</option>
          <option value={11}>Musical</option>
          <option value={12}>Fantasy</option>
          <option value={13}>War</option>
        </select>
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
          Save
        </button>
      </div>
    </form>
  );
} 