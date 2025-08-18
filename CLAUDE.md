# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MovieLogger is a full-stack web application for tracking movies, managing personal libraries, writing reviews, and maintaining watchlists. The application uses a modern tech stack with ASP.NET Core 8.0 backend, React 18 frontend, and PostgreSQL database.

## Architecture

The project follows an N-Tier architecture pattern in a monolithic deployment:

- **Frontend**: React 18 + TypeScript + Vite + Tailwind CSS (port 5173)
- **Backend**: ASP.NET Core 8.0 Web API (port 5049)
- **Database**: PostgreSQL 15 (port 5432)
- **Message Queue**: RabbitMQ (ports 5672, 15672)

### Key Architecture Layers:
- **API Layer**: Controllers in `server/src/movielogger.api/controllers/`
- **Business Logic**: Services in `server/src/movielogger.services/services/`
- **Data Access**: Entity Framework Core in `server/src/movielogger.dal/`
- **Messaging**: RabbitMQ integration in `server/src/movielogger.messaging/`

## Development Commands

### Recommended Development Workflow
Use the comprehensive Makefile or the dev-start script:
```bash
# Start full development environment with process management
make dev-start
# or
./scripts/dev-start.sh start

# Check status
make dev-status

# Stop all services
make dev-stop
```

### Individual Component Commands

#### Frontend (Client)
```bash
cd client
npm install          # Install dependencies
npm run dev          # Start development server (port 5173)
npm run build        # Build for production
npm run lint         # Lint code
```

#### Backend (Server)
```bash
cd server
dotnet restore       # Restore dependencies
dotnet run --project src/movielogger.api  # Start API server (port 5049)
dotnet build --configuration Release      # Build for production
dotnet test         # Run all tests
dotnet format       # Format code
```

#### Database & Infrastructure
```bash
# Start database and RabbitMQ
make docker-up
# or
docker-compose up -d db rabbitmq

# Run migrations
make migrate
# or
docker-compose --profile migrate up flyway

# Full Docker development environment
docker-compose up --build
```

### Testing Commands
```bash
# Run all tests
make test

# Backend tests only
cd server && dotnet test

# Frontend tests
cd client && npm test
```

### Code Quality
```bash
# Lint all code
make lint

# Format all code
make format

# Backend-specific
cd server && dotnet format --verify-no-changes

# Frontend-specific
cd client && npm run lint
```

## Project Structure

### Backend Structure (`server/`)
- `src/movielogger.api/` - Web API controllers, models, validation, middleware
- `src/movielogger.services/` - Business logic, caching, file upload services
- `src/movielogger.dal/` - Data access, Entity Framework context, entities
- `src/movielogger.messaging/` - RabbitMQ messaging and audit events
- `tests/` - Unit and integration tests

### Frontend Structure (`client/`)
- `src/components/` - React components organized by feature
- `src/pages/` - Top-level page components
- `src/contexts/` - React context providers (Auth, etc.)
- `src/services/` - API service layers
- `src/types/` - TypeScript type definitions

### Key Features Implementation
- **Authentication**: JWT-based auth with protected routes
- **Movie Management**: CRUD operations with poster upload
- **Reviews & Ratings**: Star rating system with text reviews
- **Library Management**: Personal collections with favorites/ownership toggles
- **Caching**: Memory-based caching for movies service
- **File Upload**: Image handling for movie posters
- **Audit Logging**: Event-driven audit trail via RabbitMQ

## Configuration & Environment

### Required Environment Variables
- `DATABASE_URL` - PostgreSQL connection string
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` - JWT configuration
- `RabbitMQ:Host`, `RabbitMQ:Username`, etc. - Message queue settings

### Development URLs
- Frontend: http://localhost:5173
- Backend API: http://localhost:5049
- Swagger UI: http://localhost:5049/swagger
- Database: localhost:5432
- RabbitMQ Management: http://localhost:15672

### Docker Services
The `docker-compose.yml` configures:
- PostgreSQL database with health checks
- RabbitMQ message broker with management UI
- Flyway for database migrations
- Optional development containers for API and client

## Key Frameworks & Libraries

### Backend Dependencies
- **Entity Framework Core** - ORM with PostgreSQL provider
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation
- **JWT Bearer** - Authentication
- **RabbitMQ.Client** - Message queuing
- **Swagger/OpenAPI** - API documentation

### Frontend Dependencies
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **Headless UI** - Accessible UI components
- **Heroicons** - Icon library
- **Tailwind CSS** - Utility-first styling

## Testing Strategy

### Backend Testing
- **Unit Tests**: Service layer tests using mocked dependencies
- **Integration Tests**: Full API tests with test database
- **Test Fixtures**: Custom WebApplicationFactory for integration tests
- **Test Data**: TestDataBuilder pattern for creating test entities

### Testing Patterns
- Use `BaseServiceTest` and `BaseTestController` as base classes
- Mock external dependencies (database, messaging)
- Integration tests use separate test database
- Authentication helper for testing protected endpoints

## Database Considerations

### Migration Management
- Migrations in `database/migrations/` using Flyway
- Versioned SQL files (V1.0__initial_schema.sql, etc.)
- Support for out-of-order migrations during development

### Entity Relationships
- Users have Libraries (many-to-many with Movies)
- Viewings track individual movie watches with Reviews
- Genre preferences for personalized recommendations
- Audit logs for tracking system events

## Deployment & Infrastructure

The project includes AWS CDK infrastructure code in `infrastructure/` for cloud deployment:
- S3 + CloudFront for frontend hosting
- ECS for backend container deployment
- RDS for PostgreSQL database
- Load balancer and auto-scaling configuration

## Common Development Patterns

### Backend Patterns
- **Repository Pattern**: Implemented via Entity Framework DbContext
- **Service Layer Pattern**: Business logic separated from controllers
- **CQRS-like**: Separate query and command services for complex operations
- **Factory Pattern**: MoviesServiceFactory for service resolution
- **Decorator Pattern**: CachedMoviesService wrapping base MoviesService

### Frontend Patterns
- **Context Pattern**: AuthContext for global authentication state
- **Custom Hooks**: useAuth hook for authentication logic
- **Protected Routes**: Route guards for authenticated pages
- **Service Layer**: Dedicated API service classes
- **Component Composition**: Layout components with nested routing

## Security Considerations

- JWT token-based authentication with proper validation
- CORS configured for development (review for production)
- Input validation using FluentValidation
- File upload restrictions for poster uploads
- Environment-based configuration management
- Database connection via environment variables

## Caching Strategy

The application implements configurable caching:
- Memory caching for frequently accessed movie data
- Cache invalidation on movie updates
- Configurable cache TTL via app settings
- Factory pattern for switching between cached/non-cached services