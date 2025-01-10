#!/bin/bash

# Değişkenler
REGISTRY="185.255.92.94:5000"
API_IMAGE_NAME="${REGISTRY}/pienty-diariest-api"
WORKER_IMAGE_NAME="${REGISTRY}/pienty-diariest-worker"
API_CONTAINER_NAME="pienty-diariest-api-container"
WORKER_CONTAINER_NAME="pienty-diariest-worker-container"
LOG_PREFIX="Pienty-VDS:"

# Fonksiyon: Hata durumunda çıkış
error_exit() {
  echo "$LOG_PREFIX Error: $1" >&2
  exit 1
}

# Mevcut konteynerleri durdur
echo "$LOG_PREFIX Stopping existing containers..."
docker stop "$API_CONTAINER_NAME" 2>/dev/null
docker stop "$WORKER_CONTAINER_NAME" 2>/dev/null

# Mevcut konteynerleri kaldır
echo "$LOG_PREFIX Removing existing containers..."
docker rm "$API_CONTAINER_NAME" 2>/dev/null
docker rm "$WORKER_CONTAINER_NAME" 2>/dev/null

# İmajları çek
echo "$LOG_PREFIX Pulling Docker images..."
sleep 5
docker pull "$API_IMAGE_NAME" || error_exit "Failed to pull API image."
docker pull "$WORKER_IMAGE_NAME" || error_exit "Failed to pull Worker image."

echo "$LOG_PREFIX Verifying API image..."
docker images | findstr %API_DOCKER_IMAGE_NAME%
echo "$LOG_PREFIX Verifying Worker image..."
docker images | findstr %WORKER_DOCKER_IMAGE_NAME%

# API konteynerini başlat
echo "$LOG_PREFIX Starting API container..."
docker run -d --name "$API_CONTAINER_NAME" -p 80:80 "$API_IMAGE_NAME" || error_exit "Failed to start API container."

# Worker konteynerini başlat
echo "$LOG_PREFIX Starting Worker container..."
docker run -d --name "$WORKER_CONTAINER_NAME" "$WORKER_IMAGE_NAME" || error_exit "Failed to start Worker container."

echo "$LOG_PREFIX Containers started successfully."