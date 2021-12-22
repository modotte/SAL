namespace SAL

open Avalonia.Controls


module Dialog =
    let getFolderDialog directory =
        let dialog = OpenFolderDialog()
        dialog.Directory <- directory
        dialog.Title <- "Choose where to look up for SWAT4 directory"
        
        dialog
