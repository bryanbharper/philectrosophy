module Client.Components.Markdown

open Fable.Core
open Fable.Core.JsInterop
open Fable.React.Props
open Fable.React.Helpers
open Feliz

type MarkedOptions = { sanitize: bool; smartypants: bool }

type IMarkedProps =
    | [<CompiledName("value")>] Content of string
    | [<CompiledName("markedOptions")>] Options of MarkedOptions
    interface IHTMLProp

let render content =
    let props =
        [
            Content content
            Options { sanitize = false; smartypants = true }
        ]

    Html.div [
        prop.classes [
            Client.Styles.Style.Markdown
        ]
        prop.children [
            ofImport "MarkdownPreview" "react-marked-markdown" (keyValueList CaseRules.LowerFirst props) []
        ]
    ]
