namespace SAL

open SharpCompress.Archives
open SharpCompress.Archives.Zip
open SharpCompress.Archives.Rar
open SharpCompress.Archives.SevenZip
open SharpCompress.Common
module Archive =
    let extractRarArchiveTo archive outputDir =
        using (RarArchive.Open(System.IO.File.OpenRead(archive))) (fun reader ->
            while reader.ExtractAllEntries().MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteToDirectory(outputDir, extractOptions)
                
        )

    let extractSevenZipArchiveTo archive outputDir =
        using (SevenZipArchive.Open(System.IO.File.OpenRead(archive))) (fun reader ->
            while reader.ExtractAllEntries().MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteToDirectory(outputDir, extractOptions)
        )

    let extractZipArchive archive outputDir =
        using (ZipArchive.Open(System.IO.File.OpenRead(archive))) (fun reader ->
            while reader.ExtractAllEntries().MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteToDirectory(outputDir, extractOptions)
        )