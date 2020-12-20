module Server.BlogApi

open System
open Data
open File
open Shared

let getEntriesAsync (repo: IRepository) = repo.GetBlogEntriesAsync()

let getEntryAsync (repo: IRepository) (file: IBlogContentStore) slug =
    (repo.GetBlogEntryAsync slug, file.GetBlogEntryContentAsync slug)
    |> Tuple.sequenceAsync
    |> Async.map Tuple.sequenceOption

let blogApiReader =
    reader {
        let! repo = resolve<IRepository> ()
        let! file = resolve<IBlogContentStore> ()

        return {
                   GetEntries = fun () -> getEntriesAsync repo
                   GetEntry = fun slug -> getEntryAsync repo file slug
               }
    }
