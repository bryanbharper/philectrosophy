namespace Shared

type AsyncStatus<'t> =
    | Started
    | Finished of 't

module Async =
    let map f asyncVal =
        async {
            let! x = asyncVal
            return f x
        }
