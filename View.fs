namespace SAL

module View =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open SAL.Data
    open SAL.DomainModel

    let makeModStackView (currentMod: Mod) (model: Model) dispatch =
        WrapPanel.create [
            WrapPanel.children [
                TextBlock.create [ TextBlock.text $"{currentMod.Maintainer}-{currentMod.Version}-{currentMod.Stability.ToString()}" ]
                Button.create [
                    Button.dock Dock.Bottom
                    // FIXME: Find a way to emit this state change.
                    // Button.isEnabled model.IsModRunning
                    Button.background "Green"
                    Button.onClick (fun _ -> dispatch Launch)
                    Button.content "Launch Mod"
                ]                
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Install)
                    Button.content "Install"
                ]

                Button.create [
                    Button.dock Dock.Bottom
                    Button.background "Red"
                    Button.onClick (fun _ -> dispatch Uninstall)
                    Button.content "Uninstall"
                ]
            ]
        ]    

    let getMods category mods =
        mods |> Array.filter (fun m -> m.Category = category)

    let makeModCategoriesView (mods: Mod array) (model: Model) dispatch =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.spacing 32
            StackPanel.children [
                
                StackPanel.create [
                    StackPanel.children [
                        TextBlock.create [ TextBlock.text "SEF" ]
                        StackPanel.create [
                            StackPanel.children (
                                getMods CategoryType.SEF mods
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
                                getMods CategoryType.SEF_FR mods
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
                                getMods CategoryType.SEF_BTLA mods
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
                                    TextBox.minWidth 500
                                    TextBox.onTextChanged (SwatInstallationDirectoryEntryChanged >> dispatch)
                                    TextBox.text model.SwatInstallationDirectory
                                ]
                            ]
                        ]
                        makeModCategoriesView Mods.mods model dispatch
                    ]
                ]
            ]
        ]
