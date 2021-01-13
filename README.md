# Philectrosophy

## Running for Development
```
dotnet fake build -t run
```


## Running in Production Mode
First bundle the app:
```
dotnet fake build -t bundle
```

Then run `<project-root>/deploy/Server.exe`. In your browser open http://localhost:8085/.

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

Used for dependency management in place of Nuget (backend only). Example usage:

```shell
dotnet paket add Foq -p Server
```
