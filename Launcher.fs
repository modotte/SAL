namespace SAL

open System.IO
open Elmish

module Logger =
    open Serilog
    let log = LoggerConfiguration().WriteTo.Console().CreateLogger();

module Client =
    open System.Net
    open System.Diagnostics
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

    let private asArchiveFile gameMod =
        let modArchiveName = $"{gameMod.Maintainer}-{gameMod.Version}-{gameMod.Name}"
        match gameMod.Archive with
        | Zip -> modArchiveName + ".zip"
        | Rar -> modArchiveName + ".rar"

    let downloadMod gameMod swatDir = 
        // TODO: Check on installed mod dir instead
        if File.Exists(swatDir + (asArchiveFile gameMod)) then
            Error $"{gameMod.Name} already exist!"
        else
            let a = (asArchiveFile gameMod)
            WebClient().DownloadFile(gameMod.Url, Path.Combine(swatDir, a))
            Ok $"{a} downloaded"

    let extractArchive gameMod swatDir =
        let modDirName = $"{gameMod.Maintainer}-{gameMod.Version}-{gameMod.Name}"
        let archivePath = Path.Combine(swatDir, (asArchiveFile gameMod))
        Compression.ZipFile.ExtractToDirectory(archivePath, swatDir)
        Directory.Move(
            Path.Combine(swatDir, gameMod.PreModFolder),
            Path.Combine(swatDir, modDirName))

    let launchMod gameMod swatDir =
        let modDirName = $"{gameMod.Maintainer}-{gameMod.Version}-{gameMod.Name}"
        let modDir = Path.Combine(swatDir, modDirName)
        let systemDir = Path.Combine(modDir, "System")
        
        Directory.SetCurrentDirectory(systemDir)
        Logger.log.Information($"Cded to {systemDir}")

        Logger.log.Information("Launching mod..")
        let externalProcess = new Process()
        externalProcess.StartInfo.FileName <- @"..\..\ContentExpansion\System\Swat4X.exe"
        externalProcess.StartInfo.WindowStyle <- ProcessWindowStyle.Normal
        externalProcess.Start() |> ignore
        externalProcess.WaitForExit()
        Logger.log.Information($"SWAT4 + {gameMod.Name} closed gracefully")


module Launcher =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type Model = {
        SwatInstallationDirectory: string
        Status: string
        IsModRunning: bool 
    }

    let init = { SwatInstallationDirectory = @"C:\GOG Games\SWAT 4"; Status = ""; IsModRunning = false; }, Cmd.none

    type Message =
        | SwatInstallationDirectoryEntryChanged of string
        | Install 
        | Uninstall 
        | Launch


    let update (message: Message) (model: Model) =
        let gameMod = {
            Client.Mod.Name = "SEF"
            Client.Mod.Maintainer = "eezstreet"
            Client.Mod.Version = "v7.0"
            Client.Mod.Url = "https://www.moddb.com/downloads/mirror/195627/124/084b4b2d20eb9f57e10e4b248a1df07d/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-elite-force%2Fdownloads"
            Client.Mod.Origin = Client.OriginType.Official
            Client.Mod.Archive = Client.ArchiveType.Zip
            Client.Mod.PreModFolder = "SEF"
        }

        match message with
        | SwatInstallationDirectoryEntryChanged directory ->
            { model with SwatInstallationDirectory = directory }, Cmd.none
        | Install ->
            Logger.log.Information("Download started..")
            match Client.downloadMod gameMod model.SwatInstallationDirectory with
            | Error err -> { model with Status = err }, Cmd.none
            | Ok m -> 
                Client.extractArchive gameMod model.SwatInstallationDirectory
                Logger.log.Information("Extraction started..")
                { model with Status = m}, Cmd.none
        | Uninstall -> { model with Status = "Mod uninstalled" }, Cmd.none
        | Launch ->
            Client.launchMod gameMod model.SwatInstallationDirectory |> ignore
            { model with Status = gameMod.Name + " has been launched"; IsModRunning = true }, Cmd.none
    
    let view (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    // FIXME: Find a way to emit this state change.
                    // Button.isEnabled model.IsModRunning
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

                Button.create [
                    Button.dock Dock.Bottom
                ]


                TextBox.create [
                    TextBox.dock Dock.Bottom
                    TextBox.text model.SwatInstallationDirectory
                    TextBox.onTextChanged (fun text -> dispatch (SwatInstallationDirectoryEntryChanged text))
                ]
            ] 
        ]       
