interface UploadProgressProps {
  progress: number;
  isUploading: boolean;
  fileName?: string;
}

export default function UploadProgress({ progress, isUploading, fileName }: UploadProgressProps) {
  if (!isUploading) return null;

  return (
    <div className="mt-2">
      <div className="flex items-center justify-between text-sm">
        <span className="text-gray-600">
          {fileName ? `Uploading ${fileName}...` : 'Uploading...'}
        </span>
        <span className="text-gray-500">{Math.round(progress)}%</span>
      </div>
      <div className="mt-1 bg-gray-200 rounded-full h-2">
        <div
          className="bg-blue-600 h-2 rounded-full transition-all duration-300 ease-out"
          style={{ width: `${progress}%` }}
        />
      </div>
    </div>
  );
}