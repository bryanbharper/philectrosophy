﻿module Shared.Tests.Async

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
open System.Linq
#endif

open Shared

#if !FABLE_COMPILER
let properties = testList "String Property Tests" [

    testProperty "Async.map: map id = id."
    <| fun (randVal: int) ->
        // arrange
        let randAsync = async.Return randVal
        let idMap = Async.map id

        // act
        let result = idMap randAsync

        // assert
        (result |> Async.RunSynchronously) = (randAsync |> Async.RunSynchronously)


    testProperty "Async.map: is associative."
    <| fun (randVal: int) ->
        // arrange
        let randAsync = async.Return randVal
        let f = (+) 1
        let g = (-) 2
        let fg1 = f >> g |> Async.map
        let fg2 = (Async.map f) >> (Async.map g)

        // act
        let result1 = fg1 randAsync
        let result2 = fg2 randAsync

        // assert
        (result1 |> Async.RunSynchronously) = (result2 |> Async.RunSynchronously)


]
#endif
let all = testList "Async" [

#if !FABLE_COMPILER
    properties
#endif


]


