namespace SAL.Data

open System.IO
open Elmish
open Thoth.Json.Net

open SAL.Domain
open Avalonia.FuncUI.Components.Hosts

module Storage =

    let [<Literal>] private storageFilename = "configuration.json"
    let private decoder = Decode.Auto.generateDecoder<Model>()
    let load () =
        let json = File.ReadAllText storageFilename
        let decoder = Decode.fromString decoder json

        match decoder with
        | Error r -> SAL.Logger.log.Error(r); None
        | Ok r -> Some r

    let save (model: Model) =
        File.WriteAllText(storageFilename, Encode.Auto.toString(4, model))

    let updateStorage update (message: Message) (model: Model) (window: HostWindow)  = 
        let setStorage (model: Model) =
            Cmd.OfFunc.attempt save model (string >> Failure)
        
        match message with
        | Failure _ -> (model, Cmd.none)
        | _ ->
            let (newModel, commands) = update message model window
            (newModel, Cmd.batch [ setStorage newModel; commands ])