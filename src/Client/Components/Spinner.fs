module Client.Components.Spinner

open Feliz
open Styles

let render =
    Html.div [
        prop.className [
            Bulma.HasTextCentered
            "mt-6"
        ]
        prop.children
            [
                Html.i
                    [
                        prop.classes [
                            FA.Fa
                            FA.FaCog
                            FA.FaSpin
                            FA.Fa3X
                        ]
                    ]
            ]
    ]
