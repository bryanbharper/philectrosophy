namespace Server.Config


[<CLIMutable>]
type Database = { ConnectionString: string }

[<CLIMutable>]
type Config = { Database: Database }
