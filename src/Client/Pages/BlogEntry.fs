module Client.Pages.BlogEntry

open Elmish
open Feliz

type State = { Slug: string }

type Msg = unit

let init (slug: string): State * Cmd<Msg> =
    { Slug = slug }, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> = state, Cmd.none

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    state.Slug
    |> sprintf "Blog Entry - %s: Under Construction"
    |> Html.h1
