namespace SleepTracker.Controllers

open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open SleepTracker.Models
open SleepTracker.Services
open SleepTracker.Models.Entities
open Microsoft.Extensions.Configuration

[<ApiController>]
[<Route("[controller]")>]
type SportController (logger : ILogger<SportController>, configuration: IConfiguration) =
    inherit ControllerBase()

    member this.ConnString = configuration.GetConnectionString(DatabaseService.ConnectionName)

    [<HttpGet>]
    member this.GetAll () =
        let sports = 
            async {
                let! tmpSports = DatabaseService.getSports(this.ConnString) |> Async.AwaitTask
                return tmpSports
            } |> Async.RunSynchronously
        sports |> Seq.map(fun s -> { SportID = s.SportID; SportName = s.SportName; SportNotes = s.SportNotes })

    [<HttpPost>]
    member this.Create() =
        if not (this.ModelState.IsValid) then
            this.BadRequest() :> ActionResult
        else
            let existingSports = 
                async {
                    let! tmpSports = DatabaseService.getSports(this.ConnString) |> Async.AwaitTask
                    return tmpSports
                } |> Async.RunSynchronously
            let sport = existingSports |> Seq.tryFind (fun s -> s.SportName = (this.Request.Form["SportName"] |> Seq.head))
            match sport with
            | Some _ -> this.Conflict()
            | None ->
                let newSport : SportEntity.Sport = { SportID = 0; SportName = (this.Request.Form["SportName"] |> Seq.head); SportNotes = (this.Request.Form["SportNotes"] |> Seq.head) }
                let result = DatabaseService.createSport (this.ConnString)  newSport
                match result with
                | Some sport -> this.Created("", sport) :> ActionResult
                | _ -> this.BadRequest() :> ActionResult

    [<HttpPut("{sportid}")>]
    member this.Update(sportid: int) =
        if not (this.ModelState.IsValid) then
            this.BadRequest() :> ActionResult
        else
            let existingSport = DatabaseService.getSportById (this.ConnString) sportid
            match existingSport with
            | Some sport ->
                let sportToUpdate = { sport with SportName = (this.Request.Form["SportName"] |> Seq.head); SportNotes = (this.Request.Form["SportNotes"] |> Seq.head) }
                let updatedSport = DatabaseService.updateSport (this.ConnString) sportToUpdate
                this.Ok(updatedSport) :> ActionResult
            | _ -> this.NotFound() :> ActionResult

