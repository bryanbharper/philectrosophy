module File

type IFileStore =
    abstract GetBlogEntryContent: string -> Async<string option>

type StubFileStore() =

    interface IFileStore with

        member this.GetBlogEntryContent slug =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
            |> Some
            |> async.Return

