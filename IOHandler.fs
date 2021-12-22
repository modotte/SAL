namespace SAL

open System.IO
open Logger
open Domain

module IOHandler =
    open System.Net
    open System.Diagnostics

    exception TemporaryFolderCreationException of string
    exception TemporaryFolderDeletionException of string

    let private modDirectoryOutput (gameMod: Mod) = 
        $"{gameMod.Maintainer}-{getCategory gameMod.Category}-{gameMod.Version}"

    let private asArchiveFile gameMod =
        let modArchiveName = modDirectoryOutput gameMod
        match gameMod.ArchiveFormat with
        | Zip -> modArchiveName + ".zip"
        | Rar -> modArchiveName + ".rar"
        | SevenZip -> modArchiveName + ".7z"

    let downloadMod gameMod swatDir = 
        let modInstallDir = modDirectoryOutput gameMod
        let archivePath = Path.Combine(swatDir, asArchiveFile gameMod)

        if Directory.Exists(Path.Combine(swatDir, modInstallDir)) then
            let err = $"{modInstallDir} already installed!"
            log.Error err
            Error err
        else
            if File.Exists(archivePath) then
                let msg = $"{archivePath} already exist. Reusing it to save resources.."
                log.Information(msg)
                Ok msg
                
            else
                // TODO: Replace with async stuff and update
                log.Information("Starting to download the mod archive file..")
                log.Information("Downloading from: " + gameMod.Url)
                try
                    let client = new WebClient() 
                    client.DownloadFile(gameMod.Url, archivePath)
                    Ok $"{archivePath} has been downloaded"
                with
                | :? WebException as exn ->
                    log.Error(exn.Message)
                    Error exn.Message

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
        match gameMod.ArchiveFormat with
        | Zip -> Archive.extractZipArchiveTo archivePath tempDirPath
        | Rar -> Archive.extractRarArchiveTo archivePath tempDirPath
        | SevenZip -> Archive.extractSevenZipArchiveTo archivePath tempDirPath

        log.Information("Finished extracting mod archive")

        try
            log.Information("Renaming extracted folder...")
            Directory.Move(
                Path.Combine(tempDirPath, gameMod.PreExtractFolder),
                Path.Combine(swatDir, modDirectoryOutput gameMod)
            )

            log.Information("Finished renaming extracted folder..")

            deleteTemporaryFolder tempDirPath
            log.Information("Deleted temporary folder..")
            
            log.Information(modDirectoryOutput gameMod + " installed successfully")
            InstallExtractionResult.Success gameMod

        with
        | :? IOException as exn ->
            log.Error(exn.Message)
            InstallExtractionResult.Failure (gameMod, exn.Message)
        | :? TemporaryFolderCreationException as exn ->
            InstallExtractionResult.Failure (gameMod, exn.Message)
        | :? TemporaryFolderDeletionException as exn ->
            log.Error(exn.Message)
            InstallExtractionResult.Failure (gameMod, exn.Message)

    let uninstallMod gameMod swatDir =
        let modPath = modDirectoryOutput gameMod
        if not (Directory.Exists(Path.Combine(swatDir, modPath))) then
            let err = (modDirectoryOutput gameMod) + " is not even installed!"
            log.Error err
            UninstallationResult.Failure (gameMod, err)

        else
            log.Information("Beginning to uninstall mod..")
            log.Information($"Deleting {modPath}..gonna take a few seconds..")

            try
                Directory.Delete(Path.Combine(swatDir, modPath), true)
            with
            | :? IOException as exn -> log.Error(exn.Message)

            log.Information("Finished uninstalling..")

            let msg = modPath + " uninstalled successfully"
            log.Information(msg)
            UninstallationResult.Success gameMod

    let launchMod gameMod swatDir =
        let modDir = Path.Combine(swatDir, modDirectoryOutput gameMod)
        let systemDir = Path.Combine(modDir, "System")
        
        if not (Directory.Exists(systemDir)) then
            let err = (modDirectoryOutput gameMod) + " is not installed!"
            log.Error err
            Error err

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
                Error err

            else
                let externalProcess = new Process()
                externalProcess.StartInfo.FileName <- launcher
                externalProcess.StartInfo.WindowStyle <- ProcessWindowStyle.Normal
                externalProcess.Start() |> ignore
                externalProcess.WaitForExit()
                log.Information($"SWAT4 + {modDirectoryOutput gameMod} closed gracefully")

                Directory.SetCurrentDirectory(beforeLaunchDirectory)

                Ok $"{launcher} executed and closed gracefully"
