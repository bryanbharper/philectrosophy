module Shared.Tests.Tuple

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

let all =
    testList
        "Tuple"
        [
            testCase "Tuple.sequenceOption returns None when fst tup is None"
            <| fun _ ->
                // arrange
                let tup = None, Some 2

                // act
                let result = Tuple.sequenceOption tup

                // assert
                Expect.isNone result "Result should be None."

            testCase "Tuple.sequenceOption: returns None when snd tup is None"
            <| fun _ ->
                // arrange
                let tup = Some 2, None

                // act
                let result = Tuple.sequenceOption tup

                // assert
                Expect.isNone result "Result should be None."

            testCase "Tuple.sequenceOption: returns None when both parts are None"
            <| fun _ ->
                // arrange
                let tup = None, None

                // act
                let result = Tuple.sequenceOption tup

                // assert
                Expect.isNone result "Result should be None."

            testCase "Tuple.sequenceOption: returns Some (a, b) when a & b are Some"
            <| fun _ ->
                // arrange
                let a = "hello there"
                let b = 42
                let tup = Some a, Some b

                // act
                let result = Tuple.sequenceOption tup

                // assert
                Expect.equal result (Some(a, b)) "Result should be Some (a, b)."

            testCase "Tuple.sequenceAsync: (Async<a> * Async<b>) -> Async<a * b>"
            <| fun _ ->
                // arrange
                let a = "hello there"
                let b = 42
                let tup = async.Return a, async.Return b

                // act
                let (resultA, resultB) =
                    Tuple.sequenceAsync tup |> Async.RunSynchronously

                // assert

                Expect.equal (resultA, resultB) (a, b) "Result should be Async<a * b>."

        ]
