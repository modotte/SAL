namespace SAL

open System.IO
open Logger
open Elmish
open DomainModel
open SAL.Data

module Client =
    open System.Net
    open System.Diagnostics
    
    let private modDirectoryOutput (gameMod: Mod) = 
        $"{gameMod.Maintainer}-{Storage.getCategory gameMod.Category}-{gameMod.Version}"

    let private asArchiveFile gameMod =
        let modArchiveName = modDirectoryOutput gameMod
        match gameMod.ArchiveFormat with
        | Zip -> modArchiveName + ".zip"
        | Rar -> modArchiveName + ".rar"

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
        log.Information("Creating temporary folder for archive extraction..")

        let name = System.Guid.NewGuid().ToString()
        let tempDirPath = Path.Combine(swatDir, name)
        Directory.CreateDirectory(tempDirPath) |> ignore

        log.Information("Temporary folder " + name + " has been created..")

        name

    let private deleteTemporaryFolder tempDirPath =
        log.Information ("Deleting temporary extraction folder ..")
        Directory.Delete(tempDirPath, true)
        log.Information("Deleted temporary extraction folder " + tempDirPath)

    let extractArchive gameMod swatDir =
        log.Information("Beginning to extract mod archive..")

        let tempDirName = makeTemporaryFolder swatDir
        let tempDirPath = Path.Combine(swatDir, tempDirName)
        let archivePath = Path.Combine(swatDir, (asArchiveFile gameMod))

        log.Information("Extracting mod archive..")
        match gameMod.ArchiveFormat with
        | Zip -> Archive.extractZipArchiveTo archivePath tempDirPath
        | Rar -> Archive.extractRarArchiveTo archivePath tempDirPath
        log.Information("Finished extracting mod archive")

        log.Information("Renaming extracted folder...")
        Directory.Move(
            Path.Combine(tempDirPath, gameMod.PreExtractFolder),
            Path.Combine(swatDir, modDirectoryOutput gameMod)
        )
        log.Information("Finished renaming extracted folder..")

        deleteTemporaryFolder tempDirPath
        log.Information("Deleted temporary folder..")
        
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
            // BUG: Throws violated memory protection access when
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
    module UpdateHandlers =
        let private getModById id model =
            model.Mods
            |> Array.filter (fun m -> m.Id = id)
            |> Array.head

        let withSwatDirectoryEntryChanged directory model = { model with SwatDirectory = directory }, Cmd.none

        let withInstall id model =
            let selectedMod = getModById id model
            match Client.downloadMod selectedMod model.SwatDirectory with
            | Error err -> { model with Status = err }, Cmd.none
            | Ok msg -> 
                let updateMod selectedMod =
                    if selectedMod.Id = id then { selectedMod with IsInstalled = true }
                    else selectedMod

                Client.extractArchive selectedMod model.SwatDirectory
                { model with Status = msg; Mods = Array.map updateMod model.Mods }, Cmd.none

        let withUninstall id model = 
            let selectedMod = getModById id model
            match Client.uninstallMod selectedMod model.SwatDirectory with
            | Error err -> { model with Status = err }, Cmd.none
            | Ok msg -> 
                let updateMod selectedMod =
                    if selectedMod.Id = id then { selectedMod with IsInstalled = false }
                    else selectedMod
                { model with Status = msg; Mods = Array.map updateMod model.Mods }, Cmd.none

        let withLaunch id model = 
            let selectedMod = getModById id model
            match Client.launchMod selectedMod model.SwatDirectory with
            | Ok msg -> { model with Status = msg }, Cmd.none
            | Error err -> { model with Status = err }, Cmd.none
    
    let update (msg: Message) (model: Model): Model * Cmd<Message> =
        match msg with
        | Failure err -> log.Error err; model, Cmd.none
        | SwatDirectoryEntryChanged directory -> UpdateHandlers.withSwatDirectoryEntryChanged directory model
        | Install id -> UpdateHandlers.withInstall id model
        | Uninstall id -> UpdateHandlers.withUninstall id model
        | Launch id -> UpdateHandlers.withLaunch id model
