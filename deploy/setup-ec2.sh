#!/usr/bin/env bash
# ==============================================================================
# AWS EC2 Setup Script: ASP.NET Core 8.0 + Nginx + MySQL on Ubuntu 22.04/24.04 LTS
# ==============================================================================

set -euo pipefail

echo "=========================================================="
echo "Starting AWS EC2 Server Bootstrap for ASP.NET Core MVC"
echo "=========================================================="

# 1. Update system packages
sudo apt-get update -y
sudo apt-get upgrade -y
sudo apt-get install -y wget curl git ufw nginx software-properties-common

# 2. Install Microsoft Package Repository & .NET 8 ASP.NET Core Runtime
echo "Installing .NET 8 Runtime..."
if ! command -v dotnet &> /dev/null; then
    # Add Microsoft package feed
    declare repo_version
    repo_version=$(lsb_release -rs)
    wget https://packages.microsoft.com/config/ubuntu/"$repo_version"/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    sudo apt-get update
    sudo apt-get install -y aspnetcore-runtime-8.0
fi

echo ".NET Runtime installed: $(dotnet --info | grep 'Version:' | head -1)"

# 3. Install MySQL Server (if using local MySQL on EC2 instead of AWS RDS)
echo "Installing MySQL Server..."
sudo apt-get install -y mysql-server
sudo systemctl enable mysql
sudo systemctl start mysql

# Create database and user with dynamically generated secure password
DB_NAME="devops_deployments"
DB_USER="dotnetuser"

# Generate a cryptographically secure random password dynamically on the server
# NEVER hardcode or commit database credentials in source files
DB_PASS=$(openssl rand -base64 24 | tr -dc 'a-zA-Z0-9' | head -c 24)

sudo mysql -e "CREATE DATABASE IF NOT EXISTS ${DB_NAME} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
sudo mysql -e "CREATE USER IF NOT EXISTS '${DB_USER}'@'localhost' IDENTIFIED BY '${DB_PASS}';"
sudo mysql -e "ALTER USER '${DB_USER}'@'localhost' IDENTIFIED BY '${DB_PASS}';"
sudo mysql -e "GRANT ALL PRIVILEGES ON ${DB_NAME}.* TO '${DB_USER}'@'localhost';"
sudo mysql -e "FLUSH PRIVILEGES;"
echo "MySQL configured: database '${DB_NAME}' and user '${DB_USER}' ready."

# 4. Create secure server-only environment file for secrets
echo "Configuring /etc/dotnetapp.env for secure database credentials..."
if [ ! -f /etc/dotnetapp.env ]; then
    sudo bash -c "cat <<EOF > /etc/dotnetapp.env
# Generated dynamically on server — NEVER committed to source control
ConnectionStrings__DefaultConnection=\"Server=localhost;Port=3306;Database=${DB_NAME};User=${DB_USER};Password=${DB_PASS};TreatTinyAsBoolean=true;\"
ASPNETCORE_ENVIRONMENT=Production
EOF"
    sudo chmod 600 /etc/dotnetapp.env
    sudo chown www-data:www-data /etc/dotnetapp.env
    echo "/etc/dotnetapp.env created with restricted 600 permissions."
fi

# 5. Create App Directories and set permissions
echo "Setting up application directories /var/www/dotnetapp and /var/www/dotnetapi..."
sudo mkdir -p /var/www/dotnetapp /var/www/dotnetapi
sudo chown -R www-data:www-data /var/www/dotnetapp /var/www/dotnetapi
sudo chmod -R 755 /var/www/dotnetapp /var/www/dotnetapi

# Allow deployment user (e.g., ubuntu) to write to app directories
CURRENT_USER=$(whoami)
sudo usermod -a -G www-data "$CURRENT_USER"
sudo chmod -R g+w /var/www/dotnetapp /var/www/dotnetapi

# 6. Configure systemd services (Web & API)
echo "Configuring systemd services..."
if [ -f "./app.service" ]; then
    sudo cp ./app.service /etc/systemd/system/dotnetapp.service
    sudo systemctl daemon-reload
    sudo systemctl enable dotnetapp.service
    echo "dotnetapp.service installed and enabled."
fi

if [ -f "./api.service" ]; then
    sudo cp ./api.service /etc/systemd/system/dotnetapi.service
    sudo systemctl daemon-reload
    sudo systemctl enable dotnetapi.service
    echo "dotnetapi.service installed and enabled."
fi

# 7. Configure Nginx Reverse Proxy
echo "Configuring Nginx reverse proxy..."
if [ -f "./nginx.conf" ]; then
    sudo cp ./nginx.conf /etc/nginx/sites-available/dotnetapp
    sudo ln -sf /etc/nginx/sites-available/dotnetapp /etc/nginx/sites-enabled/dotnetapp
    sudo rm -f /etc/nginx/sites-enabled/default
    sudo nginx -t
    sudo systemctl restart nginx
    echo "Nginx configured and restarted."
fi

# 8. Configure Firewall (UFW)
echo "Configuring firewall..."
sudo ufw allow 'OpenSSH'
sudo ufw allow 'Nginx Full'
# sudo ufw --force enable

echo "=========================================================="
echo "EC2 Setup Completed Successfully!"
echo "Web App Directory : /var/www/dotnetapp (:5000)"
echo "REST API Directory: /var/www/dotnetapi (:5050)"
echo "Services          : dotnetapp.service, dotnetapi.service"
echo "Status checks     : sudo systemctl status dotnetapp dotnetapi"
echo "Log checks        : sudo journalctl -u dotnetapp -u dotnetapi -f"
echo "=========================================================="
