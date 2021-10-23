module Client.Apis.Http

open Fable.SimpleHttp

module Http =
    let (|Success|Redirect|ClientError|ServerError|) statusCode =
        match statusCode with
        | sc when sc >= 200 && sc < 300 -> Success
        | sc when sc >= 300 && sc < 400 -> Redirect
        | sc when sc >= 400 && sc < 500 -> ClientError
        | sc when sc >= 500 -> ServerError
        | _ -> ServerError

    let handleResponse decoder status responseText =
        match status with
        | Success ->
            match responseText |> decoder with
            | Ok r -> r
            | Error err -> failwith err
        | _ -> failwith responseText

    let get url = Http.get url

    let post  url body =
        Http.request url
        |> Http.method POST
        |> Http.content (BodyContent.Text body)
        |> Http.header (Headers.contentType "application/json")
        |> Http.send

    let put url body =
        Http.request url
        |> Http.method PUT
        |> Http.content (BodyContent.Text body)
        |> Http.header (Headers.contentType "application/json")
        |> Http.send

    let delete  url body =
        Http.request url
        |> Http.method DELETE
        |> Http.content (BodyContent.Text body)
        |> Http.header (Headers.contentType "application/json")
        |> Http.send
