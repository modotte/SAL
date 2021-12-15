namespace SAL.Data

// TODO: Move this into mods.json.
module Mods =
    open SAL.DomainModel

    let mods: Mod array = [|
        {
            Mod.Name = "SEF"
            Mod.Maintainer = "eezstreet"
            Mod.Version = "v7.0"
            Mod.Url = "https://www.moddb.com/downloads/mirror/195627/115/35d7c155b0249f6ca4aae6fb2a366cda/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-elite-force%2Fdownloads"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF"
        }
    |]