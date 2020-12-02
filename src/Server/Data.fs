module Data

open System
open Shared

type IRepository =
    abstract GetBlogEntriesAsync: unit -> Async<BlogEntry list>

type InMemoryRepository() =
    interface IRepository with

        member this.GetBlogEntriesAsync() =
            [
                BlogEntry.create "Terminal Talk"
                BlogEntry.create "Omega"
                BlogEntry.create "Build A Digital Clock"
                BlogEntry.create "MIPS Microprocessor"
                BlogEntry.create "Inevitable Atoms"
                BlogEntry.create "Epiphenomenalism in Plantinga's E.A.A.N."
            ]
            |> async.Return
