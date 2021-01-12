module Client.Urls

open Microsoft.FSharp.Reflection


[<RequireQualifiedAccess>]
type Url =
    | About
    | Blog
    | BlogEntry of slug: string
    | Search
    | NotFound
    | UnexpectedError
    member this.asString =
        let (case, _) =
            FSharpValue.GetUnionFields(this, typeof<Url>)

        case.Name.ToLower()

module Url =
    let toString (url: Url) =
        url.asString

    let fromString (s: string) =
        let caseInfo =
            FSharpType.GetUnionCases typeof<Url>
            |> Array.tryFind (fun case -> case.Name.ToLower() = s.ToLower())

        match caseInfo with
        | Some case ->
            match case.GetFields() with
            | [||] ->
                FSharpValue.MakeUnion(case, [||]) :?> Url |> Some
            | _ ->
                FSharpValue.MakeUnion(case,  [| "" |> box |] ) :?> Url |> Some
        | _ -> None

    let parseFeliz url =
        match url with
        | [] -> Url.Blog
        | [ _: string; slug: string ] -> Url.BlogEntry slug
        | [ page: string ] ->
            match fromString page with
            | Some url -> url
            | None -> Url.NotFound
        | _ -> Url.NotFound
