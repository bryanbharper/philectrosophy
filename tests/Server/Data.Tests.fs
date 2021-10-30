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

        member this.Update _ _ _ _ = 0 |> async.Return

let all =
    testList
        "Data Tests"
        [

            testCase "BlogRepository.GetAll: returns all entries in context."
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
                        .Setup(fun r -> <@ r.GetTable Tables.BlogEntries.name @>)
                        .Returns(entries |> Seq.ofList |> async.Return)
                        .Create()

                let target: IBlogRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.GetAll()
                    |> Async.RunSynchronously

                // assert
                Expect.equal result entries ""

            testCase "BlogRepository.GetSingle: returns None when no results match"
            <| fun _ ->
                // arrange
                let slug = "blah-blah"

                let context = EmptyContext()

                let target: IBlogRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.GetSingle slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result ""

            testCase "BlogRepository.GetSingle: returns Some if more than one result"
            <| fun _ ->
                // arrange
                let expected = BlogEntry.create "one"

                let entries =
                    [
                        expected
                    ]

                let context =
                    Mock<IContext>()
                        .Setup(fun r -> <@ r.GetByValue Tables.BlogEntries.name Tables.BlogEntries.id expected.Slug @>)
                        .Returns(entries |> Seq.ofList |> async.Return)
                        .Create()

                let target: IBlogRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.GetSingle expected.Slug
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Should return Some result."
                | Some r -> Expect.equal r expected ""

            testCase "BlogRepository.Update: returns None if slug not found"
            <| fun _ ->
                // arrange
                let context = EmptyContext()

                let target: IBlogRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.Update (BlogEntry.create "I Don't Exist")
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result ""

            testCase "BlogRepository.Update: returns updated result when present"
            <| fun _ ->
                // arrange
                let expected = BlogEntry.create "one"

                let context =
                    Mock<IContext>()
                        .Setup(fun r -> <@ r.Update Tables.BlogEntries.name Tables.BlogEntries.id expected.Slug expected @>)
                        .Returns(1 |> async.Return)
                        .Create()

                let target: IBlogRepository = upcast BlogRepository(context)

                // act
                let result =
                    target.Update expected
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Should return Some result."
                | Some e -> Expect.equal e expected ""

        ]
