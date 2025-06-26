#!/bin/bash

# MovieLogger Development Startup Script
# This script properly manages development processes with clean shutdown

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
CLIENT_DIR="client"
SERVER_DIR="server"
PID_FILE="/tmp/movielogger-dev.pid"
LOG_DIR="/tmp/movielogger-logs"

# Create log directory
mkdir -p "$LOG_DIR"

# Function to log messages
log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

# Function to log errors
log_error() {
    echo -e "${RED}[$(date +'%Y-%m-%d %H:%M:%S')] ERROR:${NC} $1"
}

# Function to log warnings
log_warning() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1"
}

# Function to check if port is in use
check_port() {
    local port=$1
    local service=$2
    
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1; then
        log_warning "Port $port is already in use by $service"
        return 1
    fi
    return 0
}

# Function to kill process by PID
kill_process() {
    local pid=$1
    local name=$2
    
    if [ -n "$pid" ] && kill -0 "$pid" 2>/dev/null; then
        log "Stopping $name (PID: $pid)..."
        kill "$pid" 2>/dev/null || true
        sleep 2
        if kill -0 "$pid" 2>/dev/null; then
            log_warning "Force killing $name (PID: $pid)..."
            kill -9 "$pid" 2>/dev/null || true
        fi
    fi
}

# Function to cleanup processes
cleanup() {
    log "Shutting down development environment..."
    
    if [ -f "$PID_FILE" ]; then
        while IFS= read -r line; do
            if [ -n "$line" ]; then
                pid=$(echo "$line" | cut -d' ' -f1)
                name=$(echo "$line" | cut -d' ' -f2-)
                kill_process "$pid" "$name"
            fi
        done < "$PID_FILE"
        rm -f "$PID_FILE"
    fi
    
    log "Development environment stopped."
    exit 0
}

# Function to start database
start_database() {
    log "Starting database and RabbitMQ..."
    
    # Check if Docker is running
    if ! docker info >/dev/null 2>&1; then
        log_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
    
    # Start database and RabbitMQ
    docker-compose up -d db rabbitmq
    
    # Wait for services to be ready
    log "Waiting for database to be ready..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if docker-compose exec -T db pg_isready -U movieuser -d movielogger >/dev/null 2>&1; then
            break
        fi
        sleep 1
        timeout=$((timeout - 1))
    done
    
    if [ $timeout -eq 0 ]; then
        log_error "Database failed to start within 60 seconds"
        exit 1
    fi
    
    log "Waiting for RabbitMQ to be ready..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if docker-compose exec -T rabbitmq rabbitmq-diagnostics ping >/dev/null 2>&1; then
            break
        fi
        sleep 1
        timeout=$((timeout - 1))
    done
    
    if [ $timeout -eq 0 ]; then
        log_error "RabbitMQ failed to start within 60 seconds"
        exit 1
    fi
    
    # Run migrations
    log "Running database migrations..."
    docker-compose --profile migrate up flyway
    
    log "Database and RabbitMQ are ready!"
}

# Function to start server
start_server() {
    log "Starting backend server..."
    
    if ! check_port 5049 "backend server"; then
        log_error "Cannot start backend server - port 5049 is in use"
        return 1
    fi
    
    cd "$SERVER_DIR"
    dotnet run --project src/movielogger.api > "$LOG_DIR/server.log" 2>&1 &
    server_pid=$!
    echo "$server_pid Backend Server" >> "$PID_FILE"
    log "Backend server started (PID: $server_pid)"
    
    # Wait for server to be ready
    log "Waiting for backend server to be ready..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if curl -f http://localhost:5049/health >/dev/null 2>&1; then
            break
        fi
        sleep 1
        timeout=$((timeout - 1))
    done
    
    if [ $timeout -eq 0 ]; then
        log_error "Backend server failed to start within 60 seconds"
        return 1
    fi
    
    log "Backend server is ready!"
    return 0
}

# Function to start client
start_client() {
    log "Starting frontend client..."
    
    if ! check_port 5173 "frontend client"; then
        log_error "Cannot start frontend client - port 5173 is in use"
        return 1
    fi
    
    cd "$CLIENT_DIR"
    npm run dev > "$LOG_DIR/client.log" 2>&1 &
    client_pid=$!
    echo "$client_pid Frontend Client" >> "$PID_FILE"
    log "Frontend client started (PID: $client_pid)"
    
    # Wait for client to be ready
    log "Waiting for frontend client to be ready..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if curl -f http://localhost:5173 >/dev/null 2>&1; then
            break
        fi
        sleep 1
        timeout=$((timeout - 1))
    done
    
    if [ $timeout -eq 0 ]; then
        log_warning "Frontend client may not be ready yet"
    else
        log "Frontend client is ready!"
    fi
    
    return 0
}

# Function to show status
show_status() {
    log "Development Environment Status:"
    echo ""
    echo -e "${BLUE}Services:${NC}"
    
    if [ -f "$PID_FILE" ]; then
        while IFS= read -r line; do
            if [ -n "$line" ]; then
                pid=$(echo "$line" | cut -d' ' -f1)
                name=$(echo "$line" | cut -d' ' -f2-)
                if kill -0 "$pid" 2>/dev/null; then
                    echo -e "  ${GREEN}✓${NC} $name (PID: $pid)"
                else
                    echo -e "  ${RED}✗${NC} $name (PID: $pid) - Not running"
                fi
            fi
        done < "$PID_FILE"
    else
        echo -e "  ${YELLOW}No processes tracked${NC}"
    fi
    
    echo ""
    echo -e "${BLUE}Ports:${NC}"
    if lsof -Pi :5432 -sTCP:LISTEN -t >/dev/null 2>&1; then
        echo -e "  ${GREEN}✓${NC} Database (5432)"
    else
        echo -e "  ${RED}✗${NC} Database (5432)"
    fi
    
    if lsof -Pi :5672 -sTCP:LISTEN -t >/dev/null 2>&1; then
        echo -e "  ${GREEN}✓${NC} RabbitMQ (5672)"
    else
        echo -e "  ${RED}✗${NC} RabbitMQ (5672)"
    fi
    
    if lsof -Pi :5049 -sTCP:LISTEN -t >/dev/null 2>&1; then
        echo -e "  ${GREEN}✓${NC} Backend API (5049)"
    else
        echo -e "  ${RED}✗${NC} Backend API (5049)"
    fi
    
    if lsof -Pi :5173 -sTCP:LISTEN -t >/dev/null 2>&1; then
        echo -e "  ${GREEN}✓${NC} Frontend Client (5173)"
    else
        echo -e "  ${RED}✗${NC} Frontend Client (5173)"
    fi
    
    echo ""
    echo -e "${BLUE}URLs:${NC}"
    echo "  Database: http://localhost:5432"
    echo "  RabbitMQ Management: http://localhost:15672"
    echo "  Backend API: http://localhost:5049"
    echo "  Frontend Client: http://localhost:5173"
    echo ""
    echo -e "${BLUE}Logs:${NC}"
    echo "  Backend: $LOG_DIR/server.log"
    echo "  Frontend: $LOG_DIR/client.log"
}

# Function to show logs
show_logs() {
    local service=$1
    
    case $service in
        "server"|"backend")
            if [ -f "$LOG_DIR/server.log" ]; then
                tail -f "$LOG_DIR/server.log"
            else
                log_error "Server log file not found"
            fi
            ;;
        "client"|"frontend")
            if [ -f "$LOG_DIR/client.log" ]; then
                tail -f "$LOG_DIR/client.log"
            else
                log_error "Client log file not found"
            fi
            ;;
        *)
            log_error "Unknown service: $service. Use 'server' or 'client'"
            ;;
    esac
}

# Main script logic
case "${1:-start}" in
    "start")
        # Set up signal handlers
        trap cleanup SIGINT SIGTERM
        
        # Check if already running
        if [ -f "$PID_FILE" ]; then
            log_warning "Development environment may already be running"
            show_status
            read -p "Continue anyway? (y/N): " -n 1 -r
            echo
            if [[ ! $REPLY =~ ^[Yy]$ ]]; then
                exit 1
            fi
        fi
        
        # Start services
        start_database
        start_server
        start_client
        
        log "Development environment started successfully!"
        echo ""
        show_status
        echo ""
        log "Press Ctrl+C to stop all services"
        
        # Keep script running
        while true; do
            sleep 1
        done
        ;;
    
    "stop")
        cleanup
        ;;
    
    "status")
        show_status
        ;;
    
    "logs")
        show_logs "$2"
        ;;
    
    "restart")
        $0 stop
        sleep 2
        $0 start
        ;;
    
    "clean")
        log "Cleaning up development environment..."
        $0 stop
        docker-compose down -v --remove-orphans
        rm -rf "$LOG_DIR"
        log "Cleanup complete!"
        ;;
    
    *)
        echo "Usage: $0 {start|stop|status|logs|restart|clean}"
        echo ""
        echo "Commands:"
        echo "  start   - Start the development environment"
        echo "  stop    - Stop all development services"
        echo "  status  - Show status of all services"
        echo "  logs    - Show logs (server|client)"
        echo "  restart - Restart all services"
        echo "  clean   - Stop and clean up everything"
        exit 1
        ;;
esac 