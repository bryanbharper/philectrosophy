module Server.Tests.All

open Expecto

let all =
    testList "All"
        [
            Shared.Tests.All.all
            Markdown.all
            BlogApi.all
            File.all
            Rank.all
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all
