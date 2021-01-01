module Server.Data

open System
open Shared

type IRepository =
    abstract GetPublishedEntriesAsync: unit -> Async<BlogEntry list>
    abstract GetBlogEntryAsync: string -> Async<BlogEntry option>

type InMemoryRepository() =
    let entries =
        [
            "Terminal Talk"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "A Network Application in Python"
            |> BlogEntry.setThumbNail "img/terminal-talk-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2016, 12, 15))
            |> BlogEntry.setSynopsis
                "TerminalTalk is a terminal based chat application. It was created as a project for my computer networking class."

            "Omega"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "The Half Baked Video Game"
            |> BlogEntry.setThumbNail "img/omega-thumbnail.png"
            |> BlogEntry.setCreatedOn (DateTime(2016, 03, 12))
            |> BlogEntry.setUpdatedOn ((2020, 12, 21) |> DateTime |> Some)
            |> BlogEntry.setSynopsis
                "With a week off from school, I decided to try my hand at making a browser game using Javascript! It's rather crumby. But its mine, and I'm proud of it."

            "Build A Digital Clock"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "How, theoretically, to construct a digital clock."
            |> BlogEntry.setThumbNail "img/build-a-digital-clock-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2016, 1, 19))
            |> BlogEntry.setSynopsis
                "This post is the first of a three part series on the design and implementation a digital clock from the underlying logical design to the electronics that implement it."

            "The Identity of Indiscernibles"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "In Defense of a Logical Law"
            |> BlogEntry.setThumbNail "img/identity-of-indiscernibles-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2020, 12, 30))
            |> BlogEntry.setSynopsis
                "Are two completely indistinguishable objects the same thing? To some, myself included, the answer is obviously yes. But others have raised doubts. In this post I give my reasons for not sharing those doubts..."

            "Gunky Atoms"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "On Sider's 'the Possibility of Gunk'"
            |> BlogEntry.setThumbNail "img/inevitable-atoms-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2020, 12, 31))
            |> BlogEntry.setSynopsis
                "Let's get metaphysical! Objections to Ted Sider's 'the Possibility of Gunk'"

            "On the Evolutionary Argument Against Naturalism"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "Objections to Plantinga's argument that naturalism is self defeating."
            |> BlogEntry.setThumbNail "img/eaan-thumbnail.png"
            |> BlogEntry.setCreatedOn (DateTime(2015, 10, 2))
            |> BlogEntry.setUpdatedOn ((2020, 12, 27) |> DateTime |> Some)
            |> BlogEntry.setSynopsis
                "A response to the various forms of Alvin Plantinga's argument that naturalism is a self-defeating belief, collectively know as The Evolutionary Argument Against Naturalism."

            "The 555 Timer IC"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle "An Analysis for a Practical Understanding"
            |> BlogEntry.setThumbNail "img/555-ic-thumbnail.png"
            |> BlogEntry.setCreatedOn (DateTime(2017, 1, 6))
            |> BlogEntry.setSynopsis
                "The 555 Timer IC is a versatile chip in wide use. This entry gives a theoretical overview of its primary operation modes."

            "Not Published: You should not be seeing this"
            |> BlogEntry.create
            |> BlogEntry.setIsPublished false
        ]

    interface IRepository with

        member this.GetPublishedEntriesAsync() =
            entries
            |> List.filter (fun e -> e.IsPublished)
            |> async.Return

        member this.GetBlogEntryAsync slug =
            entries
            |> List.tryFind (fun e -> e.Slug = slug)
            |> async.Return
