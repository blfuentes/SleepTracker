namespace SleeptTracker.Models.Sport

open System.ComponentModel.DataAnnotations

type SportModel = {
    SportId: int

    [<Required>]
    SportName: string

    SportNotes: string
}