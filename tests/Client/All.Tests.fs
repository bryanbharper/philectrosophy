module Client.Tests.All

open Fable.Mocha

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.All.all
#endif
            BlogEntry.all
            Blog.all
            Urls.all
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all
