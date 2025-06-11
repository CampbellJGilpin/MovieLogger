# Getting Started with MovieLogger

This guide will help you set up and run the MovieLogger application locally.

## Prerequisites

### Backend Requirements
- .NET 8.0 SDK
- PostgreSQL 15 or higher
- Docker (optional, for containerized database)

### Frontend Requirements
- Node.js 18.0 or higher
- npm 9.0 or higher

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/campbelljgilpin/movielogger.git
cd movielogger
```

### 2. Database Setup

#### Option A: Using Docker
```bash
# Start PostgreSQL container
docker-compose up -d database

# The database will be available at:
# Host: localhost
# Port: 5432
# Database: movielogger
# Username: postgres
# Password: postgres
```

#### Option B: Using Existing PostgreSQL Instance
1. Create a new database named `movielogger`
2. Update the connection string in `server/src/movielogger.api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=movielogger;Username=your_username;Password=your_password"
  }
}
```

### 3. Backend Setup
```bash
# Navigate to the server directory
cd server

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project src/movielogger.api

# Start the API server
dotnet run --project src/movielogger.api
```

The API will be available at `http://localhost:5049`

### 4. Frontend Setup
```bash
# Navigate to the client directory
cd client

# Install dependencies
npm install

# Start the development server
npm run dev
```

The frontend will be available at `http://localhost:3000`

## Verifying the Setup

### 1. Check API Status
- Open `http://localhost:5049/swagger` in your browser
- You should see the Swagger UI with all available endpoints

### 2. Check Frontend
- Open `http://localhost:3000` in your browser
- You should see the MovieLogger landing page

### 3. Create a Test Account
1. Click "Sign Up" on the frontend
2. Create an account with:
   - Username
   - Email
   - Password
3. Log in with your credentials

## Development Workflow

### Backend Development
- API endpoints are in `server/src/movielogger.api/controllers/`
- Business logic is in `server/src/movielogger.services/`
- Data access is in `server/src/movielogger.dal/`

### Frontend Development
- Components are in `client/src/components/`
- Pages are in `client/src/pages/`
- API services are in `client/src/services/`

### Running Tests
```bash
# Backend tests
cd server
dotnet test

# Frontend tests (when implemented)
cd client
npm test
```

## Common Issues and Solutions

### Database Connection Issues
1. Verify PostgreSQL is running:
```bash
docker ps  # if using Docker
# or
pg_isready # if using local PostgreSQL
```

2. Check connection string in `appsettings.json`

### Frontend Build Issues
1. Clear node modules and reinstall:
```bash
rm -rf node_modules
npm install
```

2. Check for Node.js version compatibility:
```bash
node --version  # Should be 18.0 or higher
```

### API Connection Issues
1. Verify CORS settings in `Program.cs`
2. Check API URL in frontend environment variables

## Next Steps

1. Explore the [API Documentation](api.md)
2. Review the [Architecture Decisions](adr/README.md)
3. Check out the [Frontend Documentation](../client/README.md)
4. Read the [Backend Documentation](../server/README.md)

## Need Help?

If you encounter any issues:
1. Check the existing [GitHub Issues](https://github.com/campbelljgilpin/movielogger/issues)
2. Review the documentation in the `docs` directory
3. Create a new issue with detailed information about your problem 