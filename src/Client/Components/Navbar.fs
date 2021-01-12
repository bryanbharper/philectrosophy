module Client.Components.Navbar

open Client.Styles
open Client.Urls
open Feliz
open Feliz.Bulma
//open Feliz.Router

type State =
    {
        BurgerExpanded: bool
        ActivePage: Url
    }

type Msg =
    | BurgerClicked
    | UrlChanged of Url

let init (url: Option<Url>): State =
    match url with
    | None ->
        {
            ActivePage = Url.Blog
            BurgerExpanded = false
        }
    | Some url ->
        {
            BurgerExpanded = false
            ActivePage = url
        }

let update (msg: Msg) (state: State): State =
    match msg with
    | BurgerClicked ->
        { state with
            BurgerExpanded = not state.BurgerExpanded
        }
    | UrlChanged url -> { state with ActivePage = url }

let navLink (url: Url) isActive =
    Bulma.navbarItem.a [
        prop.classes [
            if isActive then Bulma.IsActive
        ]
        url.asString
        |> prop.href
        url.asString.ToLower() |> prop.text
    ]

let navLinkIcon (url: Url) isActive icon =
    Bulma.navbarItem.a [
        prop.classes [
            if isActive then Bulma.IsActive
        ]
        url.asString
        |> prop.href
        prop.children [
            Bulma.icon [
                Html.i [ prop.classes [ FA.Fas; icon ] ]
            ]
        ]
    ]

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    let matchBlog url =
        match url with
        | Url.Blog
        | Url.BlogEntry _ -> true
        | _ -> false

    Bulma.navbar [
        navbar.isFixedTop
        prop.classes [ Bulma.IsWhite ]
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    prop.href ""
                    prop.children [
                        Html.img [ prop.src "phi.png" ]
                    ]
                ]
                Bulma.navbarBurger [
                    if state.BurgerExpanded then navbarBurger.isActive
                    prop.onClick (fun _ -> Msg.BurgerClicked |> dispatch)
                    prop.children [
                        Html.span []
                        Html.span []
                        Html.span []
                    ]

                ]

            ]
            Bulma.navbarMenu [
                if state.BurgerExpanded then navbarMenu.isActive
                prop.children [
                    Bulma.navbarEnd.div [
                        navLink Url.Blog (state.ActivePage |> matchBlog)
                        navLink Url.About (state.ActivePage = Url.About)
                        navLinkIcon Url.Search (state.ActivePage = Url.Search) FA.FaSearch
                    ]
                ]

            ]

        ]

    ]
