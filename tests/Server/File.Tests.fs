module Server.Tests.File

open System
open Server
open Server.File
open Expecto
open Foq


let all =
    testList
        "File Tests"
        [
            testCase "PublicFileStore.ReadFileAsync: returns Async<None> when not found."
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

            testCase "PublicFileStore.ReadFileAsync: returns content from file."
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

            testCase "BlogContentStore.GetBlogEntryContentAsync: replaces math blocks"
            <| fun _ ->
                // arrange
                let slug = "some-slug"
                let path = sprintf "public/blog.posts/%s.md" slug
                let content =
                    Markdown.openTag + "\overline{Q} \frac{1}{2} X^2 R_b" + Markdown.closeTag
                    + "Some random stuff"
                    + Markdown.inlineOpenTag + "\sum x^2" + Markdown.inlineCloseTag

                let expected =
                    content
                    |> Markdown.Latex.replaceMath
                    |> Markdown.Latex.replaceInlineMath

                let fileAccess =
                    Mock<IFileAccess>().Setup(fun f -> <@ f.ReadFileAsync path @>)
                        .Returns(content |> Some |> async.Return).Create()

                let target =
                    BlogContentStore(fileAccess) :> IBlogContentStore

                // act
                let result =
                    target.GetBlogEntryContentAsync slug
                    |> Async.RunSynchronously

                // arrange
                Expect.equal result (Some expected) ""

        ]
