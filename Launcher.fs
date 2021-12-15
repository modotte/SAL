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
    }

    let [<Literal>] SWAT_INSTALLATION_DIRECTORY = "SWAT4"

    let private asArchiveFile (archiveType: ArchiveType) (modName: string) =
        match archiveType with
        | Zip -> modName + ".zip"
        | Rar -> modName + ".rar"

    let downloadMod gameMod = 
        if File.Exists(SWAT_INSTALLATION_DIRECTORY + (asArchiveFile gameMod.Archive gameMod.Name)) then
            Error $"{gameMod.Name} already exist!"
        else
            let a = (asArchiveFile gameMod.Archive gameMod.Name)
            printfn "Download started.."
            WebClient().DownloadFile(gameMod.Url, Path.Combine(SWAT_INSTALLATION_DIRECTORY, a))
            Ok $"{a} downloaded"

    let extractArchive gameMod =
        let archivePath = Path.Combine(SWAT_INSTALLATION_DIRECTORY, (asArchiveFile Zip gameMod.Name))
        Compression.ZipFile.ExtractToDirectory(archivePath, Path.Combine(SWAT_INSTALLATION_DIRECTORY, gameMod.Name))


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
            Client.Mod.Maintainer = "eezstreet"
            Client.Mod.Version = "v7.0"
            Client.Mod.Url = "https://www.moddb.com/downloads/mirror/195627/115/b7e306bbf7d472a49725194bedb0da71"
            Client.Mod.Origin = Client.OriginType.Official
            Client.Mod.Archive = Client.ArchiveType.Zip
        }

        match msg with
        | Install ->
            match Client.downloadMod gameMod with
            | Error err -> { model with Status = err }
            | Ok m -> 
                Client.extractArchive gameMod
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
