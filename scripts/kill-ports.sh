#!/bin/bash

# MovieLogger Port Killer Script
# Kills processes running on common development ports

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Common development ports
PORTS=(5432 5672 15672 5049 5173 80 3000 8080)

log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1"
}

kill_port() {
    local port=$1
    local pids=$(lsof -ti:$port 2>/dev/null)
    
    if [ -n "$pids" ]; then
        log "Killing processes on port $port (PIDs: $pids)"
        echo "$pids" | xargs kill -9 2>/dev/null || true
        sleep 1
        
        # Verify port is free
        if lsof -ti:$port >/dev/null 2>&1; then
            log_warning "Port $port is still in use after kill attempt"
        else
            log "Port $port is now free"
        fi
    else
        log "Port $port is already free"
    fi
}

# Main script
echo "MovieLogger Port Killer"
echo "======================"
echo ""

if [ "$1" = "--all" ]; then
    log "Killing processes on all common development ports..."
    for port in "${PORTS[@]}"; do
        kill_port "$port"
    done
    log "Port cleanup complete!"
elif [ -n "$1" ]; then
    # Kill specific port
    if [[ "$1" =~ ^[0-9]+$ ]]; then
        kill_port "$1"
    else
        echo "Usage: $0 [port_number|--all]"
        echo ""
        echo "Examples:"
        echo "  $0 5049    - Kill processes on port 5049"
        echo "  $0 --all   - Kill processes on all common dev ports"
        echo ""
        echo "Common ports: ${PORTS[*]}"
        exit 1
    fi
else
    # Interactive mode
    echo "Select ports to kill:"
    echo ""
    for i in "${!PORTS[@]}"; do
        port=${PORTS[$i]}
        if lsof -ti:$port >/dev/null 2>&1; then
            pids=$(lsof -ti:$port | tr '\n' ' ')
            echo "  $((i+1)). Port $port (IN USE - PIDs: $pids)"
        else
            echo "  $((i+1)). Port $port (free)"
        fi
    done
    echo "  a. Kill all ports"
    echo "  q. Quit"
    echo ""
    read -p "Enter your choice: " choice
    
    case $choice in
        [1-9])
            if [ "$choice" -le "${#PORTS[@]}" ]; then
                port=${PORTS[$((choice-1))]}
                kill_port "$port"
            else
                log_warning "Invalid choice"
            fi
            ;;
        "a"|"A")
            log "Killing processes on all common development ports..."
            for port in "${PORTS[@]}"; do
                kill_port "$port"
            done
            log "Port cleanup complete!"
            ;;
        "q"|"Q")
            log "Exiting..."
            exit 0
            ;;
        *)
            log_warning "Invalid choice"
            ;;
    esac
fi 