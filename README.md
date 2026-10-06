Uses the Docker api so this needs to be enabled.

## Running the tests

```bash
dotnet test DockerPlugin.sln
```

The test project references `PluginApi.dll` from the local Logi Plugin Service installation.

## Error handling

| Error | Cause | Handling |
| --- | --- | --- |
| Docker not running | `docker info` exits with a non-zero code | Plugin status set to Error "Docker not running"; action does nothing |
| Docker CLI missing | `docker` executable cannot be started (`Win32Exception` / `InvalidOperationException`) | Logged; treated as Docker not running |
| Docker API unavailable | `GET /version` on `localhost:2375` fails, times out (1 s) or returns non-success | Logged as warning; plugin status set to Error "Docker API not found" |
| Container listing failed | Docker API returns non-success, connection fails, request times out (30 s) or JSON is invalid | Logged; listboxes and folders show no containers; actions return failure |
| Start/stop request failed | Docker API returns a status other than 2xx or 304, or the request fails | Logged; action returns failure. Bulk actions still try every remaining container |
| Container or stack not found | Selected container ID, name or compose project no longer exists | Logged as warning; action returns failure |
| Icon resource missing | Embedded SVG not found in the plugin assembly | Logged; no image is shown |
