module Client.Pages.BlogEntry

open Client.Components
open Client.Styles
open Client.Urls
open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Bulma
open Feliz.Router
open Shared

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
    | ApiError of exn
    | CountUpdated of int option
    | GotEntry of Option<BlogEntry * string>
    | IgnorableError of exn

let init (slug: string): State * Cmd<Msg> =
    { Slug = slug; Entry = InProgress }, Cmd.OfAsync.either blogApi.GetEntry slug GotEntry ApiError

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ApiError _ -> state, Url.UnexpectedError.asString |> Cmd.navigate
    | CountUpdated None -> state, Cmd.none
    | CountUpdated (Some newCount) ->
        let newState =
            match state.Entry with
            | Idle -> state
            | InProgress -> state
            | Resolved entry ->
                let meta = { entry.Metadata with ViewCount = newCount }
                let entry = { entry with Metadata = meta }
                { state with Entry = entry |> Resolved }

        newState, Cmd.none
    | GotEntry None -> state, Url.NotFound.asString |> Cmd.navigate
    | GotEntry (Some (metadata, content)) ->
        let entry =
            {
                Metadata = metadata
                Content = content
            }

        { state with Entry = Resolved entry }, Cmd.OfAsync.either blogApi.UpdateViewCount state.Slug CountUpdated IgnorableError
    | IgnorableError _ -> state, Cmd.none

let dateHeader metadata =
        let updatedMsg =
            match metadata.UpdatedOn with
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
            prop.classes [ Bulma.Is6 ]
            prop.children [
                Html.span [
                    prop.classes [
                        Bulma.HasTextGreyLight
                        if metadata.UpdatedOn.IsSome then Style.IsStrikeThrough else Bulma.IsItalic
                    ]
                    prop.text (sprintf "Posted: %s" (Date.format metadata.CreatedOn))
                ]
                updatedMsg
            ]
        ]

let header metadata =
    [
        Bulma.title.h2 [
            prop.text metadata.Title
        ]
        match metadata.Subtitle with
        | None -> Html.none
        | Some subtitle ->
            Bulma.subtitle.h4 [
                prop.classes [ Bulma.HasTextGrey ]
                prop.text subtitle
            ]
        dateHeader metadata
        Html.hr []
    ]
    |> Html.div

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
            Markdown.render entry.Content
    ]
