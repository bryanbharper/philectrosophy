module Server.Tests.All

open Expecto

let all =
    testList "All"
        [
            Shared.Tests.All.all
            BlogApi.all
            File.all
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all
