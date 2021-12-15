namespace SAL

open System.IO

module Client =
    open System.Net
    open System.IO.Compression

    type ArchiveType = Zip | Rar
    type UrlType = UrlType of string
    type ModVersion = ModVersion of string
    type Mod = {
        Name: string
        Url: UrlType
        Version: ModVersion
        Archive: ArchiveType
    }

    let [<Literal>] SWAT_INSTALLATION_DIRECTORY = "SWAT4"

    let asArchiveFile (archiveType: ArchiveType) (modName: string) =
        match archiveType with
        | Zip -> modName + ".zip"
        | Rar -> modName + ".rar"

    let downloadMod (url: string) modName archiveType = 
        if File.Exists(SWAT_INSTALLATION_DIRECTORY + (asArchiveFile archiveType modName)) then
            Error $"{modName} already exist!"
        else
            let a = (asArchiveFile archiveType modName)
            printfn "Download started.."
            WebClient().DownloadFile(url, Path.Combine(SWAT_INSTALLATION_DIRECTORY, a))
            Ok $"{a} downloaded"

    let extractArchive modName (archivePath: string) =
        let dir = Directory.CreateDirectory(modName)
        Compression.ZipFile.ExtractToDirectory(archivePath, Path.Combine(SWAT_INSTALLATION_DIRECTORY, modName))


module Launcher =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type Model = { Status: string }
    let init = { Status = "" }

    type Msg = Install | Uninstall
    let update (msg: Msg) (model: Model) : Model =
        let gameMod = {
            Client.Mod.Name = "SEF"
            Client.Mod.Url = Client.UrlType "https://www.moddb.com/downloads/mirror/195627/115/b7e306bbf7d472a49725194bedb0da71"
            Client.Mod.Version = Client.ModVersion "v7.0"
            Client.Mod.Archive = Client.ArchiveType.Zip
        }
        
        match msg with
        | Install ->
            match Client.downloadMod "https://www.moddb.com/downloads/mirror/195627/115/b7e306bbf7d472a49725194bedb0da71" "SEF" Client.ArchiveType.Zip with
            | Error err -> { model with Status = err }
            | Ok m -> 
                Client.extractArchive "SEF" (Path.Combine(Client.SWAT_INSTALLATION_DIRECTORY, "SEF.zip"))
                { model with Status = m}
        | Uninstall -> { model with Status = "Mod uninstalled" }
    
    let view (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Uninstall)
                    Button.content "Uninstall"
                ]                
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Install)
                    Button.content "Install"
                ]
                TextBlock.create [
                    TextBlock.dock Dock.Top
                    TextBlock.fontSize 32.0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                    TextBlock.text (string model.Status)
                ]
            ]
        ]       
