module Client.Pages.Blog

open Client.Components
open Client.Urls
open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Bulma
open Feliz.Router
open Shared

let blogApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IBlogApi>

type State = { Entries: Deferred<BlogEntry list> }

type Msg =
    | ServerReturnedEntries of BlogEntry list
    | ServerReturnedError of exn

let init (): State * Cmd<Msg> =
    { Entries = InProgress }, Cmd.OfAsync.either blogApi.GetEntries () ServerReturnedEntries ServerReturnedError

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ServerReturnedEntries result -> { state with Entries = Resolved result }, Cmd.none
    | ServerReturnedError _ -> state, Url.UnexpectedError.asString |> Cmd.navigatePath

let render (state: State) (dispatch: Msg -> unit) =
    let entries =
        match state.Entries with
        | Idle -> Html.none
        | InProgress -> Spinner.render
        | Resolved entries ->
            entries
            |> List.map EntryMedia.render
            |> Html.div

    Bulma.section [
        Bulma.container [
            Bulma.title.h2 [ prop.text "Blog" ]
            Html.hr []
            entries
        ]
    ]
