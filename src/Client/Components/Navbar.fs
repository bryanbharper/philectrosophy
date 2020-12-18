﻿module Client.Components.Navbar

open Client.Styles
open Feliz
open Feliz.Bulma
open Feliz.Router

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
                        navLink "about"
                        navLinkIcon "search" FA.FaSearch
                    ]
                ]
        ]
    ]
