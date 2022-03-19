module Shared.Tests.String

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
open System.Linq
#endif


open FsCheck
open Shared.Extensions

#if !FABLE_COMPILER
let properties = testList "String Property Tests" [

    testProperty "String.contains: testOracle."
    <| fun (NonNull contained) (NonNull (container: string)) ->
        // arrange
        let oracle = container.Contains contained

        // act
        let result = container |> String.contains contained

        // assert
        result = oracle

    testProperty "String.trim: testOracle."
    <| fun (NonNull (original: string)) ->
        // arrange
        let oracle = original.Trim()

        // act
        let result = original |> String.trim

        // assert
        result = oracle

    testProperty "String.ofChars: input & reversed have the same characters."
    <| fun (NonNull (arbitraryString: string)) ->
        let arbitraryChars =
            arbitraryString
            |> Seq.map id

        let result =
            arbitraryChars
            |> String.ofChars

        // assert
        result = arbitraryString

    testProperty "String.split: oracle"
    <| fun (delimiter: char) (NonNull (original: string)) ->
        // arrange
        let oracle = original.Split delimiter

        // act
        let result = String.split delimiter original

        // assert
        result = oracle

    testProperty "String.replace: testOracle."
    <| fun (NonEmptyString (oldVal: string)) (NonNull (newVal: string)) (NonNull (original: string)) ->
        // arrange
        let oracle = original.Replace(oldVal, newVal)

        // act
        let result = original |> String.replace oldVal newVal

        // assert
        result = oracle

    testProperty "String.strip: is associative."
    <| fun (NonNull strip1) (NonNull strip2) (NonNull original) ->

        // arrange / act
        let strip1then2 = original |> String.strip strip1 |> String.strip strip2
        let strip2then1 = original |> String.strip strip2 |> String.strip strip1

        // assert
        strip1then2 = strip2then1

    testProperty "String.strip: has right identity."
    <| fun (NonNull original) ->

        // arrange / act
        let result = original |> String.strip ""


        // assert
        result = original

    testProperty "String.strip: empty string is unaffected."
    <| fun (NonNull strip) ->

        // arrange / act
        let result = "" |> String.strip strip


        // assert
        result = ""

    testProperty "String.strip: stripping again has no impact."
    <| fun (NonNull strip) (NonNull input) ->

        // arrange / act
        let firstStrip = input |> String.strip strip
        let result = firstStrip |> String.strip strip

        // assert
        result = firstStrip

    testProperty "String.strip: stripped characters are not in result"
    <| fun (NonNull strip) (NonNull input) ->
        let stripSeq = strip |> Seq.map id

        // act
        let result = input |> String.strip strip

        // assert
        result |> Seq.forall (fun c -> stripSeq.Contains(c) |> not)

    testProperty "String.urlEncode: testOracle."
    <| fun (NonNull value) ->
        // arrange
        let oracle = System.Uri.EscapeDataString value

        // act
        let result = value |> String.urlEncode

        // assert
        result = oracle

]
#endif
let all = testList "String" [
#if !FABLE_COMPILER
    properties
#endif

    testCase "String.contains returns true when characters are contained."
    <| fun _ ->
        // arrange
        let contained = "happily tuesday for brunch"
        let input = "mix doggo" + contained + "fried fish"

        // act
        let result = input |> String.contains contained

        // act
        Expect.isTrue result "The result should be true."

    testCase "String.contains returns false when characters are not contained."
    <| fun _ ->
        // arrange
        let contained = "happily tuesday for brunch"
        let input = "mix doggo" + "fried fish"

        // act
        let result = input |> String.contains contained

        // act
        Expect.isFalse result "The result should be false."

    testCase "String.trim: basic use case - removes space at head and end of string"
    <| fun _ ->
        // arrange
        let expected = "blah blah blah"
        let someStr = "  " + expected + " "

        // act
        let result = String.trim someStr

        //
        Expect.equal result expected "The result should equal expected."

    testCase "String.ofChars: basic use case - behaves as expected."
    <| fun _ ->
        // arrange
        let input = [ 'H'; 'e'; 'l'; 'l'; 'o'; ]

        // act
        let result = String.ofChars input

        //
        Expect.equal result "Hello" "The result should be 'Hello'."

    testCase "String.split: basic use case - splits a comma separated string."
    <| fun _ ->
        // arrange
        let data = "one,two,three"

        // act
        let result = String.split ',' data

        // assert
        Expect.equal result [| "one"; "two"; "three" |] "Result should be list of 'one' 'two' 'three'"

    testCase "String.split: basic use case - removes chars from input"
    <| fun _ ->
        // arrange
        let vowels = "aeiou"
        let input = "sequoia"

        // act
        let result = input |> String.strip vowels

        // assert
        Expect.equal result "sq" "Vowels should be removed from 'sequoia'"

    testCase "String.slugify: basic use case - makes sentence into slug"
    <| fun _ ->
        // arrange
        let expected = "a-journey-a-how-to-adventure-tacos-for-almost-everyone"
        let input = " A Journey: a how-to adventure & tacos for (almost) everyone!  "

        // act
        let result = input |> String.slugify

        // assert
        Expect.equal result expected "Result is equal to expected."

]


