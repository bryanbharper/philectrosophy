module Server.Tests.All

open Expecto

let all =
    testList "All"
        [
            Data.all
            BlogApi.all
            File.all
            Markdown.all
            Rank.all
            Shared.Tests.All.all
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all
