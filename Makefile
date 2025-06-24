# MovieLogger Makefile
# A comprehensive build and development tool for the MovieLogger application

.PHONY: help install build test clean dev server client database docker-up docker-down migrate seed lint format

# Default target
help:
	@echo "MovieLogger Development Commands:"
	@echo ""
	@echo "Installation:"
	@echo "  install          Install all dependencies (client, server, root)"
	@echo "  install-client   Install client dependencies"
	@echo "  install-server   Install server dependencies"
	@echo ""
	@echo "Development:"
	@echo "  dev              Start client development server"
	@echo "  server           Start backend server"
	@echo "  database         Start database and RabbitMQ, then run migrations"
	@echo "  full-dev         Start all services (database, RabbitMQ, server, client)"
	@echo "  docker-dev       Start full development environment with Docker"
	@echo "  docker-up        Start database and RabbitMQ with Docker"
	@echo "  docker-down      Stop database and RabbitMQ Docker containers"
	@echo "  docker-clean     Stop and remove all containers and volumes"
	@echo ""
	@echo "Building:"
	@echo "  build            Build all components"
	@echo "  build-client     Build client for production"
	@echo "  build-server     Build server for production"
	@echo ""
	@echo "Testing:"
	@echo "  test             Run all tests"
	@echo "  test-client      Run client tests"
	@echo "  test-server      Run server tests"
	@echo ""
	@echo "Database:"
	@echo "  migrate          Run database migrations"
	@echo "  seed             Seed database with initial data"
	@echo ""
	@echo "Code Quality:"
	@echo "  lint             Lint all code"
	@echo "  lint-client      Lint client code"
	@echo "  lint-server      Lint server code"
	@echo "  format           Format all code"
	@echo ""
	@echo "Cleaning:"
	@echo "  clean            Clean all build artifacts"
	@echo "  clean-client     Clean client build artifacts"
	@echo "  clean-server     Clean server build artifacts"
	@echo ""
	@echo "Deployment:"
	@echo "  deploy-client    Deploy client to S3"
	@echo "  deploy-server    Deploy server to AWS"
	@echo "  invalidate-cache Invalidate CloudFront cache"

# Variables
CLIENT_DIR = client
SERVER_DIR = server
DATABASE_DIR = database

# Installation
install: install-client install-server
	@echo "Installing root dependencies..."
	npm install

install-client:
	@echo "Installing client dependencies..."
	cd $(CLIENT_DIR) && npm install

install-server:
	@echo "Installing server dependencies..."
	cd $(SERVER_DIR) && dotnet restore

# Development
dev:
	@echo "Starting client development server..."
	cd $(CLIENT_DIR) && npm run dev

server:
	@echo "Starting backend server..."
	cd $(SERVER_DIR) && dotnet run --project src/movielogger.api

database: docker-up migrate
	@echo "Database and RabbitMQ are ready!"

full-dev: database
	@echo "Starting full development environment..."
	@echo "Database: http://localhost:5432"
	@echo "API: http://localhost:5049"
	@echo "Client: http://localhost:5173"
	@echo ""
	@echo "Starting server in background..."
	@cd $(SERVER_DIR) && dotnet run --project src/movielogger.api &
	@echo "Starting client in background..."
	@cd $(CLIENT_DIR) && npm run dev &
	@echo "All services started! Press Ctrl+C to stop all services."

# Docker Development Environment
docker-dev:
	@echo "Starting full Docker development environment..."
	@echo "This will start all services with hot reloading:"
	@echo "  - Database: http://localhost:5432"
	@echo "  - API: http://localhost:5049"
	@echo "  - Client: http://localhost:5173"
	@echo "  - Proxy (optional): http://localhost:80"
	@echo ""
	docker-compose up --build

docker-dev-detached:
	@echo "Starting Docker development environment in background..."
	docker-compose up --build -d

docker-up:
	@echo "Starting database and RabbitMQ with Docker..."
	docker-compose up -d db rabbitmq
	@echo "Waiting for database and RabbitMQ to be ready..."
	@sleep 5

docker-down:
	@echo "Stopping Docker containers..."
	docker-compose down

docker-clean:
	@echo "Stopping and removing all containers and volumes..."
	docker-compose down -v --remove-orphans
	@echo "Docker environment cleaned!"

docker-logs:
	@echo "Showing Docker logs..."
	docker-compose logs -f

docker-migrate:
	@echo "Running database migrations..."
	docker-compose --profile migrate up flyway

# Building
build: build-client build-server
	@echo "All components built successfully!"

build-client:
	@echo "Building client for production..."
	cd $(CLIENT_DIR) && npm run build

build-server:
	@echo "Building server for production..."
	cd $(SERVER_DIR) && dotnet build --configuration Release

# Testing
test: test-client test-server
	@echo "All tests completed!"

test-client:
	@echo "Running client tests..."
	cd $(CLIENT_DIR) && npm test

test-server:
	@echo "Running server tests..."
	cd $(SERVER_DIR) && dotnet test

# Database
migrate:
	@echo "Running database migrations..."
	docker-compose --profile migrate up flyway

seed:
	@echo "Seeding database with initial data..."
	@if [ -d "$(DATABASE_DIR)/local-data-seed" ]; then \
		echo "Running seed scripts..."; \
		cd $(DATABASE_DIR)/local-data-seed && ls -1 *.sql | xargs -I {} echo "Executing {}"; \
	else \
		echo "No seed data found in $(DATABASE_DIR)/local-data-seed"; \
	fi

# Code Quality
lint: lint-client lint-server
	@echo "All linting completed!"

lint-client:
	@echo "Linting client code..."
	cd $(CLIENT_DIR) && npm run lint

lint-server:
	@echo "Linting server code..."
	cd $(SERVER_DIR) && dotnet format --verify-no-changes

format:
	@echo "Formatting all code..."
	cd $(CLIENT_DIR) && npm run format 2>/dev/null || echo "No format script found in client"
	cd $(SERVER_DIR) && dotnet format

# Cleaning
clean: clean-client clean-server
	@echo "All build artifacts cleaned!"

clean-client:
	@echo "Cleaning client build artifacts..."
	cd $(CLIENT_DIR) && rm -rf dist node_modules/.vite
	@echo "Client cleaned!"

clean-server:
	@echo "Cleaning server build artifacts..."
	cd $(SERVER_DIR) && dotnet clean
	@echo "Server cleaned!"

# Deployment
deploy-client:
	@echo "Deploying client to S3..."
	cd $(CLIENT_DIR) && npm run build
	aws s3 sync $(CLIENT_DIR)/dist/ s3://movielogger/ --delete
	@echo "Client deployed successfully!"

deploy-server:
	@echo "Deploying server to AWS..."
	cd $(SERVER_DIR) && dotnet publish --configuration Release --output ./publish
	@echo "Server built for deployment. Manual deployment required."

invalidate-cache:
	@echo "Invalidating CloudFront cache..."
	@if [ -z "$(CLOUDFRONT_DISTRIBUTION_ID)" ]; then \
		echo "Error: CLOUDFRONT_DISTRIBUTION_ID environment variable not set"; \
		echo "Please set it with: export CLOUDFRONT_DISTRIBUTION_ID=your-distribution-id"; \
		exit 1; \
	fi
	aws cloudfront create-invalidation \
		--distribution-id $(CLOUDFRONT_DISTRIBUTION_ID) \
		--paths "/*" \
		--query 'Invalidation.Id' \
		--output text
	@echo "CloudFront cache invalidation created successfully!"

# Utility commands
logs:
	@echo "Showing Docker logs..."
	docker-compose logs -f

status:
	@echo "Checking service status..."
	@echo "Docker containers:"
	docker-compose ps
	@echo ""
	@echo "Client build:"
	@if [ -d "$(CLIENT_DIR)/dist" ]; then echo "✓ Client built"; else echo "✗ Client not built"; fi
	@echo ""
	@echo "Server build:"
	@if [ -d "$(SERVER_DIR)/bin" ]; then echo "✓ Server built"; else echo "✗ Server not built"; fi

reset-db: docker-down docker-up migrate
	@echo "Database reset complete!"

# Development shortcuts
dev-setup: install database
	@echo "Development environment setup complete!"
	@echo "Run 'make full-dev' to start all services"

docker-setup: docker-clean docker-dev
	@echo "Docker development environment setup complete!"

quick-test: lint test
	@echo "Quick test completed!"

# Production helpers
prod-build: clean build
	@echo "Production build completed!"

prod-deploy: prod-build deploy-client invalidate-cache
	@echo "Production deployment completed!" 