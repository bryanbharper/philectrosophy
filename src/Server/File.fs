module Server.File

open System.IO

open Shared.Extensions

type IFileAccess =
    abstract ReadFileAsync: string -> Async<string option>

type IBlogContentStore =
    abstract GetBlogEntryContentAsync: string -> Async<string option>

type PublicFileStore() =

    interface IFileAccess with

        member this.ReadFileAsync fullName =
            let fileInfo = FileInfo fullName

            if fileInfo.Exists then
                File.ReadAllTextAsync(fileInfo.FullName)
                |> Async.AwaitTask
                |> Async.map Some
            else
                None |> async.Return

type BlogContentStore(fileAccess: IFileAccess) =

    interface IBlogContentStore with

        member this.GetBlogEntryContentAsync slug =
            let preprocessor =
                Markdown.Latex.convertMath
                >> Markdown.Bulma.convertPopovers

            sprintf "public/blog.posts/%s.md" slug
            |> fileAccess.ReadFileAsync
            |> Async.map (Option.map preprocessor)
