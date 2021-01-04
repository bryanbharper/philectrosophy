module Server.Tests.Data

open Server.Data
open Expecto
open Foq
open Shared

type EmptyContext() =
    interface IContext with
        member this.GetTable<'a> _ =
            Seq.empty<'a> |> async.Return

        member this.GetByValue<'a, 'b> _ _ (_: 'a) =
            Seq.empty<'b> |> async.Return

let all =
    testList
        "Data Tests"
        [

            testCase "BlogRepository.GetBlogEntriesAsync: returns all entries in context."
            <| fun _ ->
                // arrange
                let entries =
                    [
                        BlogEntry.create "one"
                        BlogEntry.create "two"
                        BlogEntry.create "three"
                    ]

                let context =
                    Mock<IContext>()
                        .Setup(fun r -> <@ r.GetTable "blogentries" @>)
                        .Returns(entries |> Seq.ofList |> async.Return)
                        .Create()

                let target: IRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.GetBlogEntriesAsync()
                    |> Async.RunSynchronously

                // assert
                Expect.equal result entries ""

            testCase "BlogRepository.GetBlogEntryAsync: returns None when no results match"
            <| fun _ ->
                // arrange
                let slug = "blah-blah"

                let context = EmptyContext()

                let target: IRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.GetBlogEntryAsync slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result ""

            testCase "BlogRepository.GetBlogEntryAsync: returns Some if more than one result"
            <| fun _ ->
                // arrange
                let expected = BlogEntry.create "one"

                let entries =
                    [
                        expected
                    ]

                let context =
                    Mock<IContext>()
                        .Setup(fun r -> <@ r.GetByValue "blogentries" "Slug" expected.Slug @>)
                        .Returns(entries |> Seq.ofList |> async.Return)
                        .Create()

                let target: IRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.GetBlogEntryAsync expected.Slug
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Should return Some result."
                | Some r -> Expect.equal r expected ""

        ]
