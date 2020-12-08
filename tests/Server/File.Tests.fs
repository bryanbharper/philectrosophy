module FileTests

open System
open File
open Expecto


let all =
    testList
        "File Tests"
        [
            testCase "PublicFileStore.GetBlogEntryContentAsync: returns Async<None> when not found."
            <| fun _ ->
                // arrange
                let target = PublicFileStore() :> IFileStore

                // act
                let result =
                    Guid.NewGuid().ToString()
                    |> target.GetBlogEntryContentAsync
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result "Returns none when file not found."

//            testCase "PublicFileStore.GetBlogEntryContentAsync: returns content from file."
//            <| fun _ ->
//                // arrange
//                let target = PublicFileStore() :> IFileStore
//
//                // act
//                let result =
//                    "for-tests"
//                    |> target.GetBlogEntryContentAsync
//                    |> Async.RunSynchronously
//
//                // assert
//                Expect.equal result (Some "test test 123") "Result is Some content."

        ]
