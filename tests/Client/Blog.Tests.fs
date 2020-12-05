module BlogTests

open System
open Client.Pages.Blog
open Fable.Mocha
open Shared

let all =
    testList "All"
        [
            testCase "Blog.init sets State.Entries to InProgress"
            <| fun _ ->
                // act
                let result = init () |> fst

                // assert
                Expect.equal result.Entries Deferred.InProgress "Result should be InProgress."

            testCase "Blog.update - Resolves result from Msg.GotEntries to State.Entries"
            <| fun _ ->
                // arrange
                let entries = []
                let msg = entries |> Msg.GotEntries
                let state = { Entries = InProgress }

                // act
                let result = update msg state |> fst

                // arrange
                match result.Entries with
                | Resolved entries' ->
                    Expect.equal entries' entries "result.Entries is Resolved."
                | _ -> failwith "result.Entries was not Resolved."

            testCase "Blog.update - ApiError doesn't change state"
            <| fun _ ->
                // arrange
                let msg = Exception() |> Msg.ApiError
                let state = { Entries = InProgress }

                // act
                let result = update msg state |> fst

                // arrange
                Expect.equal result state "State is unchanged."

        ]
