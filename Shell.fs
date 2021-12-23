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

        let withInstallDownload id model =
            let selectedMod = getModById id model
            let message = async {
                let result = IOHandler.downloadArchive selectedMod model.SwatDirectory
                return AfterInstallDownload result
            }

            let status = $"Downloading {IOHandler.modDirectoryOutput selectedMod} mod archive.."
            { model with IsInProgress = true; ProgressStatus = Some status }, Cmd.OfAsync.result message
        
        let withAfterInstallDownload result model =
            match result with
            | InstallDownloadResult.Failure (m, err) -> 
                { model with IsInProgress = false }, Cmd.ofMsg (OpenErrorPopup err)
            | InstallDownloadResult.Success m -> model, Cmd.ofMsg (InstallExtract m.Id)
        
        let withInstallExtract id model =
            let selectedMod = getModById id model
            let message = async {
                let result = IOHandler.extractArchive selectedMod model.SwatDirectory
                return AfterInstallExtract result
            }

            let status = $"Extracting {IOHandler.modDirectoryOutput selectedMod} mod archive.."
            { model with IsInProgress = true; ProgressStatus = Some status }, Cmd.OfAsync.result message
            
        let withAfterInstallExtract result model =
            match result with
            | InstallExtractionResult.Failure (m, err) -> { model with IsInProgress = false }, Cmd.ofMsg (OpenErrorPopup err)
            | InstallExtractionResult.Success m -> 
                let updateMod selectedMod =
                        if selectedMod.Id = m.Id then { selectedMod with IsInstalled = true }
                        else selectedMod
                { model with Mods = Array.map updateMod model.Mods; IsInProgress = false }, Cmd.none

        let withUninstall id model = 
            let selectedMod = getModById id model
            let message = async {
                let result = IOHandler.uninstallMod selectedMod model.SwatDirectory 

                return AfterUninstall result
            }

            let status = $"Uninstalling {IOHandler.modDirectoryOutput selectedMod} mod.."
            { model with IsInProgress = true; ProgressStatus = Some status }, Cmd.OfAsync.result message
        let withAfterUninstall result model =
            match result with
            | UninstallationResult.Failure (m, err) -> { model with IsInProgress = false }, Cmd.ofMsg (OpenErrorPopup err)
            | UninstallationResult.Success m ->
                let updateMod selectedMod =
                    if selectedMod.Id = m.Id then { selectedMod with IsInstalled = false }
                    else selectedMod
                { model with Mods = Array.map updateMod model.Mods; IsInProgress = false }, Cmd.none

        let withLaunch id model = 
            let selectedMod = getModById id model
            let message = async {
                let result = IOHandler.launchMod selectedMod model.SwatDirectory
                
                return AfterLaunch result
            }
            
            let status = $"Launching {IOHandler.modDirectoryOutput selectedMod} mod.."
            { model with IsInProgress = true; ProgressStatus = Some status }, Cmd.OfAsync.result message
            
        let withAfterLaunch result model =
            match result with
            | LaunchResult.Failure (m, err) -> { model with IsInProgress = false }, Cmd.ofMsg (OpenErrorPopup err)
            | LaunchResult.Success m ->
                { model with IsInProgress = false }, Cmd.none

        let withOpenNewFolderDialog window model =
            let dialog = Dialog.getFolderDialog model.SwatDirectory
            let previousSwatDir = model.SwatDirectory
            let showDialog w = async {
                let! result = dialog.ShowAsync(w) |> Async.AwaitTask

                // Use previous entered SwatDirectory value if user
                // close the dialog by closing or clicking cancel button.
                if String.IsNullOrEmpty result then
                    return previousSwatDir
                else
                    return result
            }
            model, Cmd.OfAsync.perform showDialog (window :> Window) FolderDialogOpened

        let withNewFolderFolderOpened directory model =
            { model with SwatDirectory = directory }, Cmd.none
        

    let update (message: Message) (model: Model) (window: HostWindow): Model * Cmd<Message> =
        match message with
        | Failure err -> log.Error err; model, Cmd.none
        | QuitProgram -> UpdateHandler.withQuitProgram window model
        | SwatDirectoryEntryChanged directory -> UpdateHandler.withSwatDirectoryEntryChanged directory model

        | InstallDownload id -> UpdateHandler.withInstallDownload id model
        | AfterInstallDownload installDownloadResult -> UpdateHandler.withAfterInstallDownload installDownloadResult model

        | InstallExtract id -> UpdateHandler.withInstallExtract id model
        | AfterInstallExtract installExtractResult -> UpdateHandler.withAfterInstallExtract installExtractResult model

        | Uninstall id -> UpdateHandler.withUninstall id model
        | AfterUninstall uninstallResult -> UpdateHandler.withAfterUninstall uninstallResult model
        
        | Launch id -> UpdateHandler.withLaunch id model
        | AfterLaunch launchResult -> UpdateHandler.withAfterLaunch launchResult model

        | OpenFolderDialog -> UpdateHandler.withOpenNewFolderDialog window model
        | FolderDialogOpened directory -> UpdateHandler.withNewFolderFolderOpened directory model

        | OpenInfoPopup message ->
            { model with CurrentScreen = InfoPopup; ProgressCompletedStatus = Some message }, Cmd.none
        | CloseInfoPopup ->
            { model with CurrentScreen = Primary }, Cmd.none

        | OpenErrorPopup message ->
            { model with CurrentScreen = ErrorPopup; ProgressCompletedStatus = Some message }, Cmd.none
        | CloseErrorPopup ->
            { model with CurrentScreen = Primary }, Cmd.none
            

    type ShellWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "SAL: SEF Alternative Launcher"
            base.Width <- 800.0
            base.Height <- 400.0
            base.MinWidth <- 526.0
            base.MinHeight <- 326.0
            let updateWithServices (message: Message) (model: Model) =
                Storage.updateStorage update message model this
            
            Program.mkProgram (Storage.load >> init) updateWithServices View.view
            |> Program.withHost this
            |> Program.run