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
                Expect.equal result entries "Result is equal to entries from repo."

        ]
