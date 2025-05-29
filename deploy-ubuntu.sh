#!/bin/bash

# KPI Management System - Ubuntu Deployment Script
# This script installs Docker, Docker Compose, and deploys the KPI application

set -e

echo "=========================================="
echo "KPI Management System - Ubuntu Deployment"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running as root
if [ "$EUID" -ne 0 ]; then
    print_error "Please run this script as root (use sudo)"
    exit 1
fi

# Update system
print_status "Updating Ubuntu system packages..."
apt-get update -y
apt-get upgrade -y

# Install required packages
print_status "Installing required packages..."
apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
    software-properties-common \
    git \
    unzip

# Install Docker
print_status "Installing Docker..."
if ! command -v docker &> /dev/null; then
    # Add Docker's official GPG key
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg

    # Set up the stable repository
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null

    # Install Docker Engine
    apt-get update -y
    apt-get install -y docker-ce docker-ce-cli containerd.io

    # Start and enable Docker
    systemctl start docker
    systemctl enable docker

    print_status "Docker installed successfully"
else
    print_warning "Docker is already installed"
fi

# Install Docker Compose
print_status "Installing Docker Compose..."
if ! command -v docker-compose &> /dev/null; then
    # Download and install Docker Compose
    DOCKER_COMPOSE_VERSION="2.24.0"
    curl -L "https://github.com/docker/compose/releases/download/v${DOCKER_COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    chmod +x /usr/local/bin/docker-compose

    print_status "Docker Compose installed successfully"
else
    print_warning "Docker Compose is already installed"
fi

# Create application directory
APP_DIR="/opt/kpi-app"
print_status "Creating application directory: $APP_DIR"
mkdir -p $APP_DIR

# Function to deploy from Git repository
deploy_from_git() {
    read -p "Enter Git repository URL: " REPO_URL
    if [ -z "$REPO_URL" ]; then
        print_error "Repository URL cannot be empty"
        return 1
    fi

    print_status "Cloning repository from $REPO_URL"
    git clone $REPO_URL $APP_DIR/kpi-source

    if [ -f "$APP_DIR/kpi-source/docker-compose.yml" ]; then
        cd $APP_DIR/kpi-source
        print_status "Found docker-compose.yml, deploying..."
        docker-compose up --build -d
        print_status "Application deployed successfully from Git repository"
    else
        print_error "docker-compose.yml not found in repository"
        return 1
    fi
}

# Function to deploy from uploaded files
deploy_from_files() {
    print_status "Please upload your project files to: $APP_DIR"
    print_status "Make sure your project contains:"
    echo "  - docker-compose.yml"
    echo "  - docker-compose.override.yml (optional)"
    echo "  - Dockerfile"
    echo "  - Source code in src/ directory"
    echo ""
    read -p "Press Enter when files are uploaded and ready to deploy..."

    if [ -f "$APP_DIR/docker-compose.yml" ]; then
        cd $APP_DIR
        print_status "Building and starting containers..."
        docker-compose up --build -d
        print_status "Application deployed successfully"
    else
        print_error "docker-compose.yml not found in $APP_DIR"
        return 1
    fi
}

# Choose deployment method
echo ""
echo "Choose deployment method:"
echo "1) Deploy from Git repository"
echo "2) Deploy from uploaded files"
read -p "Enter your choice (1 or 2): " DEPLOY_METHOD

case $DEPLOY_METHOD in
    1)
        deploy_from_git
        ;;
    2)
        deploy_from_files
        ;;
    *)
        print_error "Invalid choice. Please run the script again."
        exit 1
        ;;
esac

# Configure firewall (optional)
if command -v ufw &> /dev/null; then
    print_status "Configuring firewall..."
    ufw allow 8080/tcp
    ufw allow 5001/tcp
    ufw allow 22/tcp
    print_status "Firewall configured to allow ports 8080, 5001, and 22"
fi

# Show deployment status
print_status "Checking deployment status..."
docker ps

echo ""
echo "=========================================="
print_status "Deployment completed successfully!"
echo "=========================================="
echo ""
echo "Application URLs:"
echo "  - Main application: http://$(hostname -I | awk '{print $1}'):8080"
echo "  - Alternative port: http://$(hostname -I | awk '{print $1}'):5001"
echo ""
echo "Useful commands:"
echo "  - View logs: docker logs kpi-application"
echo "  - Restart app: docker-compose restart"
echo "  - Stop app: docker-compose down"
echo "  - Update app: docker-compose up --build -d"
echo ""
echo "Database connection (from host):"
echo "  - Host: $(hostname -I | awk '{print $1}')"
echo "  - Port: 3307"
echo "  - Username: root"
echo "  - Password: kpipassword"
echo ""
