namespace SleepTracker.Services


module DatabaseService =

    open System.Data.SqlClient
    open Dapper.FSharp.MSSQL
    open SleepTracker.Models.Entities

    let getSports (connectionString: string) =
        let sportTable = table<SportEntity.Sport>
        let conn = new SqlConnection(connectionString)
        conn.Open()
        select {
            for s in sportTable do
            selectAll
        } |> conn.SelectAsync<SportEntity.Sport>

    let createSport (connectionString: string, sport: SportEntity.Sport) =
        let sportTable = table<SportEntity.Sport>
        let conn = new SqlConnection(connectionString)
        conn.Open()
        try
            let result = 
                insert {
                    for s in sportTable do
                    value sport
                    excludeColumn sport.SportID
                } |> conn.InsertAsync
            result
        with
            | _ -> System.Threading.Tasks.Task.FromResult(0)