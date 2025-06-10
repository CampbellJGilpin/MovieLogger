import api from '../api/config';
import type { Genre } from '../types';

export async function getAllGenres(): Promise<Genre[]> {
  const response = await api.get<Genre[]>('/genres');
  return response.data;
}

export async function getGenre(id: number): Promise<Genre> {
  const response = await api.get<Genre>(`/genres/${id}`);
  return response.data;
}

export async function createGenre(genre: Omit<Genre, 'id'>): Promise<Genre> {
  const response = await api.post<Genre>('/genres', genre);
  return response.data;
}

export async function updateGenre(id: number, genre: Omit<Genre, 'id'>): Promise<Genre> {
  const response = await api.put<Genre>(`/genres/${id}`, genre);
  return response.data;
}

export async function deleteGenre(id: number): Promise<void> {
  await api.delete(`/genres/${id}`);
} 