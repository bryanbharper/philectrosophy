module Client.Components.Navbar

open Feliz
open Feliz.Router
open Feliz.Bulma
open Styles

let navLink (name: string) =
    Bulma.navbarItem.a [
        prop.href (Router.format name)
        prop.text name
    ]

let navLinkIcon (name: string) icon =
    Bulma.navbarItem.a [
        prop.href (Router.format name)
        prop.children
            [
                Bulma.icon
                    [
                        Html.i [ prop.classes [ FA.Fas; icon ] ]
                    ]
            ]
    ]

let render =
    Bulma.navbar [
        navbar.isFixedTop
        prop.children [
            Bulma.navbarBrand.div
                [
                    Bulma.navbarItem.a [
                        prop.href "#"
                        prop.children [ Html.img [ prop.src "phi.png" ] ]
                    ]
                ]
            Bulma.navbarMenu
                [
                    Bulma.navbarEnd.div [
                        navLink "blog"
                        navLink "lexicon"
                        navLink "about"
                        navLinkIcon "search" FA.FaSearch
                    ]
                ]
        ]
    ]
