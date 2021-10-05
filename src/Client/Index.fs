module Client.Index

(*******************************************
*               TYPES
*******************************************)
open Client.Urls
open Client.Components
open Client.Pages

[<RequireQualifiedAccess>]
type Page =
    | About of About.State
    | Blog of Blog.State
    | BlogEntry of BlogEntry.State
    | Music of Music.Types.State
    | Search of Search.State
    | NotFound
    | UnexpectedError

type State =
    {
        CurrentPage: Page
        Navbar: Navbar.State
    }

[<RequireQualifiedAccess>]
type Msg =
    | About of About.Msg
    | Blog of Blog.Msg
    | Music of Music.Types.Msg
    | BlogEntry of BlogEntry.Msg
    | Navbar of Navbar.Msg
    | Search of Search.Msg

(*******************************************
*               INIT
*******************************************)
open Elmish.Navigation
open Elmish

let initFromUrl url =
    let pageInit (state, cmd) pageMapper msgMapper =
        {
            CurrentPage = pageMapper state
            Navbar = url |> Some |> Navbar.init
        },
        Cmd.map msgMapper cmd

    match url with
    | Url.About -> pageInit (About.init ()) Page.About Msg.About
    | Url.Blog -> pageInit (Blog.init ()) Page.Blog Msg.Blog
    | Url.BlogEntry slug -> pageInit (BlogEntry.init slug) Page.BlogEntry Msg.BlogEntry
    | Url.Music -> pageInit (Music.Component.init ()) Page.Music Msg.Music
    | Url.Search -> pageInit (Search.init ()) Page.Search Msg.Search
    | Url.UnexpectedError ->
        {
            CurrentPage = Page.UnexpectedError
            Navbar = url |> Some |> Navbar.init
        },
        Cmd.none
    | Url.NotFound ->
        {
            CurrentPage = Page.NotFound
            Navbar = url |> Some |> Navbar.init
        },
        Cmd.none

let init (url: Option<Url>): State * Cmd<Msg> =
    match url with
    | Some url -> initFromUrl url
    | None ->
        let state =
            {
                CurrentPage = Page.UnexpectedError
                Navbar = Navbar.init None
            }
        state, Navigation.newUrl "unexpected-error"

(*******************************************
*               UPDATE
*******************************************)
let update (msg: Msg) (state: State): State * Cmd<Msg> =
    let updater pageMsg pageState pageUpdater msgMapper pageMapper =
        let newState, newCmd = pageUpdater pageMsg pageState
        let cmd = Cmd.map msgMapper newCmd

        { state with
            CurrentPage = pageMapper newState
        },
        cmd

    match msg, state.CurrentPage with
    | Msg.About msg', Page.About state' -> updater msg' state' About.update Msg.About Page.About
    | Msg.Blog msg', Page.Blog state' -> updater msg' state' Blog.update Msg.Blog Page.Blog
    | Msg.BlogEntry msg', Page.BlogEntry state' -> updater msg' state' BlogEntry.update Msg.BlogEntry Page.BlogEntry
    | Msg.Music msg', Page.Music state' -> updater msg' state' Music.Component.update Msg.Music Page.Music
    | Msg.Search msg', Page.Search state' -> updater msg' state' Search.update Msg.Search Page.Search
    | Msg.Navbar msg', _ ->
        { state with
            Navbar = Navbar.update msg' state.Navbar
        },
        Cmd.none
    | _ -> state, Cmd.none

(*******************************************
*               RENDER
*******************************************)
open Feliz

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    let activePage =
        match state.CurrentPage with
        | Page.About state -> About.render state (Msg.About >> dispatch)
        | Page.Blog state -> Blog.render state (Msg.Blog >> dispatch)
        | Page.BlogEntry state -> BlogEntry.render state (Msg.BlogEntry >> dispatch)
        | Page.Music state -> Music.View.render state (Msg.Music >> dispatch)
        | Page.Search state -> Search.render state (Msg.Search >> dispatch)
        | Page.UnexpectedError -> UnexpectedError.render
        | Page.NotFound -> NotFound.render

    Html.div [
        prop.children [
            Navbar.render state.Navbar (Msg.Navbar >> dispatch)
            activePage
        ]
    ]
