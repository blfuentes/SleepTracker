namespace SleeptTracker.Models.Account

open System.ComponentModel.DataAnnotations

type LoginModel = {
    [<Required>]
    [<EmailAddress>]
    Email: string

    [<Required>]
    [<DataType(DataType.Password)>]
    Password: string
}
