namespace SAL

module DomainModel =
    open System
    open Elmish

    type OriginType = Official | Fork
    type CategoryType = SEF | SEF_FR | SEF_BTLA
    type StabilityType = Stable | Beta | Alpha | Nightly | Experimental
    type ArchiveFormatType = Zip | Rar

    type Mod = {
        Id: Guid
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
    }

    type Message =
        | SwatDirectoryEntryChanged of string
        | Install of Guid
        | Uninstall of Guid
        | Launch of Guid
        | Failure of string

    let defaultMods: Mod array = [|
        {
            Id = System.Guid.NewGuid()
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
            Id = System.Guid.NewGuid()
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
        }, Cmd.none