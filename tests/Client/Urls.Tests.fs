module UrlsTests

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

            testCase "Urls.toString: returns string version of Url"
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let testParams =
                    [
                        (Url.About, "About")
                        (Url.Blog, "Blog")
                        (Url.BlogEntry slug, "BlogEntry")
                        (Url.Search, "Search")
                        (Url.NotFound, "NotFound")
                        (Url.UnexpectedError, "UnexpectedError")
                    ]

                for url, expected in testParams do
                    // act
                    let result = Url.toString url

                    // assert
                    Expect.equal
                        result
                        expected
                        (sprintf "%A should become %s, but got %A instead." url expected result)

            testCase "Urls.fromString: converts valid string to Url"
            <| fun _ ->
                // arrange
                let testParams =
                    [
                        (Url.About, "About")
                        (Url.Blog, "Blog")
                        (Url.BlogEntry "", "BlogEntry")
                        (Url.Search, "Search")
                        (Url.NotFound, "NotFound")
                        (Url.UnexpectedError, "UnexpectedError")
                    ]

                for expected, asString in testParams do
                    // act
                    let result = Url.fromString asString

                    // assert
                    match result with
                    | Some result ->
                        Expect.equal
                            result
                            expected
                            (sprintf "%s should become %A, but got %A instead." asString expected result)
                    | _ -> failwith (sprintf "%A should have been %A" result expected)

            testCase "Urls.fromString: is case insensitive"
            <| fun _ ->
                // arrange
                let testParams =
                    [
                        (Url.About, "about")
                        (Url.Blog, "bLog")
                        (Url.BlogEntry "", "bLoGeNtRy")
                        (Url.Search, "SEArch")
                        (Url.NotFound, "notFound")
                        (Url.UnexpectedError, "UnExpectEDError")
                    ]

                for expected, asString in testParams do
                    // act
                    let result = Url.fromString asString

                    // assert
                    match result with
                    | Some result ->
                        Expect.equal
                            result
                            expected
                            (sprintf "%s should become %A, but got %A instead." asString expected result)
                    | _ -> failwith (sprintf "%A should have been %A" result expected)

            testCase "Url.parseFeliz maps to the correct page"
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
                    let result = Url.parseFeliz url

                    // assert
                    Expect.equal result page (sprintf "%A should parse to %A, but got %A instead." url page result)
        ]
