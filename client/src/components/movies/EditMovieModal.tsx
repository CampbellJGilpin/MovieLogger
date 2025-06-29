import { Fragment } from 'react';
import { Dialog, Transition } from '@headlessui/react';
import type { Movie, MovieCreateRequest } from '../../types';
import MovieForm from './MovieForm';

interface EditMovieModalProps {
  movie: Movie;
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (movieId: number, movieData: MovieCreateRequest) => void;
}

export default function EditMovieModal({ movie, isOpen, onClose, onSubmit }: EditMovieModalProps) {
  const handleSubmit = (movieData: MovieCreateRequest) => {
    onSubmit(movie.id, movieData);
  };

  return (
    <Transition appear show={isOpen} as={Fragment}>
      <Dialog as="div" className="relative z-10" onClose={onClose}>
        <Transition.Child
          as={Fragment}
          enter="ease-out duration-300"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="ease-in duration-200"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-black bg-opacity-25" />
        </Transition.Child>

        <div className="fixed inset-0 overflow-y-auto">
          <div className="flex min-h-full items-center justify-center p-4 text-center">
            <Transition.Child
              as={Fragment}
              enter="ease-out duration-300"
              enterFrom="opacity-0 scale-95"
              enterTo="opacity-100 scale-100"
              leave="ease-in duration-200"
              leaveFrom="opacity-100 scale-100"
              leaveTo="opacity-0 scale-95"
            >
              <Dialog.Panel className="w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-6 text-left align-middle shadow-xl transition-all">
                <Dialog.Title
                  as="h3"
                  className="text-lg font-medium leading-6 text-gray-900"
                >
                  Edit Movie
                </Dialog.Title>

                <MovieForm
                  movie={movie}
                  onSubmit={handleSubmit}
                  onCancel={onClose}
                />
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
} 