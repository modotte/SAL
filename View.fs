namespace SAL

open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.DSL

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
                        ScrollViewer.create [
                            ScrollViewer.content [
                                
                            ]
                        ]
                    ]
                ]
            ]
        ]
