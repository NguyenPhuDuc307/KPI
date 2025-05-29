# KPI Management System - Ubuntu Deployment Guide

## Overview
This guide provides step-by-step instructions for deploying the KPI Management System on Ubuntu Server using Docker containers.

## System Requirements

### Minimum Requirements
- Ubuntu 20.04 LTS or newer
- 2 GB RAM
- 20 GB free disk space
- Internet connection for downloading Docker images

### Recommended Requirements
- Ubuntu 22.04 LTS
- 4 GB RAM or more
- 50 GB free disk space
- SSD storage for better performance

## Prerequisites

### 1. Ubuntu Server Setup
```bash
# Update system packages
sudo apt update && sudo apt upgrade -y

# Install basic utilities
sudo apt install -y curl wget git unzip
```

### 2. User Setup (Optional but Recommended)
```bash
# Create a dedicated user for the application
sudo adduser kpiapp
sudo usermod -aG sudo kpiapp
sudo usermod -aG docker kpiapp

# Switch to the new user
su - kpiapp
```

## Deployment Methods

### Method 1: Automated Deployment (Recommended)

1. **Download the deployment script:**
```bash
wget https://your-repo/deploy-ubuntu.sh
chmod +x deploy-ubuntu.sh
```

2. **Run the deployment script:**
```bash
sudo ./deploy-ubuntu.sh
```

3. **Follow the interactive prompts** to choose between Git repository deployment or file upload deployment.

### Method 2: Manual Deployment

#### Step 1: Install Docker
```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Start and enable Docker
sudo systemctl start docker
sudo systemctl enable docker

# Add current user to docker group
sudo usermod -aG docker $USER

# Log out and log back in, or run:
newgrp docker
```

#### Step 2: Install Docker Compose
```bash
# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker --version
docker-compose --version
```

#### Step 3: Deploy the Application

**Option A: From Git Repository**
```bash
# Clone the repository
git clone <your-repo-url> /opt/kpi-app
cd /opt/kpi-app

# Build and start containers
docker-compose up --build -d
```

**Option B: From Project Files**
```bash
# Create application directory
sudo mkdir -p /opt/kpi-app
cd /opt/kpi-app

# Upload your project files here (docker-compose.yml, Dockerfile, src/, etc.)
# Then run:
docker-compose up --build -d
```

## Configuration

### Environment Variables
Create a `.env` file in the project root with the following variables:

```env
# Database Configuration
MYSQL_ROOT_PASSWORD=kpipassword
MYSQL_DATABASE=kpidb
MYSQL_USER=kpiuser
MYSQL_PASSWORD=kpipassword

# Application Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft.AspNetCore=Warning
```

### Firewall Configuration
```bash
# Enable firewall
sudo ufw enable

# Allow application ports
sudo ufw allow 8080/tcp
sudo ufw allow 5001/tcp
sudo ufw allow 22/tcp

# Check firewall status
sudo ufw status
```

### SSL/HTTPS Setup (Optional)
```bash
# Install Nginx for reverse proxy
sudo apt install -y nginx

# Install Certbot for Let's Encrypt certificates
sudo apt install -y certbot python3-certbot-nginx

# Get SSL certificate
sudo certbot --nginx -d yourdomain.com

# Configure Nginx (create /etc/nginx/sites-available/kpi-app)
```

Example Nginx configuration:
```nginx
server {
    listen 80;
    server_name yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl;
    server_name yourdomain.com;

    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;

    location / {
        proxy_pass http://localhost:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## Application Access

### Default URLs
- **Main Application**: `http://server-ip:8080`
- **Alternative Port**: `http://server-ip:5001`
- **With Domain**: `https://yourdomain.com` (if SSL configured)

### Default Login
The application will create default admin user on first run. Check the application logs for credentials:
```bash
docker logs kpi-application | grep -i "admin\|password\|user"
```

## Database Access

### Connection Details
- **Host**: `server-ip`
- **Port**: `3307`
- **Database**: `kpidb`
- **Username**: `root`
- **Password**: `kpipassword`

### External Database Tools
You can connect using tools like MySQL Workbench, phpMyAdmin, or command line:
```bash
mysql -h server-ip -P 3307 -u root -pkpipassword kpidb
```

## Management Commands

### Application Management
```bash
# View running containers
docker ps

# View application logs
docker logs kpi-application

# View database logs
docker logs kpi-mysql

# Restart application
docker-compose restart kpi-app

# Stop application
docker-compose down

# Update application
git pull  # if using git
docker-compose up --build -d

# Clean up unused Docker resources
docker system prune -f
```

### Database Management
```bash
# Backup database
docker exec kpi-mysql mysqldump -u root -pkpipassword kpidb > backup.sql

# Restore database
docker exec -i kpi-mysql mysql -u root -pkpipassword kpidb < backup.sql

# Access MySQL shell
docker exec -it kpi-mysql mysql -u root -pkpipassword kpidb
```

### Monitoring
```bash
# Monitor resource usage
docker stats

# Monitor disk usage
df -h

# Monitor logs in real-time
docker logs -f kpi-application
```

## Troubleshooting

### Common Issues

#### 1. Container Won't Start
```bash
# Check container logs
docker logs kpi-application

# Check if ports are already in use
sudo netstat -tulpn | grep :8080
sudo netstat -tulpn | grep :3307

# Restart Docker service
sudo systemctl restart docker
```

#### 2. Database Connection Issues
```bash
# Check if MySQL container is healthy
docker ps

# Test database connection
docker exec kpi-mysql mysql -u root -pkpipassword -e "SELECT 1;"

# Check database logs
docker logs kpi-mysql
```

#### 3. Permission Issues
```bash
# Fix file permissions
sudo chown -R $USER:$USER /opt/kpi-app
chmod +x /opt/kpi-app/deploy-ubuntu.sh
```

#### 4. Out of Disk Space
```bash
# Clean Docker system
docker system prune -af

# Remove old images
docker image prune -af

# Check disk usage
df -h
du -sh /var/lib/docker
```

### Performance Tuning

#### Docker Resource Limits
Add to `docker-compose.yml`:
```yaml
services:
  kpi-app:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '0.5'
        reservations:
          memory: 512M
          cpus: '0.25'
```

#### MySQL Optimization
Add to MySQL service in `docker-compose.yml`:
```yaml
  mysql:
    command: --innodb-buffer-pool-size=512M --max-connections=200
```

## Security Recommendations

1. **Change Default Passwords**
   - Update MySQL root password
   - Change application admin password

2. **Network Security**
   - Use firewall to restrict access
   - Consider VPN for database access
   - Use SSL/HTTPS for web access

3. **Regular Updates**
   - Keep Ubuntu system updated
   - Update Docker and Docker Compose
   - Update application regularly

4. **Backup Strategy**
   - Schedule regular database backups
   - Backup application configuration
   - Test restore procedures

## Maintenance

### Regular Tasks
```bash
# Weekly cleanup
docker system prune -f

# Monthly security updates
sudo apt update && sudo apt upgrade -y

# Database backup (schedule with cron)
0 2 * * * docker exec kpi-mysql mysqldump -u root -pkpipassword kpidb > /backup/kpi-$(date +\%Y\%m\%d).sql
```

### Log Rotation
```bash
# Configure log rotation for Docker
sudo tee /etc/logrotate.d/docker > /dev/null <<EOF
/var/lib/docker/containers/*/*.log {
    rotate 7
    daily
    compress
    size=100M
    missingok
    delaycompress
    copytruncate
}
EOF
```

## Support

### Log Collection for Support
```bash
# Collect system information
uname -a > system-info.txt
docker version >> system-info.txt
docker-compose version >> system-info.txt

# Collect application logs
docker logs kpi-application > app-logs.txt
docker logs kpi-mysql > db-logs.txt

# Collect container information
docker ps -a > containers.txt
docker images > images.txt
```

### Performance Monitoring
```bash
# Monitor resource usage
top
htop
iotop

# Monitor Docker containers
docker stats --no-stream

# Check application health
curl -f http://localhost:8080/health || echo "Health check failed"
```

---

For additional support or questions, please refer to the project documentation or contact the development team.
