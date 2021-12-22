namespace SAL

open Avalonia.Dialogs
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Media


module Dialog =
    let getInfoDialog title =
        let dialog = AboutAvaloniaDialog()
        dialog.Title <- title
        dialog.Background <- Brush.Parse("Gray")
        dialog.Content <- [ TextBlock.create [ TextBlock.text "Hello world" ] ]
        
        dialog
    let getFolderDialog directory =
        let dialog = OpenFolderDialog()
        dialog.Directory <- directory
        dialog.Title <- "Choose where to look up for SWAT4 directory"
        
        dialog
