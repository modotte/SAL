namespace SAL.Data

// TODO: Move this into mods.json.
module Mods =
    open SAL.DomainModel

    let getCategory = function
    | SEF -> "SEF"
    | SEF_FR -> "SEF_FR"
    | SEF_BTLA -> "SEF_BTLA"

    let mods: Mod array = [|
        {
            Mod.Id = System.Guid.NewGuid()
            Mod.Category = SEF
            Mod.Maintainer = "eezstreet"
            Mod.Version = "v7.0"
            Mod.Url = "http://localhost:6792/SEF-v7.0.zip"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF"
            Mod.Stability = StabilityType.Stable
        }

        {
            Mod.Id = System.Guid.NewGuid()
            Mod.Category = SEF
            Mod.Maintainer = "eezstreet"
            Mod.Version = "v7.1"
            Mod.Url = "https://www.moddb.com/downloads/mirror/195627/115/35d7c155b0249f6ca4aae6fb2a366cda/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-elite-force%2Fdownloads"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF"
            Mod.Stability = StabilityType.Beta
        }

        {
            Mod.Id = System.Guid.NewGuid()
            Mod.Category = SEF_FR
            Mod.Maintainer = "beppe_goodoldrebel"
            Mod.Version = "v0.65"
            Mod.Url = "https://www.moddb.com/downloads/mirror/216323/114/f0c528726f0780610e51d3d49241b1c8/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fsef-first-responders%2Fdownloads"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF_FR"
            Mod.Stability = StabilityType.Stable
        }

        {
            Mod.Id = System.Guid.NewGuid()
            Mod.Category = SEF_FR
            Mod.Maintainer = "beppe_goodoldrebel"
            Mod.Version = "v0.66"
            Mod.Url = "http://localhost:6792/SEF_FRv66b.1.zip"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF_FR"
            Mod.Stability = StabilityType.Beta
        }

        {
            Mod.Id = System.Guid.NewGuid()
            Mod.Category = SEF_BTLA
            Mod.Maintainer = "EFdee"
            Mod.Version = "v1.2.7"
            Mod.Url = "https://www.moddb.com/downloads/mirror/214404/121/2653989c215d6ed23b68cd61a0e55fe8/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-back-to-la%2Fdownloads"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF_BTLA"
            Mod.Stability = StabilityType.Stable
        }

        {
            Mod.Id = System.Guid.NewGuid()
            Mod.Category = SEF_BTLA
            Mod.Maintainer = "EFdee"
            Mod.Version = "v1.5.7"
            Mod.Url = "https://www.moddb.com/downloads/mirror/220769/126/7df9a2ae56274b5b8b99bfd387b5865d/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-back-to-la%2Fdownloads"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF_BTLA"
            Mod.Stability = StabilityType.Stable
        }
    |]