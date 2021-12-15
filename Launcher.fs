namespace SAL

module Client =
    open System.Net
    open System.IO

    type Archive = Zip | Rar

    let [<Literal>] SWAT_INSTALLATION_DIRECTORY = "SWAT4"

    let asArchiveFile (archiveType: Archive) (modName: string) =
        match archiveType with
        | Zip -> modName + ".zip"
        | Rar -> modName + ".rar"

    let downloadMod (url: string) modName archiveType = 
        let downloaDir = Path.Combine(SWAT_INSTALLATION_DIRECTORY)

        if Directory.Exists(downloaDir + (asArchiveFile archiveType modName)) then
            Error $"{modName} already exist!"
        else
            let a = (asArchiveFile archiveType modName)
            printfn "Download started.."
            WebClient().DownloadFile(url, Path.Combine(downloaDir, a))
            Ok $"{a} downloaded"

module Launcher =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type Model = { Status: string }
    let init = { Status = "" }

    type Msg = Install | Uninstall

    let update (msg: Msg) (model: Model) : Model =
        match msg with
        | Install ->
            match Client.downloadMod "https://www.moddb.com/downloads/mirror/195627/115/b7e306bbf7d472a49725194bedb0da71" "SEF" Client.Archive.Zip with
            | Error err -> { model with Status = err }
            | Ok m -> { model with Status = m}
        | Uninstall -> { model with Status = "Mod uninstalled" }
    
    let view (model: Model) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Uninstall)
                    Button.content "Uninstall"
                ]                
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Install)
                    Button.content "Install"
                ]
                TextBlock.create [
                    TextBlock.dock Dock.Top
                    TextBlock.fontSize 32.0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                    TextBlock.text (string model.Status)
                ]
            ]
        ]       
