﻿namespace SAL

open System.IO
open Logger
open Elmish
open DomainModel
open SAL.Data

module Client =
    open System.Net
    open System.Diagnostics
    open Mods

    let private modDirectoryOutput gameMod = $"{gameMod.Maintainer}-{getCategory gameMod.Category}-{gameMod.Version}"

    let private asArchiveFile gameMod =
        let modArchiveName = modDirectoryOutput gameMod
        modArchiveName + ".zip"

    let downloadMod gameMod swatDir = 
        // TODO: Check on installed mod dir instead
        if File.Exists(swatDir + (asArchiveFile gameMod)) then
            Error $"{gameMod.Category} already exist!"
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
        let launcher = @"..\..\ContentExpansion\System\Swat4X.exe"
        if not (File.Exists(launcher)) then
            log.Error(launcher + " doesn't exist!")

        let externalProcess = new Process()
        externalProcess.StartInfo.FileName <- launcher
        externalProcess.StartInfo.WindowStyle <- ProcessWindowStyle.Normal
        externalProcess.Start() |> ignore
        externalProcess.WaitForExit()
        log.Information($"SWAT4 + {gameMod.Category} closed gracefully")


module Launcher =
    let OnSwatInstallationDirectoryEntryChanged directory model =
        { model with SwatInstallationDirectory = directory }, Cmd.none

    let OnInstall gameMod model = 
        log.Information("Download started..")
        match Client.downloadMod gameMod model.SwatInstallationDirectory with
        | Error err -> { model with Status = err }, Cmd.none
        | Ok m -> 
            Client.extractArchive gameMod model.SwatInstallationDirectory
            log.Information("Extraction started..")
            { model with Status = m; IsModInstalled = true}, Cmd.none

    let OnUninstall model = { model with Status = "Mod uninstalled" }, Cmd.none

    let OnLaunch gameMod model = 
        Client.launchMod gameMod model.SwatInstallationDirectory |> ignore
        { model with Status = (Mods.getCategory gameMod.Category) + " has been launched"; IsModRunning = true }, Cmd.none

    let update (message: Message) (model: Model) =
        let gameMod = Mods.mods[0]

        match message with
        | SwatInstallationDirectoryEntryChanged directory -> OnSwatInstallationDirectoryEntryChanged directory model
        | Install -> OnInstall gameMod model
        | Uninstall -> OnUninstall model
        | Launch -> OnLaunch gameMod model
