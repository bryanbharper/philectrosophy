module Client.Apis.GoogleSearchApi

open System
open Shared
open Thoth.Json
open Client.Apis.Http

type CseImage =
    {
        Src: string
    }

type PageMap =
    {
        Cse_image: CseImage list
    }

type SearchItem =
    {
        Title: string
        Link: string
        Snippet: string
        Pagemap: PageMap
    }

type SearchResult =
    {
        Items: SearchItem list
    }

let private searchItemToBlogEntry (si: SearchItem) : BlogEntry =
    {
        Author = "Bryan B. Harper"
        CreatedOn = DateTime.Now
        IsPublished = true
        Slug = si.Link |> String.split '/' |> Array.last
        Synopsis = si.Snippet
        Subtitle = None
        Tags = ""
        ThumbNailUrl = si.Pagemap.Cse_image |> List.head |> fun x -> x.Src
        Title = si.Title
        UpdatedOn = None
        ViewCount = 0
    }

let private apiKey = "AIzaSyCeQ_gV9X0sSdZt4A_0QBw9XggLj8rRzHI" // Not sensitive. Anyone's welcome to use this search.
let private engineId = "6651ef59f22c7d088"

let search query =
    let url = sprintf "https://www.googleapis.com/customsearch/v1?key=%s&cx=%s&q=%s" apiKey engineId query
    let decoder json = Decode.Auto.fromString<SearchResult> (json, caseStrategy = CamelCase)

    async {
        let! status, result = Http.get url
        let searchResult = Http.handleResponse decoder status result

        return
            searchResult.Items
            |> List.map searchItemToBlogEntry
    }

