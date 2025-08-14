import { useState, useEffect, useCallback, useRef } from 'react';
import type { FormEvent } from 'react';
import type { Movie, Genre } from '../../types';
import * as movieService from '../../services/movieService';
import UploadProgress from '../common/UploadProgress';

interface MovieFormProps {
  movie?: Movie;
  onSubmit: (movieData: FormData, onUploadProgress?: (progressEvent: { loaded: number; total?: number }) => void) => void;
  onCancel: () => void;
}

interface ValidationErrors {
  title?: string;
  description?: string;
  poster?: string;
}

export default function MovieForm({ movie, onSubmit, onCancel }: MovieFormProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [releaseDate, setReleaseDate] = useState(new Date().toISOString().split('T')[0]);
  const [genreId, setGenreId] = useState(1);
  const [genres, setGenres] = useState<Genre[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [errors, setErrors] = useState<ValidationErrors>({});
  const [poster, setPoster] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [currentPosterUrl, setCurrentPosterUrl] = useState<string | null>(null);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [isUploading, setIsUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

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
      
      // Set current poster URL if exists
      if (movie.posterPath) {
        const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5049';
        const baseUrl = apiBaseUrl.replace('/api', '');
        const fullPosterUrl = movie.posterPath.startsWith('http') 
          ? movie.posterPath 
          : `${baseUrl}${movie.posterPath}`;
        setCurrentPosterUrl(fullPosterUrl);
      }
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

  const validateFile = (file: File): string | null => {
    const maxSize = 5 * 1024 * 1024; // 5MB
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/webp'];
    
    if (file.size > maxSize) {
      return 'File size must be less than 5MB';
    }
    
    if (!allowedTypes.includes(file.type)) {
      return 'Only JPEG, PNG, and WebP images are allowed';
    }
    
    return null;
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    
    // Clear previous errors
    if (errors.poster) {
      setErrors(prev => ({ ...prev, poster: undefined }));
    }
    
    if (selectedFile) {
      const validationError = validateFile(selectedFile);
      if (validationError) {
        setErrors(prev => ({ ...prev, poster: validationError }));
        setPoster(null);
        setPreviewUrl(null);
        if (fileInputRef.current) {
          fileInputRef.current.value = '';
        }
        return;
      }
      
      setPoster(selectedFile);
      
      // Create preview URL
      const objectUrl = URL.createObjectURL(selectedFile);
      setPreviewUrl(objectUrl);
    } else {
      setPoster(null);
      setPreviewUrl(null);
    }
  };

  const handleRemovePoster = () => {
    setPoster(null);
    setPreviewUrl(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  // Clean up preview URL on unmount
  useEffect(() => {
    return () => {
      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
    };
  }, [previewUrl]);

  const handleUploadProgress = (progressEvent: { loaded: number; total?: number }) => {
    const progress = progressEvent.total ? (progressEvent.loaded / progressEvent.total) * 100 : 0;
    setUploadProgress(progress);
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    e.stopPropagation();

    setErrors({});
    if (!validateForm()) {
      return;
    }

    const formData = new FormData();
    formData.append('title', title.trim());
    formData.append('description', description.trim());
    formData.append('releaseDate', new Date(releaseDate).toISOString());
    formData.append('genreId', genreId.toString());
    formData.append('isDeleted', 'false');
    if (poster) {
      formData.append('poster', poster);
      setIsUploading(true);
      setUploadProgress(0);
    }

    try {
      onSubmit(formData, poster ? handleUploadProgress : undefined);
    } catch (error) {
      console.error('Error submitting form:', error);
    } finally {
      setIsUploading(false);
      setUploadProgress(0);
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

      <div>
        <label htmlFor="poster" className="block text-sm font-medium text-gray-700">
          Poster Image
        </label>
        
        {/* Current/Preview Image Display */}
        {(previewUrl || currentPosterUrl) && (
          <div className="mt-2 mb-3">
            <div className="relative inline-block">
              <img
                src={previewUrl || currentPosterUrl || ''}
                alt="Poster preview"
                className="h-32 w-24 object-cover rounded-lg border border-gray-200 shadow-sm"
              />
              <button
                type="button"
                onClick={handleRemovePoster}
                className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs hover:bg-red-600 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2"
                title="Remove poster"
              >
                ×
              </button>
            </div>
            <p className="text-sm text-gray-500 mt-1">
              {previewUrl ? 'New poster selected' : 'Current poster'}
            </p>
          </div>
        )}

        <input
          type="file"
          id="poster"
          name="poster"
          accept="image/jpeg,image/jpg,image/png,image/webp"
          ref={fileInputRef}
          onChange={handleFileChange}
          className={`mt-1 block w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100 ${
            errors.poster ? 'border-red-300' : ''
          }`}
        />
        
        <p className="mt-1 text-xs text-gray-500">
          Maximum file size: 5MB. Supported formats: JPEG, PNG, WebP
        </p>
        
        {errors.poster && (
          <p className="mt-1 text-sm text-red-600">{errors.poster}</p>
        )}
        
        <UploadProgress
          progress={uploadProgress}
          isUploading={isUploading}
          fileName={poster?.name}
        />
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