﻿namespace SAL

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts

open SAL.Data

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "SAL: SWAT4 Alternative Launcher"
        base.Width <- 800.0
        base.Height <- 600.0
        
        //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
        //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true


        // TODO: Turn Launcher.update to (Storage.updateStorage Launcher.update) when auto update is needed
        // for configuration.json
        Program.mkProgram (Storage.load >> DomainModel.init) Launcher.update View.view
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
    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseWin32()
            .StartWithClassicDesktopLifetime(args)
