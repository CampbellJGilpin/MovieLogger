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
  userRating?: number;
}

export interface MovieCreateRequest {
  title: string;
  description: string;
  releaseDate: string;
  genreId: number;
  isDeleted?: boolean;
}

export interface MovieInLibrary extends Movie {
  isFavorite: boolean;
}

export interface Review {
  id: number;
  movieId: number;
  userId: number;
  rating: number;
  comment: string;
  dateReviewed: string;
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