namespace SAL

open Avalonia.FuncUI.DSL

module View =
    open Avalonia.Controls
    open Avalonia.Layout

    open SAL.Domain

    let menuBar dispatch =
        Menu.create [
            Menu.dock Dock.Top
            Menu.viewItems [
                MenuItem.create [
                    MenuItem.header "File"
                    MenuItem.viewItems [
                        MenuItem.create [
                            MenuItem.header "Quit"
                            MenuItem.onClick (fun _ -> dispatch QuitProgram)
                        ]
                    ]
                ]
            ]
        ]

    let getMods category mods =
        mods |> Array.filter (fun m -> m.Category = category)

    let makeSwatDirectoryChooser model dispatch =
        StackPanel.create [
            StackPanel.verticalAlignment VerticalAlignment.Top
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.spacing 8.0
            StackPanel.margin 8.0
            StackPanel.orientation Orientation.Horizontal
                            
            // BUG: Reuse previous value if user close dialog
            StackPanel.children [
                TextBlock.create [
                    TextBlock.fontSize 15.0
                    TextBlock.text "SWAT4 Folder: "
                ]

                TextBox.create [
                    TextBlock.isEnabled false
                    TextBox.minWidth 500.0
                    TextBox.text model.SwatDirectory
                ]

                Button.create [
                    Button.isEnabled (not model.IsInProgress)
                    Button.content "Choose SWAT4 folder"
                    Button.onClick (fun _ -> dispatch OpenFolderDialog)
                ]
            ]
        ]
    
    let makeModStackView selectedMod model dispatch =
        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.children [
                let isInstalledText = if selectedMod.IsInstalled then "[INSTALLED]" else ""
                TextBlock.create [ TextBlock.text $"{selectedMod.Maintainer}-{selectedMod.Version}-{selectedMod.Stability.ToString()} {isInstalledText}" ]
                
                match selectedMod.Description with
                | None -> ()
                | Some desc ->
                    Expander.create [
                        Expander.header "More Info"
                        Expander.content (StackPanel.create [ StackPanel.children [ TextBlock.create [ TextBlock.text desc ] ] ])
                    ]

                
                if selectedMod.IsInstalled then
                    Button.create [
                        Button.dock Dock.Bottom
                        // FIXME: Find a way to emit this state change.
                        // Button.isEnabled model.IsModRunning
                        Button.isEnabled (not model.IsInProgress)
                        Button.background "Green"
                        Button.onClick (fun _ -> dispatch (Launch selectedMod.Id))
                        Button.content "Launch Mod"
                    ]                

                    Button.create [
                        Button.dock Dock.Bottom
                        Button.isEnabled (not model.IsInProgress)
                        Button.background "Red"
                        Button.onClick (fun _ -> dispatch (Uninstall selectedMod.Id))
                        Button.content "Uninstall"
                    ]

                else
                    Button.create [
                        Button.dock Dock.Bottom
                        Button.background "Gray"
                        Button.isEnabled (not model.IsInProgress)
                        Button.onClick (fun _ -> dispatch (InstallDownload selectedMod.Id))
                        Button.content "Install"
                    ]
            ]
        ]
        

    let makeModCategoriesView model dispatch =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.spacing 32.0
            StackPanel.children [
                
                StackPanel.create [
                    StackPanel.children [
                            
                        TextBlock.create [ TextBlock.text "SEF" ]
                        StackPanel.create [
                            StackPanel.children (
                                getMods SEF model.Mods
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
                                getMods SEF_FR model.Mods
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
                                getMods SEF_BTLA model.Mods
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
                menuBar dispatch
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.children [
                        makeSwatDirectoryChooser model dispatch
                        
                        ScrollViewer.create [
                            ScrollViewer.content (makeModCategoriesView model dispatch)
                            
                        ]
                    ]
                ]
            ]
        ]
