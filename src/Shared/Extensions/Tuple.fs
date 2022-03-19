module Shared.Extensions.Tuple

let sequenceOption tup =
    match tup with
    | Some a, Some b -> Some(a, b)
    | _ -> None

let sequenceAsync tup =
    async {
        let! a = fst tup
        let! b = snd tup
        return a, b
    }
