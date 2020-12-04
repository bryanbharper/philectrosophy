module BaseError

open Client.Components.Layout
open Feliz
open Styles

let private layout (contents: ReactElement) =
    Html.div [
        prop.className Bulma.Columns
        prop.children [
            Html.div [ prop.className Bulma.Column ]
            Html.div [
                prop.classes [
                    Bulma.Column
                    Bulma.IsHalf
                ]
                prop.children contents
            ]
            Html.div [ prop.className Bulma.Column ]
        ]
    ]

let private messageBlock color (headMsg: string) (bodyMsg: string) =
    Html.article [
        prop.classes [
            Bulma.Message
            color
            Bulma.IsMedium
            Bulma.HasTextCentered
        ]
        prop.children [
            Html.div [
                prop.className Bulma.MessageHeader
                prop.text headMsg
            ]
            Html.div [
                prop.className Bulma.MessageBody
                prop.text bodyMsg
            ]
        ]
    ]

let private imageBlock src =
    Html.div [
        prop.className Bulma.HasTextCentered
        prop.children
            [
                Html.figure [
                    prop.classes [
                        Bulma.Image
                        Bulma.IsInlineBlock
                    ]
                    prop.children [ Html.img [ prop.src src ] ]
                ]
            ]
    ]

let render color headerMsg bodyMsg imgSrc =
    Section.render [
        messageBlock color headerMsg bodyMsg
        |> layout
        imageBlock imgSrc
    ]
