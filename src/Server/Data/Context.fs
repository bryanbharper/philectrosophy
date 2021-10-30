namespace Server.Data

open Dapper.FSharp
open Dapper.FSharp.MySQL
open Microsoft.Extensions.Configuration
open MySql.Data.MySqlClient
open Server.Config
open Shared
open System.Data


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
