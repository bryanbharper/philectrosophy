module Client.App

open Elmish
open Elmish.React
open Elmish.UrlParser
open Elmish.Navigation
open Urls

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

let pageParser : Parser<Url->Url, Url> =
  oneOf
    [
      map Url.About (s Url.About.asString)
      map Url.Blog (s Url.Blog.asString)
      map Url.Blog (s "")
      map Url.BlogEntry (s Url.Blog.asString </> str)
      map Url.NotFound (s Url.NotFound.asString)
      map Url.Search (s Url.Search.asString)
      map Url.UnexpectedError (s Url.UnexpectedError.asString)
    ]

let urlUpdate (result:Option<Url>) model =
  match result with
  | Some url ->
      Index.initFromUrl url
  | None ->
      model, Navigation.newUrl Url.NotFound.asString


Program.mkProgram Index.init Index.update Index.render
|> Program.toNavigable (parsePath pageParser) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
