module Shared.Extensions.Option

let sequenceAsync (asyncOp: Option<Async<'a>>): Async<Option<'a>> =
    match asyncOp with
    | None -> None |> async.Return
    | Some x ->
        async {
            let! r = x
            return Some r
        }
