# Philectrosophy

## Cli Tools

### FAKE
Used for building, running, testing, bundling, publishing... just about anything. Find, edit, and add scripts in `build.fsx`. Example usage:

```shell
dotnet fake build -t test
```

### Femto
Used for install packages that have both npm and nuget/paket requirements. Example usage:
```shell
dotnet femto .\src\Client\Client.fsproj install Feliz.Bulma
```

### Paket

Ssed for dependency management in place of Nuget (backend only). Example usage:

```shell
dotnet packet add Foq -p Server
```
