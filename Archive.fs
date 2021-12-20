namespace SAL

open System.IO
open System.IO.Compression

open SharpCompress.Common;
open SharpCompress.Readers;
open SharpCompress.Readers.Rar

module Archive =
    let extractZipArchiveTo archive outputDir =
        use reader = ZipFile.Open(archive, ZipArchiveMode.Read)
        try
            try
                reader.ExtractToDirectory(outputDir)
            with
            | :? IOException as exn ->
                Logger.log.Error(exn.Message)
        finally
            reader.Dispose()

 
    // BUG: Doesn't work yet due unable to dispose opened
    // archive file
    let extractRarArchiveTo archive outputDir =
        use reader = RarReader.Open(File.OpenRead(archive))
        while reader.MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteEntryToDirectory(outputDir, extractOptions)