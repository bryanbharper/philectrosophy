open System.Net

#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"
#r "netstandard"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Farmer
open Farmer.Builders
open Fake.JavaScript
open FluentFTP
open FSharp.Data

Target.initEnvironment ()

type Config = JsonProvider<"src/Server/appsettings.Development.json">

// Constants
let appName = "philectrosophy"
let blogImagePath =
    Path.getFullName "./src/Server/public/blog.posts/img"
let clientPublicDir = Path.getFullName "./src/Client/public"
let config = Config.GetSample()
let deployDir = "deploy"
let deployPath =  sprintf "./%s" deployDir |> Path.getFullName
let serverTestsPath = Path.getFullName "./tests/Server"
let serverPath = Path.getFullName "./src/Server"
let sharedPath = Path.getFullName "./src/Shared"
let sharedTestsPath = Path.getFullName "./tests/Shared"


// Helpers
let buildRawCmd cmd args workingDir =
    let path =
        match ProcessUtils.tryFindFileOnPath cmd with
        | Some path -> path
        | None ->
            sprintf "%s was not found in path. Please install and add it to path." cmd
            |> failwith

    RawCommand(path, args |> String.split ' ' |> Arguments.OfArgs)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir

let execRawHandled cmd args workingDir =
    buildRawCmd cmd args workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let execRaw cmd args workingDir =
    buildRawCmd cmd args workingDir |> Proc.run

let sass input output =
    let args = sprintf "%s %s" input output
    execRawHandled "sass" args "src\Client\public"

let dotnet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""

    if result.ExitCode <> 0
    then failwithf "'dotnet %s' failed in %s" cmd workingDir

let printSection msg =
    Trace.traceLine ()
    Trace.tracefn "%s" msg
    Trace.traceLine ()

// Targets
Target.create "Clean"
<| fun _ ->
    "Clean Deploy Directory" |> printSection
    Shell.cleanDir deployPath

Target.create "BlogImages"
<| fun _ ->
    "Moving blog images to client." |> printSection
    Shell.copyDir (Path.combine clientPublicDir "img") blogImagePath (fun _ -> true)

Target.create "BundleStyles"
<| fun _ ->
    "Transpiling .scss files into styles.css" |> printSection
    sass "styles.scss" "styles.css"

Target.create "InstallClient"
<| fun _ ->
    "Installing Client" |> printSection
    Npm.install id

Target.create "Bundle"
<| fun _ ->
    "Bundling App" |> printSection
    dotnet (sprintf "publish -c Release -o \"%s\"" deployPath) serverPath
    Npm.run "build" id

Target.create "Deploy"
<| fun _ ->
    "Deploying App" |> printSection

    use client = new FtpClient(config.Ftp.Host)
    client.Credentials <- NetworkCredential(config.Ftp.User, config.Ftp.Secret)
    client.Connect()

    client.UploadDirectory(deployPath, sprintf "/%s/%s" appName deployDir, FtpFolderSyncMode.Mirror) |> ignore

    client.Disconnect()


Target.create "Azure"
<| fun _ ->
    "Deploying to Azure" |> printSection

    let web =
        webApp {
            name appName
            sku WebApp.Sku.D1
            app_insights_off
            zip_deploy "deploy"
        }

    let deployment =
        arm {
            location Location.CentralUS
            add_resource web
        }

    deployment
    |> Deploy.execute appName Deploy.NoParameters
    |> ignore


Target.create "Run"
<| fun _ ->
    "Starting App" |> printSection
    dotnet "build" sharedPath
    [
        async { dotnet "watch run" serverPath }
        async { Npm.run "start" id }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

Target.create "BuildSharedTests"
<| fun _ ->
    "Building Shared Tests" |> printSection
    dotnet "build" sharedTestsPath

Target.create "Test"
<| fun _ ->
    "Run Server Tests" |> printSection
    dotnet "run" serverTestsPath
    "Run Client Tests" |> printSection
    Npm.run "test" id

Target.create "LiveTest"
<| fun _ ->
    "Running Live Tests" |> printSection
    [
        async { dotnet "watch run" serverTestsPath }
        async { Npm.run "test:live" id }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

Target.create "List"
<| fun _ -> Target.listAvailable ()

open Fake.Core.TargetOperators

"Clean"
==> "InstallClient"
==> "BlogImages"
==> "BundleStyles"

"BundleStyles"
==> "Bundle"

"Bundle"
==> "Azure"

"Bundle"
==> "Deploy"

"BundleStyles"
==> "Run"

"Clean"
==> "InstallClient"
==> "BuildSharedTests"

"BuildSharedTests"
==> "Test"

"BuildSharedTests"
==> "LiveTest"

Target.runOrDefault "List"
