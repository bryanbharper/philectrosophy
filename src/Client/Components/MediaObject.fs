module Client.Components.MediaObject

open Feliz
open Styles

let private figure imgUrl =
    Html.figure [
        prop.className Bulma.MediaLeft
        prop.children
            [
                Html.p [
                    prop.classes [
                        Bulma.Image
                        Bulma.Is96X96
                    ]
                    prop.children [ Html.img [ prop.src imgUrl ] ]
                ]
            ]
    ]

let private content (contents: ReactElement list) =
    Html.div [
        prop.className Bulma.MediaContent
        prop.children
            [
                Html.div [
                    prop.className Bulma.Content
                    prop.children contents
                ]
            ]
    ]

let render imageUrl contents =
    Html.article [
        prop.className Bulma.Media
        prop.children [
            figure imageUrl
            content contents
        ]
    ]
