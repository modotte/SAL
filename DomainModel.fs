namespace SAL
module DomainModel =
    open System
    open SAL.Data.Settings
    open SAL.Data.Mods
    open Elmish

    type Model = {
        GameMods: Mod array
        SwatInstallationDirectory: string
        Status: string
        IsModRunning: bool
    }

    let sid = currentSettings.SwatInstallationDirectory

    let init = {
        GameMods = mods
        SwatInstallationDirectory = sid
        Status = ""
        IsModRunning = false }, Cmd.none

    type Message =
        | SwatInstallationDirectoryEntryChanged of string
        | Install of Guid
        | Uninstall of Guid
        | Launch of Guid
        | Failure of string