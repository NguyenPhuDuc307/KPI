# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY src/KPISolution.csproj src/
RUN dotnet restore src/KPISolution.csproj

# Install Entity Framework tools
RUN dotnet tool install --global dotnet-ef

# Copy source code
COPY src/ src/

# Build application
WORKDIR /src/src
RUN dotnet build KPISolution.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish KPISolution.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install wkhtmltopdf for DinkToPdf, netcat for database wait, and mysql-client for connection testing
RUN apt-get update && apt-get install -y \
    wkhtmltopdf \
    xvfb \
    netcat-openbsd \
    default-mysql-client \
    && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Copy startup script
COPY src/docker-entrypoint.sh .
RUN chmod +x docker-entrypoint.sh

# Create logs directory
RUN mkdir -p logs

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["./docker-entrypoint.sh"]
