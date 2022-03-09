module Server.Tests.Api.BlogApi

open System
open Fable.Mocha
open Expecto
open Foq

open Shared

open Server.Data
open Server.File
open Server.Api

let all =
    testList
        "BlogApi"
        [
            testCase "getEntriesAsync: returns entries ordered by created date."
            <| fun _ ->
                // arrange
                let earliest =
                    BlogEntry.create "Earliest"
                    |> BlogEntry.setCreatedOn (DateTime(2017, 1, 1))

                let middle =
                    BlogEntry.create "Middle"
                    |> BlogEntry.setCreatedOn (DateTime(2018, 1, 1))

                let latest =
                    BlogEntry.create "Latest"
                    |> BlogEntry.setCreatedOn (DateTime(2019, 1, 1))

                let entries = [ middle; latest; earliest ]

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun r -> <@ r.GetAll() @>)
                        .Returns(entries |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.getEntriesAsync repo
                    |> Async.RunSynchronously

                // assert
                Expect.sequenceContainsOrder
                    result
                    (entries
                     |> List.sortByDescending (fun e -> e.CreatedOn))
                    "Results are ordered by CreatedOn"

            testCase "getEntriesAsync: returns published entries only."
            <| fun _ ->
                // arrange
                let published =
                    BlogEntry.create "Published"
                    |> BlogEntry.setCreatedOn (DateTime(2017, 1, 1))
                    |> BlogEntry.setIsPublished true

                let draft =
                    BlogEntry.create "Not Published"
                    |> BlogEntry.setCreatedOn (DateTime(2018, 1, 1))
                    |> BlogEntry.setIsPublished false


                let entries = [ draft; published ]

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun r -> <@ r.GetAll() @>)
                        .Returns(entries |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.getEntriesAsync repo
                    |> Async.RunSynchronously

                // assert
                Expect.hasCountOf result 1u (fun _ -> true) "Only one entry was published."
                Expect.equal result.[0] published "Only entry is published."

            testCase "getEntryAsync: returns metadata from repo and content from file store."
            <| fun _ ->
                // arrange
                let slug = "some-slug"
                let entry = BlogEntry.create "Terminal Talk"
                let expectedContent = "blah blah blah blather blither"

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun x -> <@ x.GetSingle slug @>)
                        .Returns(Some entry |> async.Return)
                        .Create()

                let file =
                    Mock<IBlogContentStore>()
                        .Setup(fun f -> <@ f.GetBlogEntryContentAsync slug @>)
                        .Returns(expectedContent |> Some |> async.Return)
                        .Create()

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

            testCase "getEntryAsync: returns None when metadata not found."
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun x -> <@ x.GetSingle slug @>)
                        .Returns(None |> async.Return)
                        .Create()

                let file =
                    Mock<IBlogContentStore>()
                        .Setup(fun f -> <@ f.GetBlogEntryContentAsync slug @>)
                        .Returns("blah" |> Some |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.getEntryAsync repo file slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result "Result should be None."

            testCase "getEntryAsync: returns None when content not found."
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun x -> <@ x.GetSingle slug @>)
                        .Returns("blah" |> BlogEntry.create |> Some |> async.Return)
                        .Create()

                let file =
                    Mock<IBlogContentStore>()
                        .Setup(fun f -> <@ f.GetBlogEntryContentAsync slug @>)
                        .Returns(None |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.getEntryAsync repo file slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result "Result should be None."

            testCase "getSearchResults: correctly ranks entries"
            <| fun _ ->
                // arrange
                let loMatch = "alPha bRavo charLie"
                let midMatch = loMatch + "dElta echO foXtrot"
                let hiMatch = midMatch + "Golf hoTel inDia"

                let lowest =
                    BlogEntry.empty
                    |> BlogEntry.setTitle loMatch

                let middle =
                    BlogEntry.empty
                    |> BlogEntry.setTags (midMatch |> String.split ' ' |> String.join ',')

                let highest =
                    BlogEntry.empty
                    |> BlogEntry.setSynopsis hiMatch

                let entries = [ lowest; highest; middle ]

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun r -> <@ r.GetAll() @>)
                        .Returns(entries |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.getSearchResults repo hiMatch
                    |> Async.RunSynchronously

                // assert
                Expect.equal result.[0] highest "Highest should be first"
                Expect.equal result.[1] middle "Middle should be second"
                Expect.equal result.[2] lowest "Lowest should be last"

            testCase "updateViewCount: returns None without matching entry"
            <| fun _ ->
                // arrange
                let slug = "not-here"

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun r -> <@ r.GetSingle slug @>)
                        .Returns(None |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.updateViewCount repo slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result ""

            testCase "updateViewCount: returns None if update fails"
            <| fun _ ->
                // arrange
                let entry = BlogEntry.create "Hello world!"

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun r -> <@ r.GetSingle entry.Slug @>)
                        .Returns(entry |> Some |> async.Return)
                        .Setup(fun r ->
                            <@ r.Update
                                { entry with
                                    ViewCount = entry.ViewCount + 1
                                } @>)
                        .Returns(None |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.updateViewCount repo entry.Slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result ""

            testCase "updateViewCount: returns Some updatedCount"
            <| fun _ ->
                // arrange
                let entry = BlogEntry.create "Hello world!"

                let updated =
                    { entry with
                        ViewCount = entry.ViewCount + 1
                    }

                let repo =
                    Mock<IBlogRepository>()
                        .Setup(fun r -> <@ r.GetSingle entry.Slug @>)
                        .Returns(entry |> Some |> async.Return)
                        .Setup(fun r -> <@ r.Update updated @>)
                        .Returns(updated |> Some |> async.Return)
                        .Create()

                // act
                let result =
                    BlogApi.updateViewCount repo entry.Slug
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Result should be Some value."
                | Some r -> Expect.equal r updated.ViewCount ""

        ]
