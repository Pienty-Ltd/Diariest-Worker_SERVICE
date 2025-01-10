@echo off
echo Pienty: Starting build and push process...

:: Get the directory of the batch script
set PROJECT_DIR=%~dp0
set PROJECT_DIR=%PROJECT_DIR:~0,-1%

set API_PROJECT_DIR=%PROJECT_DIR%\Pienty.Diariest.API
set WORKER_PROJECT_DIR=%PROJECT_DIR%\Pienty.Diariest.Worker
set TARGET_API_DIR=%API_PROJECT_DIR%
set TARGET_WORKER_DIR=%WORKER_PROJECT_DIR%
set API_DOCKER_IMAGE_NAME=pienty-diariest-api
set WORKER_DOCKER_IMAGE_NAME=pienty-diariest-worker

:: Docker build (API)
echo Pienty: Building API docker image...
docker build -f "%TARGET_API_DIR%\Dockerfile" --force-rm -t pienty-diariest-api  "%PROJECT_DIR%" --build-arg target=Release

:: Docker tag (API)
echo Pienty: Tagging API docker image...
docker tag pienty-diariest-api:latest 185.255.92.94:5000/pienty-diariest-api:latest

:: Docker build (Worker)
echo Pienty: Building Worker docker image...
docker build -f "%TARGET_WORKER_DIR%\Dockerfile" --force-rm -t pienty-diariest-worker  "%PROJECT_DIR%" --build-arg target=Release

:: Docker tag (Worker)
echo Pienty: Tagging Worker docker image...
docker tag pienty-diariest-worker:latest 185.255.92.94:5000/pienty-diariest-worker

:: Docker push (API)
echo Pienty: Pushing API docker image...
docker push 185.255.92.94:5000/pienty-diariest-api

:: Docker push (Worker)
echo Pienty: Pushing Worker docker image...
docker push 185.255.92.94:5000/pienty-diariest-worker

:: Remote deploy
echo Pienty: Deploying to VDS...
call plink 185.255.92.94 -batch -l root -pw jr9uk78QH4 ./publish-diariest.sh

echo Pienty: Docker build and push completed.
pause