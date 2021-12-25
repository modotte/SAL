// Copyright (c) 2021 Modotte
// Copyright (c) 2019 Josua Jäger (Original template code under MIT license, please see LICENSE.MIT)
// This software is licensed under the GPL-3.0 license. For more details,
// please read the LICENSE file content.

namespace SAL

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
        
type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
        this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- Shell.ShellWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseWin32()
            .StartWithClassicDesktopLifetime(args)
