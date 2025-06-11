import { StarIcon as StarOutline } from '@heroicons/react/24/outline';
import { StarIcon as StarSolid } from '@heroicons/react/24/solid';

interface StarRatingProps {
  rating: number;
  maxStars?: number;
  className?: string;
}

export default function StarRating({ rating, maxStars = 5, className = '' }: StarRatingProps) {
  return (
    <div className={`flex items-center ${className}`}>
      {[...Array(maxStars)].map((_, index) => (
        <span key={index} className="text-yellow-400">
          {index < rating ? (
            <StarSolid className="w-4 h-4" />
          ) : (
            <StarOutline className="w-4 h-4" />
          )}
        </span>
      ))}
    </div>
  );
} 