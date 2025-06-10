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

export interface Movie {
  id: number;
  title: string;
  description: string;
  releaseDate: string;
  genre: Genre;
  isDeleted: boolean;
}

export interface MovieInLibrary extends Movie {
  isWatched: boolean;
  isWatchLater: boolean;
  isFavorite: boolean;
  userRating?: number;
}

export interface Review {
  id: number;
  movieId: number;
  userId: number;
  rating: number;
  comment: string;
  viewDate: string;
} 