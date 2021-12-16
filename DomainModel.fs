namespace SAL

module DomainModel =
    type OriginType = Official | Fork

    type ModCategory = SEF | SEF_FR | SEF_BTLA
    type StabilityType = Stable | Beta | Alpha | Nightly | Experimental
    type Mod = {
        Id: System.Guid
        Category: ModCategory
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        PreExtractFolder: string
        Stability: StabilityType
    }