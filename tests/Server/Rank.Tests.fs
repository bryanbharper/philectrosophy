module Server.Tests.Rank

open Expecto
open Server
open Shared

let all =
    testList
        "Rank"
        [
            testCase "cleanText: strips unwanted characters"
            <| fun _ ->
                // arrange
                let charsToRemove = "!\"$%&'()*+,./:;?@[\]^_`{|}~"
                let expected = "i should remain"
                let input = charsToRemove + expected + charsToRemove

                // act
                let result = Rank.cleanText charsToRemove input

                // assert
                Expect.equal result expected "Failed to remove specified characters from input"

            testCase "cleanText: lowercase input"
            <| fun _ ->
                // arrange
                let input = "tHiS iS a TeSt"

                // act
                let result = Rank.cleanText "" input

                // assert
                Expect.equal result (input |> String.toLower) "Failed to lowercase input"

            testCase "cleanText: trim spaces"
            <| fun _ ->
                // arrange
                let input = "  this is a test       "

                // act
                let result = Rank.cleanText "" input

                // assert
                Expect.equal result (input |> String.trim) "Failed to trim spaces from input"


            testCase "entries: excludes zeros"
            <| fun _ ->
                // arrange
                let query = "hippo talk duck walk book eat cheese"

                let entries =
                    [
                        "Zero Entry"
                        |> BlogEntry.create
                        |> BlogEntry.setTags "this,is,a,disjoint,list"

                        "Nada Post"
                        |> BlogEntry.create
                        |> BlogEntry.setTags "these,words,are,not,above"
                    ]

                // act
                let result = Rank.entries query entries

                // assert
                Expect.isEmpty result ""

            testCase "entries: correctly ranks entries"
            <| fun _ ->
                // arrange
                let loMatch = "alPha (bRavo) charLie."
                let midMatch = loMatch + " dElta echO [foXtrot]"
                let hiMatch = midMatch + " Golf! hoTel inDia?"

                let lowest =
                    BlogEntry.empty
                    |> BlogEntry.setTitle loMatch

                let middle =
                    BlogEntry.empty
                    |> BlogEntry.setTags (midMatch |> String.split ' ' |> String.join ',')

                let highest =
                    BlogEntry.empty
                    |> BlogEntry.setSynopsis hiMatch

                // act
                let result = Rank.entries hiMatch [lowest; highest; middle]

                // assert
                Expect.equal result.[0] highest "Highest should be first"
                Expect.equal result.[1] middle "Middle should be second"
                Expect.equal result.[2] lowest "Lowest should be last"

        ]
