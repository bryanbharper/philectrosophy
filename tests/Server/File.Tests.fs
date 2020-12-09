module FileTests

open System
open File
open Expecto
open Foq


let all =
    testList
        "File Tests"
        [
            testCase "PublicFileStore.GetFileAsync: returns Async<None> when not found."
            <| fun _ ->
                // arrange
                let target = PublicFileStore() :> IFileAccess

                // act
                let result =
                    Guid.NewGuid().ToString()
                    |> target.ReadFileAsync
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result "Returns none when file not found."

            testCase "PublicFileStore.GetBlogEntryContentAsync: returns content from file."
            <| fun _ ->
                // arrange
                let target = PublicFileStore() :> IFileAccess

                let expected = """Hello world!
"""

                // act
                let result =
                    "test-files/test.txt"
                    |> target.ReadFileAsync
                    |> Async.RunSynchronously

                // assert
                Expect.equal result (Some expected) "Result is Some content."

            testCase "BlogContentStore.GetBlogEntryContentAsync: returns result from FileStore"
            <| fun _ ->
                // arrange
                let slug = "some-slug"
                let path = sprintf "public/blog.posts/%s.md" slug
                let expected = "expected content"

                let fileAccess =
                    Mock<IFileAccess>().Setup(fun f -> <@ f.ReadFileAsync path @>)
                        .Returns(expected |> Some |> async.Return).Create()

                let target =
                    BlogContentStore(fileAccess) :> IBlogContentStore

                // act
                let result =
                    target.GetBlogEntryContentAsync slug
                    |> Async.RunSynchronously

                // arrange
                Expect.equal result (Some expected) "Result should be 'expected content'"

        ]
