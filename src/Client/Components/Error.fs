module Client.Components.Error

open Feliz

let render (errorMsg: string) =
    Html.h1 [
        prop.style [ style.color.red ]
        prop.text errorMsg
    ]
