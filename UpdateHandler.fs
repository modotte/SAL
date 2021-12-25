// Copyright (c) 2021 Modotte
// This software is licensed under the GPL-3.0 license. For more details,
// please read the LICENSE file content.

namespace SAL

open System

open Elmish
open Avalonia.FuncUI.Components.Hosts

open Domain

module UpdateHandler =
    let private getModById id model =
        model.Mods
        |> Array.filter (fun m -> m.Id = id)
        |> Array.head

    let withFailure _ model = model, Cmd.none

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

    let withOpenFolderDialog window model =
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
        model, Cmd.OfAsync.perform showDialog window FolderDialogOpened

    let withFolderFolderOpened directory model = { model with SwatDirectory = directory }, Cmd.none

    let withOpenInfoPopup message model =
        { model with CurrentScreen = InfoPopup; ProgressCompletedStatus = Some message }, Cmd.none
    let withCloseInfoPopup model = { model with CurrentScreen = Primary }, Cmd.none

    let withOpenErrorPopup message model = { model with CurrentScreen = ErrorPopup; ProgressCompletedStatus = Some message }, Cmd.none
    let withCloseErrorPopup model = { model with CurrentScreen = Primary }, Cmd.none

    let withVisitLink link model =
        Avalonia.Dialogs.AboutAvaloniaDialog.OpenBrowser(link)
        model, Cmd.none