module Server.Program

open Dapper.FSharp
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open File
open Giraffe.Core
open Giraffe.ResponseWriters
open Giraffe.SerilogExtensions
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Saturn
open Serilog

open Shared.Contracts

open Server.Data
open Server.Data.SongRepository
open Server.Api

let configureServices (services : IServiceCollection) =
    services
        .AddSingleton<IContext, DbContext>()
        .AddSingleton<IBlogRepository, BlogRepository>()
        .AddSingleton<ISongRepository, SongRepository>()
        .AddSingleton<IFileAccess, PublicFileStore>()
        .AddSingleton<IBlogContentStore, BlogContentStore>()

let blogApi =
    Remoting.createApi()
    |> Remoting.fromReader BlogApi.blogApiReader
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withErrorHandler Error.handler
    |> Remoting.buildHttpHandler

let songApi: HttpFunc -> HttpContext -> HttpFuncResult =
    Remoting.createApi()
    |> Remoting.fromReader SongApi.songApiReader
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withErrorHandler Error.handler
    |> Remoting.buildHttpHandler

let fallbackApi = router {
    not_found_handler (setStatusCode 200 >=> htmlFile "public/index.html")
}

let fullApi: HttpHandler =
    choose [ blogApi; songApi; fallbackApi ]
    |> SerilogAdapter.Enable

OptionTypes.register ()

Log.Logger <-
    LoggerConfiguration()
      .Destructure.FSharpTypes()
      .WriteTo.Console() // https://github.com/serilog/serilog-sinks-console
      .CreateLogger()
      // add more sinks etc.

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router fullApi
        service_config configureServices
        memory_cache
        use_static "public"
        use_gzip
    }

run app
