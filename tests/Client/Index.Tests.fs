module IndexTests

open Client
open Client.Index
open Client.Pages
open Client.Urls
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
                        ([ "about" ], Url.About)
                        ([], Url.Blog)
                        ([ "blog" ], Url.Blog)
                        ([ "blog"; slug ], Url.BlogEntry "some-slug")
                        ([ "notfound" ], Url.NotFound)
                        ([ "Notfound" ], Url.NotFound)
                        ([ "notFound" ], Url.NotFound)
                        ([ "search" ], Url.Search)
                        ([ "tac-o-cat" ], Url.NotFound)
                        ([ "UnexpectedError" ], Url.UnexpectedError)
                        ([ "unexpectedError" ], Url.UnexpectedError)
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
                        Url.Search, Search.init() |> fst |> Page.Search
                        Url.NotFound, Page.NotFound
                    ]

                for url, expectedPageState in testParams do
                    // act
                    let state, _ = Target.update (Msg.UrlChanged url) someState

                    // assert
                    Expect.equal state { CurrentUrl = url; CurrentPage = expectedPageState } "States should be equal."

            testCase "Index.onUrlChanged doesNothing when current and new urls are equal."
            <| fun _ ->
                // arrange
                let mutable dispatchWasCalled = false
                let dispatch = fun _ ->
                        dispatchWasCalled <- true

                let url = [ "about" ]
                let state = { CurrentUrl = Url.About; CurrentPage = Page.NotFound }

                // act
                onUrlChanged state dispatch url

                // assert
                Expect.isFalse dispatchWasCalled "Dispatch should not have been called."

            testCase "Index.onUrlChanged calls dispatch with correct message when current and new urls differ."
            <| fun _ ->
                // arrange
                let url = [ "about" ]
                let state = { CurrentUrl = Url.NotFound; CurrentPage = Page.NotFound }

                let mutable conditionsMet = false
                let dispatch = fun msg ->
                        conditionsMet <- msg = (Msg.UrlChanged Url.About)

                // act
                onUrlChanged state dispatch url

                // assert
                Expect.isTrue conditionsMet "Dispatch should have been called with correct Msg."
        ]
