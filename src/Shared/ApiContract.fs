namespace Shared

type IBlogApi =
    {
        GetEntries: unit -> Async<BlogEntry list>
        GetEntry: string -> Async<Option<BlogEntry * string>>
    }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName
