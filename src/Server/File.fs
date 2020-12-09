module File

type IFileAccess =
    abstract ReadFileAsync: string -> Async<string option>

type IBlogContentStore =
    abstract GetBlogEntryContentAsync: string -> Async<string option>


open Shared
open System.IO

type PublicFileStore() =

    interface IFileAccess with

        member this.ReadFileAsync fullName =
            let fileInfo = FileInfo fullName

            if fileInfo.Exists then
                File.ReadAllTextAsync(fileInfo.FullName)
                |> Async.AwaitTask
                |> Async.map (fun s -> Some s)
            else
                None |> async.Return

type BlogContentStore(fileAccess: IFileAccess) =

    interface IBlogContentStore with

        member this.GetBlogEntryContentAsync slug =
            sprintf "public/blog.posts/%s.md" slug
            |> fileAccess.ReadFileAsync
