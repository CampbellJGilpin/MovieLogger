# Development Process Management

This guide explains how to properly manage your MovieLogger development environment to avoid port conflicts and hanging processes.

## The Problem

You mentioned regularly needing to check PIDs for ports and kill processes to start up full dev again. This happens because:

1. **Background processes don't shut down properly** when using `&` in shell commands
2. **No process tracking** - the system doesn't know which processes belong to your dev environment
3. **Port conflicts** - processes remain bound to ports even after the parent process ends
4. **No graceful shutdown** - Ctrl+C only stops the main process, not background processes

## Solution: New Development Scripts

I've created improved scripts that properly manage development processes:

### 1. **dev-start.sh** - Main Development Script

This script provides proper process management with:
- **Process tracking** - Records PIDs of all started processes
- **Graceful shutdown** - Properly stops all processes on Ctrl+C
- **Port checking** - Warns about port conflicts before starting
- **Health checks** - Waits for services to be ready
- **Logging** - Captures output from all services

### 2. **kill-ports.sh** - Port Management Utility

A utility to quickly kill processes on common development ports:
- **Interactive mode** - Shows which ports are in use
- **Batch mode** - Kill all common dev ports at once
- **Specific ports** - Kill processes on specific ports

## Usage

### Recommended Workflow

```bash
# Start development environment (recommended)
make dev-start

# Stop all services
make dev-stop

# Check status
make dev-status

# View logs
make dev-logs server    # Backend logs
make dev-logs client    # Frontend logs

# Restart everything
make dev-restart

# Clean up everything
make dev-clean
```

### Port Management

```bash
# Interactive port killer
make kill-ports

# Kill all common dev ports
./scripts/kill-ports.sh --all

# Kill specific port
./scripts/kill-ports.sh 5049
```

### Legacy Commands (Not Recommended)

```bash
# These may leave processes running
make full-dev
make dev
make server
```

## What Each Command Does

### `make dev-start`
1. Checks if Docker is running
2. Starts database and RabbitMQ containers
3. Waits for services to be healthy
4. Runs database migrations
5. Starts backend server (tracks PID)
6. Starts frontend client (tracks PID)
7. Waits for services to be ready
8. Shows status
9. Keeps running until Ctrl+C

### `make dev-stop`
1. Reads tracked PIDs from `/tmp/movielogger-dev.pid`
2. Gracefully stops each process
3. Force kills if necessary
4. Cleans up PID file

### `make dev-status`
1. Shows which tracked processes are running
2. Shows which ports are in use
3. Shows service URLs
4. Shows log file locations

### `make kill-ports`
1. Shows interactive menu of common ports
2. Displays which ports are in use
3. Allows selective or batch port killing

## Common Ports Used

| Port | Service | Description |
|------|---------|-------------|
| 5432 | PostgreSQL | Database |
| 5672 | RabbitMQ | Message broker |
| 15672 | RabbitMQ | Management UI |
| 5049 | .NET API | Backend server |
| 5173 | Vite | Frontend dev server |
| 80 | Nginx | Reverse proxy (optional) |

## Troubleshooting

### Port Already in Use

```bash
# Check what's using a port
lsof -i :5049

# Kill processes on that port
make kill-ports
# Then select the port from the menu

# Or kill all dev ports
./scripts/kill-ports.sh --all
```

### Processes Not Stopping

```bash
# Force stop everything
make dev-clean

# Or manually kill by PID
ps aux | grep dotnet
kill -9 <PID>
```

### Docker Issues

```bash
# Reset Docker environment
docker-compose down -v --remove-orphans
docker system prune -f
make dev-start
```

### Logs

All service logs are stored in `/tmp/movielogger-logs/`:
- `server.log` - Backend server logs
- `client.log` - Frontend client logs

View logs with:
```bash
make dev-logs server
make dev-logs client
```

## Best Practices

### 1. Always Use `make dev-start`
- Proper process management
- Automatic cleanup on shutdown
- Health checks and status reporting

### 2. Use `make dev-stop` to Shut Down
- Don't just close the terminal
- Ensures all processes are properly stopped

### 3. Check Status Before Starting
```bash
make dev-status
```

### 4. Use Port Killer When Needed
```bash
make kill-ports
```

### 5. Clean Up Regularly
```bash
make dev-clean
```

## Migration from Old Workflow

### Before (Problematic)
```bash
make full-dev
# ... work ...
# Ctrl+C or close terminal
# Processes left running!
# Next time: port conflicts
```

### After (Recommended)
```bash
make dev-start
# ... work ...
# Ctrl+C
# All processes properly stopped
# Next time: clean startup
```

## Script Features

### dev-start.sh Features
- ✅ Process tracking with PID files
- ✅ Graceful shutdown with signal handlers
- ✅ Port conflict detection
- ✅ Health checks for all services
- ✅ Comprehensive logging
- ✅ Status reporting
- ✅ Error handling and recovery

### kill-ports.sh Features
- ✅ Interactive mode
- ✅ Batch mode for all ports
- ✅ Specific port targeting
- ✅ Process verification
- ✅ Color-coded output
- ✅ Safe process termination

## Why This Solves Your Problem

1. **No More Hanging Processes**: All processes are tracked and properly terminated
2. **No More Port Conflicts**: Port checking prevents conflicts, port killer resolves them
3. **Clean Shutdown**: Ctrl+C properly stops everything
4. **Easy Debugging**: Status and log commands help diagnose issues
5. **Consistent Environment**: Same startup/shutdown process every time

## Quick Reference

```bash
# Start development
make dev-start

# Stop development
make dev-stop

# Check what's running
make dev-status

# Kill port conflicts
make kill-ports

# View logs
make dev-logs server
make dev-logs client

# Restart everything
make dev-restart

# Nuclear option
make dev-clean
```

This new workflow should eliminate your need to manually check PIDs and kill processes! 