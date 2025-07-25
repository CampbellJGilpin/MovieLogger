import api from '../api/config';
import type { GenrePreference, GenrePreferencesSummary } from '../types/index';

export async function getUserGenrePreferences(): Promise<GenrePreferencesSummary> {
  const response = await api.get<GenrePreferencesSummary>('/genre-preferences/summary');
  return response.data;
}

export async function getTopGenresByWatchCount(count: number = 5): Promise<GenrePreference[]> {
  const response = await api.get<GenrePreference[]>(`/genre-preferences/top-by-watches?count=${count}`);
  return response.data;
}

export async function getTopGenresByRating(count: number = 5): Promise<GenrePreference[]> {
  const response = await api.get<GenrePreference[]>(`/genre-preferences/top-by-rating?count=${count}`);
  return response.data;
}

export async function getLeastWatchedGenres(count: number = 5): Promise<GenrePreference[]> {
  const response = await api.get<GenrePreference[]>(`/genre-preferences/least-watched?count=${count}`);
  return response.data;
}

export async function getGenreWatchTrends(months: number = 6): Promise<Record<string, number>> {
  const response = await api.get<Record<string, number>>(`/genre-preferences/trends?months=${months}`);
  return response.data;
} 