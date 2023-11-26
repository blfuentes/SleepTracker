namespace SleepTracker.Controllers

open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open SleepTracker.Models
open SleepTracker.Services
open SleepTracker.Models.Entities

type SportController (logger : ILogger<SportController>) =
    inherit Controller()

    member this.Index () =
        let sports = 
            async {
                let! tmpSports = DatabaseService.getSports() |> Async.AwaitTask
                return tmpSports
            } |> Async.RunSynchronously
        this.View(sports |> Seq.map(fun s -> { SportID = s.SportID; SportName = s.SportName; SportNotes = s.SportNotes }))

    [<ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)>]
    member this.Error () =
        let reqId = 
            if isNull Activity.Current then
                this.HttpContext.TraceIdentifier
            else
                Activity.Current.Id

        this.View({ RequestId = reqId })
