namespace SAL.Data

// TODO: Move this into mods.json.
module Mods =
    type OriginType = Official | Fork
    type CategoryType = SEF | SEF_FR | SEF_BTLA
    type StabilityType = Stable | Beta | Alpha | Nightly | Experimental
    type ArchiveFormatType = Zip | Rar

    type Mod = {
        Id: System.Guid
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

    let mods: Mod array = [|
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
            Category = SEF
            Maintainer = "eezstreet"
            Version = "v7.1"
            Url = "https://www.moddb.com/downloads/mirror/195627/115/35d7c155b0249f6ca4aae6fb2a366cda/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-elite-force%2Fdownloads"
            Origin = Official
            PreExtractFolder = "SEF"
            Stability = Beta
            ArchiveFormat = Zip
            IsInstalled = false
        }

        {
            Id = System.Guid.NewGuid()
            Category = SEF_FR
            Maintainer = "beppe_goodoldrebel"
            Version = "v0.65"
            Url = "https://www.moddb.com/downloads/mirror/216323/114/f0c528726f0780610e51d3d49241b1c8/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fsef-first-responders%2Fdownloads"
            Origin = Official
            PreExtractFolder = "SEF_FR"
            Stability = Stable
            ArchiveFormat = Rar
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

        {
            Id = System.Guid.NewGuid()
            Category = SEF_BTLA
            Maintainer = "EFdee"
            Version = "v1.2.7"
            Url = "https://www.moddb.com/downloads/mirror/214404/121/2653989c215d6ed23b68cd61a0e55fe8/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-back-to-la%2Fdownloads"
            Origin = Official
            PreExtractFolder = "SEF-BTLA"
            Stability = Stable
            ArchiveFormat = Rar
            IsInstalled = false
        }

        {
            Id = System.Guid.NewGuid()
            Category = SEF_BTLA
            Maintainer = "EFdee"
            Version = "v1.5.7"
            Url = "http://localhost:6792/SEF-BTLA_v1.5.7z"
            Origin = Official
            PreExtractFolder = "SEF-BTLA"
            Stability = Stable
            ArchiveFormat = Rar
            IsInstalled = false
        }
    |]