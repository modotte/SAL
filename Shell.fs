// Copyright (c) 2021 Modotte
// Copyright (c) 2019 Josua Jäger under MIT license

namespace SAL

module Shell =
    open System
    open Elmish
    open Avalonia
    open Avalonia.Controls
    open Avalonia.Input
    open Avalonia.Layout
    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.DSL
    open SAL.Data
    open Domain
    open Logger

    module UpdateHandler =
        let private getModById id model =
            model.Mods
            |> Array.filter (fun m -> m.Id = id)
            |> Array.head

        let withQuitProgram (window: HostWindow) model =
            window.Close()
            model, Cmd.none

        let withSwatDirectoryEntryChanged directory model = { model with SwatDirectory = directory }, Cmd.none

        let withInstall id model =
            let selectedMod = getModById id model
            match IOHandler.downloadMod selectedMod model.SwatDirectory with
            | Error _ -> model, Cmd.none
            | Ok _ -> 
                let updateMod selectedMod =
                    if selectedMod.Id = id then { selectedMod with IsInstalled = true }
                    else selectedMod

                IOHandler.extractArchive selectedMod model.SwatDirectory
                { model with Mods = Array.map updateMod model.Mods }, Cmd.none

        let withUninstall id model = 
            let selectedMod = getModById id model
            match IOHandler.uninstallMod selectedMod model.SwatDirectory with
            | Error _ -> model, Cmd.none
            | Ok _ -> 
                let updateMod selectedMod =
                    if selectedMod.Id = id then { selectedMod with IsInstalled = false }
                    else selectedMod
                { model with Mods = Array.map updateMod model.Mods }, Cmd.none

        let withLaunch id model = 
            let selectedMod = getModById id model
            match IOHandler.launchMod selectedMod model.SwatDirectory with
            | Ok _ -> model, Cmd.none
            | Error _ -> model, Cmd.none

        let withOpenNewFolderDialog window model =
            let dialog = Dialog.getFolderDialog model.SwatDirectory
            let showDialog w = dialog.ShowAsync(w) |> Async.AwaitTask
            model, Cmd.OfAsync.perform showDialog window FolderDialogOpened

        let withNewFolderFolderOpened directory model =
            { model with SwatDirectory = directory }, Cmd.none
        

    let update (message: Message) (model: Model) (window: HostWindow): Model * Cmd<Message> =
        match message with
        | Failure err -> log.Error err; model, Cmd.none
        | QuitProgram -> UpdateHandler.withQuitProgram window model
        | SwatDirectoryEntryChanged directory -> UpdateHandler.withSwatDirectoryEntryChanged directory model
        | Install id -> UpdateHandler.withInstall id model
        | Uninstall id -> UpdateHandler.withUninstall id model
        | Launch id -> UpdateHandler.withLaunch id model
        | OpenFolderDialog -> UpdateHandler.withOpenNewFolderDialog window model
        | FolderDialogOpened directory -> UpdateHandler.withNewFolderFolderOpened directory model

    type ShellWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "SAL: SEF Alternative Launcher"
            base.Width <- 800.0
            base.Height <- 600.0
            base.MinWidth <- 526.0
            base.MinHeight <- 526.0
    #if DEBUG
            this.AttachDevTools(KeyGesture(Key.F12))
    #endif
            let updateWithServices (message: Message) (model: Model) =
                Storage.updateStorage update message model this
            
            Program.mkProgram (Storage.load >> init) updateWithServices View.view
            |> Program.withHost this
    #if DEBUG
            |> Program.withConsoleTrace
    #endif
            |> Program.run