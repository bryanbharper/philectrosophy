namespace Shared

type Deferred<'T> =
    | Idle
    | InProgress
    | Resolved of 'T

type DeferredResult<'T, 'E> =
    Deferred<Result<'T, 'E>>

[<RequireQualifiedAccess>]
module Deferred =
    // Todo: Test this
    let map (transform: 'T -> 'U) (deferred: Deferred<'T>) : Deferred<'U> =
        match deferred with
        | Idle -> Idle
        | InProgress -> InProgress
        | Resolved value -> Resolved (transform value)

    // Todo: Test this
    let bind (transform: 'T -> Deferred<'U>) (deferred: Deferred<'T>) : Deferred<'U> =
        match deferred with
        | Idle -> Idle
        | InProgress -> InProgress
        | Resolved value -> transform value

    // Todo: Test this
    let resolved = function
        | Idle -> false
        | InProgress -> false
        | Resolved _ -> true

    // Todo: Test this
    let inProgress = function
        | Idle -> false
        | InProgress -> true
        | Resolved _ -> false

    // Todo: Test this
    let exists (predicate: 'T -> bool) = function
        | Idle -> false
        | InProgress -> false
        | Resolved value -> predicate value
