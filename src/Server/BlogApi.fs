module Server.BlogApi

open System
open Data
open File
open Shared

let getEntriesAsync (repo: IRepository) =
    repo.GetPublishedEntriesAsync()
    |> Async.map (List.sortByDescending (fun e -> e.CreatedOn))

let getEntryAsync (repo: IRepository) (fileStore: IBlogContentStore) slug =
    (repo.GetBlogEntryAsync slug, fileStore.GetBlogEntryContentAsync slug)
    |> Tuple.sequenceAsync
    |> Async.map Tuple.sequenceOption

let getSearchResults (repo: IRepository) query =
    repo.GetPublishedEntriesAsync()
    |> Async.map (Rank.entries query)


let blogApiReader =
    reader {
        let! repo = resolve<IRepository> ()
        let! fileStore = resolve<IBlogContentStore> ()

        return
            {
                GetEntries = fun () -> getEntriesAsync repo
                GetEntry = getEntryAsync repo fileStore
                GetSearchResults = getSearchResults repo
            }
    }
