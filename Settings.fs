namespace SAL.Data

open System.IO
open FSharp.Json

// TODO: Move this into settings.json
module Settings = 
    type AppSettings = { SwatDirectory: string }
    
    let fetchSettings filename =
        File.ReadAllText filename
        |> Json.deserialize<AppSettings>

    let currentSettings = {
        SwatDirectory = @"C:\GOG Games\SWAT 4"
    }