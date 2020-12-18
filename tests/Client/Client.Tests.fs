module Client.Tests

open Fable.Mocha

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.all
#endif
            BlogEntryTests.all
            BlogTests.all
            IndexTests.all
            UrlsTests.all
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all
