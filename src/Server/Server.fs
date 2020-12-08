module Server

open Data
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open File
open Microsoft.Extensions.DependencyInjection
open Saturn

open Shared

let configureServices (services : IServiceCollection) =
    services
        .AddSingleton<IRepository, InMemoryRepository>()
        .AddSingleton<IFileStore, PublicFileStore>()

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromReader BlogApi.blogApiReader
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        service_config configureServices
        memory_cache
        use_static "public"
        use_gzip
    }

run app
