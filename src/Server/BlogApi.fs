module BlogApi

open Data
open Microsoft.Extensions.Logging
open Shared

let getEntriesAsync (repo: IRepository) (log: ILogger) =
    try
        // Todo: failure is throwing 500
        repo.GetBlogEntriesAsync()
        |> Async.map Ok
    with
    | ex ->
        log.LogError("Error retrieving blog entries from repo: " + ex.Message)

        Error "An error occurred while retrieving blog entries."
        |> async.Return


let blogApiReader =
    reader {
        let! repo = resolve<IRepository>()
        let! log = resolve<ILogger>()
        return {
            GetEntries = fun () -> getEntriesAsync repo log
        }
    }
