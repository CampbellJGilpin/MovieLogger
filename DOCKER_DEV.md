# Docker Development Environment

This document explains how to use the Docker-based development environment for MovieLogger.

## 🚀 Quick Start

### Option 1: Full Docker Environment (Recommended)
```bash
# Start everything with hot reloading
make docker-dev

# Or start in background
make docker-dev-detached
```

### Option 2: Individual Services
```bash
# Start just the database
make docker-up

# Start database + migrations
make migrate

# Start individual services locally
make server  # Backend
make dev     # Frontend
```

## 📋 What's Included

### Services
- **Database**: PostgreSQL 15 with persistent storage
- **Backend**: .NET 8 API with hot reloading
- **Frontend**: React/TypeScript with Vite dev server
- **Migrations**: Flyway for database schema management
- **Proxy**: Optional nginx reverse proxy

### Features
- ✅ **Hot Reloading**: Code changes automatically refresh
- ✅ **Bind Mounts**: Source code is mounted for live editing
- ✅ **Health Checks**: Services wait for dependencies
- ✅ **Persistent Data**: Database data survives container restarts
- ✅ **Network Isolation**: Services communicate via Docker network

## 🔧 Ports

| Service | Port | URL |
|---------|------|-----|
| Database | 5432 | `postgresql://movieuser:hotrod1@localhost:5432/movielogger` |
| API | 5049 | `http://localhost:5049` |
| API (HTTPS) | 7049 | `https://localhost:7049` |
| Frontend | 5173 | `http://localhost:5173` |
| Proxy | 80 | `http://localhost` |

## 🛠️ Development Workflow

### 1. Start Development Environment
```bash
make docker-dev
```

### 2. Make Changes
- Edit files in `client/` or `server/`
- Changes are automatically detected and reloaded
- No need to restart containers

### 3. View Logs
```bash
make docker-logs
```

### 4. Stop Environment
```bash
make docker-down
```

## 📁 File Structure

```
MovieLogger/
├── docker-compose.yml          # Main orchestration
├── nginx.dev.conf             # Reverse proxy config
├── client/
│   ├── Dockerfile.dev         # Frontend container
│   └── .dockerignore          # Build exclusions
├── server/
│   ├── Dockerfile.dev         # Backend container
│   └── .dockerignore          # Build exclusions
└── database/
    └── migrations/            # Database schema
```

## 🔄 Hot Reloading

### Frontend (React/Vite)
- **File watching**: All `.tsx`, `.ts`, `.css` files
- **HMR**: Hot Module Replacement for instant updates
- **WebSocket**: Real-time communication for updates

### Backend (.NET)
- **dotnet watch**: Automatically rebuilds and restarts
- **File watching**: All `.cs` files trigger rebuilds
- **Fast restart**: Only changed assemblies are reloaded

## 🗄️ Database Management

### Run Migrations
```bash
make migrate
```

### Reset Database
```bash
make reset-db
```

### Seed Data
```bash
make seed
```

### View Database
```bash
# Connect with psql
psql -h localhost -U movieuser -d movielogger

# Or use a GUI tool like pgAdmin
```

## 🐛 Troubleshooting

### Common Issues

#### Port Already in Use
```bash
# Check what's using the port
lsof -i :5432
lsof -i :5049
lsof -i :5173

# Kill the process or change ports in docker-compose.yml
```

#### Container Won't Start
```bash
# Check logs
make docker-logs

# Rebuild containers
docker-compose build --no-cache
```

#### Database Connection Issues
```bash
# Ensure database is healthy
docker-compose ps

# Check database logs
docker-compose logs db
```

#### Hot Reloading Not Working
```bash
# Check file permissions
ls -la client/
ls -la server/

# Restart containers
make docker-down
make docker-dev
```

### Clean Slate
```bash
# Remove everything and start fresh
make docker-clean
make docker-dev
```

## 🔧 Configuration

### Environment Variables
- `VITE_API_URL`: Frontend API endpoint
- `ASPNETCORE_ENVIRONMENT`: Backend environment
- `ConnectionStrings__DefaultConnection`: Database connection

### Customization
Edit `docker-compose.yml` to:
- Change ports
- Add environment variables
- Modify volume mounts
- Add additional services

## 📊 Monitoring

### Health Checks
All services include health checks:
- Database: `pg_isready`
- API: HTTP endpoint `/health`
- Frontend: HTTP endpoint `/`

### Logs
```bash
# All services
make docker-logs

# Specific service
docker-compose logs api
docker-compose logs client
docker-compose logs db
```

## 🚀 Production vs Development

### Development
- Hot reloading enabled
- Source code mounted
- Debug information
- Development certificates

### Production
- Optimized builds
- No source code mounting
- Production certificates
- Health checks
- Resource limits

## 📚 Additional Resources

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Hot Reload](https://docs.microsoft.com/en-us/aspnet/core/test/hot-reload)
- [Vite Development Server](https://vitejs.dev/guide/cli.html)
- [PostgreSQL Docker](https://hub.docker.com/_/postgres) 