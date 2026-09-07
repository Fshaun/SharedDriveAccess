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

```

