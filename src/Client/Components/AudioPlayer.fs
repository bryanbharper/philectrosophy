module Client.Components.AudioPlayer

open Fable.Core
open Fable.Core.JsInterop
open Feliz

[<Erase>]
type player =
    static member inline autoPlay (value: bool) = Interop.mkAttr "autoPlay" value
    static member inline autoPlayAfterSrcChange (value: bool) = Interop.mkAttr "autoPlayAfterSrcChange" value
    static member inline loop (value: bool) = Interop.mkAttr "loop" value
    static member inline onClickNext (value: unit -> unit) = Interop.mkAttr "onClickNext" value
    static member inline onClickPrevious (value: unit -> unit) = Interop.mkAttr "onClickPrevious" value
    // static member inline onPlay (value: string -> unit) = Interop.mkAttr "onPlay" value
    static member inline onEnded (value: unit -> unit) = Interop.mkAttr "onEnded" value
    static member inline showDownloadProgress (value: bool) = Interop.mkAttr "showDownloadProgress" value
    static member inline showJumpControls (value: bool) = Interop.mkAttr "showJumpControls" value
    static member inline showSkipControls (value: bool) = Interop.mkAttr "showSkipControls" value
    static member inline src (value: string) = Interop.mkAttr "src" value

type AudioPlayer =
    static member inline render (props: IReactProperty list) =
        Interop.reactApi.createElement (importDefault "react-h5-audio-player", createObj !!props)
