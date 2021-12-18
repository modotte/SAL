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
        File.ReadAllText storageFilename
        |> unbox
        |> Option.bind (
            Decode.fromString decoder
            >> function
            | Ok data -> Some data
            | _ -> None
        )