module Client.Pages.BlogEntry

open Client.Components
open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Bulma
open Feliz.Router
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
        Bulma.title.h2 [
            prop.text metadata.Title
        ]
        Bulma.subtitle.h4 [
            prop.classes [ Bulma.HasTextGrey ]
            prop.text metadata.Subtitle
        ]
        Bulma.subtitle.h6 [
            prop.classes [
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
    Bulma.section [
        Bulma.columns [
            Bulma.column [
                column.isOffsetOneQuarter
                column.isHalf
                prop.children contents
            ]
        ]

    ]

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    layout [
        match state.Entry with
        | Idle -> Html.none
        | InProgress -> Spinner.render
        | Resolved entry ->
            header entry.Metadata
            content entry.Content
    ]
