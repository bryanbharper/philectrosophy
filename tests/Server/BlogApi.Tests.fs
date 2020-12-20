module Server.Tests.BlogApi

open Server
open Server.Data
open Server.File
open Expecto
open Foq
open Shared

let all =
    testList
        "BlogApi Tests"
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
                    Mock<IRepository>().Setup(fun r -> <@ r.GetBlogEntriesAsync() @>).Returns(entries |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.getEntriesAsync repo
                    |> Async.RunSynchronously

                // assert
                Expect.equal result entries "Result is equal to entries from repo."

            testCase "BlogApi.getEntryAsync: returns metadata from repo and content from file store."
            <| fun _ ->
                // arrange
                let slug = "some-slug"
                let entry = BlogEntry.create "Terminal Talk"
                let expectedContent = "blah blah blah blather blither"

                let repo =
                    Mock<IRepository>().Setup(fun x -> <@ x.GetBlogEntryAsync slug @>)
                        .Returns(Some entry |> async.Return).Create()

                let file =
                    Mock<IBlogContentStore>().Setup(fun f -> <@ f.GetBlogEntryContentAsync slug @>)
                        .Returns(expectedContent |> Some |> async.Return).Create()

                // act
                let result =
                    BlogApi.getEntryAsync repo file slug
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Should return Some result."
                | Some (metadata, content) ->
                    Expect.equal metadata entry "Metadata should be from repo."
                    Expect.equal content expectedContent "Content should be from file store."

            testCase "BlogApi.getEntryAsync: returns None when metadata not found."
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let repo =
                    Mock<IRepository>().Setup(fun x -> <@ x.GetBlogEntryAsync slug @>).Returns(None |> async.Return)
                        .Create()

                let file =
                    Mock<IBlogContentStore>().Setup(fun f -> <@ f.GetBlogEntryContentAsync slug @>)
                        .Returns("blah" |> Some |> async.Return).Create()

                // act
                let result =
                    BlogApi.getEntryAsync repo file slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result "Result should be None."

            testCase "BlogApi.getEntryAsync: returns None when content not found."
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let repo =
                    Mock<IRepository>().Setup(fun x -> <@ x.GetBlogEntryAsync slug @>)
                        .Returns("blah" |> BlogEntry.create |> Some |> async.Return).Create()

                let file =
                    Mock<IBlogContentStore>().Setup(fun f -> <@ f.GetBlogEntryContentAsync slug @>)
                        .Returns(None |> async.Return).Create()

                // act
                let result =
                    BlogApi.getEntryAsync repo file slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result "Result should be None."

        ]
