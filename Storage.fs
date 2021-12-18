namespace SAL.Data

open System.IO
open Thoth.Json.Net

open SAL.DomainModel

// TODO: Move this into mods.json.
module Storage =
    let getCategory = function
    | SEF -> "SEF"
    | SEF_FR -> "SEF_FR"
    | SEF_BTLA -> "SEF_BTLA"

    let [<Literal>] private storageFilename = "configuration.json"
    let private decoder = Decode.Auto.generateDecoder<Model>()
    let load () =
        let json = File.ReadAllText storageFilename
        let decoder = Decode.fromString decoder json

        match decoder with
        | Error r -> SAL.Logger.log.Error(r); None
        | Ok r -> Some r