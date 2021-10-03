module Client.Pages.Music

open Client.Components
open Elmish
open Feliz

type Track =
    {
        Title: string
        Src: string
    }

let playlist =
    [
        { Title = "The Cannonball"; Src = "songs/TheCannonball.mp3" }
        { Title = "Head Drama"; Src = "songs/HeadDrama.mp3" }
        { Title = "Philanthropic Nuclear Masterpiece"; Src = "songs/PhilanthropicNuclearMasterpiece.mp3" }
    ]

let playlistLength = List.length playlist

let getIndex title =
    playlist
    |> List.findIndex (fun t -> t.Title = title)

let nextTrack title =
    let nextIndex =
        title
        |> getIndex
        |> (+)1

    playlist.[nextIndex % playlistLength]

let previousTrack title =
    let nextIndex =
        title
        |> getIndex
        |> (-)1

    playlist.[nextIndex % playlistLength]

/////////////////////////////////////////////////////
/// Elmish
/////////////////////////////////////////////////////
type State =
    {
        CurrentTrack: Track option
    }

type Msg =
    | TrackEnded
    | UserClickedNext
    | UserClickedPlayBtn
    | UserClickedPrevious
    | UserClickedStopBtn
    | UserClickedTrack of Track

let init() =
    { CurrentTrack = None }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | TrackEnded
    | UserClickedNext ->
        match state.CurrentTrack with
        | Some current ->
            { state with CurrentTrack = current.Title |> nextTrack |> Some }, Cmd.none
        | _ -> state, Cmd.none
    | UserClickedPlayBtn ->
        { state with CurrentTrack = playlist |> List.head |> Some }, Cmd.none
    | UserClickedPrevious ->
        match state.CurrentTrack with
        | Some current ->
            { state with CurrentTrack = current.Title |> previousTrack |> Some }, Cmd.none
        | _ -> state, Cmd.none
    | UserClickedStopBtn ->
        { state with CurrentTrack = None }, Cmd.none
    | UserClickedTrack  track ->
        { state with CurrentTrack = Some track }, Cmd.none

////////// RENDER ///////////
open Feliz.Bulma
open Client.Styles
open Client.Components.AudioPlayer

let playlistTable dispatch (current: Track option) =
    let trackRow isSelected track =
        Html.tr [
            prop.classes [
                if isSelected then Bulma.IsSelected
                Style.Clickable
            ]
            prop.children [
                Html.td [
                    getIndex track.Title
                    |> (+)1
                    |> prop.text
                ]
                Html.td [
                    track.Title
                    |> prop.text
                ]
            ]
            prop.onClick (fun _ -> track |> Msg.UserClickedTrack |> dispatch)
        ]

    let isSelected target =
        match current with
        | None -> false
        | Some curr -> curr.Title = target.Title

    Html.table [
        prop.classes [ Bulma.Table ]
        prop.children [
            Html.thead [
                Html.tr [
                    Html.th "#"
                    Html.th "Title"
                ]
            ]
            Html.tbody [
                playlist
                |> List.map (fun t -> trackRow (isSelected t) t)
                |> prop.children
            ]
        ]
    ]

let bigBtn dispatch state =
    let icon, color, msg =
        if Option.isNone state.CurrentTrack
        then FA.FaPlay, Bulma.IsSuccess, Msg.UserClickedPlayBtn
        else FA.FaStop, Bulma.IsDanger, Msg.UserClickedStopBtn

    Html.button [
        prop.onClick (fun _ -> dispatch msg)
        prop.classes [ Bulma.Button; color; Bulma.IsLarge; Style.Rounded; Bulma.My3 ]
        prop.children [
            Html.span [
                prop.classes [ Bulma.Icon; Bulma.IsLarge ]
                prop.children [
                    Html.i [
                        prop.classes [ FA.Fa; icon ]
                    ]
                ]
            ]
        ]
    ]

let render (state: State) (dispatch: Msg -> unit) =
    Bulma.section [
        Bulma.container [
            Bulma.title.h2 [ prop.text "Tunes" ]
            Html.hr []

            Html.div [
                Html.p [
                    Html.strong "Note: "
                    Html.text "Headphones recommended."
                ]
                Html.p [
                    Html.strong "Another Note: "
                    Html.text "I don't claim to be good at music. Please don't judge me too harshly."
                ]
            ]

            bigBtn dispatch state

            playlistTable dispatch state.CurrentTrack
            Html.hr []

            match state.CurrentTrack with
            | None ->
                Html.div [
                    prop.classes [ Bulma.Notification ]
                    prop.children [
                        Html.strong "Nothing to Play: "
                        Html.text "Select a track or hit play..."
                    ]
                ]
            | Some current ->
                AudioPlayer.render [
                    player.src current.Src
                    player.autoPlay true
                    player.autoPlayAfterSrcChange true
                    player.showSkipControls true
                    player.onClickNext (fun () -> Msg.UserClickedNext |> dispatch)
                    player.onClickPrevious (fun () -> Msg.UserClickedPrevious |> dispatch)
                    player.onEnded (fun _ -> Msg.TrackEnded |> dispatch)
                ]
        ]
    ]
