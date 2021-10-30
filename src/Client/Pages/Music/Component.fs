module Client.Pages.Music.Component

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

let findBySlug slug songs =
    songs |> List.find (fun s -> s.Slug = slug)

let updatePath slug =
    slug |> sprintf "music/%s" |> Interop.setPath

let songApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ISongApi>

////////// INIT ///////////
let init () =
    {
        CurrentSong = None
        Shuffle = false
        Songs = Deferred.InProgress
    },
    Cmd.OfAsync.either songApi.GetSongs () ServerReturnedSongs ServerReturnedError

////////// UPDATE ///////////
let update (msg: Msg) (state: State) =
    let currentSlug =
        match Router.currentPath() with
        | [ _; slug ] -> Some slug
        | _ -> None

    match msg with
    | ServerReturnedError _ -> state, Url.UnexpectedError.asString |> Cmd.navigatePath
    | ServerReturnedSongs songs ->
        let current =
            match currentSlug with
            | None -> songs |> List.find (fun s -> s.Placement = 0)
            | Some slug -> songs |> findBySlug slug

        current.Slug |> updatePath

        { state with
            Songs = songs |> List.sortBy (fun s -> s.Placement) |> Resolved
            CurrentSong = Some current
        },
        Cmd.none
    | TrackEnded
    | UserClickedNext ->
        match state.Songs, state.CurrentSong with
        | Resolved songs, Some song ->
            let nextSong =
                if state.Shuffle
                then List.rand songs
                else getNextTrack songs song

            nextSong.Slug |> updatePath

            { state with
                CurrentSong = Some nextSong
            }, Cmd.none
        | _ -> state, Cmd.none
    | UserClickedPrevious ->
        match state.Songs, state.CurrentSong with
        | Resolved songs, Some song ->
            let prevTrack = getPrevTrack songs song
            prevTrack.Slug |> updatePath

            { state with
                CurrentSong = Some prevTrack
            }, Cmd.none

        | _ -> state, Cmd.none
    | UserClickedShuffleBtn -> { state with Shuffle = not state.Shuffle }, Cmd.none
    | UserClickedTrack track ->
        track.Slug |> updatePath
        { state with CurrentSong = Some track; }, Cmd.none
