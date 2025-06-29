import api from '../api/config';
import type { Viewing } from '../types/index';

interface ViewingCreateRequest {
  userId: number;
  movieId: number;
  dateWatched: string;
}

interface ViewingUpdateRequest {
  dateWatched: string;
}

export async function getViewing(viewingId: number): Promise<Viewing> {
  const response = await api.get<Viewing>(`/viewings/${viewingId}`);
  return response.data;
}

export async function createViewing(viewing: ViewingCreateRequest): Promise<Viewing> {
  const response = await api.post<Viewing>(`/viewings`, viewing);
  return response.data;
}

export async function updateViewing(viewingId: number, viewing: ViewingUpdateRequest): Promise<Viewing> {
  const response = await api.put<Viewing>(`/viewings/${viewingId}`, viewing);
  return response.data;
} 