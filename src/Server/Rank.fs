module Server.Rank

open Shared

let prepareQuery =
    String.toLower
    >> String.trim
    >> String.strip "!\"#$%&'()*+,./:;?@[\]^_`{|}~"
    >> String.split ' '
    >> List.ofArray
    >> List.filter (fun s -> not <| String.isNullOrWhiteSpace s)

let prepareTags = String.split ',' >> List.ofArray

let score queryTerms tags =
    let folder score el =
        if List.contains el tags then score + 1 else score

    queryTerms
    |> List.fold folder 0

let entries query entries =
    let searchTerms = prepareQuery query

    entries
    |> List.map (fun e ->
        let tags = prepareTags e.Tags
        score searchTerms tags, e)
    |> List.sortByDescending fst
    |> List.filter (fun (score, _) -> score > 0)
    |> List.map snd
