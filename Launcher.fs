namespace SAL

open System.IO
open Logger
open Elmish
open DomainModel
open SAL.Data

module Client =
    open System.Net
    open System.Diagnostics

    let private modDirectoryOutput gameMod = $"{gameMod.Maintainer}-{gameMod.Name}-{gameMod.Version}"

    let private asArchiveFile gameMod =
        let modArchiveName = modDirectoryOutput gameMod
        modArchiveName + ".zip"

    let downloadMod gameMod swatDir = 
        // TODO: Check on installed mod dir instead
        if File.Exists(swatDir + (asArchiveFile gameMod)) then
            Error $"{gameMod.Name} already exist!"
        else
            let archive = (asArchiveFile gameMod)
            // TODO: Replace with async stuff and update
            WebClient().DownloadFile(gameMod.Url, Path.Combine(swatDir, archive))
            Ok $"{archive} downloaded"

    let extractArchive gameMod swatDir =
        let archivePath = Path.Combine(swatDir, (asArchiveFile gameMod))
        Compression.ZipFile.ExtractToDirectory(archivePath, swatDir)
        Directory.Move(
            Path.Combine(swatDir, gameMod.PreExtractFolder),
            Path.Combine(swatDir, modDirectoryOutput gameMod))

    let launchMod gameMod swatDir =
        let modDir = Path.Combine(swatDir, modDirectoryOutput gameMod)
        let systemDir = Path.Combine(modDir, "System")
        
        Directory.SetCurrentDirectory(systemDir)
        log.Information($"Cded to {systemDir}")

        log.Information("Launching mod..")
        let executable = @"..\..\ContentExpansion\System\Swat4X.exe"
        if not (File.Exists(executable)) then
            log.Error(executable + " doesn't exist!")

        let externalProcess = new Process()
        externalProcess.StartInfo.FileName <- executable
        externalProcess.StartInfo.WindowStyle <- ProcessWindowStyle.Normal
        externalProcess.Start() |> ignore
        externalProcess.WaitForExit()
        log.Information($"SWAT4 + {gameMod.Name} closed gracefully")


module Launcher =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type Model = {
        SwatInstallationDirectory: string
        Status: string
        IsModRunning: bool 
    }

    let sid = Settings.currentSettings.SwatInstallationDirectory
    let init = { 
        SwatInstallationDirectory = sid
        Status = ""
        IsModRunning = false }, Cmd.none

    type Message =
        | SwatInstallationDirectoryEntryChanged of string
        | Install 
        | Uninstall 
        | Launch


    let update (message: Message) (model: Model) =
        let gameMod = {
            Mod.Name = "SEF"
            Mod.Maintainer = "eezstreet"
            Mod.Version = "v7.0"
            Mod.Url = "https://www.moddb.com/downloads/mirror/195627/115/35d7c155b0249f6ca4aae6fb2a366cda/?referer=https%3A%2F%2Fwww.moddb.com%2Fmods%2Fswat-elite-force%2Fdownloads"
            Mod.Origin = OriginType.Official
            Mod.PreExtractFolder = "SEF"
        }

        match message with
        | SwatInstallationDirectoryEntryChanged directory ->
            { model with SwatInstallationDirectory = directory }, Cmd.none
        | Install ->
            log.Information("Download started..")
            match Client.downloadMod gameMod model.SwatInstallationDirectory with
            | Error err -> { model with Status = err }, Cmd.none
            | Ok m -> 
                Client.extractArchive gameMod model.SwatInstallationDirectory
                log.Information("Extraction started..")
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

                TextBox.create [
                    TextBox.dock Dock.Bottom
                    TextBox.text model.SwatInstallationDirectory
                    TextBox.onTextChanged (fun text -> dispatch (SwatInstallationDirectoryEntryChanged text))
                ]
            ] 
        ]       
