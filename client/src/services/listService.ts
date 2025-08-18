import api from '../api/config';
import type { Movie } from '../types/index';

export interface List {
  id: number;
  userId: number;
  name: string;
  description?: string;
  createdDate: string;
  updatedDate: string;
  movieCount: number;
  movies?: Movie[];
}

export interface ListSummary {
  id: number;
  userId: number;
  name: string;
  description?: string;
  createdDate: string;
  updatedDate: string;
  movieCount: number;
}

export interface CreateListRequest {
  name: string;
  description?: string;
}

export interface UpdateListRequest {
  name: string;
  description?: string;
}

export interface AddMovieToListRequest {
  movieId: number;
}

class ListService {
  async getUserLists(userId: number): Promise<ListSummary[]> {
    const response = await api.get(`/users/${userId}/lists`);
    return response.data;
  }

  async getList(userId: number, listId: number): Promise<List> {
    const response = await api.get(`/users/${userId}/lists/${listId}`);
    return response.data;
  }

  async createList(userId: number, request: CreateListRequest): Promise<ListSummary> {
    const response = await api.post(`/users/${userId}/lists`, request);
    return response.data;
  }

  async updateList(userId: number, listId: number, request: UpdateListRequest): Promise<ListSummary> {
    const response = await api.put(`/users/${userId}/lists/${listId}`, request);
    return response.data;
  }

  async deleteList(userId: number, listId: number): Promise<void> {
    await api.delete(`/users/${userId}/lists/${listId}`);
  }

  async addMovieToList(userId: number, listId: number, movieId: number): Promise<void> {
    await api.post(`/users/${userId}/lists/${listId}/movies`, { movieId });
  }

  async removeMovieFromList(userId: number, listId: number, movieId: number): Promise<void> {
    await api.delete(`/users/${userId}/lists/${listId}/movies/${movieId}`);
  }

  async getListMovies(userId: number, listId: number): Promise<Movie[]> {
    const response = await api.get(`/users/${userId}/lists/${listId}/movies`);
    return response.data;
  }

  async isMovieInList(userId: number, listId: number, movieId: number): Promise<boolean> {
    const response = await api.get(`/users/${userId}/lists/${listId}/movies/${movieId}/check`);
    return response.data.inList;
  }
}

const listService = new ListService();
export default listService;