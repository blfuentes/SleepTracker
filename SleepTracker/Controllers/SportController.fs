namespace SleepTracker.Controllers

open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open SleepTracker.Models
open SleepTracker.Services
open SleepTracker.Models.Entities
open Microsoft.Extensions.Configuration

type SportController (logger : ILogger<SportController>, configuration: IConfiguration) =
    inherit Controller()

    member this.Index () =
        let sports = 
            async {
                let! tmpSports = DatabaseService.getSports(configuration.GetConnectionString("DefaultConnection")) |> Async.AwaitTask
                return tmpSports
            } |> Async.RunSynchronously
        this.View(sports |> Seq.map(fun s -> { SportID = s.SportID; SportName = s.SportName; SportNotes = s.SportNotes }))

    [<HttpGet>]
    member this.Create () =
        this.View()

    [<HttpPost>]
    [<ActionName("Create")>] // This is necessary because the method name is different from the action name
    member this.CreatePost() =
        if not (this.ModelState.IsValid) then
            this.BadRequest() :> ActionResult
        else
            let existingSports = 
                async {
                    let! tmpSports = DatabaseService.getSports(configuration.GetConnectionString("DefaultConnection")) |> Async.AwaitTask
                    return tmpSports
                } |> Async.RunSynchronously
            let sport = existingSports |> Seq.tryFind (fun s -> s.SportName = (this.Request.Form["SportName"] |> Seq.head))
            match sport with
            | Some _ -> this.Conflict() :> ActionResult
            | None ->
                let newSport : SportEntity.Sport = { SportID = 0; SportName = (this.Request.Form["SportName"] |> Seq.head); SportNotes = (this.Request.Form["SportNotes"] |> Seq.head) }
                let result = DatabaseService.createSport(configuration.GetConnectionString("DefaultConnection"), newSport) |> Async.AwaitTask |> Async.RunSynchronously
                match result with
                | 1 -> this.Created() :> ActionResult
                | _ -> this.BadRequest() :> ActionResult
