```markdown name=README.md
# SharedDriveAccess

SharedDriveAccess is a small .NET 8 console application that mounts/reads files from a network (CIFS) share and copies them into a local directory. It's intended as a lightweight utility to move files from a mounted network drive into a local folder (for processing, backups, testing, etc.). The repository includes a Dockerfile and a docker-compose configuration to run the app in a container with the network share mounted.

## Stack
- Language: C# (.NET 8)
- Runtime: .NET 8 console application
- Notable files/libraries:
  - SharedDriveAccess/Program.cs — main program that finds files in the network mount and copies them to the local volume
  - SharedDriveAccess/SharedDriveAccess.csproj — project manifest
  - SharedDriveAccess/Dockerfile — container build instructions
  - docker-compose.yml — service and volume definitions

## Quick summary — what it does
- Checks a network-mounted directory at `/app/network`.
- Lists files found there and copies each file into `/app/data` (overwrites by default).
- Exits with informative console logs; basic try/catch around the whole operation.

## Repository layout
```
SharedDriveAccess.sln              # Visual Studio solution
SharedDriveAccess/                 # .NET project
  Program.cs                       # entry point — copies files from /app/network -> /app/data
  SharedDriveAccess.csproj         # project file (TargetFramework: net8.0)
  Dockerfile                       # builds/publishes the app into a runtime image
docker-compose.yml                 # mounts network and local volumes and runs the container
docker-compose.override.yml
.dockerignore
bin/ obj/ .vs/                     # build/IDE artifacts (ignored)
```

How it fits together:
- The console app runs the copy logic (Program.cs).
- The Dockerfile builds the app and sets the container ENTRYPOINT to run the published DLL.
- docker-compose.yml declares two volumes:
  - `network-drive` mounted into the container at `/app/network` (expected to be a CIFS mount of your SMB share).
  - `local-drive` mounted into the container at `/app/data` (binds to a host path).
  - The compose file is configured so the container reads from the CIFS-mounted `network-drive` and writes into `local-drive`.

## How to run

Prerequisites:
- .NET 8 SDK (for local development)
- Docker and Docker Compose (if running in containers)
- Access to the network share (CIFS/SMB) you want to mount

Run locally (dotnet):
```bash
# from repo root
dotnet build SharedDriveAccess/SharedDriveAccess.csproj -c Release
dotnet run --project SharedDriveAccess/SharedDriveAccess.csproj --configuration Release
```

Build and run with Docker:
```bash
# Build image locally
docker build -t shareddriveaccess -f SharedDriveAccess/Dockerfile .

# Run locally with explicit mounts (example)
docker run --rm \
  -v /path/to/your/network/mount:/app/network:ro \
  -v /path/to/your/local/out:/app/data \
  shareddriveaccess

# Or using docker-compose (recommended for the provided config)
docker compose up --build
```

Notes:
- The repository's docker-compose.yml includes a CIFS volume configuration (driver_opts) that must be adapted to your environment. Do NOT commit real credentials to the repository.
- On Windows hosts with Docker Desktop, bind-mount `local-drive` to a Windows path (compose file shows an example). On Linux hosts, adapt device and options accordingly.

## Configuration

Files/paths used by the app:
- Network mount inside the container: /app/network
- Local output inside the container: /app/data

Environment and compose variables:
- DOCKER_REGISTRY — optional prefix used by the compose image name (already present in the compose file). Example: set to `myregistry.azurecr.io/` if you push images.

## Behavior & limitations
- The app performs a simple file enumeration and copy. It does not:
  - Preserve advanced metadata (ACLs, timestamps beyond standard copy behavior).
  - Handle subdirectory recursion (current logic enumerates files from the top-level of the network directory).
  - Provide retry/backoff for transient network errors beyond a single attempt.
  - Perform concurrent copies — files are processed sequentially.
- The app logs to console only. For production use you may want to add structured logging and error reporting.

## Troubleshooting
- "Network drive not found." — ensure your CIFS mount is available inside the container at /app/network and that container user permissions allow reading.
- Permission denied copying files — check the write permissions for /app/data inside the container and the host folder bound to it.
- CIFS mount issues — verify SMB/CIFS drivers and network access; test mounting outside Docker first.
- If docker-compose fails with permission or mount-driver errors, try mounting the share on the host and bind-mounting it into the container instead of using driver_opts.



```

