module Client.Urls

open Microsoft.FSharp.Reflection

[<RequireQualifiedAccess>]
type Url =
    | About
    | Blog
    | BlogEntry of slug: string
    | Music
    | NotFound
    | Search
    | UnexpectedError
    member this.asString =
        let this' =
            match this with
            | BlogEntry _ -> Blog
            | _ -> this

        let (case, _) =
            FSharpValue.GetUnionFields(this', typeof<Url>)

        case.Name.ToLower()

module Url =
    let toString (url: Url) = url.asString

    let fromString (s: string) =
        let caseInfo =
            FSharpType.GetUnionCases typeof<Url>
            |> Array.tryFind (fun case -> case.Name.ToLower() = s.ToLower())

        match caseInfo with
        | Some case ->
            match case.GetFields() with
            | [||] -> FSharpValue.MakeUnion(case, [||]) :?> Url |> Some
            | _ ->
                FSharpValue.MakeUnion(case, [| "" |> box |]) :?> Url
                |> Some
        | _ -> None
