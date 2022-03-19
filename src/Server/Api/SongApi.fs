namespace Server.Api

open Shared.Extensions
open Shared.Contracts

open Server.Data.SongRepository

module SongApi =

    let getSongsAsync (repo: ISongRepository) =
        repo.GetAll()

    let updateListenCount (repo: ISongRepository) slug =
        async {
            let! songOption = repo.GetSingle slug

            return!
                songOption
                |> Option.map (fun e -> repo.Update { e with PlayCount = e.PlayCount + 1 })
                |> Option.sequenceAsync
                |> Async.map Option.flatten
        }

    let songApiReader =
        reader {
            let! repo = resolve<ISongRepository> ()

            return
                {
                    GetSongs = fun () -> getSongsAsync repo
                    UpdateListenCount = updateListenCount repo
                }
        }


