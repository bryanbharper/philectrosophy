module Client.Components.EntryMedia

open Client.Styles
open Client.Urls
open Feliz
open Feliz.Router
open Feliz.Bulma
open Shared

let render (entry: BlogEntry) =
    let title = Bulma.title.h4 [ prop.text entry.Title ]

    let subTitle =
        let updatedMsg =
            match entry.UpdatedOn with
            | None -> Html.none
            | Some date ->
                Html.span [
                    prop.classes [
                        Bulma.HasTextGrey
                        Bulma.IsItalic
                        Bulma.Ml1
                    ]
                    prop.text (sprintf "Updated: %s" (Date.format date))
                ]

        Bulma.subtitle.p [
            prop.classes [ Bulma.Is6; Bulma.Mb1 ]
            prop.children [
                Html.span [
                    prop.classes [
                        Bulma.HasTextGreyLight
                        if entry.UpdatedOn.IsSome then Style.IsStrikeThrough else Bulma.IsItalic
                    ]
                    prop.text (sprintf "Posted the %s" (Date.format entry.CreatedOn))
                ]
                updatedMsg
            ]
        ]

    let synopsis = Html.p entry.Synopsis

    let media =
        [ title; subTitle; synopsis ]
        |> MediaObject.render entry.ThumbNailUrl

    Html.div [
        prop.classes [
            Bulma.Mb6
        ]

        prop.children [
            Html.a [
                prop.classes [ Style.SecretAnchor ]

                (Url.Blog.asString, entry.Slug)
                |> Router.format
                |> prop.href

                prop.children media
            ]
        ]

    ]
