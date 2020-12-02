module Client.Components.MediaObject

open Browser.Types
open Feliz
open Styles

type MediaData =
    {
        Contents: ReactElement list
        ImageUrl: string
        OnClick: MouseEvent -> unit
    }

let private figure imgUrl =
    Html.figure
        [
            prop.className Bulma.MediaLeft
            prop.children
                [
                    Html.p
                        [
                            prop.classes [ Bulma.Image; Bulma.Is64X64 ]
                            prop.children
                                [
                                    Html.img
                                        [
                                            prop.src imgUrl
                                        ]
                                ]
                        ]
                ]
        ]

let private content (contents: ReactElement list) =
    Html.div
        [
            prop.className Bulma.MediaContent
            prop.children
                [
                    Html.div
                        [
                            prop.className Bulma.Content
                            prop.children contents
                        ]
                ]
        ]

let render imageUrl contents onClick =
    Html.article
        [
            prop.onClick onClick
            prop.classes [
                Bulma.Media
                Style.Clickable
            ]
            prop.children
                [
                    figure imageUrl
                    content contents
                ]
        ]
