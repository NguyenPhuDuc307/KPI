#!/bin/bash

# Script to rebuild and test the KPI application deployment
# This script will clean up, rebuild, and test the Docker containers

set -e  # Exit on any error

echo "🧹 Cleaning up existing containers and volumes..."

# Stop and remove containers
docker-compose down --volumes --remove-orphans

# Remove existing images
echo "🗑️ Removing existing Docker images..."
docker rmi kpi-kpi-app:latest 2>/dev/null || echo "No kpi-app image to remove"
docker rmi mysql:8.0 2>/dev/null || echo "MySQL image retained"

# Clean up any orphaned volumes
echo "🧽 Cleaning up Docker volumes..."
docker volume prune -f

echo "🔨 Building and starting fresh containers..."

# Build and start the containers
docker-compose up --build -d

echo "⏳ Waiting for services to be ready..."

# Wait for MySQL to be ready
echo "Waiting for MySQL to be ready..."
sleep 10

# Check MySQL health
until docker exec kpi-mysql mysqladmin ping -h localhost --silent; do
    echo "MySQL is not ready yet, waiting..."
    sleep 5
done

echo "✅ MySQL is ready!"

# Wait a bit more for the application to start
sleep 15

echo "🔍 Checking application logs..."

# Show logs for debugging
echo "=== MYSQL LOGS ==="
docker logs kpi-mysql --tail=20

echo "=== APPLICATION LOGS ==="
docker logs kpi-application --tail=30

echo "🌐 Testing application..."

# Test if the application is responding
if curl -f http://localhost:8080 > /dev/null 2>&1; then
    echo "✅ Application is responding on http://localhost:8080"
else
    echo "❌ Application is not responding, checking detailed logs..."
    echo "=== DETAILED APPLICATION LOGS ==="
    docker logs kpi-application
    exit 1
fi

echo "🎉 Deployment test completed successfully!"
echo "📊 You can access the application at: http://localhost:8080"
echo "🗄️ MySQL is accessible at: localhost:3307"
