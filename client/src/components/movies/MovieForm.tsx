import { useState, useEffect, useCallback } from 'react';
import type { FormEvent } from 'react';
import type { Movie, MovieCreateRequest, Genre } from '../../types';
import * as movieService from '../../services/movieService';

interface MovieFormProps {
  movie?: Movie;
  onSubmit: (movieData: MovieCreateRequest) => void;
  onCancel: () => void;
}

interface ValidationErrors {
  title?: string;
  description?: string;
}

export default function MovieForm({ movie, onSubmit, onCancel }: MovieFormProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [releaseDate, setReleaseDate] = useState(new Date().toISOString().split('T')[0]);
  const [genreId, setGenreId] = useState(1);
  const [genres, setGenres] = useState<Genre[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errors, setErrors] = useState<ValidationErrors>({});

  const loadGenres = useCallback(async () => {
    try {
      const genresData = await movieService.getGenres();
      setGenres(genresData);
      if (!movie && genresData.length > 0) {
        setGenreId(genresData[0].id);
      }
      setIsLoading(false);
    } catch (error) {
      console.error('Error loading genres:', error);
      setIsLoading(false);
    }
  }, [movie]);

  useEffect(() => {
    loadGenres();
  }, [loadGenres]);

  useEffect(() => {
    if (movie) {
      setTitle(movie.title);
      setDescription(movie.description);
      setReleaseDate(new Date(movie.releaseDate).toISOString().split('T')[0]);
      setGenreId(movie.genre.id);
    }
  }, [movie]);

  const validateForm = (): boolean => {
    const newErrors: ValidationErrors = {};

    // Validate title
    if (!title.trim()) {
      newErrors.title = 'Title is required';
    }

    // Validate description
    if (!description.trim()) {
      newErrors.description = 'Description is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    e.stopPropagation();

    // Clear previous errors
    setErrors({});

    // Validate form before submission
    if (!validateForm()) {
      return; // Stop submission if validation fails
    }

    const movieData: MovieCreateRequest = {
      title: title.trim(),
      description: description.trim(),
      releaseDate: new Date(releaseDate).toISOString(),
      genreId,
      isDeleted: false
    };

    try {
      onSubmit(movieData);
    } catch (error) {
      console.error('Error submitting form:', error);
    }
  };

  const handleInputChange = (field: 'title' | 'description', value: string) => {
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
    
    if (field === 'title') {
      setTitle(value);
    } else if (field === 'description') {
      setDescription(value);
    }
  };

  if (isLoading) {
    return <div className="text-center py-4">Loading...</div>;
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label htmlFor="title" className="block text-sm font-medium text-gray-700">
          Title <span className="text-red-500">*</span>
        </label>
        <input
          type="text"
          id="title"
          value={title}
          onChange={(e) => handleInputChange('title', e.target.value)}
          className={`mt-1 block w-full rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
            errors.title 
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500' 
              : 'border-gray-300'
          }`}
        />
        {errors.title && (
          <p className="mt-1 text-sm text-red-600">{errors.title}</p>
        )}
      </div>

      <div>
        <label htmlFor="description" className="block text-sm font-medium text-gray-700">
          Description <span className="text-red-500">*</span>
        </label>
        <textarea
          id="description"
          value={description}
          onChange={(e) => handleInputChange('description', e.target.value)}
          rows={3}
          className={`mt-1 block w-full rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
            errors.description 
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500' 
              : 'border-gray-300'
          }`}
        />
        {errors.description && (
          <p className="mt-1 text-sm text-red-600">{errors.description}</p>
        )}
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
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
        />
      </div>

      <div>
        <label htmlFor="genre" className="block text-sm font-medium text-gray-700">
          Genre
        </label>
        <select
          id="genre"
          value={genreId}
          onChange={(e) => setGenreId(parseInt(e.target.value, 10))}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
        >
          {genres.map((genre) => (
            <option key={genre.id} value={genre.id}>
              {genre.title}
            </option>
          ))}
        </select>
      </div>

      <div className="flex justify-end space-x-3">
        <button
          type="button"
          onClick={onCancel}
          className="inline-flex justify-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          Cancel
        </button>
        <button
          type="submit"
          className="inline-flex justify-center rounded-md border border-transparent bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          {movie ? 'Update' : 'Create'}
        </button>
      </div>
    </form>
  );
} 