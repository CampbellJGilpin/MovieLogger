export interface User {
  id: number;
  userName: string;
  email: string;
  isAdmin: boolean;
  isDeleted: boolean;
}

export interface Genre {
  id: number;
  title: string;
}

export interface MovieCreateRequest {
  title: string;
  description: string;
  releaseDate: string;
  genreId: number;
  isDeleted?: boolean;
}

export interface Movie {
  id: number;
  title: string;
  description: string;
  releaseDate: string;
  genre: Genre;
  isDeleted: boolean;
}

export interface MovieInLibrary extends Movie {
  inLibrary: boolean;
  isWatchLater: boolean;
  isFavorite: boolean;
  userRating?: number;
  reviews?: Review[];
}

export interface Review {
  id: number;
  score: number;
  reviewText: string;
  dateViewed: string;
  movieTitle: string;
}

export interface Viewing {
  id: number;
  userId: number;
  favourite: boolean;
  ownsMovies: boolean;
  upcomingViewDate: string;
  dateViewed: string;
  movie: Movie;
} 