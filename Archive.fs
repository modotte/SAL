namespace SAL

open SharpCompress.Common;
open SharpCompress.Readers;
open SharpCompress.Readers.Zip;
open SharpCompress.Readers.Rar;
module Archive =
    let extractZipArchiveTo archive outputDir =
        using (ZipReader.Open(System.IO.File.OpenRead(archive))) (fun reader ->
            while reader.MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteEntryToDirectory(outputDir, extractOptions)
                
        )

    let extractRarArchiveTo archive outputDir =
        using (RarReader.Open(System.IO.File.OpenRead(archive))) (fun reader ->
            while reader.MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteEntryToDirectory(outputDir, extractOptions)   
        )