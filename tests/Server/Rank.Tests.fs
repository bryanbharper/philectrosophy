module Server.Tests.Rank

open System
open System.Linq
open Expecto

open Server

open Shared.Extensions
open Shared.Dtos

type TestData =
    {
        Name: string
        Description: string
        Number: int
    }

let testData =
     [
        { Name = "abcd"; Description = "efgh"; Number = 1 }
        { Name = "ijkl"; Description = "mnop"; Number = 2 }
        { Name = "qrst"; Description = "uvwx"; Number = 3 }
        { Name = "yzab"; Description = "cdef"; Number = 4 }
     ]

let ninja =
    testList
        "Ninja"
        [
            testCase "search: all data supplied when search term not supplied"
                <| fun _ ->
                    // arrange

                    // act
                    let result =
                        testData
                        |> Ninja.search [| Rank.FunAs.LinqExpression(fun td -> td.Name) |]

                    // assert
                    Expect.equal (Seq.toList result) testData "Input and result should be equivalent"

            testCase "search |> containing: returns only results with match"
                <| fun _ ->
                    // arrange
                    let searchTerm = "cd"

                    // act
                    let result =
                        testData
                        |> Ninja.search [| Rank.FunAs.LinqExpression(fun td -> td.Name) |]
                        |> Ninja.containing [| searchTerm |]

                    // assert
                    Expect.all result (fun td -> td.Name.Contains(searchTerm)) "All results contain match"

            testCase "search |> containing: case insensitive for search term"
                <| fun _ ->
                    // arrange
                    let searchTerm = "CD"

                    // act
                    let result =
                        testData
                        |> Ninja.search [| Rank.FunAs.LinqExpression(fun td -> td.Name) |]
                        |> Ninja.containing [| searchTerm |]

                    // assert
                    Expect.all result (fun td -> td.Name.Contains(String.toLower searchTerm)) "All results contain match"
                    Expect.equal (result.Count()) 1 "There should be one result"

            testCase "search |> containing: case insensitive for test data"
                <| fun _ ->
                    // arrange
                    let searchTerm = "cd"
                    let testData' =
                        [
                            { Name = "abCD"; Description = "efgh"; Number = 1 }
                            { Name = "ijkl"; Description = "mnop"; Number = 2 }
                            { Name = "qrst"; Description = "uvwx"; Number = 3 }
                            { Name = "yzab"; Description = "cdef"; Number = 4 }
                        ]

                    // act
                    let result =
                        testData'
                        |> Ninja.search [| Rank.FunAs.LinqExpression(fun td -> td.Name) |]
                        |> Ninja.containing [| searchTerm |]

                    // assert
                    Expect.all result (fun td -> td.Name.ToLower().Contains(String.toLower searchTerm)) "All results contain match"
                    Expect.equal (result.Count()) 1 "There should be one result"

            testCase "search |> containing: returns results matching either term"
                <| fun _ ->
                    // arrange
                    let searchTerm1 = "cd"
                    let searchTerm2 = "jk"

                    // act
                    let result =
                        testData
                        |> Ninja.search [| Rank.FunAs.LinqExpression(fun td -> td.Name) |]
                        |> Ninja.containing [| searchTerm1; searchTerm2 |]

                    // assert
                    Expect.all
                        result
                        (fun td -> td.Name.Contains(searchTerm1) || td.Name.Contains(searchTerm2))
                        "Results match either search term"

            testCase "search |> containing: searches across multiple properties"
                <| fun _ ->
                    // arrange
                    let searchTerm = "cd"

                    // act
                    let result =
                        testData
                        |> Ninja.search
                               [|
                                   Rank.FunAs.LinqExpression(fun td -> td.Name)
                                   Rank.FunAs.LinqExpression(fun td -> td.Description)
                               |]
                        |> Ninja.containing [| searchTerm |]

                    // assert
                    Expect.all
                        result
                        (fun td -> td.Name.Contains(searchTerm)  || td.Description.Contains(searchTerm))
                        "All results contain match"


            testCase "search |> containing: handles multiples terms and properties"
                <| fun _ ->
                    // arrange
                    let searchTerm1 = "cd"
                    let searchTerm2 = "jk"

                    // act
                    let result =
                        testData
                        |> Ninja.search
                               [|
                                   Rank.FunAs.LinqExpression(fun td -> td.Name)
                                   Rank.FunAs.LinqExpression(fun td -> td.Description)
                               |]
                        |> Ninja.containing [| searchTerm1; searchTerm2 |]

                    // assert
                    Expect.all
                        result
                        (fun td ->
                            td.Name.Contains(searchTerm1)  || td.Description.Contains(searchTerm1)
                            || td.Name.Contains(searchTerm2)  || td.Description.Contains(searchTerm2)
                        )
                        "All results contain match"
        ]

let rank =
    testList
        "Rank"
        [
            testCase "entries: returns empty list without matches"
            <| fun _ ->
                // arrange
                let query = "hippo talk duck walk book eat cheese"

                let entries =
                    [
                        "Zero Entry"
                        |> BlogEntry.create
                        |> BlogEntry.setTags "this,is,a,disjoint,list"

                        "Nada Post"
                        |> BlogEntry.create
                        |> BlogEntry.setTags "these,words,are,not,above"
                    ]

                // act
                let result = Rank.entries query entries

                // assert
                Expect.isEmpty result ""

            testCase "entries: does not rank on common words"
            <| fun _ ->
                // arrange
                let query = "again cat because into a the about under through taco"

                let entries =
                    [
                        "Zero Entry"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle "because into a the about"

                        "Nada Post"
                        |> BlogEntry.create
                        |> BlogEntry.setSynopsis "the about under through"

                        "Taco Cat"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle "taco"
                        |> BlogEntry.setSynopsis "cat"
                    ]

                // act
                let result = Rank.entries query entries

                // assert
                Expect.equal (List.length result) 1 "Should have one result"
                Expect.equal (List.head result) (List.last entries) "Result should match"

            testCase "entries: ranks on subtitle"
            <| fun _ ->
                // arrange
                let loMatch = "alPha (bRavo) charLie."
                let midMatch = loMatch + " dElta echO [foXtrot]"
                let hiMatch = midMatch + " Golf! hoTel inDia?"

                let entries =
                    [
                        "Hi Match"
                        |> BlogEntry.create
                        |> BlogEntry.setSubtitle (Some hiMatch)

                        "Lo Match"
                        |> BlogEntry.create
                        |> BlogEntry.setSubtitle (Some loMatch)

                        "Mid Match"
                        |> BlogEntry.create
                        |> BlogEntry.setSubtitle (Some midMatch)

                    ]

                // act
                let result = Rank.entries hiMatch entries

                // assert
                Expect.equal result.[0] entries.[0] "Highest should be first"
                Expect.equal result.[1] entries.[2] "Middle should be second"
                Expect.equal result.[2] entries.[1] "Lowest should be last"

            testCase "entries: ranks on synopsis"
            <| fun _ ->
                // arrange
                let loMatch = "alPha (bRavo) charLie."
                let midMatch = loMatch + " dElta echO [foXtrot]"
                let hiMatch = midMatch + " Golf! hoTel inDia?"

                let entries =
                    [
                        "Hi Match"
                        |> BlogEntry.create
                        |> BlogEntry.setSynopsis hiMatch

                        "Lo Match"
                        |> BlogEntry.create
                        |> BlogEntry.setSynopsis loMatch

                        "Mid Match"
                        |> BlogEntry.create
                        |> BlogEntry.setSynopsis midMatch

                    ]

                // act
                let result = Rank.entries hiMatch entries

                // assert
                Expect.equal result.[0] entries.[0] "Highest should be first"
                Expect.equal result.[1] entries.[2] "Middle should be second"
                Expect.equal result.[2] entries.[1] "Lowest should be last"

            testCase "entries: ranks on tags"
            <| fun _ ->
                // arrange
                let loMatch = "alPha (bRavo) charLie."
                let midMatch = loMatch + " dElta echO [foXtrot]"
                let hiMatch = midMatch + " Golf! hoTel inDia?"

                let entries =
                    [
                        "Hi Match"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle hiMatch

                        "Lo Match"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle loMatch

                        "Mid Match"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle midMatch

                    ]

                // act
                let result = Rank.entries hiMatch entries

                // assert
                Expect.equal result.[0] entries.[0] "Highest should be first"
                Expect.equal result.[1] entries.[2] "Middle should be second"
                Expect.equal result.[2] entries.[1] "Lowest should be last"

            testCase "entries: ranks on title"
            <| fun _ ->
                // arrange
                let loMatch = "alPha (bRavo) charLie."
                let midMatch = loMatch + " dElta echO [foXtrot]"
                let hiMatch = midMatch + " Golf! hoTel inDia?"

                let entries =
                    [
                        "Hi Match"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle (hiMatch) // |> String.split ' ' |> String.join ',')

                        "Lo Match"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle (loMatch) // |> String.split ' ' |> String.join ',')

                        "Mid Match"
                        |> BlogEntry.create
                        |> BlogEntry.setTitle (midMatch) // |> String.split ' ' |> String.join ',')

                    ]

                // act
                let result = Rank.entries hiMatch entries

                // assert
                Expect.equal result.[0] entries.[0] "Highest should be first"
                Expect.equal result.[1] entries.[2] "Middle should be second"
                Expect.equal result.[2] entries.[1] "Lowest should be last"

            testCase "entries: correctly ranks on multiple fields"
            <| fun _ ->
                // arrange
                let loMatch = "alPha (bRavo) charLie."
                let midMatch = loMatch + " dElta echO [foXtrot]"
                let hiMatch = midMatch + " Golf! hoTel inDia?"

                let lowest =
                    BlogEntry.empty
                    |> BlogEntry.setTitle loMatch

                let middle =
                    BlogEntry.empty
                    |> BlogEntry.setTags (midMatch) // |> String.split ' ' |> String.join ',')

                let highest =
                    BlogEntry.empty
                    |> BlogEntry.setSynopsis hiMatch

                // act
                let result = Rank.entries hiMatch [lowest; highest; middle]

                // assert
                Expect.equal result.[0] highest "Highest should be first"
                Expect.equal result.[1] middle "Middle should be second"
                Expect.equal result.[2] lowest "Lowest should be last"

        ]

let all =
    testList
        "All"
        [
            rank
            ninja
        ]
