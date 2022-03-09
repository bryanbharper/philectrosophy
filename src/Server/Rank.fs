module Server.Rank

open Shared

let cleanText charsToRemove input =
    input
    |> String.toLower
    |> String.trim
    |> String.strip charsToRemove

let textToKeywords delimiter input =
    input
    |> String.split delimiter
    |> Seq.filter (fun s -> not <| String.isNullOrWhiteSpace s)

let getInputTerms searchQuery =
    searchQuery
    |> cleanText "!\"'()*,./:;?[\]^_`{|}~"
    |> textToKeywords ' '

let getTargetTerms (entry: BlogEntry) =
    let tags =
        entry.Tags
        |> cleanText "!\"'()*./:;?[\]^_`{|}~"
        |> textToKeywords ','

    let otherText =
        entry.Title + entry.Synopsis
        |> cleanText "!\"'()*,./:;?[\]^_`{|}~"
        |> textToKeywords ' '

    tags |> Seq.append otherText

let scoreTerms inputTerms targetTerms =
    let folder score inputTerm =
        if targetTerms |> Seq.contains inputTerm
        then score + 1
        else score

    inputTerms
    |> Seq.fold folder 0

let scoreEntry searchQuery (entry: BlogEntry) =
    let targetTerms = getTargetTerms entry
    let searchTerms = getInputTerms searchQuery

    let result = scoreTerms searchTerms targetTerms, entry
    result

let entries searchQuery entries =
    entries
    |> List.map (scoreEntry searchQuery)
    |> List.sortByDescending fst
    |> List.filter (fun (score, _) -> score > 0)
    |> List.map snd
