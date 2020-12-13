module Client.Components.Layout

open Feliz
open Styles

module Container =
    let render (contents: ReactElement list) =
        Html.div [
            prop.className Bulma.Container
            prop.children contents
        ]

module Section =
    let render (contents: ReactElement list): Fable.React.ReactElement =
        Html.section
            [
                prop.className Bulma.Section
                prop.children contents
            ]
