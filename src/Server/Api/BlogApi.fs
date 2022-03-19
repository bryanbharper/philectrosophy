namespace Server.Api

open Shared.Contracts
open Shared.Extensions

open Server
open Server.Data
open Server.File

module BlogApi =

    let getEntriesAsync (repo: IBlogRepository) =
        repo.GetAll()
        |> Async.map
            (List.filter (fun e -> e.IsPublished)
             >> List.sortByDescending (fun e -> e.CreatedOn))

    let getEntryAsync (repo: IBlogRepository) (fileStore: IBlogContentStore) slug =
        (repo.GetSingle slug, fileStore.GetBlogEntryContentAsync slug)
        |> Tuple.sequenceAsync
        |> Async.map Tuple.sequenceOption

    let getSearchResults (repo: IBlogRepository) query =
        repo.GetAll() |> Async.map (Rank.entries query)

    let updateViewCount (repo: IBlogRepository) slug =
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
            let! repo = resolve<IBlogRepository> ()
            let! fileStore = resolve<IBlogContentStore> ()

            return
                {
                    GetEntries = fun () -> getEntriesAsync repo
                    GetEntry = getEntryAsync repo fileStore
                    GetSearchResults = getSearchResults repo
                    UpdateViewCount = updateViewCount repo
                }
        }
