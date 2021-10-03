module Client.Components.Navbar

open Client.Styles
open Client.Urls
open Feliz
open Feliz.Bulma

type State =
    {
        BurgerExpanded: bool
        ActivePage: Url
    }

type Msg =
    | UserClickedBurger
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
    | UserClickedBurger ->
        { state with
            BurgerExpanded = not state.BurgerExpanded
        }
    | UrlChanged url -> { state with ActivePage = url }

let navLink (url: Url) isActive =
    Bulma.navbarItem.a [
        prop.classes [
            if isActive then Bulma.IsActive
        ]
        url |> Url.toString |> prop.href
        url |> Url.toString |> prop.text
    ]

let navLinkIcon (url: Url) isActive fontAwesomeIconName =
    Bulma.navbarItem.a [
        prop.classes [
            if isActive then Bulma.IsActive
        ]
        url |> Url.toString |> prop.href
        prop.children [
            Bulma.icon [
                Html.i [ prop.classes [ FA.Fas; fontAwesomeIconName ] ]
            ]
        ]
    ]

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    Bulma.navbar [
        navbar.isFixedTop
        prop.classes [ Bulma.IsWhite ]
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    prop.classes [ Style.Logo ]
                    prop.href ""
                    prop.children [
                        Html.img [ prop.src "phi.png" ]
                        Html.p [
                            prop.classes [ Style.LogoText; Bulma.IsSize5; Bulma.Px1 ]
                            prop.innerHtml "&#295;i&#8467;&epsilon;c&tau;&#8477;&sigma;&#8747;&theta;&rho;&#295;&gamma;"
                        ]
                    ]
                ]
                Bulma.navbarBurger [
                    if state.BurgerExpanded then navbarBurger.isActive
                    prop.onClick (fun _ -> Msg.UserClickedBurger |> dispatch)
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
                        navLink Url.Blog (state.ActivePage = Url.Blog)
                        navLink Url.About (state.ActivePage = Url.About)
                        navLinkIcon Url.Search (state.ActivePage = Url.Search) FA.FaSearch
                    ]
                ]

            ]

        ]

    ]
