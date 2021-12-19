namespace SAL

module View =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open SAL.DomainModel

    let makeModStackView (selectedMod: Mod) dispatch =
        WrapPanel.create [
            WrapPanel.children [
                TextBlock.create [ TextBlock.text $"{selectedMod.Maintainer}-{selectedMod.Version}-{selectedMod.Stability.ToString()}" ]
                
                if selectedMod.IsInstalled then
                    Button.create [
                        Button.dock Dock.Bottom
                        // FIXME: Find a way to emit this state change.
                        // Button.isEnabled model.IsModRunning
                        Button.background "Green"
                        Button.onClick (fun _ -> dispatch (Launch selectedMod.Id))
                        Button.content "Launch Mod"
                    ]                

                    Button.create [
                        Button.dock Dock.Bottom
                        Button.background "Red"
                        Button.onClick (fun _ -> dispatch (Uninstall selectedMod.Id))
                        Button.content "Uninstall"
                    ]

                else
                    Button.create [
                        Button.dock Dock.Bottom
                        Button.onClick (fun _ -> dispatch (Install selectedMod.Id))
                        Button.content "Install"
                    ]
            ]
        ]    

    let getMods category mods =
        mods |> Array.filter (fun m -> m.Category = category)

    let makeModCategoriesView (model: Model) dispatch =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.spacing 32.0
            StackPanel.children [
                
                StackPanel.create [
                    StackPanel.children [
                        
                        ProgressBar.create [
                            ProgressBar.orientation Orientation.Horizontal
                            ProgressBar.height 20.0
                            ProgressBar.value 50.0
                        ]
                        
                        if model.Loading then
                            TextBlock.create [ TextBlock.text "LOADING...." ]
                        else
                            TextBlock.create [ TextBlock.text model.Status ]
                            
                        Button.create [
                            Button.isEnabled (not model.Loading)
                            Button.onClick (fun _ -> dispatch StatusDelayed)
                            Button.content "Delayed Status"
                        ]
                            
                        TextBlock.create [ TextBlock.text "SEF" ]
                        StackPanel.create [
                            StackPanel.children (
                                getMods SEF model.Mods
                                |> Array.toList
                                |> List.map  (fun m -> makeModStackView m dispatch)
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
                                |> List.map  (fun m -> makeModStackView m dispatch)
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
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.children [

                        StackPanel.create [
                            StackPanel.verticalAlignment VerticalAlignment.Top
                            StackPanel.horizontalAlignment HorizontalAlignment.Center
                            StackPanel.spacing 8.0
                            StackPanel.margin 8.0
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.fontSize 15.0
                                    TextBlock.text "SWAT4 Folder: "
                                ]

                                TextBox.create [
                                    TextBox.minWidth 500.0
                                    // TODO: Don't update like this. Useless amount of computation
                                    TextBox.onTextChanged (SwatDirectoryEntryChanged >> dispatch)
                                    TextBox.text model.SwatDirectory
                                ]
                            ]
                        ]
                        makeModCategoriesView model dispatch
                    ]
                ]
            ]
        ]
