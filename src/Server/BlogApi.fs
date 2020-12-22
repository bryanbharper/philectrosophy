module Server.BlogApi

open System
open Data
open File
open Shared

let getEntriesAsync (repo: IRepository) =
    repo.GetBlogEntriesAsync()
    |> Async.map (List.sortByDescending (fun e -> e.CreatedOn))

let getEntryAsync (repo: IRepository) (fileStore: IBlogContentStore) slug =
    (repo.GetBlogEntryAsync slug, fileStore.GetBlogEntryContentAsync slug)
    |> Tuple.sequenceAsync
    |> Async.map Tuple.sequenceOption

let blogApiReader =
    reader {
        let! repo = resolve<IRepository> ()
        let! filreStore = resolve<IBlogContentStore> ()

        return {
                   GetEntries = fun () -> getEntriesAsync repo
                   GetEntry = getEntryAsync repo filreStore
               }
    }
