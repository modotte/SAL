namespace SAL

open System.IO

module Client =
    open System.Net
    open System.IO.Compression
    type OriginType = Official | Fork
    type ArchiveType = Zip | Rar
    type Mod = {
        Name: string
        Maintainer: string
        Version: string
        Url: string
        Origin: OriginType
        Archive: ArchiveType
        PreModFolder: string
    }

    let [<Literal>] SWAT_INSTALLATION_DIRECTORY = @"C:\GOG Games\SWAT 4"

    let private asArchiveFile gameMod =
        let modArchiveName = $"{gameMod.Maintainer}-{gameMod.Version}-{gameMod.Name}"
        match gameMod.Archive with
        | Zip -> modArchiveName + ".zip"
        | Rar -> modArchiveName + ".rar"

    let downloadMod gameMod = 
        // TODO: Check on installed mod dir instead
        if File.Exists(SWAT_INSTALLATION_DIRECTORY + (asArchiveFile gameMod)) then
            Error $"{gameMod.Name} already exist!"
        else
            let a = (asArchiveFile gameMod)
            WebClient().DownloadFile(gameMod.Url, Path.Combine(SWAT_INSTALLATION_DIRECTORY, a))
            Ok $"{a} downloaded"

    let extractArchive gameMod =
        let modDirName = $"{gameMod.Maintainer}-{gameMod.Version}-{gameMod.Name}"
        let archivePath = Path.Combine(SWAT_INSTALLATION_DIRECTORY, (asArchiveFile gameMod))
        Compression.ZipFile.ExtractToDirectory(archivePath, SWAT_INSTALLATION_DIRECTORY)
        Directory.Move(
            Path.Combine(SWAT_INSTALLATION_DIRECTORY, gameMod.PreModFolder),
            Path.Combine(SWAT_INSTALLATION_DIRECTORY, modDirName))

    let launchMod gameMod =
        let modDirName = $"{gameMod.Maintainer}-{gameMod.Version}-{gameMod.Name}"
        let modDir = Path.Combine(SWAT_INSTALLATION_DIRECTORY, modDirName)
        let systemDir = Path.Combine(modDir, "System")
        
        Directory.SetCurrentDirectory(systemDir)

        let command = @"/C ..\..\ContentExpansion\System\Swat4X.exe"
        System.Diagnostics.Process.Start(command)


module Launcher =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type Model = { Status: string }
    let init = { Status = "" }

    type Msg = Install | Uninstall | Launch
    let update (msg: Msg) (model: Model) : Model =
        let gameMod = {
            Client.Mod.Name = "SEF"
            Client.Mod.Maintainer = "eezstreet"
            Client.Mod.Version = "v7.0"
            Client.Mod.Url = "https://www.moddb.com/downloads/mirror/195627/124/084b4b2d20eb9f57e10e4b248a1df07d/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-elite-force%2Fdownloads"
            Client.Mod.Origin = Client.OriginType.Official
            Client.Mod.Archive = Client.ArchiveType.Zip
            Client.Mod.PreModFolder = "SEF"
        }

        match msg with
        | Install ->
            printfn "Download started.."
            match Client.downloadMod gameMod with
            | Error err -> { model with Status = err }
            | Ok m -> 
                Client.extractArchive gameMod
                printfn "Extraction started.."
                { model with Status = m}
        | Uninstall -> { model with Status = "Mod uninstalled" }
        | Launch ->
            Client.launchMod gameMod |> ignore
            { model with Status = gameMod.Name + " has been launched" }
    
    let view (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Launch)
                    Button.content "Launch Mod"
                ]                
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
