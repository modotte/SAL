namespace SAL.Data

open System.IO
open FSharp.Json

// TODO: Move this into settings.json
module Settings = 
    type SettingsType = { SwatInstallationDirectory: string }

    let currentSettings = {
        SwatInstallationDirectory = @"C:\GOG Games\SWAT 4"
    }