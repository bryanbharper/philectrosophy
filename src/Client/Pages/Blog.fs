module Client.Pages.Blog

open Client.Components
open Client.Components.Layout
open Elmish
open Fable.Remoting.Client
open Feliz
open Shared
open Styles

let blogApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IBlogApi>

type State =
    {
        Entries: Deferred<BlogEntry list>
    }

type Msg = GotEntries of BlogEntry list

let init (): State * Cmd<Msg> =
    { Entries = InProgress }, Cmd.OfAsync.perform blogApi.GetEntries () (GotEntries)

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | GotEntries result -> { state with Entries = Resolved result }, Cmd.none

let renderEntry dispatch entry =
    let title =
        Html.h4 [
            prop.classes [ Bulma.IsMarginless ]
            prop.text entry.Title
        ]

    let subTitle =
        let creationDate =
            entry.CreatedOn.ToString("dddd, dd MMMM yyyy")

        Html.p [
            prop.classes [ Bulma.HasTextGreyLight; "mb-1" ]
            prop.text (sprintf "Posted by %s on %s" entry.Author creationDate)
        ]

    let synopsis = Html.p entry.Synopsis
    let contents = [ title; subTitle; synopsis ]

    MediaObject.render entry.ThumbNailUrl contents (fun e -> printf "Media was clicked")

let render (state: State) (dispatch: Msg -> unit) =
    let entries =
        match state.Entries with
        | Idle -> Html.none
        | InProgress -> Spinner.render
        | Resolved entries ->
            entries
            |> List.map (renderEntry dispatch)
            |> Html.div

    Container.render
        [
            Section.render
                [
                    Html.h1
                        [
                            prop.className Bulma.Title
                            prop.text "Blog"
                        ]
                    entries
                ]

        ]
