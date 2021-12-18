namespace SAL

open System.IO

module DomainModel =
    open System
    open SAL.Data.Storage
    open Elmish

    type Model = {
        Mods: Mod array
        SwatDirectory: string
        Status: string
    }

    type Message =
        | SwatDirectoryEntryChanged of string
        | Install of Guid
        | Uninstall of Guid
        | Launch of Guid
        | Failure of string

    let init = 
        {
            SwatDirectory = @"C:\GOG Games\SWAT 4"
            Mods = defaultMods
            Status = ""
        }, Cmd.none