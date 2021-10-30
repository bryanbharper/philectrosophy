﻿module Client.Pages.Music.Component

open Client
open Fable.Remoting.Client
open Elmish
open Feliz.Router

open Shared

open Client.Urls
open Client.Extensions
open Client.Pages.Music.Types

let (-%) n m =
    match n % m with
    | i when i >= 0 -> i
    | i -> abs m + i

let getNextTrack (playlist: Song list) current =
    let nextIndex =
        current.Placement + 1 -% (List.length playlist)

    playlist.[nextIndex]

let getPrevTrack (playlist: Song list) current =
    let prevIndex =
        current.Placement - 1 -% (List.length playlist)

    playlist.[prevIndex]

let findBySlug slug playlist =
    playlist |> List.find (fun t -> t.Slug = slug)

let updatePath slug =
    slug |> sprintf "music/%s" |> Interop.setPath

let songApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ISongApi>

////////// INIT ///////////
let init () =
    {
        CurrentTrack = None
        PlayerState = Stopped
        Shuffle = false
        Playlist = InProgress
    },
    Cmd.OfAsync.either songApi.GetSongs () ServerReturnedTracks ServerReturnedError

////////// UPDATE ///////////
let update (msg: Msg) (state: State) =
    let currentSlug =
        match Router.currentPath () with
        | [ _; slug ] -> Some slug
        | _ -> None

    match msg with
    | ServerReturnedError _ -> state, Url.UnexpectedError.asString |> Cmd.navigatePath
    | ServerReturnedTracks playlist ->
        let current =
            match currentSlug with
            | None -> playlist |> List.find (fun t -> t.Placement = 0)
            | Some slug -> playlist |> findBySlug slug

        current.Slug |> updatePath

        { state with
            Playlist =
                playlist
                |> List.sortBy (fun t -> t.Placement)
                |> Resolved
            CurrentTrack = Some current
        },
        Cmd.none
    | ServerUpdatedTrackPlayCount (Some serverTrack) ->
        let newState =
            match state.Playlist with
            | Idle -> state
            | InProgress -> state
            | Resolved playlist ->
                let newPlaylist =
                    playlist
                    |> List.filter (fun t -> t.Slug <> serverTrack.Slug)
                    |> List.append [ serverTrack ]
                    |> List.sortBy (fun t -> t.Placement)

                { state with
                    Playlist = Resolved newPlaylist
                }

        newState, Cmd.none
    | TrackEnded
    | UserClickedNext ->
        match state.Playlist, state.CurrentTrack with
        | Resolved playlist, Some current ->
            let nextTrack =
                if state.Shuffle then
                    List.rand playlist
                else
                    getNextTrack playlist current

            nextTrack.Slug |> updatePath

            { state with
                CurrentTrack = Some nextTrack
                PlayerState = Playing
            },
            Cmd.none
        | _ -> state, Cmd.none
    | UserClickedPause -> { state with PlayerState = Paused }, Cmd.none
    | UserClickedPlay ->
        match state.CurrentTrack with
        | Some currentTrack ->
            let cmd =
                match state.PlayerState with
                | Paused -> Cmd.none
                | _ -> Cmd.OfAsync.perform songApi.UpdateListenCount currentTrack.Slug ServerUpdatedTrackPlayCount

            { state with PlayerState = Playing }, cmd
        | _ -> state, Cmd.none
    | UserClickedPrevious ->
        match state.Playlist, state.CurrentTrack with
        | Resolved playlist, Some currentTrack ->
            let prevTrack = getPrevTrack playlist currentTrack
            prevTrack.Slug |> updatePath

            { state with
                CurrentTrack = Some prevTrack
                PlayerState = Playing
            },
            Cmd.none

        | _ -> state, Cmd.none
    | UserClickedShuffleBtn ->
        { state with
            Shuffle = not state.Shuffle
        },
        Cmd.none
    | UserClickedTrack track ->
        track.Slug |> updatePath

        { state with
            CurrentTrack = Some track
            PlayerState = Playing
        },
        Cmd.none
    | _ -> state, Cmd.none
