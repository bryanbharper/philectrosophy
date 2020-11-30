module Pulma

open Feliz
open Styles

let nav (children: ReactElement list): ReactElement =
    [
        prop.className Bulma.Navbar
        prop.role "navigation"
        prop.ariaLabel "main navigation"
        prop.children children
    ]
    |> Html.nav


module Navbar =
    module Brand =
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

    let brand href src =
        Brand.icon href src |> List.singleton |> Brand.div

    let menu (children: ReactElement list) =
        [
            prop.className Bulma.NavbarMenu
            prop.children children
        ]
        |> Html.div

    let end' (children: ReactElement list) =
        [
            prop.className Bulma.NavbarEnd
            prop.children children
        ]
        |> Html.div
        |> List.singleton
        |> menu

    let textItem href (text: string) =
        [
            prop.className Bulma.NavbarItem
            prop.href href
            prop.text text
        ]
        |> Html.a

    let item href (children: ReactElement list) =
        [
            prop.className Bulma.NavbarItem
            prop.href href
            prop.children children
        ]
        |> Html.a

    let iconItem href faIcon =
        Html.span [
            prop.className Bulma.Icon
            prop.children
                [
                    Html.i [ prop.classes [ FA.Fas; faIcon ] ]
                ]
        ]
        |> List.singleton
        |> item href
