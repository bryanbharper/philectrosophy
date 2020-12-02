namespace Shared

type IBlogApi =
    { GetEntries : unit -> Async<Result<BlogEntry list, string>> }

module Route =
    // Todo: Test this
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName
