namespace Shared

type Deferred<'T> =
    | Idle
    | InProgress
    | Resolved of 'T

type DeferredResult<'T, 'E> =
    Deferred<Result<'T, 'E>>

//[<RequireQualifiedAccess>]
//module Deferred =
//    let map (transform: 'T -> 'U) (deferred: Deferred<'T>) : Deferred<'U> =
//        match deferred with
//        | Idle -> Idle
//        | InProgress -> InProgress
//        | Resolved value -> Resolved (transform value)
//
//    let bind (transform: 'T -> Deferred<'U>) (deferred: Deferred<'T>) : Deferred<'U> =
//        match deferred with
//        | Idle -> Idle
//        | InProgress -> InProgress
//        | Resolved value -> transform value
//
//    let resolved = function
//        | Idle -> false
//        | InProgress -> false
//        | Resolved _ -> true
//
//    let inProgress = function
//        | Idle -> false
//        | InProgress -> true
//        | Resolved _ -> false
//
//    let exists (predicate: 'T -> bool) = function
//        | Idle -> false
//        | InProgress -> false
//        | Resolved value -> predicate value
