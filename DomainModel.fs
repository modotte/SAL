namespace SAL

module DomainModel =
    open Elmish

    type OriginType = Official | Fork
    type CategoryType = SEF | SEF_FR | SEF_BTLA
    type StabilityType = Stable | Beta | Alpha | Nightly | Experimental
    type ArchiveFormatType = Zip | Rar
    
    type Deferred<'a> =
      | HasNotStartedYet
      | InProgress
      | Resolved of 'a

    type AsyncOperationStatus<'a> =
      | Started
      | Finished of 'a

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

    type Model = {
        Mods: Mod array
        SwatDirectory: string
        Status: string
        Loading: bool
    }

    type Message =
        | SwatDirectoryEntryChanged of string
        | Install of int
        | Uninstall of int
        | Launch of int
        | Failure of string
        | NowUpdateStatus
        | StatusDelayed

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
    | Some oldModel -> oldModel, Cmd.none
    | _ -> 
        {
            SwatDirectory = @"C:\GOG Games\SWAT 4"
            Mods = defaultMods
            Status = ""
            Loading = false
        }, Cmd.none