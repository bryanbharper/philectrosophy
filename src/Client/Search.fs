module Client.Pages.Search

open Elmish
open Feliz
open Client.Pages

type State = unit

type Msg = unit

let init (): State * Cmd<Msg> = (), Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> = (), Cmd.none

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    NotImplemented.render
