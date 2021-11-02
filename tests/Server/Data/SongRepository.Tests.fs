module Server.Tests.Data.SongRepository


open Expecto
open Foq

open Server.Data.SongRepository
open Shared

open Server.Data

type EmptyContext() =
    interface IContext with
        member this.GetTable<'a> _ =
            Seq.empty<'a> |> async.Return

        member this.GetByValue<'a, 'b> _ _ (_: 'a) =
            Seq.empty<'b> |> async.Return

        member this.Update _ _ _ _ = 0 |> async.Return

let all =
    testList
        "Song Repository"
        [

            testCase "GetAll: returns all songs in context."
            <| fun _ ->
                // arrange
                let songs =
                    [
                        Song.create "one"
                        Song.create "two"
                        Song.create "three"
                    ]

                let context =
                    Mock<IContext>()
                        .Setup(fun r -> <@ r.GetTable Tables.Songs.name @>)
                        .Returns(songs |> Seq.ofList |> async.Return)
                        .Create()

                let target: ISongRepository = upcast SongRepository(context)

                // act
                let result =
                    target.GetAll()
                    |> Async.RunSynchronously

                // assert
                Expect.equal result songs ""

            testCase "GetSingle: returns None when no results match"
            <| fun _ ->
                // arrange
                let slug = "blah-blah"

                let context = EmptyContext()

                let target: ISongRepository = upcast SongRepository(context)

                // act
                let result =
                    target.GetSingle slug
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result ""

            testCase "GetSingle: returns Some if more than one result"
            <| fun _ ->
                // arrange
                let expected = Song.create "one"

                let songs =
                    [
                        expected
                    ]

                let context =
                    Mock<IContext>()
                        .Setup(fun r -> <@ r.GetByValue Tables.Songs.name Tables.Songs.id expected.Slug @>)
                        .Returns(songs |> Seq.ofList |> async.Return)
                        .Create()

                let target: ISongRepository = upcast SongRepository(context)

                // act
                let result =
                    target.GetSingle expected.Slug
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Should return Some result."
                | Some r -> Expect.equal r expected ""

            testCase "Update: returns None if slug not found"
            <| fun _ ->
                // arrange
                let context = EmptyContext()

                let target: ISongRepository = upcast SongRepository(context)

                // act
                let result =
                    target.Update (Song.create "I Don't Exist")
                    |> Async.RunSynchronously

                // assert
                Expect.isNone result ""

            testCase "Update: returns updated result when present"
            <| fun _ ->
                // arrange
                let expected = Song.create "one"

                let context =
                    Mock<IContext>()
                        .Setup(fun r -> <@ r.Update Tables.Songs.name Tables.Songs.id expected.Slug expected @>)
                        .Returns(1 |> async.Return)
                        .Create()

                let target: ISongRepository = upcast SongRepository(context)

                // act
                let result =
                    target.Update expected
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Should return Some result."
                | Some e -> Expect.equal e expected ""

        ]
