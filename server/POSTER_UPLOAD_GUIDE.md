# Movie Poster Upload Implementation Guide

## Overview
The MovieLogger application now supports complete poster upload functionality for both creating and updating movies. This guide documents the implementation and usage.

## Features Implemented

### Frontend Features
- ✅ **File Upload Interface**: Modern drag-and-drop style file input with preview
- ✅ **Image Preview**: Live preview of selected poster images before upload
- ✅ **Current Poster Display**: Shows existing poster when editing movies
- ✅ **File Validation**: Client-side validation for file type, size, and format
- ✅ **Upload Progress**: Real-time progress indicators during upload
- ✅ **Error Handling**: User-friendly error messages for upload failures

### Backend Features
- ✅ **File Upload Service**: Centralized service for poster management
- ✅ **Local Development Storage**: Stores files in `wwwroot/uploads/` for development
- ✅ **File Validation**: Server-side validation for security and file integrity
- ✅ **Poster Cleanup**: Automatically removes old posters when updating
- ✅ **Static File Serving**: Properly configured file serving for local development

## File Structure

```
server/
├── src/movielogger.services/
│   ├── services/FileUploadService.cs      # Core upload logic
│   └── interfaces/IFileUploadService.cs   # Service interface
├── src/movielogger.api/
│   └── controllers/MoviesController.cs    # API endpoints with upload
└── wwwroot/
    └── uploads/                           # Local file storage

client/
├── src/components/
│   ├── movies/
│   │   ├── MovieForm.tsx                  # Enhanced form with upload
│   │   ├── AddMovieModal.tsx              # Create movie modal
│   │   └── EditMovieModal.tsx             # Edit movie modal
│   └── common/
│       └── UploadProgress.tsx             # Progress indicator component
└── src/services/
    └── movieService.ts                    # API calls with progress support
```

## Usage

### For Users
1. **Adding a Movie**: Click "Add Movie" → select poster file → preview appears → submit form
2. **Editing a Movie**: Click "Edit Movie" → current poster shows → optionally select new poster → submit
3. **File Requirements**: JPEG, PNG, or WebP images, maximum 5MB file size

### For Developers

#### API Endpoints
```http
POST /api/movies
PUT /api/movies/{id}
```
Both endpoints accept `multipart/form-data` with optional `poster` file field.

#### Frontend Integration
```typescript
// Create movie with progress tracking
const handleAddMovie = async (movieData: FormData, onUploadProgress?: (progressEvent) => void) => {
  await movieService.createMovie(movieData, onUploadProgress);
};
```

## Configuration

### Environment Variables
- `VITE_API_URL`: Frontend API base URL (default: `http://localhost:5049/api`)
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` for local file storage

### File Validation Settings
- **Max File Size**: 5MB
- **Allowed Types**: image/jpeg, image/png, image/webp
- **Allowed Extensions**: .jpg, .jpeg, .png, .webp

## Local Development Setup

1. **Backend Setup**:
   ```bash
   cd server
   dotnet build
   dotnet run --project src/movielogger.api
   ```

2. **Frontend Setup**:
   ```bash
   cd client
   npm install
   npm run dev
   ```

3. **File Storage**: Files are automatically stored in `server/wwwroot/uploads/`

## Technical Implementation Details

### File Storage Strategy
- **Development**: Local filesystem (`wwwroot/uploads/`)
- **Production**: Configured for S3 upload (implementation pending)

### Security Measures
- File type validation on both client and server
- File size limits enforced
- Unique filename generation using GUIDs
- Extension validation to prevent malicious uploads

### Performance Features
- Upload progress tracking
- Automatic cleanup of old files
- Optimized image preview generation
- Proper error handling and user feedback

## Testing

### Manual Testing Checklist
- [ ] Create movie with poster upload
- [ ] Create movie without poster
- [ ] Edit movie and add poster
- [ ] Edit movie and replace existing poster
- [ ] Edit movie without changing poster
- [ ] Test file size validation (try >5MB file)
- [ ] Test file type validation (try .txt file)
- [ ] Test upload progress indicator
- [ ] Verify poster display in movie cards and details
- [ ] Check poster cleanup when updating

### Test Files Location
Example poster files for testing are available in: `server/wwwroot/uploads/`

## Troubleshooting

### Common Issues
1. **Poster not displaying**: Check `VITE_API_URL` configuration
2. **Upload fails**: Verify file size and type restrictions
3. **Files not saved**: Ensure `wwwroot/uploads/` directory exists and has write permissions

### Debug Mode
Enable detailed logging by checking browser console and server logs during upload operations.

## Future Enhancements
- [ ] S3 integration for production deployments
- [ ] Image compression and resizing
- [ ] Drag-and-drop upload interface
- [ ] Bulk poster upload functionality
- [ ] Poster URL validation for external images

---

Last Updated: August 2025