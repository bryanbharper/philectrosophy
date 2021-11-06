module Client.Tests.Music

open Client.Pages.Music
open Fable.Mocha

open Shared

let all =
    testList "Music.Component"
        [
            testCase "getNextTrack: gets next song when in range"
            <| fun _ ->
                // arrange
                let current = { Song.create "current" with Placement = 1 }
                let next = { Song.create "next" with Placement = current.Placement + 1 }
                let playlist = [
                    current
                    { Song.create "rand" with Placement = 0 }
                    next
                ]

                // act
                let result =
                    current
                    |> Component.getNextTrack playlist

                // assert
                Expect.equal result next "Result should be next track."

            testCase "getNextTrack: gets next song when outside range"
            <| fun _ ->
                // arrange
                let current = { Song.create "last" with Placement = 2 }
                let next = { Song.create "first" with Placement = 0 }
                let playlist = [
                    current
                    { Song.create "rand" with Placement = 1 }
                    next
                ]

                // act
                let result =
                    current
                    |> Component.getNextTrack playlist

                // assert
                Expect.equal result next "Result should be next track."

            testCase "getPrevTrack: gets previous song when in range"
            <| fun _ ->
                // arrange
                let current = { Song.create "current" with Placement = 2 }
                let previous = { Song.create "next" with Placement = current.Placement - 1 }
                let playlist = [
                    current
                    { Song.create "rand" with Placement = 0 }
                    previous
                ]

                // act
                let result =
                    current
                    |> Component.getPrevTrack playlist

                // assert
                Expect.equal result previous "Result should be previous track."

            testCase "getPrevTrack: gets previous song when outside range"
            <| fun _ ->
                // arrange
                let current = { Song.create "first" with Placement = 0 }
                let next = { Song.create "last" with Placement = 2 }
                let playlist = [
                    current
                    { Song.create "rand" with Placement = 1 }
                    next
                ]

                // act
                let result =
                    current
                    |> Component.getPrevTrack playlist

                // assert
                Expect.equal result next "Result should be previous track."

            testCase "findBySlug: finds track with matching slug"
            <| fun _ ->
                // arrange
                let slug = "this-is-a-slug-not-a-snail"
                let expected = { Song.create "a" with Slug = slug }
                let playlist = [
                    { Song.create "b" with Slug = "a" }
                    expected
                    { Song.create "c" with Slug = "b" }
                ]

                // act
                let result =
                    playlist |> Component.findBySlug slug

                // assert
                Expect.equal result expected "Result should be previous track."

        ]
