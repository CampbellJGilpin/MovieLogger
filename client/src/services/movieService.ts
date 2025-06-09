import type { Movie, MovieInLibrary, Review } from '../types';
import api from '../api/config';

export async function getAllMovies(): Promise<MovieInLibrary[]> {
  const response = await api.get<MovieInLibrary[]>('/movies');
  return response.data;
}

export async function getMovie(id: string): Promise<MovieInLibrary> {
  const response = await api.get<MovieInLibrary>(`/movies/${id}`);
  return response.data;
}

export async function createMovie(movieData: Omit<Movie, 'id'>): Promise<Movie> {
  const response = await api.post<Movie>('/movies', movieData);
  return response.data;
}

export async function updateMovie(id: string, movieData: Omit<Movie, 'id'>): Promise<Movie> {
  const response = await api.put<Movie>(`/movies/${id}`, movieData);
  return response.data;
}

export async function deleteMovie(id: string): Promise<void> {
  await api.delete(`/movies/${id}`);
}

export async function getMovieReviews(movieId: string): Promise<Review[]> {
  const response = await api.get<Review[]>(`/movies/${movieId}/reviews`);
  return response.data;
}

export async function addMovieReview(
  movieId: string,
  reviewData: { rating: number; comment: string }
): Promise<Review> {
  const response = await api.post<Review>(`/movies/${movieId}/reviews`, reviewData);
  return response.data;
}

export async function toggleWatched(movieId: string): Promise<void> {
  await api.post(`/movies/${movieId}/watched`);
}

export async function toggleWatchLater(movieId: string): Promise<void> {
  await api.post(`/movies/${movieId}/watch-later`);
}

export async function toggleFavorite(movieId: string): Promise<void> {
  await api.post(`/movies/${movieId}/favorite`);
}

export async function getMyLibrary(): Promise<MovieInLibrary[]> {
  const response = await api.get<MovieInLibrary[]>('/movies/library');
  return response.data;
}

export async function searchMovies(query: string): Promise<MovieInLibrary[]> {
  const response = await api.get<MovieInLibrary[]>('/movies/search', {
    params: { q: query }
  });
  return response.data;
} 