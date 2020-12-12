module Client.Pages.Blog

open Client.Components
open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Bulma
open Feliz.Router
open Shared
open Styles

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
    | ApiError _ -> state, Cmd.navigate "500"

let renderEntry dispatch entry =
    let title = Bulma.title.h4 [ prop.text entry.Title ]

    let subTitle =
        Bulma.subtitle.p [
            prop.classes [
                Bulma.Is6
                Bulma.HasTextGreyLight
                Bulma.IsItalic
            ]
            prop.text (sprintf "Posted by %s on %s" entry.Author (Date.format entry.CreatedOn))
        ]

    let synopsis = Html.p entry.Synopsis

    let media =
        [ title; subTitle; synopsis ]
        |> MediaObject.render entry.ThumbNailUrl

    Html.div [
        prop.classes [ Style.Clickable; "mb-6" ]
        prop.onClick (fun _ -> ("blog", entry.Slug) |> Router.navigate)
        prop.children media
    ]

let render (state: State) (dispatch: Msg -> unit) =
    let entries =
        match state.Entries with
        | Idle -> Html.none
        | InProgress -> Spinner.render
        | Resolved entries ->
            entries
            |> List.map (renderEntry dispatch)
            |> Html.div

    Bulma.section [
        Bulma.container [
            Bulma.title.h2 [ prop.text "Blog" ]
            Html.hr []
            entries
        ]
    ]
