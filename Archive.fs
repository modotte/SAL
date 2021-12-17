namespace SAL

open System.IO
open SharpCompress.Common;
open SharpCompress.Readers;
open SharpCompress.Readers.Zip;
open SharpCompress.Readers.Rar;

open Logger

module Archive =
    let extractZipArchiveTo archive outputDir =
        use reader = RarReader.Open(File.OpenRead(archive))
        while reader.MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteEntryToDirectory(outputDir, extractOptions)
 
    let extractRarArchiveTo archive outputDir =
        use reader = RarReader.Open(File.OpenRead(archive))
        while reader.MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteEntryToDirectory(outputDir, extractOptions)