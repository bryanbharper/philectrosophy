module Shared.Tests.DomainModels

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open System
open Expecto.Logging
open Shared

let all = testList "DomainModel" [
   testCase "BlogEntry.create initializes with default values"
   <| fun _ ->
       // arrange
       let title = "An introduction to property-based testing"

       // act
       let beforeTime = DateTime.UtcNow
       let result = BlogEntry.create title
       let afterTime = DateTime.UtcNow

       // assert
       Expect.equal result.Author "Bryan B. Harper" "I am default author."
       Expect.isTrue (result.CreatedOn >= beforeTime) "Created date is UtcNow" // isGreaterThanOrEqual not defined in Mocha
       Expect.isTrue (result.CreatedOn <= afterTime) "Created date is UtcNow" // isLessThanOrEqual not defined in Mocha
       Expect.equal result.Slug (String.slugify title) "result.Slug is slugified title"
       Expect.equal result.Synopsis (sprintf "A blog about: %s" title) "Synopsis generated out of title"
       Expect.equal result.Tags "" "Tags is empty by default"
       Expect.equal result.ThumbNailUrl "https://picsum.photos/100/100" "Uses Lorem Pic-sum 100 x 100"
       Expect.equal result.Title title "result.Title is equal to title provided"
       Expect.equal result.UpdatedOn None "UpdatedOn is None by default"

]
