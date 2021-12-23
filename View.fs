namespace SAL

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.FuncUI.Components
open Avalonia.Media

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
                let category = 
                    match selectedMod.Category with
                    | SEF -> "SEF"
                    | SEF_FR -> "SEF - First Responders"
                    | SEF_BTLA -> "SEF - Back To Los Angeles"

                let isInstalledText = if selectedMod.IsInstalled then "[INSTALLED]" else ""
                TextBlock.create [ 
                    TextBlock.fontSize 16.0
                    TextBlock.fontWeight FontWeight.ExtraBold
                    TextBlock.text category
                ]
                TextBlock.create [
                    TextBlock.text $": {selectedMod.Maintainer}-{selectedMod.Version}-{selectedMod.Stability.ToString()} {isInstalledText}"
                ]

                
                if selectedMod.IsInstalled then
                    Button.create [
                        Button.dock Dock.Bottom
                        Button.isEnabled (not model.IsInProgress)
                        Button.background "Light Green"
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
        

    let modItemTemplate selectedMod model dispatch =
        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.children [ makeModStackView selectedMod model dispatch ]
        ]
    let makeModCategoriesView model dispatch =
        ListBox.create [
            ListBox.verticalAlignment VerticalAlignment.Top
            ListBox.width 800.0
            ListBox.minWidth 400.0
            ListBox.horizontalAlignment HorizontalAlignment.Center
            ListBox.dataItems model.Mods
            ListBox.itemTemplate (DataTemplateView<Mod>.create(fun m-> modItemTemplate m model dispatch))
        ]

    let makeProgressBarIndicator model =
        if model.IsInProgress then
            StackPanel.create [
                StackPanel.children [
                    match model.ProgressStatus with
                    | Some status -> Utility.simpleTextBlock status
                    | _ -> ()
                    ProgressBar.create [ ProgressBar.isIndeterminate true ]
                ]]
        else
            StackPanel.create []

    let makePopup prefixMessage (borderBackgroundColor: string) closeMessage model dispatch =
        Border.create [
            Border.background borderBackgroundColor
            Border.child (
                StackPanel.create [
                    StackPanel.margin 32.0
                    StackPanel.spacing 32.0
                    StackPanel.children [
                        Utility.simpleTextBlock 
                            $"{prefixMessage} {model.ProgressCompletedStatus.Value}"
                        Button.create [
                            Button.onClick (fun _ -> dispatch closeMessage)
                            Button.content "Ok"
                        ]
                    ]
                ]
            )
        ]

    let rootScreen model dispatch = 
        StackPanel.create [
            
            match model.CurrentScreen with
            | Primary ->
                StackPanel.verticalAlignment VerticalAlignment.Top
                StackPanel.spacing 8.0
                StackPanel.margin 8.0
            | _ -> 
                StackPanel.verticalAlignment VerticalAlignment.Center

            StackPanel.children [

                match model.CurrentScreen with
                | Primary ->
                    menuBar dispatch
                    makeSwatDirectoryChooser model dispatch
                    makeProgressBarIndicator model
                    makeModCategoriesView model dispatch
                | InfoPopup ->
                    makePopup "Info: " "Black" CloseInfoPopup model dispatch
                | ErrorPopup ->
                    makePopup "Error occured. Reason: " "Red" CloseErrorPopup model dispatch
            ]

        ]
    
    let view (model: Model) dispatch =
        rootScreen model dispatch
