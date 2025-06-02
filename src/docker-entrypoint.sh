#!/bin/bash
set -e

echo "Starting KPI Application..."

# Function to test MySQL connection
test_mysql_connection() {
    timeout 30s mysql -h mysql -P 3307 -u kpiuser -pkpipassword -e "SELECT 1;" kpi > /dev/null 2>&1
}

# Wait for MySQL to be ready with more robust checking
echo "Waiting for MySQL server to be ready..."
until nc -z mysql 3307; do
  echo "MySQL port is not open - sleeping 2 seconds"
  sleep 2
done

echo "MySQL port is open. Waiting for database to accept connections..."
RETRY_COUNT=0
MAX_RETRIES=30

while ! test_mysql_connection && [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    RETRY_COUNT=$((RETRY_COUNT + 1))
    echo "MySQL is not ready for connections (attempt $RETRY_COUNT/$MAX_RETRIES) - sleeping 2 seconds"
    sleep 2
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "ERROR: MySQL failed to become ready after $MAX_RETRIES attempts"
    exit 1
fi

echo "MySQL is ready! Starting application..."
echo "Note: Database migrations will be handled automatically by the application"

# Start application - migrations are handled in Program.cs
exec dotnet KPISolution.dll
