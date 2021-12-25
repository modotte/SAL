// Copyright (c) 2021 Modotte
// This software is licensed under the GPL-3.0 license. For more details,
// please read the LICENSE file content.

namespace SAL

open Avalonia.Controls

module Dialog =
    // Window dialog
    let getFolderDialog directory =
        let dialog = OpenFolderDialog()
        dialog.Title <- "Choose where to look up for SWAT4 directory"
        dialog.Directory <- directory
        
        dialog
