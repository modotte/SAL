namespace SAL
module DomainModel =
    open SAL.Data
    open Elmish

    type OriginType = Official | Fork
    type CategoryType = SEF | SEF_FR | SEF_BTLA
    type StabilityType = Stable | Beta | Alpha | Nightly | Experimental
    type Mod = {
        Id: System.Guid
        Category: CategoryType
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        PreExtractFolder: string
        Stability: StabilityType
    }

    type Model = {
    SwatInstallationDirectory: string
    Status: string
    IsModRunning: bool
    IsModInstalled: bool
    }

    let sid = Settings.currentSettings.SwatInstallationDirectory
    
    let init = { 
        SwatInstallationDirectory = sid
        Status = ""
        IsModRunning = false
        IsModInstalled = false }, Cmd.none

    type Message =
        | SwatInstallationDirectoryEntryChanged of string
        | Install 
        | Uninstall 
        | Launch