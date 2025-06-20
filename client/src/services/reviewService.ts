import api from '../api/config';
import type { Review } from '../types/index';

export async function getUserReviews(userId: number): Promise<Review[]> {
  const response = await api.get<Review[]>(`/api/users/${userId}/reviews`);
  return response.data;
}

export async function createReview(viewingId: number, review: { score: number; reviewText: string }): Promise<Review> {
  const response = await api.post<Review>(`/api/viewings/${viewingId}/reviews`, review);
  return response.data;
}

export async function updateReview(reviewId: number, review: { score: number; reviewText: string }): Promise<Review> {
  const response = await api.put<Review>(`/api/reviews/${reviewId}`, review);
  return response.data;
} 