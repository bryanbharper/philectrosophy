module Client.Tests.BlogEntry

open Client.Pages
open Fable.Mocha

let all =
    testList "BlogEntry.Component"
        [
            testCase "init adds slug to state"
            <| fun _ ->
                // arrange
                let slug = "hello-there"

                // act
                let result = BlogEntry.init slug |> fst

                // assert
                Expect.equal result.Slug slug "Slugs should be equal."

        ]
