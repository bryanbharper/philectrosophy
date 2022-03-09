module Server.Tests.File

open System
open Server
open Server.File
open Expecto
open Foq


let all =
    testList
        "File"
        [
            testCase "PublicFileStore.ReadFileAsync: returns Async<None> when not found"
            <| fun _ ->
                // arrange
                let target = PublicFileStore() :> IFileAccess

                // act
                let result =
                    Guid.NewGuid().ToString()
                    |> target.ReadFileAsync
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result "Returns none when file not found"

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
                Expect.equal result (Some expected) "Result is Some content"

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
                    Markdown.displayMathOpenTag + "\overline{Q} \frac{1}{2} X^2 R_b" + Markdown.displayMathCloseTag
                    + "Some random stuff"
                    + Markdown.inlineMathOpenTag + "\sum x^2" + Markdown.inlineMathCloseTag

                let expected =
                    content
                    |> Markdown.Latex.convertDisplayMath
                    |> Markdown.Latex.convertInlineMath

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

            testCase "BlogContentStore.GetBlogEntryContentAsync: replaces popover blocks"
            <| fun _ ->
                // arrange
                let slug = "some-slug"
                let path = sprintf "public/blog.posts/%s.md" slug
                let content =
                    "Some random stuff"
                    + Markdown.popoverOpenTag + "Hello! I'm in a popover." + Markdown.popoverCloseTag
                    + "Some more random stuff"

                let expected =
                    content
                    |> Markdown.Bulma.convertPopovers

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
