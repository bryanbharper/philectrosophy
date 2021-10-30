module Client.Interop

open Fable.Core

type ILocation = { mutable href: string; mutable pathname: string }
type IHistory = { pushState: string ->  string -> string -> unit }
type IWindow = { location: ILocation; history: IHistory }

let private setWindowHref (window: IWindow) href =
    window.location.href <- href

[<Global>]
let private window : IWindow = jsNative

let setHref href =
    href
    |> setWindowHref window

let setPath path =
    window.history.pushState "" "" path
