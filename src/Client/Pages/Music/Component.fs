module Client.Pages.Music.Component

open System
open Elmish
open Client.Pages.Music.Types
open Client.Extensions

let playlist =
    [
        { Title = "The Cannonball"; Src = "songs/TheCannonball.mp3" }
        { Title = "Head Drama"; Src = "songs/HeadDrama.mp3" }
        { Title = "Between Shifts"; Src = "songs/Between Shifts.mp3" }
        { Title = "Philanthropic Nuclear Masterpiece"; Src = "songs/PhilanthropicNuclearMasterpiece.mp3" }
        { Title = "Orphan"; Src = "songs/Orphan.mp3" }
        { Title = "L'Inconnue De La Sein"; Src = "songs/L'Inconnue De La Sein.mp3" }
    ]

let playlistLength = List.length playlist

let getIndex track =
    playlist
    |> List.findIndex (fun t -> t.Title = track.Title)

let nextTrack track =
    let nextIndex =
        track
        |> getIndex
        |> (+)1

    playlist.[nextIndex % playlistLength]

let previousTrack track =
    let nextIndex =
        track
        |> getIndex
        |> (-)1

    playlist.[nextIndex % playlistLength]

////////// INIT ///////////
let init() =
    { CurrentTrack = playlist |> List.head; Shuffle = false }, Cmd.none

////////// UPDATE ///////////
let update (msg: Msg) (state: State) =
    match msg with
    | TrackEnded
    | UserClickedNext ->
        if state.Shuffle
        then { state with CurrentTrack = playlist |> List.rand}, Cmd.none
        else { state with CurrentTrack = state.CurrentTrack |> nextTrack }, Cmd.none
    | UserClickedPrevious ->
        { state with CurrentTrack = state.CurrentTrack |> previousTrack }, Cmd.none
    | UserClickedShuffleBtn -> { state with Shuffle = not state.Shuffle }, Cmd.none
    | UserClickedTrack  track ->
        { state with CurrentTrack = track }, Cmd.none
