export interface User {
  id: number;
  userName: string;
  email: string;
  isAdmin: boolean;
  isDeleted: boolean;
}

export interface Movie {
  id: string;
  title: string;
  description: string;
  releaseYear: number;
  genre: string;
  posterUrl?: string;
}

export interface Review {
  id: string;
  movieId: string;
  userId: string;
  rating: number;
  comment: string;
  viewDate: string;
}

export interface MovieInLibrary extends Movie {
  isWatched: boolean;
  isWatchLater: boolean;
  isFavorite: boolean;
  userRating?: number;
} 