module Server.Data

open Dapper.FSharp
open Dapper.FSharp.MySQL
open Microsoft.Extensions.Configuration
open MySql.Data.MySqlClient
open Server.Config
open Shared
open System.Data

module Tables =
    module BlogEntries =
        let name = "blogentries"
        let id = "Slug"

type IContext =
    abstract GetTable: tableName:string -> Async<'a seq>
    abstract GetByValue: tableName:string -> column:string -> exactValue:'a -> Async<'b seq>
    abstract Update: tableName:string -> idColumn:string -> id:'a -> entry:'b -> Async<int>

type DbContext(config: IConfiguration) =
    let connStr =
        config.Get<Config>().Database.ConnectionString

    let connection: IDbConnection = upcast new MySqlConnection(connStr)

    interface IContext with
        member this.GetTable<'a> tableName =
            select { table tableName }
            |> connection.SelectAsync<'a>
            |> Async.AwaitTask

        member this.GetByValue<'a, 'b> tableName column (exactValue: 'a) =
            select {
                table tableName
                where (eq column exactValue)
            }
            |> connection.SelectAsync<'b>
            |> Async.AwaitTask

        member this.Update tableName idCol id entry =
            update {
                table tableName
                set entry
                where (eq idCol id)
            }
            |> connection.UpdateAsync
            |> Async.AwaitTask

type IRepository =
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

    interface IRepository with
        member this.GetAll() = getEntries () |> Async.map List.ofSeq

        member this.GetSingle slug = getEntry slug

        member this.Update entry =
            async {
                let! rowsAffected = updater entry.Slug entry
                return if rowsAffected > 0 then Some entry else None
            }
