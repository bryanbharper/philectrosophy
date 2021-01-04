module Server.Program

open Dapper.FSharp
open Data
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open File
open Giraffe.SerilogExtensions
open Microsoft.Extensions.DependencyInjection
open Saturn

open Serilog
open Shared

let configureServices (services : IServiceCollection) =
    services
        .AddSingleton<IContext, DbContext>()
        .AddSingleton<IRepository, BlogRepository>()
        .AddSingleton<IFileAccess, PublicFileStore>()
        .AddSingleton<IBlogContentStore, BlogContentStore>()

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromReader BlogApi.blogApiReader
    |> Remoting.withErrorHandler Error.handler
    |> Remoting.buildHttpHandler

let webAppWithLogging = SerilogAdapter.Enable(webApp)

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
        use_router webAppWithLogging
        service_config configureServices
        memory_cache
        use_static "public"
        use_gzip
    }

run app
