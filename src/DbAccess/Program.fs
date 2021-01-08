open System.Data
open Dapper.FSharp
open Dapper.FSharp.MySQL
open FSharp.Data
open MySql.Data.MySqlClient
open Server.Data

type Config =  JsonProvider<"appsettings.DbAccess.json">


[<EntryPoint>]
let main argv =
    OptionTypes.register ()

    use connection: IDbConnection =
        upcast new MySqlConnection(Config.GetSample().ProdConnectionString)

    let repo: IRepository = upcast InMemoryRepository()
    let entries = repo.GetBlogEntriesAsync() |> Async.RunSynchronously

    insert {
        table "blogentries"
        values entries
    }
    |> connection.InsertAsync
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

    printfn "Data Access Complete"
    0 // return an integer exit code
