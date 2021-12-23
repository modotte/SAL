namespace SAL

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout

open SAL.Domain

[<RequireQualifiedAccess>]
module Utility =
    let simpleTextBlock text = TextBlock.create [ TextBlock.text text ]

module View =
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
                Utility.simpleTextBlock $"{selectedMod.Maintainer}-{selectedMod.Version}-{selectedMod.Stability.ToString()} {isInstalledText}"
                
                match selectedMod.Description with
                | None -> ()
                | Some desc ->
                    Expander.create [
                        Expander.header "More Info"
                        Expander.content (StackPanel.create [ StackPanel.children [ Utility.simpleTextBlock desc ] ])
                    ]

                
                if selectedMod.IsInstalled then
                    Button.create [
                        Button.dock Dock.Bottom
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
                
                let showMods category =
                    getMods category model.Mods
                    |> Array.map (fun m -> makeModStackView m model dispatch :> Avalonia.FuncUI.Types.IView)
                    |> Array.toList

                StackPanel.create [
                    StackPanel.children [
                        Utility.simpleTextBlock "SEF"
                        StackPanel.create [
                            StackPanel.children (showMods SEF)
                        ]
                    ]
                ]
                

                StackPanel.create [
                    StackPanel.children [
                        Utility.simpleTextBlock "SEF - First Responders"
                        StackPanel.create [
                            StackPanel.children (showMods SEF_FR)
                        ]
                    ]
                ]

                StackPanel.create [
                    StackPanel.children [
                        Utility.simpleTextBlock "SEF - Back To Los Angeles"
                        StackPanel.create [
                            StackPanel.children (showMods SEF_BTLA)
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
                        makeModCategoriesView model dispatch
                    ]
                ]
            ]
        ]
