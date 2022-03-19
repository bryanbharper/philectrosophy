namespace Shared.Extensions

type AsyncStatus<'t> =
    | Started
    | Finished of 't


module Async =
    let map mapping asyncOp =
        async.Bind(asyncOp, mapping >> async.Return)
