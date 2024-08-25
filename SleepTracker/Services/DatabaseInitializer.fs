namespace SleepTracker.Services


module DatabaseInitializer =
    open Dapper.FSharp
    open System.Data
    open Microsoft.Data.Sqlite
    open Microsoft.Extensions.Configuration
    open SleepTracker.Utils.ExtensionMethods

    let mutable isAlreadyInitialized = false
    let DbName = "SleepTracker"

    let safeInit (conn:IDbConnection) =
        task {
            if System.IO.File.Exists("./Data/SleepTracker.sqlite") then
                System.IO.File.Delete("./Data/SleepTracker.sqlite")

            if isAlreadyInitialized |> not then
                conn.Open()

                isAlreadyInitialized <- true
                Dapper.FSharp.SQLite.OptionTypes.register()

                let scriptFile = System.IO.File.ReadAllText("./Data/setup.sql")
                do! conn.ExecuteIgnore(scriptFile)
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously