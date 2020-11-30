module Index

open Elmish
open Fable.Remoting.Client
open Shared
open Feliz
open Styles
open Feliz.Router

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>

type State = { CurrentUrl: string list }

type Msg = UrlChanged of string list

let init (): State * Cmd<Msg> =
    { CurrentUrl = Router.currentUrl () }, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | UrlChanged url -> { state with CurrentUrl = url }, Cmd.none

open Fable.React

let navbar =
    Pulma.nav [
        Pulma.Navbar.brand "#" "phi.png"
        Pulma.Navbar.end' [
            Pulma.Navbar.textItem (Router.format "blog") "blog"
            Pulma.Navbar.textItem (Router.format "lexicon") "lexicon"
            Pulma.Navbar.textItem (Router.format "about") "about"
            Pulma.Navbar.iconItem (Router.format "search") FA.FaSearch
        ]
    ]

let activePage currentUrl =
    match currentUrl with
    | [] -> Html.h1 "Home / Blog"
    | [ "blog" ] -> Html.h1 "Home / Blog"
    | [ "lexicon" ] -> Html.h1 "Lexicon"
    | [ "search" ] -> Html.h1 "Search"
    | _ -> Html.h1 "Not Found"

let view (state: State) (dispatch: Msg -> unit): ReactElement =
    Html.div
        [
            prop.children [
                navbar
                React.router [
                    router.onUrlChanged (UrlChanged >> dispatch)
                    router.children [ state.CurrentUrl |> activePage ]
                ]
            ]
        ]
