module Server.Tests.All

open Expecto

let all =
    testList "Server Tests"
        [
            Data.BlogRepository.all
            Data.SongRepository.all
            Api.BlogApi.all
            Api.SongApi.all
            File.all
            Markdown.all
            Rank.all
            Shared.Tests.All.all
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all
