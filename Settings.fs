namespace SAL

open System.IO
open FSharp.Json

module Settings = 
    type SettingsType = { SwatInstallationDirectory: string }

    let currentSettings = {
        SwatInstallationDirectory = @"C:\GOG Games\SWAT 4"
    }