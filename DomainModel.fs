namespace SAL

open System.IO

module DomainModel =
    open System
    open SAL.Data.Settings
    open SAL.Data.Mods
    open Elmish

    type Model = {
        GameMods: Mod array
        SwatDirectory: string
        Status: string
    }

    type Message =
        | SwatDirectoryEntryChanged of string
        | Install of Guid
        | Uninstall of Guid
        | Launch of Guid
        | Failure of string
        | FetchSettings
        | SettingsFetched of SettingsType

    let private sid = currentSettings.SwatDirectory
    let init = 
        {
            SwatDirectory = sid
            GameMods = mods
            Status = ""
        }, Cmd.none