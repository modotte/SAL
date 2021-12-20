namespace SAL

open System
open Avalonia.Controls


module Dialog =
    let getFolderDialog directory =
        let dialog = OpenFolderDialog()
        dialog.Directory <- directory
        dialog.Title <- "Choose where to look up for music"
        dialog
