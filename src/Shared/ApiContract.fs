namespace Shared

type IBlogApi =
    {
        GetEntries: unit -> Async<BlogEntry list>
    }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName
