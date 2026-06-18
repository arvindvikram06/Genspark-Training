# Day 38: Introduction to Docker

## Directory Overview

- **`docker-training-/WeatherApi`**: Contains a starter `.NET` project named `WeatherApi`. This project serves as a practical example for creating basic Docker images.
- **`docs`**: Includes introductory lab handouts (`Docker_Day1_Session1_Lab_Handout.pdf` and `Docker_Day1_Session2_Student_Handbook.pdf`).

## Concepts Explored

### 1. Docker Images
An **image** is a lightweight, standalone, executable package of software that includes everything needed to run an application—code, runtime, system tools, libraries, and settings. In this lab, we explored how to package the `WeatherApi` into a reusable Docker image.

### 2. Docker Containers
A **container** is a running instance of an image. It isolates the application from the underlying host system, ensuring consistency across environments. The focus was on starting, stopping, and managing these containers using the Docker CLI.

## Key Commands Used

- `docker build -t weatherapi:latest .`
  - **Explanation**: Builds a new Docker image from the `Dockerfile` in the current directory (`.`) and tags (`-t`) it with the name `weatherapi:latest`.
- `docker run -d -p 8080:80 weatherapi:latest`
  - **Explanation**: Runs the `weatherapi:latest` image as a container. `-d` runs it in detached mode (in the background). `-p 8080:80` maps port 80 inside the container to port 8080 on your host machine.
- `docker ps`
  - **Explanation**: Lists all currently running containers, showing their Container ID, image name, and port mappings.
- `docker ps -a`
  - **Explanation**: Lists all containers (both running and stopped).
- `docker stop <container_id>`
  - **Explanation**: Gracefully stops a running container.
- `docker rm <container_id>`
  - **Explanation**: Removes a stopped container, freeing up system resources.
- `docker images`
  - **Explanation**: Lists all Docker images currently stored locally on your machine.
