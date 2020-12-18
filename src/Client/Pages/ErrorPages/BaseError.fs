module Client.Pages.BaseError

open Client.Styles
open Feliz
open Feliz.Bulma

let private layout (contents: ReactElement) =
    Bulma.section [
        Bulma.columns [
            Bulma.column [
                column.isOffsetOneQuarter
                column.isHalf
                prop.children contents
            ]
        ]
    ]

let private messageBlock color (headMsg: string) (bodyMsg: string) =
    Bulma.message [
        prop.classes [
            color
            Bulma.IsMedium
            Bulma.HasTextCentered
        ]
        prop.children [
            Bulma.messageHeader [
                prop.text headMsg
            ]
            Bulma.messageBody [ prop.text bodyMsg ]
        ]
    ]

let private imageBlock src =
    Html.div [
        prop.className Bulma.HasTextCentered
        prop.children [
            Bulma.image [
                prop.className Bulma.IsInlineBlock
                prop.children [
                    Html.img [ prop.src src ]
                ]
            ]
        ]
    ]

let render color headerMsg bodyMsg imgSrc =
    Bulma.section [
        messageBlock color headerMsg bodyMsg |> layout
        imageBlock imgSrc
    ]
