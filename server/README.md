# MovieLogger Server

The backend server for MovieLogger, built with ASP.NET Core.

## Tech Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **API Documentation**: OpenAPI/Swagger
- **Testing**: xUnit & NSubstitute

## Project Structure

```
server/
├── src/
│   ├── movielogger.api/          # API layer
│   │   ├── controllers/          # API endpoints
│   │   └── Program.cs           # Application entry point
│   ├── movielogger.services/     # Business logic layer
│   │   ├── interfaces/          # Service contracts
│   │   └── implementations/     # Service implementations
│   └── movielogger.domain/       # Domain layer
│       ├── entities/            # Database entities
│       └── dtos/               # Data transfer objects
└── tests/
    └── movielogger.services.tests/ # Unit tests
```

## Key Features

- RESTful API endpoints
- Layered architecture
- Dependency injection
- Async/await patterns
- Comprehensive error handling
- Unit testing coverage

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL
- Docker (optional)

### Setup

1. Clone the repository:
```bash
git clone https://github.com/campbelljgilpin/movielogger.git
cd movielogger/server
```

2. Set up the database:
```bash
# Using Docker
docker-compose up -d database

# Or connect to existing PostgreSQL instance
# Update connection string in appsettings.json
```

3. Run migrations:
```bash
dotnet ef database update
```

4. Start the server:
```bash
dotnet run --project src/movielogger.api
```

## Development

### API Endpoints

The API follows RESTful conventions with these main resources:
- `/api/movies` - Movie management
- `/api/movies/{id}/reviews` - Review management
- `/api/users` - User management
- `/api/users/{id}/library` - User library management

Full API documentation is available via Swagger UI at `/swagger` when running in development.

### Testing

Run unit tests:
```bash
dotnet test
```

### Dependencies

- **AutoMapper** - Object mapping
- **Entity Framework Core** - Database access
- **FluentValidation** - Request validation
- **xUnit** - Testing framework
- **NSubstitute** - Mocking framework

## Coding Conventions

- Use async/await for all I/O operations
- Follow REST API naming conventions
- Implement interface-based design
- Use dependency injection
- Include XML documentation for public APIs
- Follow C# naming conventions