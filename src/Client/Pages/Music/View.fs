module Client.Pages.Music.View

open Client.Components
open Client.Pages.Music.Types
open Feliz
open Feliz.Bulma
open Client.Styles
open Client.Components.AudioPlayer
open Feliz.Bulma
open Shared

let shuffleBtn dispatch state =
    Html.button [
        prop.onClick (fun _ -> dispatch Msg.UserClickedShuffleBtn)
        prop.classes [ Bulma.Button; Bulma.IsLarge; Style.Rounded; Bulma.My3; if state.Shuffle then Bulma.IsPrimary ]
        prop.children [
            Html.span [
                prop.classes [ Bulma.Icon; Bulma.IsLarge ]
                prop.children [
                    Html.i [
                        prop.classes [ FA.Fa; FA.FaRandom ]
                    ]
                ]
            ]
        ]
    ]

let playlistTable dispatch playlist (current: Song) =
    let trackRow track =
        Html.tr [
            prop.classes [
                if current.Title = track.Title then Bulma.IsSelected
                Style.Clickable
            ]
            prop.children [
                Html.td [
                    track.Placement
                    |> (+)1
                    |> prop.text
                ]
                Html.td [
                    track.Title
                    |> prop.text
                ]
                Html.td [
                    let coverText =
                        match track.CoverOf with
                        | None -> ""
                        | Some coverBand -> coverBand |> sprintf "by %s"
                    prop.text coverText
                    prop.classes [ Bulma.HasTextGrey ]
                ]
            ]
            prop.onClick (fun _ -> track |> Msg.UserClickedTrack |> dispatch)
        ]

    Html.table [
        prop.classes [ Bulma.Table ]
        prop.children [
            Html.thead [
                Html.tr [
                    Html.th "#"
                    Html.th "Title"
                    Html.th ""
                ]
            ]
            Html.tbody [
                playlist
                |> List.map trackRow
                |> prop.children
            ]
        ]
    ]

////////// RENDER ///////////
let render (state: State) (dispatch: Msg -> unit) =
    Bulma.section [
        Bulma.container [
            Bulma.title.h2 [ prop.text "Tunes" ]

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

            Html.br []
            shuffleBtn dispatch state

            match state.Songs, state.CurrentSong with
            | Idle, _ -> Html.none
            | InProgress, _ -> Spinner.render
            | Resolved songs, Some current ->
                Html.div [
                    playlistTable dispatch songs current

                    AudioPlayer.render [
                        player.src current.Path
                        player.autoPlay true
                        player.autoPlayAfterSrcChange true
                        player.showSkipControls true
                        player.onClickNext (fun () -> Msg.UserClickedNext |> dispatch)
                        player.onClickPrevious (fun () -> Msg.UserClickedPrevious |> dispatch)
                        player.onEnded (fun _ -> Msg.TrackEnded |> dispatch)
                    ]
                ]
            | Resolved songs, None ->
                Html.div [
                    let current' = songs |> List.head
                    playlistTable dispatch songs current'

                    AudioPlayer.render [
                        player.src current'.Path
                        player.autoPlay true
                        player.autoPlayAfterSrcChange true
                        player.showSkipControls true
                        player.onClickNext (fun () -> Msg.UserClickedNext |> dispatch)
                        player.onClickPrevious (fun () -> Msg.UserClickedPrevious |> dispatch)
                        player.onEnded (fun _ -> Msg.TrackEnded |> dispatch)
                    ]
                ]


        ]
    ]
