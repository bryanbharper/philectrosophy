module Shared.Tests.Option

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

let all =
    testList
        "Option"
        [
            testCase "Option.sequenceOption returns Async<None> when input is None"
            <| fun _ ->
                // arrange
                // act
                let result = Option.sequenceAsync None |> Async.RunSynchronously

                // assert
                Expect.isNone result "Result should be None."

            testCase "Option.sequenceOption returns Async<Some<x>> when input is Some."
            <| fun _ ->
                // arrange
                let expected = 42
                let input = expected |> async.Return |> Some

                // act
                let result =
                    input
                    |> Option.sequenceAsync
                    |> Async.RunSynchronously

                // assert
                match result with
                | None -> failtest "Result should have been Some; instead was None."
                | Some r ->
                    Expect.equal r expected ""
        ]
