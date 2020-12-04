module Client.Index

open Client.Components
open Client.Pages
open Elmish
open Shared
open Feliz
open Feliz.Router

[<RequireQualifiedAccess>]
type Page =
    | About of About.State
    | Blog of Blog.State
    | BlogEntry of BlogEntry.State
    | Lexicon of Lexicon.State
    | Search of Search.State
    | NotFound
    | UnexpectedError

[<RequireQualifiedAccess>]
type Url =
    | About
    | Blog
    | BlogEntry of slug: string
    | Lexicon
    | Search
    | NotFound
    | UnexpectedError

let parseUrl =
    function
    | [ "about" ] -> Url.About
    | []
    | [ "blog" ] -> Url.Blog
    | [ "blog"; slug: string ] -> Url.BlogEntry slug
    | [ "lexicon" ] -> Url.Lexicon
    | [ "search" ] -> Url.Search
    | [ "500" ] -> Url.UnexpectedError
    | _ -> Url.NotFound

type State = { CurrentUrl: Url; CurrentPage: Page }

[<RequireQualifiedAccess>]
type Msg =
    | About of About.Msg
    | Blog of Blog.Msg
    | BlogEntry of BlogEntry.Msg
    | Lexicon of Lexicon.Msg
    | Search of Search.Msg
    | UrlChanged of Url

let pageInitFromUrl url =
    let initializer (state, cmd) pageMapper msgMapper =
        {
            CurrentUrl = url
            CurrentPage = pageMapper state
        },
        Cmd.map msgMapper cmd

    match url with
    | Url.About -> initializer (About.init ()) Page.About Msg.About
    | Url.Blog -> initializer (Blog.init ()) Page.Blog Msg.Blog
    | Url.BlogEntry slug -> initializer (BlogEntry.init slug) Page.BlogEntry Msg.BlogEntry
    | Url.Lexicon -> initializer (Lexicon.init ()) Page.Lexicon Msg.Lexicon
    | Url.Search -> initializer (Search.init ()) Page.Search Msg.Search
    | Url.UnexpectedError ->
        {
            CurrentUrl = url
            CurrentPage = Page.UnexpectedError
        },
        Cmd.none
    | Url.NotFound ->
        {
            CurrentUrl = url
            CurrentPage = Page.NotFound
        },
        Cmd.none

let init (): State * Cmd<Msg> =
    Router.currentUrl ()
    |> parseUrl
    |> pageInitFromUrl

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
    | Msg.Lexicon msg', Page.Lexicon state' -> updater msg' state' Lexicon.update Msg.Lexicon Page.Lexicon
    | Msg.Search msg', Page.Search state' -> updater msg' state' Search.update Msg.Search Page.Search
    | Msg.UrlChanged nextUrl, _ -> pageInitFromUrl nextUrl
    | _ -> state, Cmd.none


open Fable.React

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    let activePage =
        match state.CurrentPage with
        | Page.About state -> About.render state (Msg.About >> dispatch)
        | Page.Blog state -> Blog.render state (Msg.Blog >> dispatch)
        | Page.BlogEntry state -> BlogEntry.render state (Msg.BlogEntry >> dispatch)
        | Page.Lexicon state -> Lexicon.render state (Msg.Lexicon >> dispatch)
        | Page.Search state -> Search.render state (Msg.Search >> dispatch)
        | Page.UnexpectedError -> UnexpectedError.render
        | Page.NotFound -> NotFound.render

    Html.div
        [
            prop.children [
                Navbar.render
                React.router [
                    router.onUrlChanged (parseUrl >> Msg.UrlChanged >> dispatch)
                    router.children [ activePage ]
                ]
            ]
        ]
