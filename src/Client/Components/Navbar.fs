module Client.Components.Navbar

open Feliz
open Feliz.Router
open Styles

let private nav (children: ReactElement list): ReactElement =
    [
        prop.className [ Bulma.Navbar; Bulma.IsFixedTop ]
        prop.role "navigation"
        prop.ariaLabel "main navigation"
        prop.children children
    ]
    |> Html.nav

module private Brand =
    let div (children: ReactElement list) =
        [
            prop.className Bulma.NavbarBrand
            prop.children children
        ]
        |> Html.div

    let icon href src =
        [
            prop.className Bulma.NavbarItem
            prop.href href
            prop.children [ Html.img [ prop.src src ] ]
        ]
        |> Html.a

let private brand href src =
    Brand.icon href src |> List.singleton |> Brand.div

let private menu (children: ReactElement list) =
    [
        prop.className Bulma.NavbarMenu
        prop.children children
    ]
    |> Html.div
let private end' (children: ReactElement list) =
    [
        prop.className Bulma.NavbarEnd
        prop.children children
    ]
    |> Html.div
    |> List.singleton
    |> menu
let private textItem href (text: string) =
    [
        prop.className Bulma.NavbarItem
        prop.href href
        prop.text text
    ]
    |> Html.a
let private item href (children: ReactElement list) =
    [
        prop.className Bulma.NavbarItem
        prop.href href
        prop.children children
    ]
    |> Html.a
let private iconItem href faIcon =
    Html.span [
        prop.className Bulma.Icon
        prop.children
            [
                Html.i [ prop.classes [ FA.Fas; faIcon ] ]
            ]
    ]
    |> List.singleton
    |> item href

let render =
    nav [
        brand "#" "phi.png"
        end' [
            textItem (Router.format "blog") "blog"
            textItem (Router.format "lexicon") "lexicon"
            textItem (Router.format "about") "about"
            iconItem (Router.format "search") FA.FaSearch
        ]
    ]
