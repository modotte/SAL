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

    let OnSwatInstallationDirectoryEntryChanged directory model =
        { model with SwatInstallationDirectory = directory }, Cmd.none

    let OnInstall gameMod model = 
        log.Information("Download started..")
        match Client.downloadMod gameMod model.SwatInstallationDirectory with
        | Error err -> { model with Status = err }, Cmd.none
        | Ok m -> 
            Client.extractArchive gameMod model.SwatInstallationDirectory
            log.Information("Extraction started..")
            { model with Status = m}, Cmd.none

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

    let makeModStackView (currentMod: Mod) (model: Model) dispatch =
        WrapPanel.create [
            WrapPanel.children [
                TextBlock.create [ TextBlock.text $"{currentMod.Maintainer}-{currentMod.Version}-{currentMod.Stability.ToString()}" ]
                Button.create [
                    Button.dock Dock.Bottom
                    // FIXME: Find a way to emit this state change.
                    // Button.isEnabled model.IsModRunning
                    Button.background "Green"
                    Button.onClick (fun _ -> dispatch Launch)
                    Button.content "Launch Mod"
                ]                
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Install)
                    Button.content "Install"
                ]

                Button.create [
                    Button.dock Dock.Bottom
                    Button.background "Red"
                    Button.onClick (fun _ -> dispatch Uninstall)
                    Button.content "Uninstall"
                ]
            ]
        ]    

    let getMods category mods =
        mods |> Array.filter (fun m -> m.Category = category)

    let makeModCategoriesView (mods: Mod array) (model: Model) dispatch =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.spacing 32
            StackPanel.children [
                
                StackPanel.create [
                    StackPanel.children [
                        TextBlock.create [ TextBlock.text "SEF" ]
                        StackPanel.create [
                            StackPanel.children (
                                getMods ModCategory.SEF mods
                                |> Array.toList
                                |> List.map  (fun m -> makeModStackView m model dispatch)
                            )
                        ]
                    ]
                ]
                

                StackPanel.create [
                    StackPanel.children [
                        TextBlock.create [ TextBlock.text "SEF - First Responders" ]
                        StackPanel.create [
                            StackPanel.children (
                                getMods ModCategory.SEF_FR mods
                                |> Array.toList
                                |> List.map  (fun m -> makeModStackView m model dispatch)
                            )
                        ]
                    ]
                ]

                StackPanel.create [
                    StackPanel.children [
                        TextBlock.create [ TextBlock.text "SEF - Back To Los Angeles" ]
                        StackPanel.create [
                            StackPanel.children (
                                getMods ModCategory.SEF_BTLA mods
                                |> Array.toList
                                |> List.map  (fun m -> makeModStackView m model dispatch)
                            )
                        ]
                    ]
                ]
            ]
        ]
    
    let view (model: Model) dispatch =
        StackPanel.create [
            StackPanel.verticalAlignment VerticalAlignment.Top
            StackPanel.spacing 8.0
            StackPanel.margin 8.0
            StackPanel.children [    
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.children [

                        StackPanel.create [
                            StackPanel.verticalAlignment VerticalAlignment.Top
                            StackPanel.horizontalAlignment HorizontalAlignment.Center
                            StackPanel.spacing 8.0
                            StackPanel.margin 8.0
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.fontSize 15.0
                                    TextBlock.text "SWAT4 Folder: "
                                ]

                                TextBox.create [
                                    TextBox.minWidth 500
                                    TextBox.onTextChanged (SwatInstallationDirectoryEntryChanged >> dispatch)
                                    TextBox.text model.SwatInstallationDirectory
                                ]
                            ]
                        ]
                        makeModCategoriesView Mods.mods model dispatch
                    ]
                ]
            ]
        ]
