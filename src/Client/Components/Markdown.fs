module Client.Components.Markdown

open Fable.Core
open Fable.Core.JsInterop
open Fable.React.Props
open Fable.React.Helpers

type MarkedOptions = { sanitize: bool; smartypants: bool }

type IMarkedProps =
    | [<CompiledName("value")>] Content of string
    | [<CompiledName("markedOptions")>] Options of MarkedOptions
    | [<CompiledName("className")>] Class of string
    interface IHTMLProp

let render content =
    let props =
        [
            Content content
            Options { sanitize = false; smartypants = true }
            Class Client.Styles.Style.Markdown
        ]

    ofImport "MarkdownPreview" "react-marked-markdown" (keyValueList CaseRules.LowerFirst props) []
