import api from '../api/config';
import type { Movie, MovieInLibrary, Review, Genre } from '../types/index';

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

interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

interface LibraryItemResponse {
  movieId: number;
  movieTitle: string;
  releaseDate: string;
  genre: string;
  inLibrary: string;
  watchLater: string;
  favourite: string;
}

function mapLibraryItemToMovieInLibrary(item: LibraryItemDto): MovieInLibrary {
  return {
    id: item.movieId,
    title: item.movieTitle,
    releaseDate: item.releaseDate,
    genre: { id: 0, title: item.genre }, // We don't have genre ID from the API
    description: "", // We don't have description from the API
    inLibrary: item.inLibrary === "true",
    isWatchLater: item.watchLater === "true",
    isFavorite: item.favourite === "true",
    isDeleted: false // Movies in the library are not deleted
  };
}

export async function getAllMovies(userId?: number, page = 1, pageSize = 10): Promise<PaginatedResponse<MovieInLibrary>> {
  const [moviesResponse, libraryResponse] = await Promise.all([
    api.get<PaginatedResponse<MovieInLibrary>>(`/movies?page=${page}&pageSize=${pageSize}`),
    userId ? api.get<LibraryDto>(`/users/${userId}/library`) : Promise.resolve({ data: { libraryItems: [] } })
  ]);

  const libraryItems = libraryResponse.data.libraryItems || [];
  
  const movies = moviesResponse.data.items.map(movie => {
    const libraryItem = libraryItems.find(item => item.movieId === movie.id);
    return {
      ...movie,
      isFavorite: libraryItem ? libraryItem.favourite === "true" : false,
      inLibrary: libraryItem ? libraryItem.inLibrary === "true" : false,
      isWatchLater: libraryItem ? libraryItem.watchLater === "true" : false
    };
  });

  return {
    ...moviesResponse.data,
    items: movies
  };
}

export async function getMovie(id: number): Promise<MovieInLibrary> {
  const [movieResponse, userResponse] = await Promise.all([
    api.get<MovieInLibrary>(`/movies/${id}`),
    api.get<{ id: number }>('/accounts/me')
  ]);

  const userId = userResponse.data.id;
  const libraryResponse = await api.get<LibraryDto>(`/users/${userId}/library`);
  const libraryItems = libraryResponse.data.libraryItems || [];
  const libraryItem = libraryItems.find(item => item.movieId === id);

  return {
    ...movieResponse.data,
    isFavorite: libraryItem ? libraryItem.favourite === "true" : false,
    inLibrary: libraryItem ? libraryItem.inLibrary === "true" : false,
    isWatchLater: libraryItem ? libraryItem.watchLater === "true" : false
  };
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

export async function toggleLibrary(movieId: number): Promise<MovieInLibrary | undefined> {
  const response = await api.get('/accounts/me');
  const userId = response.data.id;
  const currentLibrary = await api.get<LibraryDto>(`/users/${userId}/library`);
  const existingItem = currentLibrary.data.libraryItems.find(item => item.movieId === movieId);

  if (existingItem && existingItem.inLibrary === "true") {
    // Remove from library
    await api.delete(`/users/${userId}/library/${movieId}`);
  } else {
    // Add to library
    await api.post(`/users/${userId}/library/${movieId}`);
  }

  // Fetch and return the updated item
  const refreshed = await api.get<LibraryDto>(`/users/${userId}/library`);
  const refreshedItem = refreshed.data.libraryItems.find(item => item.movieId === movieId);
  return refreshedItem ? mapLibraryItemToMovieInLibrary(refreshedItem) : undefined;
}

export async function toggleWatchLater(movieId: number): Promise<void> {
  await api.post(`/movies/${movieId}/watch-later`);
}

export async function toggleFavorite(movieId: number): Promise<MovieInLibrary> {
  const response = await api.get('/accounts/me');
  const userId = response.data.id;
  const currentItem = await api.get<LibraryDto>(`/users/${userId}/library`);
  const existingItem = currentItem.data.libraryItems.find(item => item.movieId === movieId);

  if (!existingItem) {
    return Promise.reject(new Error("Movie must be in your library to favorite it."));
  }

  if (existingItem.favourite === "true") {
    // Remove from favorites
    await api.delete(`/users/${userId}/favorites/${movieId}`);
  } else {
    // Add to favorites
    await api.post(`/users/${userId}/favorites/${movieId}`);
  }

  // Fetch the updated item
  const refreshed = await api.get<LibraryDto>(`/users/${userId}/library`);
  const refreshedItem = refreshed.data.libraryItems.find(item => item.movieId === movieId);
  return mapLibraryItemToMovieInLibrary(refreshedItem!);
}

export async function getMyLibrary(): Promise<MovieInLibrary[]> {
  const response = await api.get('/accounts/me');
  const userId = response.data.id;
  const libraryResponse = await api.get<LibraryDto>(`/users/${userId}/library`);
  return libraryResponse.data.libraryItems.map(mapLibraryItemToMovieInLibrary);
}

export async function searchMovies(query: string, userId?: number): Promise<MovieInLibrary[]> {
  const [searchResponse, libraryResponse] = await Promise.all([
    api.get<MovieInLibrary[]>(`/movies/search?q=${encodeURIComponent(query)}`),
    userId ? api.get<LibraryDto>(`/users/${userId}/library`) : Promise.resolve({ data: { libraryItems: [] } })
  ]);

  const libraryItems = libraryResponse.data.libraryItems || [];
  
  const movies = searchResponse.data.map(movie => {
    const libraryItem = libraryItems.find(item => item.movieId === movie.id);
    return {
      ...movie,
      isFavorite: libraryItem ? libraryItem.favourite === "true" : false,
      inLibrary: libraryItem ? libraryItem.inLibrary === "true" : false,
      isWatchLater: libraryItem ? libraryItem.watchLater === "true" : false
    };
  });

  // Sort movies by title
  return movies.sort((a, b) => a.title.localeCompare(b.title));
}

export async function getGenres(): Promise<Genre[]> {
  const response = await api.get('/genres');
  return response.data;
} 