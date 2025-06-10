import api from '../api/config';
import type { Movie, MovieInLibrary, Review, Genre } from '../types';
import * as reviewService from './reviewService';

interface MovieCreateRequest {
  title: string;
  description: string;
  releaseDate: string;
  genreId: number;
}

interface LibraryItemDto {
  movieId: number;
  movieTitle: string;
  releaseDate: string;
  genre: string;
  inLibrary: string;
  watchLater: string;
  favourite: string;
}

interface LibraryDto {
  libraryItems: LibraryItemDto[];
}

function mapLibraryItemToMovieInLibrary(item: LibraryItemDto): MovieInLibrary {
  return {
    id: item.movieId,
    title: item.movieTitle,
    releaseDate: item.releaseDate,
    genre: { id: 0, title: item.genre }, // We don't have genre ID from the API
    description: "", // We don't have description from the API
    isWatched: item.inLibrary === "true",
    isWatchLater: item.watchLater === "true",
    isFavorite: item.favourite === "true",
    isDeleted: false // Movies in the library are not deleted
  };
}

interface ViewingResponse {
  id: number;
  userId: number;
  movieId: number;
  favourite: boolean;
  ownsMovie: boolean;
  upcomingViewDate: string | null;
  movie: {
    id: number;
    title: string;
    description: string;
    releaseDate: string;
    genreId: number;
  };
  review?: Review;
}

export async function getAllMovies(): Promise<MovieInLibrary[]> {
  const response = await api.get<MovieInLibrary[]>('/movies');
  return response.data;
}

export async function getMovie(id: number): Promise<MovieInLibrary> {
  const response = await api.get<MovieInLibrary>(`/movies/${id}`);
  return response.data;
}

export async function createMovie(movieData: MovieCreateRequest): Promise<Movie> {
  const response = await api.post<Movie>('/movies', movieData);
  return response.data;
}

export async function updateMovie(id: number, movieData: MovieCreateRequest): Promise<Movie> {
  const response = await api.put<Movie>(`/movies/${id}`, movieData);
  return response.data;
}

export async function deleteMovie(id: number): Promise<void> {
  await api.delete(`/movies/${id}`);
}

export async function getMovieReviews(movieId: number): Promise<Review[]> {
  // First get the current user's ID
  const userResponse = await api.get<{ id: number }>('/accounts/me');
  const userId = userResponse.data.id;
  
  // Get reviews for this movie and user
  const response = await api.get<Review[]>(`/movies/${movieId}/users/${userId}/reviews`);
  console.log('Movie Reviews:', response.data);
  
  return response.data;
}

export async function addMovieReview(movieId: number, review: { score: number; reviewText: string }): Promise<Review> {
  // Get the current user's ID
  const userResponse = await api.get<{ id: number }>('/accounts/me');
  const userId = userResponse.data.id;

  // Create the review directly
  const response = await api.post<Review>(`/movies/${movieId}/reviews`, {
    userId,
    ...review
  });
  
  return response.data;
}

export async function toggleWatched(movieId: number): Promise<void> {
  await api.post(`/movies/${movieId}/watched`);
}

export async function toggleWatchLater(movieId: number): Promise<void> {
  await api.post(`/movies/${movieId}/watch-later`);
}

export async function toggleFavorite(movieId: number): Promise<void> {
  await api.post(`/movies/${movieId}/favorite`);
}

export async function getMyLibrary(): Promise<MovieInLibrary[]> {
  const response = await api.get('/accounts/me');
  const userId = response.data.id;
  const libraryResponse = await api.get<LibraryDto>(`/users/${userId}/library`);
  return libraryResponse.data.libraryItems.map(mapLibraryItemToMovieInLibrary);
}

export async function searchMovies(query: string): Promise<MovieInLibrary[]> {
  const response = await api.get<MovieInLibrary[]>(`/movies/search?q=${encodeURIComponent(query)}`);
  return response.data;
}

export async function getGenres(): Promise<Genre[]> {
  const response = await api.get('/genres');
  return response.data;
} 