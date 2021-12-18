namespace SAL.Data

open System.IO
open FSharp.Json

// TODO: Move this into settings.json
module Settings = 
    type AppSettings = { SwatDirectory: string }

    let [<Literal>] private settingsPath = "settings.json"
    let load =
        File.ReadAllText settingsPath
        |> Json.deserialize<AppSettings>