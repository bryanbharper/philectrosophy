module Client.Pages.BlogEntry

open Client.Components
open Client.Components.Layout
open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Router
open Fulma
open Shared
open Styles

let blogApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IBlogApi>

type Entry =
    {
        Metadata: BlogEntry
        Content: string
    }

type State =
    {
        Slug: string
        Entry: Deferred<Entry>
    }

type Msg =
    | GotEntry of Option<BlogEntry * string>
    | ApiError of exn

let init (slug: string): State * Cmd<Msg> =
    { Slug = slug; Entry = InProgress }, Cmd.OfAsync.either blogApi.GetEntry slug GotEntry ApiError

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ApiError _ -> state, Cmd.navigate "500"
    | GotEntry None -> state, Cmd.navigate "not-found"
    | GotEntry (Some (metadata, content)) ->
        let entry =
            {
                Metadata = metadata
                Content = content
            }

        { state with Entry = Resolved entry }, Cmd.none

let header metadata =
    [
        Html.p [
            prop.classes [ Bulma.Title; Bulma.Is2 ]
            prop.text metadata.Title
        ]
        Html.p [
            prop.classes [
                Bulma.Subtitle
                Bulma.Is3
                Bulma.HasTextGrey
            ]
            prop.text metadata.Subtitle
        ]
        Html.p [
            prop.classes [
                Bulma.Subtitle
                Bulma.Is5
                Bulma.HasTextGreyLight
                Bulma.IsItalic
            ]
            prop.text (sprintf "Posted by %s on %s" metadata.Author (Date.format metadata.CreatedOn))
        ]
        Html.hr []
    ]
    |> Html.div


let content (content: string) =
    Markdown.render [
        Markdown.Content content
        Markdown.Options [
            Markdown.LangPrefix "hljs"
            Markdown.GithubFlavoured
        ]
    ]

let layout (contents: ReactElement list) =
    Section.render
        [
            Html.div [
                prop.className Bulma.Columns
                prop.children
                    [
                        Html.div [
                            prop.classes [
                                Bulma.Column
                                Bulma.IsOffsetOneQuarter
                                Bulma.IsHalf
                            ]
                            prop.children contents
                        ]
                    ]
            ]
        ]

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    layout
        [
            match state.Entry with
            | Idle -> Html.none
            | InProgress -> Spinner.render
            | Resolved entry ->
                header entry.Metadata
                content entry.Content
        ]
