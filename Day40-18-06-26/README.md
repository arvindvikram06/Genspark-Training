# Day 40: Docker Compose and Orchestration

## Directory Overview

- **`compose-lab`**: Contains a `docker-compose.yml` file configuring a PostgreSQL database (`postgres:16`) and pgAdmin for database management. It demonstrates the use of `.env` files for secrets, healthchecks, and Docker profiles.
- **`compose-postgres-api-lab`**: Contains a full-stack `docker-compose.yml` that orchestrates the `LibraryManagement` API alongside a PostgreSQL database. It demonstrates container dependencies, environment variable injection for connection strings, and custom bridge networks.
- **`docs`**: Contains the lab manual for Docker Compose, PostgreSQL, and .NET API integration (`Docker_Compose_PostgreSQL_DotNet_API_Lab_Manual.pdf` and `Docker_Day3_Session1_Student_Handbook.pdf`).

## Concepts Explored

### 1. Docker Compose
Docker Compose is a tool for defining and running multi-container Docker applications using a single YAML configuration file (`docker-compose.yml`). It simplifies the process of starting up complex, multi-service environments.

### 2. Service Definitions and Dependencies
Compose allows you to define services (e.g., `api`, `postgres`) and specify their relationships. By using `depends_on` with `condition: service_healthy`, you can ensure that the API container only starts *after* the database container is fully initialized and ready to accept connections.

### 3. Environment Variables and Networking
- **Environment Variables**: Compose dynamically passes configuration like database passwords and connection strings into containers at runtime, often utilizing a `.env` file to keep secrets out of source control.
- **Networking**: Compose automatically sets up a custom bridge network for the application. Both the API and Database containers attach to this network, allowing the API to securely connect to the database using its service name (`compose-postgres-db`) as the hostname.

## Key Commands Used

- `docker-compose up -d`
  - **Explanation**: Reads the `docker-compose.yml` file in the current directory, creates the defined networks and volumes, builds images if necessary, and starts all services in the background (detached mode).
- `docker-compose down`
  - **Explanation**: Stops all running containers for the project and removes the containers, networks, and default volumes created by `up`. (Note: Named volumes are *not* deleted by default unless you add the `-v` flag).
- `docker-compose build`
  - **Explanation**: Rebuilds the custom images for services defined in the compose file (like the `api` service) without starting the containers. Useful when you've made changes to the source code or `Dockerfile`.
- `docker-compose logs -f`
  - **Explanation**: Fetches and follows (`-f`) the combined log output from all running services defined in the compose file, which is extremely helpful for debugging inter-service communication.
- `docker-compose ps`
  - **Explanation**: Lists the status and port bindings of all containers managed by the current compose project.
