namespace SleepTracker.Services



module DatabaseService =

    open Microsoft.Data.Sqlite
    open Dapper.FSharp.SQLite
    open SleepTracker.Models.Entities

    let ConnectionName = "SqliteConnection"

    let getConnection (connectionString:string) =
        new SqliteConnection(connectionString)

    let getSports (connectionString: string) =
        let sportTable = table<SportEntity.Sport>
        let conn = getConnection(connectionString)
        conn.Open()
        select {
            for s in sportTable do
            selectAll
        } |> conn.SelectAsync<SportEntity.Sport>

    let getSportById (connectionString: string) (sportId: int) =
        let sportTable = table<SportEntity.Sport>
        let conn = getConnection(connectionString)
        conn.Open()
        let result = 
            select {
                for s in sportTable do
                where (s.SportID = sportId)
                selectAll
            } 
            |> conn.SelectAsync<SportEntity.Sport>
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> Seq.tryHead
        result


    let createSport (connectionString: string) (sport: SportEntity.Sport) : Option<SportEntity.Sport> =
        let sportTable = table<SportEntity.Sport>
        let conn = getConnection(connectionString)
        conn.Open()
        try
            let oldIds = 
                select {
                    for s in sportTable do
                    selectAll
                } 
                |> conn.SelectAsync<SportEntity.Sport> 
                |> Async.AwaitTask 
                |> Async.RunSynchronously
                |> Seq.map (fun s -> s.SportID)
                |> Set.ofSeq
            let result' = 
                insert {
                    for s in sportTable do
                    value sport
                    excludeColumn sport.SportID
                } |> conn.InsertAsync
            let newIds = 
                select {
                    for s in sportTable do
                    selectAll
                }
                |> conn.SelectAsync<SportEntity.Sport>
                |> Async.AwaitTask
                |> Async.RunSynchronously 
                |> Seq.map (fun s -> s.SportID)
                |> Set.ofSeq
            let insertedId = (newIds - oldIds) |> Set.minElement
            let result = 
                select {
                    for s in sportTable do
                    where (s.SportID = insertedId)
                }
                |> conn.SelectAsync<SportEntity.Sport>
                |> Async.AwaitTask
                |> Async.RunSynchronously
                |> Seq.head
            Some(result)
        with
            | _ -> None

    let updateSport (connectionString: string) (sport: SportEntity.Sport) : Option<SportEntity.Sport> =
        let sportTable = table<SportEntity.Sport>
        let conn = getConnection(connectionString)
        conn.Open()
        try
            let result = 
                update {
                    for s in sportTable do
                    setColumn s.SportName sport.SportName
                    setColumn s.SportNotes sport.SportNotes
                    where (s.SportID = sport.SportID)
                } |> conn.UpdateAsync
            Some(sport)
        with
            | _ -> None