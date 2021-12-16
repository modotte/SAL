﻿namespace SAL

open System.IO
open Logger
open Elmish
open DomainModel
open SAL.Data

module Client =
    open System.Net
    open System.Diagnostics
    
    let private modDirectoryOutput (gameMod: Mods.Mod) = $"{gameMod.Maintainer}-{Mods.getCategory gameMod.Category}-{gameMod.Version}"

    let private asArchiveFile gameMod =
        let modArchiveName = modDirectoryOutput gameMod
        modArchiveName + ".zip"

    let downloadMod gameMod swatDir = 
        let archive = modDirectoryOutput gameMod
        let archivePath = Path.Combine(swatDir, archive)

        if Directory.Exists(archivePath) then
            let err = $"{archive} already installed!"
            log.Error err
            Error err
        else
            // TODO: Replace with async stuff and update
            log.Information("Starting to download the mod..")
            log.Information("Downloading from " + gameMod.Url)
            WebClient().DownloadFile(gameMod.Url, archivePath)
            Ok $"{archive} downloaded"

    let extractArchive gameMod swatDir =
        log.Information("Beginning to extract mod archive..")
        let archivePath = Path.Combine(swatDir, (asArchiveFile gameMod))
        Compression.ZipFile.ExtractToDirectory(archivePath, swatDir)
        Directory.Move(
            Path.Combine(swatDir, gameMod.PreExtractFolder),
            Path.Combine(swatDir, modDirectoryOutput gameMod))
        log.Information("Finished extracting mod archive")

    let launchMod gameMod swatDir =
        let modDir = Path.Combine(swatDir, modDirectoryOutput gameMod)
        let systemDir = Path.Combine(modDir, "System")
        
        if not (Directory.Exists(systemDir)) then
            let err = (modDirectoryOutput gameMod) + " is not installed!"
            log.Error err
            Error err

        else
            Directory.SetCurrentDirectory(systemDir)
            log.Information($"Change current working directory into {systemDir}")

            log.Information("Launching mod..")
            let launcher = @"..\..\ContentExpansion\System\Swat4X.exe"
            if not (File.Exists(launcher)) then
                let err = launcher + " doesn't exist! Possible corrupted mod installation!"
                log.Error(err)
                Error err

            else
                let externalProcess = new Process()
                externalProcess.StartInfo.FileName <- launcher
                externalProcess.StartInfo.WindowStyle <- ProcessWindowStyle.Normal
                externalProcess.Start() |> ignore
                externalProcess.WaitForExit()
                log.Information($"SWAT4 + {modDirectoryOutput gameMod} closed gracefully")

                Ok $"{launcher} executed and closed gracefully"


module Launcher =
    let getModById id model =
        model.GameMods
        |> Array.filter (fun m -> m.Id = id)
        |> Array.head
    let OnSwatInstallationDirectoryEntryChanged directory model = { model with SwatInstallationDirectory = directory }, Cmd.none

    let OnInstall id model =
        let selectedMod = getModById id model
        match Client.downloadMod selectedMod model.SwatInstallationDirectory with
        | Error err -> { model with Status = err }, Cmd.none
        | Ok msg -> 
            Client.extractArchive selectedMod model.SwatInstallationDirectory
            { model with Status = msg }, Cmd.none

    let OnUninstall id model = { model with Status = "Mod uninstalled" }, Cmd.none

    let OnLaunch id model = 
        let selectedMod = getModById id model
        match Client.launchMod selectedMod model.SwatInstallationDirectory with
        | Ok msg -> { model with Status = msg }, Cmd.none
        | Error err -> { model with Status = err }, Cmd.none

    let update (message: Message) (model: Model) =
        match message with
        | Failure err -> log.Error err; model, Cmd.none
        | SwatInstallationDirectoryEntryChanged directory -> OnSwatInstallationDirectoryEntryChanged directory model
        | Install id -> OnInstall id model
        | Uninstall id -> OnUninstall id model
        | Launch id -> OnLaunch id model
