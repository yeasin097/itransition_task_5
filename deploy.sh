#!/bin/bash

# deploy.sh - Simplified deployment script for Task5 on a Linux server (Ubuntu 20.04) for intern review

# Exit on any error
set -e

# Variables
APP_NAME="Task5"
APP_DIR="/home/$USER/task5"
PUBLISH_DIR="$APP_DIR/publish"
SERVER_IP=$(curl -s ifconfig.me)  # Automatically detect the server's public IP
APP_PORT=5166  # Port for the application

# Log function for better output
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1"
}

# Step 1: Update the system and install prerequisites
log "Updating system and installing prerequisites..."
sudo apt-get update
sudo apt-get install -y wget curl apt-transport-https software-properties-common

# Step 2: Install .NET 8 SDK
log "Installing .NET 8 SDK..."
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
dotnet --version

# Step 3: Publish the application
log "Publishing the application..."
cd "$(dirname "$0")"  # Navigate to the script's directory (repo root)
dotnet publish -c Release -o "$PUBLISH_DIR"

# Step 4: Open firewall port for Kestrel
log "Opening firewall port $APP_PORT..."
sudo ufw allow $APP_PORT/tcp
sudo ufw status

# Step 5: Final instructions
log "Setup complete!"
log "To run the application, execute the following commands:"
log "  cd $PUBLISH_DIR"
log "  ASPNETCORE_URLS=http://+:$APP_PORT dotnet Task5.dll"
log "Then access the app at http://$SERVER_IP:$APP_PORT"
log "To stop the app, press Ctrl+C in the terminal where it's running."