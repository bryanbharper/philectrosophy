module Shared.Tests.All

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

#if !FABLE_COMPILER
let routeProperty =
    testProperty "Route.builder properly formats route, any inputs."
    <| fun typeName methodName ->
        // arrange
        // act
        let result = Route.builder typeName methodName

        // assert
        result = (sprintf "/api/%s/%s" typeName methodName)
#endif

let route =
    testCase "Route.builder properly formats route."
    <| fun _ ->
        // arrange
        let typeName = "type"
        let method = "method"

        // act
        let result = Route.builder typeName method

        // assert
        Expect.equal result (sprintf "/api/%s/%s" typeName method) "Should be '/api/type/method.'"


let all = testList "Shared Tests" [
    String.all
    DomainModels.all
    Async.all
    Date.all
    Math.all
    route

#if !FABLE_COMPILER
    routeProperty
#endif

]
