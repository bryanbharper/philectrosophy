module Server.Error

open System
open Fable.Remoting.Server
open Microsoft.AspNetCore.Http
open Giraffe.SerilogExtensions

let handler (ex: Exception) (routeInfo: RouteInfo<HttpContext>) =

    let contextLogger = routeInfo.httpContext.Logger()

    let errorMsgTemplate =
        "Error occured while invoking {MethodName} at {RoutePath}"
    contextLogger.Error(ex, errorMsgTemplate, routeInfo.methodName, routeInfo.path)

    // See https://zaid-ajaj.github.io/Fable.Remoting/src/error-handling.html
    match ex with
    | _ -> Ignore
