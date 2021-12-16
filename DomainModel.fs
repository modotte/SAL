namespace SAL

module DomainModel =
    type OriginType = Official | Fork

    type Mod = {
        Category: string
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        PreExtractFolder: string
    }