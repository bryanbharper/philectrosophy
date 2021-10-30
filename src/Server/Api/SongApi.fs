namespace Server.Api

open Server.Data.SongRepository

module SongApi =

    open Shared

    let getSongsAsync (repo: ISongRepository) =
        repo.GetAll()

    let updateListenCount (repo: ISongRepository) slug =
        async {
            let! entryOp = repo.GetSingle slug

            return!
                entryOp
                |> Option.map (fun e -> repo.Update { e with PlayCount = e.PlayCount + 1 })
                |> Option.sequenceAsync
                |> Async.map (Option.flatten >> Option.map (fun e -> e.PlayCount))
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


