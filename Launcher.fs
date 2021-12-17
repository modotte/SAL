namespace SAL

open System.IO
open Logger
open Elmish
open DomainModel
open SAL.Data

module Client =
    open System.Net
    open System.Diagnostics
    
    let private modDirectoryOutput (gameMod: Mods.Mod) = 
        $"{gameMod.Maintainer}-{Mods.getCategory gameMod.Category}-{gameMod.Version}"

    let private asArchiveFile gameMod =
        let modArchiveName = modDirectoryOutput gameMod
        modArchiveName + ".zip"

    let downloadMod gameMod swatDir = 
        let archive = modDirectoryOutput gameMod
        let archivePath = Path.Combine(swatDir, asArchiveFile gameMod)

        if Directory.Exists(Path.Combine(swatDir, archive)) then
            let err = $"{archive} already installed!"
            log.Error err
            Error err
        else
            // TODO: Replace with async stuff and update
            log.Information("Starting to download the mod..")
            log.Information("Downloading from " + gameMod.Url)
            try
                WebClient().DownloadFile(gameMod.Url, archivePath)
                Ok $"{archive} downloaded"
            with
            | :? WebException as exn ->
                log.Error(exn.Message)
                Error exn.Message

    let private makeTemporaryFolder swatDir =
        log.Information("Creating temporary folder for archive extraction..")

        let name = System.Guid.NewGuid().ToString()
        let tempDirPath = Path.Combine(swatDir, name)
        Directory.CreateDirectory(tempDirPath) |> ignore

        log.Information("Temporary folder " + name + " has been created..")

        name

    let private deleteTemporaryFolder tempDirPath =
        log.Information ("Deleting temporary folder ..")
        Directory.Delete(tempDirPath, true)
        log.Information("Deleted temporary folder " + tempDirPath)

    let extractArchive gameMod swatDir =
        log.Information("Beginning to extract mod archive..")

        let tempDirName = makeTemporaryFolder swatDir
        let tempDirPath = Path.Combine(swatDir, tempDirName)

        let archivePath = Path.Combine(swatDir, (asArchiveFile gameMod))
        log.Information("Extracting mod archive..")
        try
            Compression.ZipFile.ExtractToDirectory(archivePath, tempDirPath)
        with
        | :? System.IO.DirectoryNotFoundException as exn ->
            log.Error(exn.Message)
        | :? System.IO.PathTooLongException as exn ->
            log.Error(exn.Message)
        | :? System.IO.InvalidDataException as exn ->
            log.Error("Corrupt zip archive")
            log.Error(exn.Message)

        log.Information("Finished extracting mod archive")

        log.Information("Renaming extracted folder...")
        Directory.Move(
            Path.Combine(tempDirPath, gameMod.PreExtractFolder),
            Path.Combine(swatDir, modDirectoryOutput gameMod)
        )
        log.Information("Finished renaming extracted folder..")

        log.Information("Deleting temporary folder for extraction..")
        deleteTemporaryFolder tempDirPath
        log.Information("Deleted temporary folder..")

        log.Information("Deleting redundant archive..")
        File.Delete(archivePath)
        log.Information("Deleted archive")
        
        log.Information(modDirectoryOutput gameMod + " installed successfully")

    let uninstallMod gameMod swatDir =
        let modPath = modDirectoryOutput gameMod
        if not (Directory.Exists(Path.Combine(swatDir, modPath))) then
            let err = (modDirectoryOutput gameMod) + " is not even installed!"
            log.Error err
            Error err

        else
            log.Information("Beginning to uninstall mod..")
            log.Information($"Deleting {modPath}..gonna take a few seconds..")
            // FIXME: Throws violated memory protection access when
            // clicking uninstall after closing a direct launched game.
            // Possibly resource was not fully released and cause this issue.
            Directory.Delete(Path.Combine(swatDir, modPath), true)

            log.Information("Finished uninstalling..")

            let msg = modPath + " uninstalled successfully"
            log.Information(msg)
            Ok msg

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

    let OnUninstall id model = 
        let selectedMod = getModById id model
        match Client.uninstallMod selectedMod model.SwatInstallationDirectory with
        | Ok msg -> { model with Status = msg }, Cmd.none
        | Error err -> { model with Status = err }, Cmd.none

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
