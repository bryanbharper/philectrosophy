module Shared.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

let all = testList "Shared" [
    StringTests.all
]
