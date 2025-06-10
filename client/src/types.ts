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
}

export interface Review {
  id: number;
  rating: number;
  comment: string;
  viewDate: string;
  userId: number;
  movieId: number;
}

export interface Viewing {
  id: number;
  dateWatched: string;
  userMovie: UserMovie;
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