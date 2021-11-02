module Shared.Tests.Math

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif


open FsCheck
open Shared.Math.Operators

let regularTests =
    testList
        "Math"
        [
            testCase "-% handles negative values"
            <| fun _ ->
                // arrange
                let modulus = 10
                let inputs = { -1 .. -20 }
                let expectedResults =
                    { 9 .. 0 }
                    |> Seq.replicate 2
                    |> Seq.concat

                // act
                let results =
                    inputs
                    |> Seq.map (fun i -> i -% modulus)

                // assert
                Seq.iter2 (fun e r -> Expect.equal e r "") expectedResults results

        ]


#if !FABLE_COMPILER
let propertyTests =
    testList
        "Math Property Tests"
        [

            testProperty "-% Returns remainder of positive integers"
            <| fun (PositiveInt (randVal: int)) ->
                // arrange
                let modulus = 5
                let oracle = randVal % modulus

                // act
                let result = randVal -% modulus

                // assert
                result = oracle

            testProperty "-% returns zero when n = 0"
            <| fun (PositiveInt (randMod: int)) ->
                // arrange
                // act
                let result = 0 -% randMod

                // assert
                result = 0


            testProperty "-% returns zero when n = m"
            <| fun (PositiveInt (randMod: int)) ->
                // arrange
                // act
                let result = randMod -% randMod

                // assert
                result = 0

            testProperty "-% returns correct value for n < 0"
            <| fun (NegativeInt (randVal: int)) ->
                // arrange
                let modulus = 10

                let expected =
                    (randVal % modulus
                    |> abs
                    |> (-) modulus) % modulus

                // act
                let result = randVal -% modulus

                // assert
                result = expected
        ]
#endif


let all =
    testList
        "All Math"
        [
            regularTests
#if !FABLE_COMPILER
            propertyTests
#endif
        ]
