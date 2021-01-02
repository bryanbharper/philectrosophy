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
            |> BlogEntry.setSubtitle (Some "A Network Application in Python")
            |> BlogEntry.setThumbNail "img/terminal-talk-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2016, 12, 15))
            |> BlogEntry.setTags "terminaltalk,terminal,talk,computer,networking,networks,chat,room,application,python,programming,code,client,server,david,chalmers,zombie,hilary,putnam,ludwig,xyz,software,ip,user,users,api"
            |> BlogEntry.setSynopsis
                "TerminalTalk is a terminal based chat application. It was created as a project for my computer networking class."

            "Omega"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle (Some "The Half Baked Video Game")
            |> BlogEntry.setThumbNail "img/omega-thumbnail.png"
            |> BlogEntry.setCreatedOn (DateTime(2016, 03, 12))
            |> BlogEntry.setUpdatedOn ((2020, 12, 21) |> DateTime |> Some)
            |> BlogEntry.setTags "omega,video,game,games,javascript,programming,code,2d,brecht,vissers,hunter,johnson,software"
            |> BlogEntry.setSynopsis
                "With a week off from school, I decided to try my hand at making a browser game using Javascript! It's rather crumby. But its mine, and I'm proud of it."

            "Build A Digital Clock"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle (Some "How, theoretically, to construct a digital clock.")
            |> BlogEntry.setThumbNail "img/build-a-digital-clock-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2016, 1, 19))
            |> BlogEntry.setTags "digital,clock,build,diy,electronics,electronic,circuits,circuit,logic,systems,decoder,counter,clk,signal,binary,time,555,ic,timer,7-segment,7,segment,flip-flop,flip,flop,latch,logism,k-map,asynchronous,synchronous,sequential,combinational"
            |> BlogEntry.setSynopsis
                "This post is the first of a three part series on the design and implementation a digital clock from the underlying logical design to the electronics that implement it."

            "The Identity of Indiscernibles"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle (Some "In Defense of a Logical Law")
            |> BlogEntry.setThumbNail "img/identity-of-indiscernibles-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2020, 12, 30))
            |> BlogEntry.setTags "identity,identical,identicals,indiscernible,indiscernibles,logic,law,axiom,max,black,ian,hacking,robert,adams,gottfried,leibniz,leibniz's,sphere,spheres,possible,world,worlds,conceivable,conceivability,argument,philosophy,metaphysics"
            |> BlogEntry.setSynopsis
                "Are two completely indistinguishable objects the same thing? To some, myself included, the answer is obviously yes. But others have raised doubts. In this post I give my reasons for not sharing those doubts..."

            "Gunky Atoms"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle (Some "On Sider's 'the Possibility of Gunk'")
            |> BlogEntry.setThumbNail "img/inevitable-atoms-thumbnail.jpg"
            |> BlogEntry.setCreatedOn (DateTime(2020, 12, 31))
            |> BlogEntry.setTags "gunk,gunky,atom,atoms,world,worlds,universe,nerdicon,adventure,time,ted,sider,philosophy,metaphysics,argument,identity,identical,identicals,indiscernible,indiscernibles,logic,leibniz,leibniz's,conceivable,conceivability,logic,possible,possibility,part,parts,parthood,part-hood,mereology,mereological,composition,compositional,nihilism,philosophical,definition,contradiction"
            |> BlogEntry.setSynopsis
                "Let's get metaphysical! Objections to Ted Sider's 'the Possibility of Gunk'"

            "On the Evolutionary Argument Against Naturalism"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle (Some "Objections to Plantinga's argument that naturalism is self defeating.")
            |> BlogEntry.setThumbNail "img/eaan-thumbnail.png"
            |> BlogEntry.setCreatedOn (DateTime(2015, 10, 2))
            |> BlogEntry.setUpdatedOn ((2020, 12, 27) |> DateTime |> Some)
            |> BlogEntry.setTags "alvin,plantinga,theism,naturalism,evolution,evolve,evolved,argument,naturalist,belief,beliefs,content,neurophysiological,philosophy,metaphysics,epistemology,reliable,reliability,epiphenomenalism,epiphenomenal,mental,cause,causal,tiger,behavior,adaptive,natural,selection,phenomenal,paradox,david,chalmers,skepticism,self-defeating,self,defeating,against,eaan,mind"
            |> BlogEntry.setSynopsis
                "A response to the various forms of Alvin Plantinga's argument that naturalism is a self-defeating belief, collectively know as The Evolutionary Argument Against Naturalism."

            "The 555 Timer IC"
            |> BlogEntry.create
            |> BlogEntry.setSubtitle (Some "An Analysis for a Practical Understanding")
            |> BlogEntry.setThumbNail "img/555-ic-thumbnail.png"
            |> BlogEntry.setCreatedOn (DateTime(2017, 1, 6))
            |> BlogEntry.setTags "555,timer,ic,chip,integrated,electronics,electronic,circuit,circuits,clock,clk,astable,bistable,monostable,comparator,digital"
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
