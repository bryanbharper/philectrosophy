module Client.Tests.Urls

open Client.Urls
open Fable.Mocha

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
                        (Url.About, "about")
                        (Url.Blog, "blog")
                        (Url.BlogEntry slug, "blog")
                        (Url.Search, "search")
                        (Url.NotFound, "notfound")
                    ]

                for url, expected in testParams do
                    // act
                    let result = Url.toString url

                    // assert
                    Expect.equal result expected ""

            testCase "Urls.asString: returns string version of Url"
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let testParams =
                    [
                        (Url.About, "about")
                        (Url.Blog, "blog")
                        (Url.BlogEntry slug, "blog")
                        (Url.Search, "search")
                        (Url.NotFound, "notfound")
                    ]

                for url, expected in testParams do
                    // act
                    let result = url.asString

                    // assert
                    Expect.equal result expected ""

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
                        Expect.equal result expected ""
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
                        Expect.equal result expected ""
                    | _ -> failwith (sprintf "%A should have been %A" result expected)

        ]
