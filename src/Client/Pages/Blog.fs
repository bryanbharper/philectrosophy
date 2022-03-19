module Client.Pages.Blog

open Fable.Remoting.Client
open Elmish
open Elmish.Navigation
open Feliz
open Feliz.Router
open Feliz.Bulma

open Client.Components
open Client.Urls

open Shared
open Shared.Dtos
open Shared.Contracts

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
    | ServerReturnedError _ -> state, Url.UnexpectedError.asString |> Navigation.newUrl

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
