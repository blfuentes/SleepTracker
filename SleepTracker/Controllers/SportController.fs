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

    [<HttpGet>]
    member _.Get () =
        let sports = 
            async {
                let! tmpSports = DatabaseService.getSports(configuration.GetConnectionString(DatabaseService.ConnectionName)) |> Async.AwaitTask
                return tmpSports
            } |> Async.RunSynchronously
        sports |> Seq.map(fun s -> { SportID = s.SportID; SportName = s.SportName; SportNotes = s.SportNotes })

    [<HttpPost>]
    member this.CreatePost() =
        if not (this.ModelState.IsValid) then
            this.BadRequest() :> ActionResult
        else
            let existingSports = 
                async {
                    let! tmpSports = DatabaseService.getSports(configuration.GetConnectionString(DatabaseService.ConnectionName)) |> Async.AwaitTask
                    return tmpSports
                } |> Async.RunSynchronously
            let sport = existingSports |> Seq.tryFind (fun s -> s.SportName = (this.Request.Form["SportName"] |> Seq.head))
            match sport with
            | Some _ -> this.Conflict() :> ActionResult
            | None ->
                let newSport : SportEntity.Sport = { SportID = 0; SportName = (this.Request.Form["SportName"] |> Seq.head); SportNotes = (this.Request.Form["SportNotes"] |> Seq.head) }
                let result = DatabaseService.createSport(configuration.GetConnectionString(DatabaseService.ConnectionName), newSport) |> Async.AwaitTask |> Async.RunSynchronously
                match result with
                | 1 -> this.Created() :> ActionResult
                | _ -> this.BadRequest() :> ActionResult
