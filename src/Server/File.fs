module File


type IFileStore =
    abstract GetBlogEntryContentAsync: string -> Async<string option>

type StubFileStore() =

    interface IFileStore with

        member this.GetBlogEntryContentAsync slug =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
            |> Some
            |> async.Return

open Shared
open System.IO

type PublicFileStore() =

    interface IFileStore with

        member this.GetBlogEntryContentAsync slug =
            let filename = sprintf "public/blog.posts/%s.md" slug
            let fileInfo = FileInfo filename

            if fileInfo.Exists then
                File.ReadAllTextAsync(fileInfo.FullName)
                |> Async.AwaitTask
                |> Async.map (fun s -> Some s)
            else
                None |> async.Return
