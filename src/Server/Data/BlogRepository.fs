namespace Server.Data

open Shared

type IBlogRepository =
    abstract GetAll: unit -> Async<BlogEntry list>
    abstract GetSingle: slug:string -> Async<BlogEntry option>
    abstract Update: newEntry:BlogEntry -> Async<BlogEntry option>

type BlogRepository(context: IContext) =
    let getEntries =
        fun () -> context.GetTable<BlogEntry> Tables.BlogEntries.name

    let getEntry (slug: string): Async<BlogEntry option> =
        slug
        |> context.GetByValue Tables.BlogEntries.name Tables.BlogEntries.id
        |> Async.map (fun r -> if (Seq.length r) > 0 then r |> Seq.head |> Some else None)

    let updater =
        context.Update Tables.BlogEntries.name Tables.BlogEntries.id

    interface IBlogRepository with
        member this.GetAll() = getEntries () |> Async.map List.ofSeq

        member this.GetSingle slug = getEntry slug

        member this.Update entry =
            async {
                let! rowsAffected = updater entry.Slug entry
                return if rowsAffected > 0 then Some entry else None
            }
