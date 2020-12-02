module BlogApiTests

open System
open Data
open Expecto
open Foq
open Microsoft.Extensions.Logging
open Shared

let all =
    testList "BlogApi Tests"
        [
            testCase "BlogApi.getEntriesAsync: returns all entries from repo."
            <| fun _ ->
                // arrange
                let entries =
                    [
                        BlogEntry.create "Terminal Talk"
                        BlogEntry.create "Omega"
                        BlogEntry.create "Build A Digital Clock"
                    ]

                let repo =
                    Mock<IRepository>()
                        .Setup(fun x -> <@ x.GetBlogEntriesAsync() @>)
                        .Returns(entries |> async.Return)
                        .Create()

                let log = Mock<ILogger>().Create()

                // act
                let result = BlogApi.getEntriesAsync repo log |> Async.RunSynchronously

                // assert
                match result with
                | Ok result -> Expect.equal result entries "Result is equal to entries from repo."
                | _ -> failtest "Result should be Ok."

            testCase "BlogApi.getEntriesAsync: handles error case."
            <| fun _ ->
                // arrange
                let entries =
                    [
                        BlogEntry.create "Terminal Talk"
                        BlogEntry.create "Omega"
                        BlogEntry.create "Build A Digital Clock"
                    ]

                let repo =
                    Mock<IRepository>()
                        .Setup(fun x -> <@ x.GetBlogEntriesAsync() @>)
                        .Raises<Exception>()
                        .Create()

                let log = mock()

                // act
                let result = BlogApi.getEntriesAsync repo log |> Async.RunSynchronously

                // assert

                match result with
                | Error result ->
                    // verify <@ log.LogError("Error retrieving blog entries from repo: Exception of type 'System.Exception' was thrown.") @> once
                    Expect.equal result "An error occurred while retrieving blog entries." "Error message is populated."
                | _ -> failtest "Result should be Error."

        ]
