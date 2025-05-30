#!/bin/bash

echo "Testing KPI Application Docker Deployment"
echo "=========================================="

# Clean up existing containers and volumes
echo "Cleaning up existing containers..."
docker-compose down -v --remove-orphans

# Remove old images to force rebuild
echo "Removing old images..."
docker rmi kpi-kpi-app:latest 2>/dev/null || true

# Build and start services
echo "Building and starting services..."
docker-compose up --build -d

# Wait for services to start
echo "Waiting for services to start..."
sleep 10

# Check if MySQL is healthy
echo "Checking MySQL health..."
docker-compose ps mysql

# Check if app is running
echo "Checking application status..."
docker-compose ps kpi-app

# Show logs for debugging
echo "Application logs (last 50 lines):"
echo "================================="
docker-compose logs --tail=50 kpi-app

echo ""
echo "MySQL logs (last 20 lines):"
echo "============================"
docker-compose logs --tail=20 mysql

echo ""
echo "Testing application endpoint..."
echo "=============================="
sleep 5
curl -f http://localhost:8080/health 2>/dev/null && echo "Health check: OK" || echo "Health check: FAILED"

echo ""
echo "Test complete. Check the logs above for any errors."
echo "Application should be available at: http://localhost:8080"
