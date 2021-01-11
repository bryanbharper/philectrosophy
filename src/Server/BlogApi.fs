module Server.BlogApi

open System
open Data
open File
open Shared

let getEntriesAsync (repo: IRepository) =
    repo.GetAll()
    |> Async.map
        (List.filter (fun e -> e.IsPublished)
         >> List.sortByDescending (fun e -> e.CreatedOn))

let getEntryAsync (repo: IRepository) (fileStore: IBlogContentStore) slug =
    (repo.GetSingle slug, fileStore.GetBlogEntryContentAsync slug)
    |> Tuple.sequenceAsync
    |> Async.map Tuple.sequenceOption

let getSearchResults (repo: IRepository) query =
    repo.GetAll() |> Async.map (Rank.entries query)

let updateViewCount (repo: IRepository) slug =
    async {
        let! entryOp = repo.GetSingle slug

        return!
            entryOp
            |> Option.map (fun e -> repo.Update { e with ViewCount = e.ViewCount + 1 })
            |> Option.sequenceAsync
            |> Async.map
                (Option.flatten
                 >> Option.map (fun e -> e.ViewCount))
    }

let blogApiReader =
    reader {
        let! repo = resolve<IRepository> ()
        let! fileStore = resolve<IBlogContentStore> ()

        return
            {
                GetEntries = fun () -> getEntriesAsync repo
                GetEntry = getEntryAsync repo fileStore
                GetSearchResults = getSearchResults repo
                UpdateViewCount = updateViewCount repo
            }
    }
