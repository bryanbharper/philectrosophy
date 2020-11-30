module Client.Tests

open Fable.Mocha

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.shared
#endif
            Tests.BlogEntry.all
            Tests.Index.all
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all
