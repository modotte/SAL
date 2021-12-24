namespace SAL

module Shell =
    open Elmish
    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.Components.Hosts
    open SAL.Data
    open Domain

    open UpdateHandler

    let update (message: Message) (model: Model) (window: HostWindow): Model * Cmd<Message> =
        match message with
        | Failure err -> withFailure err model
        | QuitProgram -> withQuitProgram window model
        | SwatDirectoryEntryChanged directory -> withSwatDirectoryEntryChanged directory model
        | InstallDownload id -> withInstallDownload id model
        | AfterInstallDownload installDownloadResult -> withAfterInstallDownload installDownloadResult model
        | InstallExtract id -> withInstallExtract id model
        | AfterInstallExtract installExtractResult -> withAfterInstallExtract installExtractResult model
        | Uninstall id -> withUninstall id model
        | AfterUninstall uninstallResult -> withAfterUninstall uninstallResult model
        | Launch id -> withLaunch id model
        | AfterLaunch launchResult -> withAfterLaunch launchResult model
        | OpenFolderDialog -> withOpenFolderDialog window model
        | FolderDialogOpened directory -> withFolderFolderOpened directory model
        | OpenInfoPopup message -> withOpenInfoPopup message model
        | CloseInfoPopup -> withCloseInfoPopup model
        | OpenErrorPopup message ->  withOpenErrorPopup message model
        | CloseErrorPopup -> withCloseErrorPopup model
        | VisitLink link -> withVisitLink link model
            
    type ShellWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "SAL: SWAT4 (for SEF based mods) Alternative Launcher (v0.1.2)"
            base.Width <- 800.0
            base.Height <- 400.0
            base.MinWidth <- 526.0
            base.MinHeight <- 326.0
            let updateWithServices (message: Message) (model: Model) =
                Storage.updateStorage update message model this
            
            Program.mkProgram (Storage.load >> init) updateWithServices View.view
            |> Program.withHost this
            |> Program.run