﻿// Copyright (c) 2021 Modotte
// This software is licensed under the GPL-3.0 license. For more details,
// please read the LICENSE file content.

namespace SAL

open System.Net
open System.Diagnostics
open System.IO
open Logger
open Domain

module IOHandler =

    exception TemporaryFolderCreationException of string
    exception TemporaryFolderDeletionException of string

    let private getCategory = function
        | SEF -> "SEF"
        | SEF_FR -> "SEF_FR"
        | SEF_BTLA -> "SEF_BTLA"

    let modDirectoryOutput (gameMod: Mod) = 
        $"{gameMod.Maintainer}-{getCategory gameMod.Category}-{gameMod.Version}"

    let private asArchiveFile gameMod =
        let modArchiveName = modDirectoryOutput gameMod
        match gameMod.ArchiveFormat with
        | Zip -> modArchiveName + ".zip"
        | Rar -> modArchiveName + ".rar"
        | SevenZip -> modArchiveName + ".7z"

    let executeDownload gameMod archivePath =
        if File.Exists(archivePath) then
            let msg = $"{archivePath} already exist. Reusing it to save resources.."
            log.Information(msg)
            InstallDownloadResult.Success gameMod
            
        else
            log.Information("Starting to download the mod archive file..")
            log.Information("Downloading from: " + gameMod.Url)
            try
                let client = new WebClient() 
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36")
                client.DownloadFile(gameMod.Url, archivePath)
                log.Information($"{archivePath} has been downloaded")
                InstallDownloadResult.Success gameMod
            with
            | :? WebException as exn ->
                log.Error(exn.Message)
                InstallDownloadResult.Failure (gameMod, exn.Message)

    let downloadArchive gameMod swatDir = 
        let modInstallDir = modDirectoryOutput gameMod
        let archivePath = Path.Combine(swatDir, asArchiveFile gameMod)

        if Directory.Exists(Path.Combine(swatDir, modInstallDir)) then
            let err = $"{modInstallDir} already installed!"
            log.Error(err)
            InstallDownloadResult.Failure (gameMod, err)
            
        else
            if File.Exists(archivePath) then
                let msg = $"{archivePath} already exist. Reusing it to save resources.."
                log.Information(msg)
                InstallDownloadResult.Success gameMod
                
            else
                executeDownload gameMod archivePath

    let private makeTemporaryFolder swatDir =
        try
            log.Information("Creating temporary folder for archive extraction..")

            let name = System.Guid.NewGuid().ToString()
            let tempDirPath = Path.Combine(swatDir, name)
            Directory.CreateDirectory(tempDirPath) |> ignore

            log.Information("Temporary folder " + name + " has been created..")

            name
        with
        | :? IOException as exn -> 
            raise (TemporaryFolderCreationException exn.Message)

    let private deleteTemporaryFolder tempDirPath =
        try
            log.Information("Deleting temporary extraction folder ..")
            Directory.Delete(tempDirPath, true)
            log.Information("Deleted temporary extraction folder " + tempDirPath)
        with
        | :? IOException as exn ->
            raise (TemporaryFolderDeletionException $"Failed to make temporary folder. Error: {exn.Message}")

    let extractArchive gameMod swatDir =
        log.Information("Beginning to extract mod archive..")

        let tempDirName = makeTemporaryFolder swatDir
        let tempDirPath = Path.Combine(swatDir, tempDirName)
        let archivePath = Path.Combine(swatDir, (asArchiveFile gameMod))

        log.Information("Extracting mod archive..")
        try
            match gameMod.ArchiveFormat with
            | Zip -> Archive.extractZipArchive archivePath tempDirPath
            | Rar -> Archive.extractRarArchive archivePath tempDirPath
            | SevenZip -> Archive.extractSevenZipArchive archivePath tempDirPath

            log.Information("Finished extracting mod archive")

            log.Information("Renaming extracted folder...")
            Directory.Move(
                Path.Combine(tempDirPath, gameMod.ModRootFolderName),
                Path.Combine(swatDir, modDirectoryOutput gameMod)
            )

            log.Information("Finished renaming extracted folder..")

            deleteTemporaryFolder tempDirPath
            log.Information("Deleted temporary folder..")
            
            log.Information(modDirectoryOutput gameMod + " installed successfully")
            InstallExtractionResult.Success gameMod

        with
        | :? System.IndexOutOfRangeException as exn ->
            log.Error(exn.Message)
            let err = $"{modDirectoryOutput gameMod} archive is corrupted. Please delete it and retry again later."
            InstallExtractionResult.Failure (gameMod, err)

        | _ as exn -> 
            log.Error(exn.Message)
            InstallExtractionResult.Failure (gameMod, exn.Message)

    let uninstallMod gameMod swatDir =
        let modPath = modDirectoryOutput gameMod
        if not (Directory.Exists(Path.Combine(swatDir, modPath))) then
            let err = (modDirectoryOutput gameMod) + " is not even installed!"
            log.Error err
            UninstallationResult.Failure (gameMod, err)

        else
            try
                log.Information("Beginning to uninstall mod..")
                log.Information($"Deleting {modPath}..gonna take a few seconds..")
                Directory.Delete(Path.Combine(swatDir, modPath), true)
                log.Information("Finished uninstalling..")

                let msg = modPath + " uninstalled successfully"
                log.Information(msg)
                UninstallationResult.Success gameMod
            with
            | :? IOException as exn -> 
                log.Error(exn.Message)
                UninstallationResult.Failure (gameMod, exn.Message)

    let launchMod gameMod swatDir =
        let modDir = Path.Combine(swatDir, modDirectoryOutput gameMod)
        let systemDir = Path.Combine(modDir, "System")
        
        if not (Directory.Exists(systemDir)) then
            let err = (modDirectoryOutput gameMod) + " is not installed!"
            log.Error(err)
            LaunchResult.Failure (gameMod, err)

        else
            let beforeLaunchDirectory = Directory.GetCurrentDirectory()
            log.Information(beforeLaunchDirectory)

            Directory.SetCurrentDirectory(systemDir)

            log.Information($"Change current working directory into {systemDir}")

            log.Information("Launching mod..")
            let launcher = @"..\..\ContentExpansion\System\Swat4X.exe"
            if not (File.Exists(launcher)) then
                let err = Path.Combine(swatDir, launcher) + " doesn't exist! Possible corrupted mod installation!"
                log.Error(err)
                LaunchResult.Failure (gameMod, err)

            else
                let externalProcess = new Process()
                externalProcess.StartInfo.FileName <- launcher
                externalProcess.StartInfo.WindowStyle <- ProcessWindowStyle.Normal
                externalProcess.Start() |> ignore
                externalProcess.WaitForExit()
                log.Information($"SWAT4 + {modDirectoryOutput gameMod} closed gracefully")

                Directory.SetCurrentDirectory(beforeLaunchDirectory)
                log.Information($"{launcher} executed and closed gracefully")
                LaunchResult.Success gameMod
