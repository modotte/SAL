namespace SAL

module Domain =
    open Elmish

    type OriginType = Official | Fork
    type CategoryType = SEF | SEF_FR | SEF_BTLA
    type StabilityType = Stable | Beta | Alpha | Nightly | Experimental
    type ArchiveFormatType = Zip | Rar | SevenZip

    type Mod = {
        Id: int
        Category: CategoryType
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        PreExtractFolder: string
        Stability: StabilityType
        ArchiveFormat: ArchiveFormatType
        IsInstalled: bool
    }

    let getCategory = function
        | SEF -> "SEF"
        | SEF_FR -> "SEF_FR"
        | SEF_BTLA -> "SEF_BTLA"

    // Set Mod array to Mod array option in
    // stable version 
    type Model = {
        Mods: Mod array
        SwatDirectory: string
        SelectedMod: int
        IsLoading: bool
    }

    type Message =
        | Failure of string
        | QuitProgram
        | SwatDirectoryEntryChanged of string
        | Install of int

        | Uninstall of int
        | AfterUninstall of Mod

        | Launch of int
        | OpenFolderDialog
        | FolderDialogOpened of string
        | SelectMod of int

    let defaultMods: Mod array = [|
        {
            Id = 0
            Category = SEF
            Maintainer = "eezstreet"
            Version = "v7.0"
            Url = "http://localhost:6792/SEF-v7.0.zip"
            Origin = Official
            PreExtractFolder = "SEF"
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
            PreExtractFolder = "SEF_FR"
            Stability = Beta
            ArchiveFormat = Rar
            IsInstalled = false
        }
    |]

    let init = function
    // Always set IsLoading to false on application startup
    | Some oldModel -> { oldModel with IsLoading = false }, Cmd.none
    | _ -> 
        {
            SwatDirectory = @"C:\GOG Games\SWAT 4"
            Mods = defaultMods
            SelectedMod = 0
            IsLoading = false
        }, Cmd.none