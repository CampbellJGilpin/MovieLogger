import { StarIcon } from '@heroicons/react/24/solid';
import type { Review } from '../../types/index';

interface ReviewListProps {
  reviews: Review[];
}

export default function ReviewList({ reviews }: ReviewListProps) {
  console.log('ReviewList - Received Reviews:', reviews);

  // Sort reviews by date, newest first
  const sortedReviews = [...reviews].sort(
    (a, b) => new Date(b.dateViewed).getTime() - new Date(a.dateViewed).getTime()
  );
  console.log('ReviewList - Sorted Reviews:', sortedReviews);

  if (!reviews || reviews.length === 0) {
    return (
      <div className="text-gray-500 text-center py-4">
        No reviews yet. Be the first to review!
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {sortedReviews.map((review) => (
        <div key={review.id} className="bg-white rounded-lg shadow p-4">
          <div className="flex items-center justify-between mb-2">
            <div className="flex items-center">
              {[...Array(5)].map((_, index) => (
                <StarIcon
                  key={index}
                  className={`h-5 w-5 ${
                    index < review.score ? 'text-yellow-400' : 'text-gray-300'
                  }`}
                />
              ))}
            </div>
            <div className="text-sm text-gray-500">
              {new Date(review.dateViewed).toLocaleDateString()}
            </div>
          </div>
          <p className="text-gray-700 whitespace-pre-wrap">{review.reviewText}</p>
        </div>
      ))}
    </div>
  );
} 