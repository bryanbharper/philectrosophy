namespace Shared

type IBlogApi =
    {
        GetEntries: unit -> Async<BlogEntry list>
        GetEntry: string -> Async<Option<BlogEntry * string>>
        GetSearchResults: string -> Async<BlogEntry list>
        UpdateViewCount: string -> Async<int option>
    }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName
