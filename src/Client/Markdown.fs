module Markdown

open Fable.Core
open Fable.Core.JsInterop
open Fable.React.Props
open Fable.React.Helpers

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
    ofImport "MarkdownPreview" "react-marked-markdown" (keyValueList CaseRules.LowerFirst props) []
