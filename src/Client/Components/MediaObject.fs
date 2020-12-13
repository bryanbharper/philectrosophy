module Client.Components.MediaObject

open Feliz
open Feliz.Bulma

let render imgUrl (contents: ReactElement list) =
    Bulma.media [
        Bulma.mediaLeft
            [
                prop.children
                    [
                        Bulma.image [
                            image.is96x96
                            prop.children [ Html.img [ prop.src imgUrl ] ]
                        ]
                    ]
            ]

        Bulma.mediaContent [ Bulma.content contents ]
    ]
