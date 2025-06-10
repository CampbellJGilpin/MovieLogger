export interface User {
  id: number;
  email: string;
  userName: string;
  isAdmin: boolean;
}

export interface Genre {
  id: number;
  title: string;
}

export interface Movie {
  id: number;
  title: string;
  description: string;
  releaseDate: string;
  genre: Genre;
  isDeleted: boolean;
}

export interface MovieCreateRequest {
  title: string;
  description: string;
  releaseDate: string;
  genreId: number;
  isDeleted?: boolean;
}

export interface MovieInLibrary extends Movie {
  isWatched: boolean;
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
  dateViewed: string;
  movieId: number;
  review?: Review;
}

export interface UserMovie {
  id: number;
  userId: number;
  movieId: number;
  movie: Movie;
  isWatched: boolean;
  isWatchLater: boolean;
  isFavorite: boolean;
  upcomingViewDate?: string;
} 