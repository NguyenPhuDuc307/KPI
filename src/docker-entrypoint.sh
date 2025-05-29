#!/bin/bash
set -e

echo "Starting KPI Application..."

# Wait for MySQL to be ready
echo "Waiting for MySQL to be ready..."
until nc -z mysql 3306; do
  echo "MySQL is unavailable - sleeping"
  sleep 1
done

echo "MySQL is up - starting application (migrations will be handled automatically)"

# Start application - migrations should be handled in Program.cs
exec dotnet KPISolution.dll
