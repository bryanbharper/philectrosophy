module Server.Data.SongRepository

open Shared

type ISongRepository =
    abstract GetAll: unit -> Async<Song list>
    abstract GetSingle: slug:string -> Async<Song option>
    abstract Update: newEntry:Song -> Async<Song option>

type SongRepository(context: IContext) =
    let getEntries =
        fun () -> context.GetTable<Song> Tables.Songs.name

    let getEntry (slug: string): Async<Song option> =
        slug
        |> context.GetByValue Tables.Songs.name Tables.Songs.id
        |> Async.map (fun r -> if (Seq.length r) > 0 then r |> Seq.head |> Some else None)

    let updater =
        context.Update Tables.Songs.name Tables.Songs.id

    interface ISongRepository with
        member this.GetAll() = getEntries () |> Async.map List.ofSeq

        member this.GetSingle slug = getEntry slug

        member this.Update entry =
            async {
                let! rowsAffected = updater entry.Slug entry
                return if rowsAffected > 0 then Some entry else None
            }
