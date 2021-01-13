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

let sourcePath =
    Path.getFullName "./src"
let clientPath = Path.combine sourcePath "Client"
let serverPath = Path.combine sourcePath "Server"
let sharedPath = Path.combine sourcePath "Shared"

let deployDir = "deploy"
let deployPath =  sprintf "./%s" deployDir |> Path.getFullName
let deployPublicPath = Path.combine deployPath "public"

let blogImagePath =
    Path.combine serverPath "public/blog.posts/img"
let clientPublicPath = Path.combine clientPath "public"

let testPath = Path.getFullName "./tests"
let serverTestsPath = Path.combine testPath "Server"
let sharedTestsPath = Path.combine testPath "Shared"

let config = Config.GetSample()

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
    execRawHandled "sass" args clientPublicPath

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
    Shell.copyDir (Path.combine clientPublicPath "img") blogImagePath (fun _ -> true)

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

Target.create "SmarterAsp"
<| fun _ ->
    "Deploying to SmarterAsp" |> printSection

    use client = new FtpClient(config.Ftp.Host)
    client.Credentials <- NetworkCredential(config.Ftp.User, config.Ftp.Secret)
    client.Connect()

    client.UploadDirectory(deployPath, sprintf "/%s/%s" appName deployDir, FtpFolderSyncMode.Mirror) |> ignore

    client.Disconnect()

Target.create "Sandbox"
<| fun _ ->
    "Deploying to Sandbox Environment on Azure" |> printSection

    let web =
        webApp {
            name (appName + "-sbx")
            app_insights_off
            zip_deploy deployDir
            setting "ASPNETCORE_ENVIRONMENT" "Sandbox"
        }

    let deployment =
        arm {
            location Location.CentralUS
            add_resource web
        }

    deployment
    |> Deploy.execute appName Deploy.NoParameters
    |> ignore

Target.create "Deploy"
<| fun _ ->
    "Deploying to Production Environment on Azure" |> printSection

    let web =
        webApp {
            name appName
            sku WebApp.Sku.D1
            app_insights_off
            zip_deploy deployDir
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

Target.create "TestServer"
<| fun _ ->
    "Run Server Tests" |> printSection
    dotnet "run" serverTestsPath

Target.create "TestClient"
<| fun _ ->
    "Run Client Tests" |> printSection
    Npm.run "test" id

Target.create "Test"
<| fun _ ->
    "Running All Tests" |> printSection

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
==> "BuildSharedTests"

"BuildSharedTests"
==> "TestClient"

"BuildSharedTests"
==> "LiveTest"

"TestServer"
==> "Test"

"TestClient"
==> "Test"

"Clean"
==> "InstallClient"
==> "BlogImages"
==> "BundleStyles"

"BundleStyles"
==> "Bundle"

"BundleStyles"
==> "Run"

"Bundle"
==> "SmarterAsp"

"Bundle"
==> "Sandbox"

"Bundle"
==> "Test"
==> "Deploy"

Target.runOrDefault "List"
