namespace SAL

open Avalonia.Controls


module Dialog =
    let getFolderDialog directory =
        let dialog = OpenFolderDialog()
        dialog.Title <- "Choose where to look up for SWAT4 directory"
        dialog.Directory <- directory
        
        dialog
