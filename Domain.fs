// Copyright (c) 2021 Modotte
// This software is licensed under the GPL-3.0 license. For more details,
// please read the LICENSE file content.

namespace SAL

module Domain =
    open Elmish

    type OriginType = Official | Fork
    type CategoryType = SEF | SEF_FR | SEF_BTLA
    type StabilityType = Stable | Beta | Alpha | Nightly | Experimental
    type ArchiveFormatType = Zip | Rar | SevenZip

    type ScreenType = Primary | InfoPopup | ErrorPopup

    type Mod = {
        Id: int
        Category: CategoryType
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        ModRootFolderName: string
        Stability: StabilityType
        ArchiveFormat: ArchiveFormatType
        IsInstalled: bool
    }

    // Set Mod array to Mod array option in
    // stable version 
    type Model = {
        Mods: Mod array
        SwatDirectory: string
        IsInProgress: bool
        ProgressStatus: string option
        ProgressCompletedStatus: string option
        CurrentScreen: ScreenType
    }

    [<RequireQualifiedAccess>]
    type InstallDownloadResult = Success of Mod | Failure of Mod * string
    [<RequireQualifiedAccess>]
    type InstallExtractionResult = Success of Mod | Failure of Mod * string
    [<RequireQualifiedAccess>]
    type InstallationResult = Success of Mod | Failure of Mod * string
    [<RequireQualifiedAccess>]
    type UninstallationResult = Success of Mod | Failure of Mod * string
    [<RequireQualifiedAccess>]
    type LaunchResult = Success of Mod | Failure of Mod * string

    type Message =
        | Failure of string
        | QuitProgram
        | SwatDirectoryEntryChanged of string

        | InstallDownload of int
        | AfterInstallDownload of InstallDownloadResult

        | InstallExtract of int
        | AfterInstallExtract of InstallExtractionResult

        | Uninstall of int
        | AfterUninstall of UninstallationResult
        
        | Launch of int
        | AfterLaunch of LaunchResult
        
        | OpenFolderDialog
        | FolderDialogOpened of string

        | OpenInfoPopup of string
        | CloseInfoPopup

        | OpenErrorPopup of string
        | CloseErrorPopup

        | VisitLink of string

    let defaultMods: Mod array = [|
        {
            Id = 0
            Category = SEF
            Maintainer = "eezstreet"
            Version = "v7.0"
            Url = "http://localhost:6792/SEF-v7.0.zip"
            Origin = Official
            ModRootFolderName = "SEF"
            Stability = Stable
            ArchiveFormat = Zip
            IsInstalled = false
        }

        {
            Id = 1
            Category = SEF_FR
            Maintainer = "beppe_goodoldrebel"
            Version = "v0.66"
            Url = "http://localhost:6792/SEF_FRv66b.1.rar"
            Origin = Official
            ModRootFolderName = "SEF_FR"
            Stability = Beta
            ArchiveFormat = Rar
            IsInstalled = false
        }
    |]

    let init = function
    // Always set IsLoading to false on application startup
    | Some oldModel -> 
        { oldModel with
            IsInProgress = false
            ProgressStatus = None
            ProgressCompletedStatus = None
            CurrentScreen = Primary
        }, Cmd.none
    | _ -> 
        {
            SwatDirectory = @"C:\GOG Games\SWAT 4"
            Mods = defaultMods
            IsInProgress = false
            ProgressStatus = None
            ProgressCompletedStatus = None
            CurrentScreen = Primary
        }, Cmd.none