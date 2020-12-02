module Server.Tests

open Expecto

let all =
    testList "All"
        [
            Shared.Tests.all
            BlogApiTests.all
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all
