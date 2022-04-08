namespace Server

open System
open NinjaNye.SearchExtensions

module Ninja =
    let search expressions (queryable: 'a seq) =
        queryable
            .Search(expressions)
            .SetCulture(StringComparison.OrdinalIgnoreCase)

    let containing (searchTerms: string []) (searchResult: EnumerableStringSearch<'a>) =
        searchResult.Containing(searchTerms)

    let toRanked (searchResult: EnumerableStringSearch<'a>) = searchResult.ToRanked()


open System.Linq.Expressions
open Shared
open Shared.Extensions
open Shared.Dtos

module Rank =
    type FunAs() =
        static member LinqExpression<'T, 'TResult>(e: Expression<Func<'T, 'TResult>>) = e

    let filterCommon (words: string array) =
        words
        |> Array.filter (fun w -> StopWords.all |> List.contains (String.toLower w) |> not)

    let entries (searchQuery: string) (entries: BlogEntry list) =

        let queries =
            [|
               FunAs.LinqExpression (fun e -> if e.Subtitle.IsSome then e.Subtitle.Value else "")
               FunAs.LinqExpression(fun e -> e.Synopsis)
               FunAs.LinqExpression(fun e -> e.Tags) // |> String.split ',' |> String.concat " ")
               FunAs.LinqExpression(fun e -> e.Title)
            |]

        entries
        |> Ninja.search queries
        |> Ninja.containing (searchQuery |> String.split ' ' |> filterCommon)
        |> Ninja.toRanked
        |> Seq.sortByDescending (fun r -> r.Hits)
        |> Seq.map (fun r -> r.Item)
        |> Seq.toList
