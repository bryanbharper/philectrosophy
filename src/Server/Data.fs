module Data

open Shared

type IRepository =
    abstract GetBlogEntriesAsync: unit -> Async<BlogEntry list>
    abstract GetBlogEntry: string -> Async<BlogEntry option>

type InMemoryRepository() =
    let entries =
        [
            BlogEntry.create "Terminal Talk"
            BlogEntry.create "Omega"
            BlogEntry.create "Build A Digital Clock"
            BlogEntry.create "MIPS Microprocessor"
            BlogEntry.create "Inevitable Atoms"
            BlogEntry.create "Epiphenomenalism in Plantinga's E.A.A.N."
        ]

    interface IRepository with

        member this.GetBlogEntriesAsync() =
            entries
            |> async.Return

        member this.GetBlogEntry slug =
            entries
            |> List.tryFind (fun e -> e.Slug = slug)
            |> async.Return
