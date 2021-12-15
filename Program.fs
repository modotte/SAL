﻿namespace SAL

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "SAL: SWAT4 Alternative Launcher"
        base.Width <- 800.0
        base.Height <- 600.0
        
        //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
        //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true


        Program.mkProgram (fun () -> Launcher.init) Launcher.update Launcher.view
        |> Program.withHost this
        |> Program.run

        
type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
        this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    open System
    open System.IO
    open Logger
    open FSharp.Json

    [<EntryPoint>]
    let main(args: string[]) =
        try
            File.ReadAllText("settings.json")
            |> Json.deserialize<SALSettings> |> ignore

            AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseSkia()
                .StartWithClassicDesktopLifetime(args)
        with
        | :? FileNotFoundException as exn ->
            log.Error(exn.Message)
            Environment.Exit(1)

            1
