namespace SAL.Data

// TODO: Move this into settings.json
module Settings = 
    type SettingsType = { SwatInstallationDirectory: string }

    let currentSettings = {
        SwatInstallationDirectory = @"C:\GOG Games\SWAT 4"
    }