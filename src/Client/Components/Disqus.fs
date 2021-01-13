module Client.Components.Disqus

open Fable.Core
open Fable.Core.JsInterop
open Fable.React.Helpers
open Fable.React.Props
open Shared

type DisqusConfig =
    {
        url: string
        identifier: string
        title: string
    }

type DisqusProps =
    | [<CompiledName("shortname")>] Shortname of string
    | [<CompiledName("config")>] Config of DisqusConfig
    interface IHTMLProp

let inline render slug title =
    let slug' =
        slug
        |> String.replace "-" "_"
        |> String.suffix "_0"

    let props = [
                Shortname "philectrosophy"
                Config {
                    url = sprintf "http://philectrosophy.com/blog/%s" slug
                    identifier = slug'
                    title = title
                }
            ]

    ofImport "DiscussionEmbed" "disqus-react" (keyValueList CaseRules.LowerFirst props) []
