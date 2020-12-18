module Client.Components.Spinner

open Client.Styles
open Feliz

let render =
    Html.div [
        prop.className [
            Bulma.HasTextCentered
            Bulma.Mt6
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
