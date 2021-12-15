﻿namespace SAL

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
        LauncherScript: string
    }

    let [<Literal>] SWAT_INSTALLATION_DIRECTORY = "C:\\GOG Games\\SWAT 4"

    let private asArchiveFile (archiveType: ArchiveType) (modName: string) =
        match archiveType with
        | Zip -> modName + ".zip"
        | Rar -> modName + ".rar"

    let downloadMod gameMod = 
        // TODO: Check on installed mod dir instead
        if File.Exists(SWAT_INSTALLATION_DIRECTORY + (asArchiveFile gameMod.Archive gameMod.Name)) then
            Error $"{gameMod.Name} already exist!"
        else
            let a = (asArchiveFile gameMod.Archive gameMod.Name)
            WebClient().DownloadFile(gameMod.Url, Path.Combine(SWAT_INSTALLATION_DIRECTORY, a))
            Ok $"{a} downloaded"

    let extractArchive gameMod =
        let archivePath = Path.Combine(SWAT_INSTALLATION_DIRECTORY, (asArchiveFile Zip gameMod.Name))
        Compression.ZipFile.ExtractToDirectory(archivePath, Path.Combine(SWAT_INSTALLATION_DIRECTORY, gameMod.Maintainer, gameMod.Version))

    let launchMod gameMod =
        let modDir = Path.Combine(SWAT_INSTALLATION_DIRECTORY, gameMod.Maintainer, gameMod.Version, gameMod.Name)
        let systemDir = Path.Combine(modDir, "System")
        
        Directory.SetCurrentDirectory(systemDir)

        //let command = $"/C cd .\\{systemDir} && ..\\..\\..\\..\\ContentExpansion\\System\\Swat4X.exe"
        //System.Diagnostics.Process.Start(command)


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
            Client.Mod.LauncherScript = "LaunchSEF.bat"
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
