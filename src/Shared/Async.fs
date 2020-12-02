namespace Shared

type AsyncStatus<'t> =
    | Started
    | Finished of 't

module Async =
    // Todo: Test this
    let map f op = async {
        let! x    = op
        let value = f x
        return value
    }
