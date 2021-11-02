module Server.Tests.Api.SongApi

open Fable.Mocha
open Expecto
open Foq

open Shared

open Server.Data.SongRepository
open Server.Api

let all =
    testList
        "SongApi Tests"
        [
            testCase "SongApi.getSongsAsync: returns all songs"
            <| fun _ ->
                // arrange
                let expected = [ Song.create "A"; Song.create "B"; Song.create "C" ]

                let repo =
                    Mock<ISongRepository>()
                        .Setup(fun r -> <@ r.GetAll() @>)
                        .Returns(expected |> async.Return)
                        .Create()

                // act
                let result =
                    SongApi.getSongsAsync repo
                    |> Async.RunSynchronously

                // assert
                Expect.sequenceContainsOrder result expected ""

            testCase "SongApi.updateListenCount: returns updated song from repo"
            <| fun _ ->
                // arrange
                let stored = Song.create "Tidal Wave"
                let expected = { stored with PlayCount = stored.PlayCount + 1 }

                let repo =
                    Mock<ISongRepository>()
                        .Setup(fun r -> <@ r.GetSingle stored.Slug @>)
                        .Returns(stored |> Some |> async.Return)
                        .Setup(fun r -> <@ r.Update expected @>)
                        .Returns(expected |> Some |> async.Return)
                        .Create()

                // act
                let result =
                    stored.Slug
                    |> SongApi.updateListenCount repo
                    |> Async.RunSynchronously

                // assert
                match result with
                | Some r -> Expect.equal r expected ""
                | None -> failtest "There should have been a result. Function returned None"

            testCase "SongApi.updateListenCount: returns None if specified song doesn't exist"
            <| fun _ ->
                // arrange
                let slug = "some-slug"

                let repo =
                    Mock<ISongRepository>()
                        .Setup(fun r -> <@ r.GetSingle slug @>)
                        .Returns(None |> async.Return)
                        .Create()

                // act
                let result =
                    slug
                    |> SongApi.updateListenCount repo
                    |> Async.RunSynchronously

                // assert
                match result with
                | Some _ -> failtest "Function should have returned None"
                | None -> ()
        ]
