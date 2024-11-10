namespace SleeptTracker.Models.Account

open System.ComponentModel.DataAnnotations

type RegisterModel = {
    [<Required>]
    [<EmailAddress>]
    Email: string

    [<Required>]
    [<DataType(DataType.Password)>]
    Password: string

    [<Required>]
    [<DataType(DataType.Password)>]
    ConfirmPassword: string
}
