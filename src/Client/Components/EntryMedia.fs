module Client.Components.EntryMedia

open Client.Styles
open Client.Urls
open Feliz
open Feliz.Bulma
open Shared

let private title entry = Bulma.title.h4 [ prop.text entry.Title ]

let private updatedMsg entry =
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

let private subTitle entry =
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
            updatedMsg entry
        ]
    ]

let private synopsis entry = Html.p entry.Synopsis

let private media entry =
    [
        title entry
        subTitle entry
        synopsis entry
    ]
    |> MediaObject.render entry.ThumbNailUrl

let private (</>) left right = sprintf "%s/%s" left right

let render (entry: BlogEntry) =
    Html.div [
        prop.classes [ Bulma.Mb6 ]

        prop.children [
            Html.a [
                prop.classes [ Style.SecretAnchor ]
                Url.Blog.asString </> entry.Slug |> prop.href
                entry |> media |> prop.children
            ]
        ]
    ]
