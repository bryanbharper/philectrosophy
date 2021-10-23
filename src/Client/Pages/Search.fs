module Client.Pages.Search

open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Bulma

open Shared

open Client.Apis
open Client.Components

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
    | ServerReturnedBlogEntries of BlogEntry list
    | ServerReturnedError of exn
    | UserChangedInput of string
    | UserClearedSearch
    | UserClickedSubmit

let init (): State * Cmd<Msg> =
    { Query = None; Results = Idle }, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ServerReturnedError _ -> { state with Results = Resolved [] }, Cmd.none
    | ServerReturnedBlogEntries entries ->
        { state with
            Results = Resolved entries
        },
        Cmd.none
    | UserChangedInput query ->
        match query with
        | "" ->
            { state with Results = Idle; Query = None }, Cmd.none
        | _ ->
            { state with Query = Some query }, Cmd.none
    | UserClearedSearch ->
        { state with Results = Idle }, Cmd.none
    | UserClickedSubmit ->
        match state.Query with
        | None -> state, Cmd.none
        | Some query ->
            { state with Results = InProgress },
            Cmd.OfAsync.either GoogleSearchApi.search query ServerReturnedBlogEntries ServerReturnedError

let input dispatch =
    Bulma.input.search [
        input.isRounded
        prop.onChange (UserChangedInput >> dispatch)
        prop.onKeyDown (fun ke -> if ke.key = "Enter" then dispatch UserClickedSubmit)
        prop.onEmptied (fun _ -> dispatch UserClearedSearch)
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
