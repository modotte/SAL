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
    
    let private modDirectoryOutput (gameMod: Mod) =
        $"{gameMod.Maintainer}-{getCategory gameMod.Category}-{gameMod.Version}"
    let private isInstalled (gameMod: Mod) =
        let r = Directory.Exists(Path.Combine(sid, modDirectoryOutput gameMod))
        
        Logger.log.Information(r.ToString())
        r
        
    let updateInstalledMod gameMod = { gameMod with IsInstalled = isInstalled gameMod }
            
    let updateIsInstalledModsStateOnInit (gameMods: Mod array) =
        let l = gameMods |> Array.map updateInstalledMod
        Logger.log.Information(sprintf "%A" l)
        l
    let updatedMods = updateIsInstalledModsStateOnInit mods


    let init = {
        SwatInstallationDirectory = sid
        GameMods = updatedMods
        Status = ""
        IsAModRunning = false }, Cmd.none

    type Message =
        | SwatInstallationDirectoryEntryChanged of string
        | Install of Guid
        | Uninstall of Guid
        | Launch of Guid
        | Failure of string