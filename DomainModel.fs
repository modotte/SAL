namespace SAL

module DomainModel =
    type OriginType = Official | Fork
    type ArchiveType = Zip | Rar

    type Mod = {
        Name: string
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        Archive: ArchiveType
        PreExtractFolder: string
    }