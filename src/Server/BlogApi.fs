module BlogApi

open Data
open Microsoft.Extensions.Logging
open Shared

let getEntriesAsync (repo: IRepository) (log: ILogger) =
      repo.GetBlogEntriesAsync()

let blogApiReader =
    reader {
        let! repo = resolve<IRepository>()
        let! log = resolve<ILogger>()
        return {
            GetEntries = fun () -> getEntriesAsync repo log
        }
    }
