module Client.Pages.Blog

open Client.Components
open Client.Styles
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
    | GotEntries of BlogEntry list
    | ApiError of exn

let init (): State * Cmd<Msg> =
    { Entries = InProgress }, Cmd.OfAsync.either blogApi.GetEntries () GotEntries ApiError

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | GotEntries result -> { state with Entries = Resolved result }, Cmd.none
    | ApiError _ -> state, Url.UnexpectedError.asString |> Cmd.navigate

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
