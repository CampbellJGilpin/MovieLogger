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
}

export interface MovieCreateRequest {
  title: string;
  description: string;
  releaseDate: string;
  genreId: number;
  isDeleted: boolean;
}

export interface MovieInLibrary extends Movie {
  isWatched: boolean;
  isWatchLater: boolean;
  isFavorite: boolean;
}

export interface Review {
  id: number;
  rating: number;
  comment: string;
  dateCreated: string;
  dateUpdated: string;
  viewing: Viewing;
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