module Server.Tests.Rank

open Expecto
open Server
open Shared

let all =
    testList
        "Rank Tests"
        [

            testCase "Rank.processQuery: transforms query into list of words"
            <| fun _ ->
                // arrange
                let input = "   Lorem ipsum dolor Sit !amet, {} $ malis doctus tractatos duo   ei."
                let expected = ["lorem"; "ipsum"; "dolor"; "sit"; "amet"; "malis"; "doctus"; "tractatos"; "duo"; "ei"]

                // act
                let result = Rank.prepareQuery input

                // assert
                Expect.equal result expected ""

            testCase "Rank.processTags: splits tag string into list"
            <| fun _ ->
                // arrange
                let input = "555,timer,ic,chip,integrated,electronics"
                let expected = ["555"; "timer"; "ic"; "chip"; "integrated"; "electronics" ]

                // act
                let result = Rank.prepareTags input

                // assert
                Expect.equal result expected ""

            testCase "Rank.count: counts overlap of two lists"
            <| fun _ ->
                // arrange
                let matchingTerms = [ "dog"; "cat"; "beaver" ]
                let queryTerms = matchingTerms @ [ "squash"; "tomato" ]
                let tags = [ "kale"; "ham" ] @ matchingTerms

                // act
                let result = Rank.score queryTerms tags

                // assert
                Expect.equal result (List.length matchingTerms) ""

            testCase "Rank.entries: excludes zeros"
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

            testCase "Rank.entries: returns in correct order"
            <| fun _ ->
                // arrange
                let query = "hippo talk duck walk book eat cheese"

                let lowest =
                    "Lowest Entry"
                    |> BlogEntry.create
                    |> BlogEntry.setTags "hippo,not,above"

                let middle =
                    "Middle Entry"
                    |> BlogEntry.create
                    |> BlogEntry.setTags "nope,hippo,blah,duck,nada"

                let highest =
                    "Highest Entry"
                    |> BlogEntry.create
                    |> BlogEntry.setTags "blah,talk,hippo,eat,nope"

                // act
                let result = Rank.entries query [lowest; highest; middle]

                // assert
                Expect.equal result.[0] highest "Highest should be first."
                Expect.equal result.[1] middle "Middle should be second."
                Expect.equal result.[2] lowest "Lowest should be last."
        ]
