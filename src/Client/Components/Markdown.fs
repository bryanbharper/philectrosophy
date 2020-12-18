module Client.Components.Markdown

open Fable.Core
open Fable.Core.JsInterop
open Fable.React.Props
open Fable.React.Helpers
open Feliz

type IMarkedOptions =
    | [<CompiledName("gfc")>] GithubFlavoured
    | Tables
    | Sanitize of bool
    | SmartLists
    | Pedantic
    | Breaks
    | Smartypants
    | LangPrefix of string

type IMarkedProps =
    | [<CompiledName("value")>] Content of string
    | [<CompiledName("markedOptions")>] Options of IMarkedOptions list
    interface IHTMLProp

let render (props: IHTMLProp list) =
    Html.div [
        prop.classes [ Client.Styles.Style.Markdown ]
        prop.children [
            ofImport "MarkdownPreview" "react-marked-markdown" (keyValueList CaseRules.LowerFirst props) []
        ]
    ]
