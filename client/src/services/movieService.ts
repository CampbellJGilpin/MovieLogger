import api from '../api/config';
import type { Movie, MovieInLibrary, Review, Genre, Viewing } from '../types/index';

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
    api.get<{ id: number }>(`/accounts/me`)
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

export async function createMovie(movieData: FormData): Promise<Movie> {
  const response = await api.post<Movie>(`/movies`, movieData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return response.data;
}

export async function updateMovie(id: number, movieData: FormData): Promise<Movie> {
  const response = await api.put<Movie>(`/movies/${id}`, movieData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return response.data;
}

export async function deleteMovie(id: number): Promise<void> {
  await api.delete(`/movies/${id}`);
}

export async function getMovieReviews(movieId: number): Promise<Review[]> {
  const userResponse = await api.get<{ id: number }>(`/accounts/me`);
  const userId = userResponse.data.id;
  const response = await api.get<Review[]>(`/movies/${movieId}/users/${userId}/reviews`);
  return response.data;
}

export async function addMovieReview(movieId: number, review: { score: number; reviewText: string }): Promise<Review> {
  const userResponse = await api.get<{ id: number }>(`/accounts/me`);
  const userId = userResponse.data.id;
  const response = await api.post<Review>(`/movies/${movieId}/reviews`, {
    userId,
    ...review
  });
  return response.data;
}

export async function toggleLibrary(movieId: number, isInLibrary: boolean): Promise<void> {
  const response = await api.get(`/accounts/me`);
  const userId = response.data.id;
  if (isInLibrary) {
    await api.delete(`/users/${userId}/library/${movieId}`);
  } else {
    await api.post(`/users/${userId}/library/${movieId}`);
  }
}

export async function toggleWatchLater(movieId: number): Promise<void> {
  await api.post(`/movies/${movieId}/watch-later`);
}

export async function toggleFavorite(movieId: number): Promise<void> {
  const response = await api.get(`/accounts/me`);
  const userId = response.data.id;
  const currentItem = await api.get<LibraryDto>(`/users/${userId}/library`);
  const existingItem = currentItem.data.libraryItems.find(item => item.movieId === movieId);
  if (existingItem && existingItem.favourite === "true") {
    await api.delete(`/users/${userId}/favorites/${movieId}`);
  } else {
    await api.post(`/users/${userId}/favorites/${movieId}`);
  }
}

export async function getMyLibrary(): Promise<MovieInLibrary[]> {
  const response = await api.get(`/accounts/me`);
  const userId = response.data.id;
  const libraryResponse = await api.get<LibraryDto>(`/users/${userId}/library`);
  return libraryResponse.data.libraryItems.map(mapLibraryItemToMovieInLibrary);
}

export async function getMyLibraryPaginated(
  page = 1,
  pageSize = 10
): Promise<{ items: MovieInLibrary[]; totalPages: number }> {
  const userResponse = await api.get('/accounts/me');
  const userId = userResponse.data.id;
  
  const response = await api.get<PaginatedResponse<LibraryItemDto>>(
    `/users/${userId}/library/paginated?page=${page}&pageSize=${pageSize}`
  );
  
  return {
    items: response.data.items.map(mapLibraryItemToMovieInLibrary),
    totalPages: response.data.totalPages
  };
}

export async function getMyFavoritesPaginated(
  page = 1,
  pageSize = 10
): Promise<{ items: MovieInLibrary[]; totalPages: number }> {
  const userResponse = await api.get('/accounts/me');
  const userId = userResponse.data.id;
  
  const response = await api.get<PaginatedResponse<LibraryItemDto>>(
    `/users/${userId}/library/favourites/paginated?page=${page}&pageSize=${pageSize}`
  );
  
  return {
    items: response.data.items.map(mapLibraryItemToMovieInLibrary),
    totalPages: response.data.totalPages
  };
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
  return movies.sort((a, b) => a.title.localeCompare(b.title));
}

export async function getGenres(): Promise<Genre[]> {
  const response = await api.get(`/genres`);
  return response.data;
}

export async function getAllMoviesForUser(
  userId: number,
  searchQuery = '',
  page = 1,
  pageSize = 10
): Promise<{ items: MovieInLibrary[]; totalPages: number }> {
  const params = new URLSearchParams({
    userId: userId.toString(),
    ...(searchQuery && { q: searchQuery }),
    page: page.toString(),
    pageSize: pageSize.toString(),
  });
  interface RawUserMovieResponse {
    id: number;
    title: string;
    description: string;
    releaseDate: string;
    genre: Genre;
    isDeleted: boolean;
    ownsMovie: boolean;
    isFavourite: boolean;
  }
  const response = await api.get<{ items: RawUserMovieResponse[], totalPages: number }>(`/movies/all-for-user?${params.toString()}`);
  return {
    items: response.data.items.map((movie: RawUserMovieResponse) => ({
      id: movie.id,
      title: movie.title,
      description: movie.description,
      releaseDate: movie.releaseDate,
      genre: movie.genre,
      isDeleted: movie.isDeleted,
      inLibrary: movie.ownsMovie,
      isFavorite: movie.isFavourite,
      isWatchLater: false,
      reviews: [],
    })),
    totalPages: response.data.totalPages ?? 1,
  };
} 

export async function getRecentlyWatchedMovies(): Promise<Viewing[]> {
  const userResponse = await api.get<{ id: number }>(`/accounts/me`);
  const userId = userResponse.data.id;
  const response = await api.get<Viewing[]>(`/users/${userId}/recently-watched?count=5`);
  return response.data;
} 