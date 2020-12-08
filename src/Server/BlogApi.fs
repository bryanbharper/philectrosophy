module BlogApi

open System
open Data
open File
open Shared

let getEntriesAsync (repo: IRepository) = repo.GetBlogEntriesAsync()

let getEntryAsync (repo: IRepository) (file: IFileStore) slug =
    async {
        let! metadata = repo.GetBlogEntryAsync slug
        let! content = file.GetBlogEntryContentAsync slug

        return match metadata, content with
               | Some m, Some c -> Some(m, c)
               | _ -> None
    }

let blogApiReader =
    reader {
        let! repo = resolve<IRepository> ()
        let! file = resolve<IFileStore> ()

        return {
                   GetEntries = fun () -> getEntriesAsync repo
                   GetEntry = fun slug -> getEntryAsync repo file slug
               }
    }
