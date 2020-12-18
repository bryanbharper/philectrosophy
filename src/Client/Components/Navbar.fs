module Client.Components.Navbar

open Client.Styles
open Client.Urls
open Feliz
open Feliz.Bulma
open Feliz.Router

let navLink (url: Url) isActive =
    Bulma.navbarItem.a [
        prop.classes [
            if isActive then Bulma.IsActive
        ]
        url.asString.ToLower()
        |> Router.format
        |> prop.href
        url.asString.ToLower() |> prop.text
    ]

let navLinkIcon (url: Url) isActive icon =
    Bulma.navbarItem.a [
        prop.classes [
            if isActive then Bulma.IsActive
        ]
        url.asString.ToLower()
        |> Router.format
        |> prop.href
        prop.children [
            Bulma.icon [
                Html.i [ prop.classes [ FA.Fas; icon ] ]
            ]
        ]
    ]

let render (activePage: Url) =
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
                    prop.href "#"
                    prop.children [
                        Html.img [ prop.src "phi.png" ]
                    ]
                ]
                Bulma.navbarBurger [
                    //                        navbarBurger.isActive
                    prop.children [
                        Html.span []
                        Html.span []
                        Html.span []
                    ]

                ]

            ]
            Bulma.navbarMenu [
                Bulma.navbarEnd.div [
                    navLink Url.Blog (activePage |> matchBlog)
                    navLink Url.About (activePage = Url.About)
                    navLinkIcon Url.Search (activePage = Url.Search) FA.FaSearch
                ]
            ]

        ]

    ]
