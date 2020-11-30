module Index.Tests

open Fable.Mocha
open Fable.React

let all =
    testList
        "Index"
        [
            testCase "Index.active page"
            <| fun _ ->
                // arrange
                let url = []

                // act
                let result: HTMLNode = downcast Index.activePage url

                // assert
                ()
        ]
