namespace SAL

open System.IO

module DomainModel =
    open System
    open SAL.Data.Settings
    open SAL.Data.Mods
    open Elmish

    type Model = {
        GameMods: Mod array
        SwatInstallationDirectory: string
        Status: string
        IsAModRunning: bool
    }

    let private sid = currentSettings.SwatInstallationDirectory
    let init = {
        SwatInstallationDirectory = sid
        GameMods = mods
        Status = ""
        IsAModRunning = false }, Cmd.none

    type Message =
        | SwatInstallationDirectoryEntryChanged of string
        | Install of Guid
        | Uninstall of Guid
        | Launch of Guid
        | Failure of string