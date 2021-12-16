namespace SAL

module DomainModel =
    type OriginType = Official | Fork

    type ModCategory = SEF | SEF_FR | SEF_BTLA
    type Mod = {
        Category: ModCategory
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        PreExtractFolder: string
    }