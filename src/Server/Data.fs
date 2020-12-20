module Server.Data

open System
open Shared

type IRepository =
    abstract GetBlogEntriesAsync: unit -> Async<BlogEntry list>
    abstract GetBlogEntryAsync: string -> Async<BlogEntry option>

type InMemoryRepository() =
    let entries =
        [
            "Terminal Talk"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "A Network Application in Python"
            |> BlogEntry.setThumbNail "img/terminal-talk-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2016, 12, 15))
            |> BlogEntry.setSynopsis "TerminalTalk is a terminal based chat application. It was created as a project for my computer networking class."
            "Omega"
            |> BlogEntry.create
            "Build A Digital Clock"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "The Logical Design"
            |> BlogEntry.setThumbNail "img/build-a-digital-clock-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2016, 1, 19))
            |> BlogEntry.setSynopsis "This post is the first of a three part series on the design and implementation a digital clock from the underlying logical design to the electronics that implement it."
            "MIPS Microprocessor"
            |> BlogEntry.create
            "Inevitable Atoms"
            |> BlogEntry.create
            "Epiphenomenalism in Plantinga's E.A.A.N."
            |> BlogEntry.create

            "The 555 Timer IC"
            |> BlogEntry.create
        ]

    interface IRepository with

        member this.GetBlogEntriesAsync() = entries |> async.Return

        member this.GetBlogEntryAsync slug =
            entries
            |> List.tryFind (fun e -> e.Slug = slug)
            |> async.Return
