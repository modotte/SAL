namespace SAL.Data

// TODO: Move this into settings.json
module Settings = 
    type SettingsType = { SwatDirectory: string }

    let currentSettings = {
        SwatDirectory = @"C:\GOG Games\SWAT 4"
    }