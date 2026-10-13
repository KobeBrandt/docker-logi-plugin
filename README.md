# Container Control for Docker

Control Docker containers from a Logitech or Loupedeck device. Start, stop and toggle single containers, Docker Compose stacks or everything at once with a button press.

Works on Windows and macOS. Talks to the Docker Engine API directly.

## Actions

| Action | Type | What it does |
| --- | --- | --- |
| **Container** | Action editor command | Pick a container from a list. Pressing the button toggles it: running containers stop, stopped containers start. |
| **Stack** | Action editor command | Pick a Docker Compose project (from the `com.docker.compose.project` label). Pressing the button toggles the whole stack. If most of its containers run, the running ones stop. Otherwise the stopped ones start. |
| **Containers** | Dynamic folder | Opens a folder with one button per container. Pressing a button toggles that container. |
| **Start all containers** | Command | Starts every container. |
| **Stop all containers** | Command | Stops every container. |

Before each action runs, the plugin checks that Docker is running and that the API is reachable. If not, the plugin status shows an error in the Logi Plugin Service.

## Requirements

- Logi Plugin Service (Logi Options+ or Loupedeck software) 6.0 or later
- Docker Desktop, or another Docker engine (OrbStack, Colima) on macOS
- .NET 10 SDK, to build from source

## Docker setup

- **Windows:** no setup needed. The plugin uses the Docker Desktop named pipe `\\.\pipe\docker_engine`, the same one the `docker` CLI uses. You do not need "Expose daemon on tcp://localhost:2375 without TLS". Your user must be allowed to use Docker (member of the `docker-users` group or an administrator).
- **macOS:** no setup needed. The plugin uses the Docker socket at `/var/run/docker.sock`. If that socket does not exist, it falls back to `~/.docker/run/docker.sock` (Docker Desktop), `~/.orbstack/run/docker.sock` (OrbStack) or `~/.colima/default/docker.sock` (Colima).

The plugin also runs `docker info` to check that Docker is running. The `docker` CLI must be on `PATH` or in a standard install location.

## Building

```bash
dotnet build DockerPlugin.sln
```

The build:

1. References `PluginApi.dll` from the local Logi Plugin Service installation.
2. Writes output to `bin/<Configuration>/`.
3. Creates a `.link` file in the Logi Plugin Service plugins folder that points to the build output.
4. Asks the Logi Plugin Service to reload the plugin.

After a debug build, the plugin is live on your device. No install step needed.

## Packaging and installing

Build in Release mode, then pack and install with `logiplugintool`:

```bash
dotnet build DockerPlugin.sln -c Release
```

```bash
logiplugintool pack ./bin/Release ./Docker.lplug4
```

```bash
logiplugintool install ./Docker.lplug4
```

To uninstall:

```bash
logiplugintool uninstall Docker
```

These commands are also available as VS Code tasks in `.vscode/tasks.json`.

## Running the tests

```bash
dotnet test DockerPlugin.sln
```

The test project references `PluginApi.dll` from the local Logi Plugin Service installation.

## Project structure

```
src/
  Actions/        Plugin actions (Container, Stack, Containers folder, Start/Stop all)
  Helpers/        Docker API client, container queries and operations, readiness checks
  Resources/      Embedded SVG action icons
  package/        Plugin package metadata (LoupedeckPackage.yaml, icon)
tests/
  DockerPlugin.Tests/   Unit tests with fake HTTP, process and Docker clients
```

## Error handling

| Error | Cause | Handling |
| --- | --- | --- |
| Docker not running | `docker info` exits with a non-zero code | Plugin status set to Error "Docker not running"; action does nothing |
| Docker CLI missing | `docker` executable not found in the usual install locations or on `PATH`, or cannot be started (`Win32Exception` / `InvalidOperationException`) | Logged; treated as Docker not running |
| Docker API unavailable | `GET /version` on the `docker_engine` named pipe (Windows) or the Docker socket (macOS) fails, times out (1 s) or returns non-success | Logged as warning; plugin status set to Error "Docker API not found" |
| Docker access denied | Windows user is not in the `docker-users` group and is not an administrator, so the named pipe refuses the connection | Same handling as Docker API unavailable |
| Container listing failed | Docker API returns non-success, connection fails, request times out (30 s) or JSON is invalid | Logged; listboxes and folders show no containers; actions return failure |
| Start/stop request failed | Docker API returns a status other than 2xx or 304, or the request fails | Logged; action returns failure. Bulk actions still try every remaining container |
| Container or stack not found | Selected container ID, name or compose project no longer exists | Logged as warning; action returns failure |
| Icon resource missing | Embedded SVG not found in the plugin assembly | Logged; no image is shown |

## License

MIT. Report bugs and suggestions on the [issues page](https://github.com/KobeBrandt/docker-logi-plugin/issues).

Icons are from [Font Awesome Free](https://fontawesome.com) by Fonticons, Inc., licensed under [CC BY 4.0](https://fontawesome.com/license/free). The plugin icon uses the same Font Awesome glyph as the container action icon.

Docker and the Docker logo are trademarks of Docker, Inc. This plugin is not affiliated with or endorsed by Docker, Inc.
