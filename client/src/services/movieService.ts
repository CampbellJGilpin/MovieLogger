import api from '../api/config';
import type { Movie, MovieInLibrary, Review, Genre } from '../types';

interface MovieCreateRequest {
  title: string;
  description: string;
  releaseDate: string;
  genreId: number;
}

interface MovieUpdateRequest {
  title?: string;
  description?: string;
  releaseDate?: string;
  genreId?: number;
}

interface LibraryDto {
  userId: number;
  libraryItems: LibraryItemDto[];
}

interface LibraryItemDto {
  movieId: number;
  movieTitle: string;
  releaseDate: string;
  genre: string;
  favourite: string;
  inLibrary: string;
  watchLater: string;
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
  const response = await api.get<Review[]>(`/movies/${movieId}/reviews`);
  return response.data;
}

export async function addMovieReview(movieId: number, review: { rating: number; comment: string }): Promise<Review> {
  const response = await api.post<Review>(`/reviews/viewings/${movieId}/reviews`, review);
  return response.data;
}

export async function toggleWatched(id: number): Promise<void> {
  await api.put(`/users/me/library/${id}/watched`);
}

export async function toggleWatchLater(id: number): Promise<void> {
  await api.put(`/users/me/library/${id}/watch-later`);
}

export async function toggleFavorite(id: number): Promise<void> {
  await api.put(`/users/me/library/${id}/favorite`);
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