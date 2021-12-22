namespace SAL

module View =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.FuncUI.Components

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

                MenuItem.create [
                    MenuItem.header "Help"
                    MenuItem.viewItems [
                        MenuItem.create [
                            MenuItem.header "About SAL"
                        ]
                        MenuItem.create [
                            MenuItem.header "Report bugs or suggest feedbacks"
                        ]
                        MenuItem.create [
                            MenuItem.header "Visit source code repository"
                        ]
                    ]
                ]
            ]
        ]

    let getMods category mods =
        mods |> Array.filter (fun m -> m.Category = category)
        
    let makeModsChooser model dispatch =
        ComboBox.create [
            ComboBox.isEnabled (not model.IsInProgress)
            ComboBox.minWidth 200.0
            ComboBox.dataItems model.Mods
            ComboBox.selectedItem model.Mods.[model.SelectedMod]
            ComboBox.onSelectedItemChanged (
                fun i ->
                    if i <> null then
                        let m = i |> unbox<Mod>                        
                        dispatch (SelectMod m.Id)
            )
            ComboBox.itemTemplate (
                DataTemplateView<Mod>.create(
                    fun m -> TextBlock.create [ 
                        let installedText = if m.IsInstalled then "[INSTALLED]" else ""
                        TextBlock.text $"{m.Maintainer}-{m.Version}-{m.Stability.ToString()} {installedText}" 
                    ]
                )
            )
        ]

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
                    ]
                ]

                StackPanel.create [
                    StackPanel.dock Dock.Bottom
                    StackPanel.verticalAlignment VerticalAlignment.Top
                    StackPanel.children [
                        makeModsChooser model dispatch

                        let selectedMod = 
                            model.Mods
                            |> Array.filter (fun m -> m.Id = model.SelectedMod)
                            |> Array.head

                        // BUG: Id is not synchronized on selection
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
                                Button.isEnabled (not model.IsInProgress)
                                Button.onClick (fun _ -> dispatch (InstallDownload selectedMod.Id))
                                Button.content ("Install " + selectedMod.Category.ToString() + selectedMod.Version.ToString())
                        ]

                    ]
                ]
            ]
        ]
