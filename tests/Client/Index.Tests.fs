﻿module IndexTests

open Client
open Client.Index
open Client.Pages
open Fable.Mocha

module Target = Index

let all =
    testList
        "All"
        [
            testCase "Index.parseUrl maps to the correct page"
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let testParams =
                    [
                        ([], Url.Blog)
                        ([ "blog" ], Url.Blog)
                        ([ "blog"; slug ], Url.BlogEntry slug)
                        ([ "lexicon" ], Url.Lexicon)
                        ([ "about" ], Url.About)
                        ([ "search" ], Url.Search)
                        ([ "tac-o-cat" ], Url.NotFound)
                    ]

                for url, page in testParams do
                    // act
                    let result = Target.parseUrl url

                    // assert
                    Expect.equal result page (sprintf "%A should parse to %A, but got %A instead." url page result)

            testCase "Index.pageInitFromUrl returns correct state for each page"
            <| fun _ ->
                // arrange
                let slug = "tac-o-cat"
                let testParams =
                    [
                        Url.About, About.init() |> fst |> Page.About
                        Url.Blog, Blog.init() |> fst |> Page.Blog
                        Url.BlogEntry slug, slug |> BlogEntry.init |> fst |> Page.BlogEntry
                        Url.Lexicon, Lexicon.init() |> fst |> Page.Lexicon
                        Url.Search, Search.init() |> fst |> Page.Search
                        Url.NotFound, Page.NotFound
                    ]

                for url, expectedPageState in testParams do
                    // act
                    let state, _ = Target.pageInitFromUrl url

                    // assert
                    Expect.equal state { CurrentUrl = url; CurrentPage = expectedPageState } "States should be equal."

            testCase "Index.update returns correct state for each page when msg = UrlChanged"
            <| fun _ ->
                // arrange
                let someState =
                    {
                        CurrentUrl = Url.NotFound
                        CurrentPage = Page.NotFound
                    }

                let slug = "tac-o-cat"
                let testParams =
                    [
                        Url.About, About.init() |> fst |> Page.About
                        Url.Blog, Blog.init() |> fst |> Page.Blog
                        Url.BlogEntry slug, slug |> BlogEntry.init |> fst |> Page.BlogEntry
                        Url.Lexicon, Lexicon.init() |> fst |> Page.Lexicon
                        Url.Search, Search.init() |> fst |> Page.Search
                        Url.NotFound, Page.NotFound
                    ]

                for url, expectedPageState in testParams do
                    // act
                    let state, _ = Target.update (Msg.UrlChanged url) someState

                    // assert
                    Expect.equal state { CurrentUrl = url; CurrentPage = expectedPageState } "States should be equal."

        ]
