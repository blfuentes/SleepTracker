namespace SleepTracker.Services


module DatabaseService =

    open System.Data.SqlClient
    open Dapper.FSharp.MSSQL
    open SleepTracker.Models.Entities

    let private connectionString = "Server=mssqlserver,1433;Database=SleepTracker;User Id=sa;Password=testpassword123!;"
    let getSports () =
        let sportTable = table<SportEntity.Sport>
        let conn = new SqlConnection(connectionString)
        conn.Open()
        select {
            for s in sportTable do
            selectAll
        } |> conn.SelectAsync<SportEntity.Sport>