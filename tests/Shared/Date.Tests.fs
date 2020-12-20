module Shared.Tests.Date

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open System
open Shared
open FsCheck

let months =
    [
        "January"
        "February "
        "March"
        "April"
        "May"
        "June"
        "July"
        "August"
        "September"
        "October"
        "November"
        "December"
    ]

#if !FABLE_COMPILER
let properties = testList "Date Property Tests" [

    testProperty "MonthInt.create/ToInt32: value is always between 1 and 12."
    <| fun (randVal: int) ->
        // act
        let result = MonthInt.create randVal

        // arrange
        let i = result.ToInt32()

        // assert
        i >= 1 && i <= 12

    testProperty "MonthInt.create/ToInt32: mod 12"
    <| fun (PositiveInt (randVal: int)) ->
        // act
        let result = MonthInt.create randVal

        // arrange
        let i = result.ToInt32()

        // assert
        if randVal % 12 = 0
        then i = 12
        else i = (randVal % 12)

    testProperty "DayInt.create/ToInt32: value is always between 1 and 31."
    <| fun (randVal: int) ->
        // act
        let result = DayInt.create randVal

        // arrange
        let i = result.ToInt32()

        // assert
        i >= 1 && i <= 31

    testProperty "MonthInt.create/ToInt32: mod 31"
    <| fun (PositiveInt (randVal: int)) ->
        // act
        let result = DayInt.create randVal

        // arrange
        let i = result.ToInt32()

        // assert
        if randVal % 31 = 0
        then i = 31
        else i = (randVal % 31)

    testProperty "Date.monthName: maps to correct name."
    <| fun (PositiveInt (randVal: int)) ->
        // arrange
        let mInt = MonthInt.create randVal

        // act
        let name = Date.monthName mInt

        // assert
        name = months.[mInt.ToInt32() - 1]

]
#endif
let all = testList "Date" [

#if !FABLE_COMPILER
    properties
#endif

    testCase "Date.format correctly formats the date"
    <| fun _ ->
        // arrange
        let date = DateTime(1969, 07, 20)

        // act
        let result = Date.format date

        // assert
        Expect.equal result "20th July 1969" "Houston, we have a problem."

    testCase "Date.daySuffix: maps to correct suffix."
    <| fun _ ->
        // arrange
        let getExpected i =
            match i % 10 with
            | 1 -> "st"
            | 2 -> "nd"
            | 3 -> "rd"
            | _ -> "th"

        let testData =
            [1..31]
            |> List.map (fun i -> (DayInt.create i, getExpected i))

        for d, expected in testData do
            // act
            let result = Date.daySuffix d

            // assert
            Expect.equal result expected "Suffix should be correct."

]
