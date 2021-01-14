module Client.Pages.Search

open Client.Components
open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Bulma
open Shared

let blogApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IBlogApi>

type State =
    {
        Query: string option
        Results: Deferred<BlogEntry list>
    }

type Msg =
    | Emptied
    | ResultReceived of BlogEntry list
    | Submit
    | SetQuery of string
    | ApiError of exn

let init (): State * Cmd<Msg> =
    { Query = None; Results = Idle }, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | Emptied ->
        { state with Results = Idle }, Cmd.none
    | ResultReceived entries ->
        { state with
            Results = Resolved entries
        },
        Cmd.none
    | Submit ->
        match state.Query with
        | None -> state, Cmd.none
        | Some query ->
            { state with Results = InProgress },
            Cmd.OfAsync.either blogApi.GetSearchResults query ResultReceived ApiError
    | SetQuery query ->
        match query with
        | "" ->
            { state with Results = Idle; Query = None }, Cmd.none
        | _ ->
            { state with Query = Some query }, Cmd.none
    | ApiError err -> { state with Results = Resolved [] }, Cmd.none

let input dispatch =
    Bulma.input.search [
        input.isRounded
        prop.onChange (SetQuery >> dispatch)
        prop.onKeyDown (fun ke -> if ke.key = "Enter" then dispatch Submit)
        prop.onEmptied (fun _ -> dispatch Emptied)
    ]

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    let results =
        match state.Results with
        | Idle -> Html.none
        | InProgress -> Spinner.render
        | Resolved results' ->
            match results' with
            | [] -> Bulma.subtitle.h4 "No results found..."
            | _ ->
                results'
                |> List.map EntryMedia.render
                |> Html.div

    Bulma.section [
        Bulma.container [
            Bulma.title.h2 [ prop.text "Search" ]
            Bulma.level [
                prop.children [ input dispatch ]
            ]
            results
        ]

    ]
