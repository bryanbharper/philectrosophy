module Client.Components.Spinner

open Feliz
open Styles

let render =
    Html.div [
        prop.style [
            style.textAlign.center
            style.marginTop 20
        ]
        prop.children [
            Html.i [
                prop.classes [
                    FA.Fa
                    FA.FaCog
                    FA.FaSpin
                    FA.Fa2X
                ]
            ]
        ]
    ]
