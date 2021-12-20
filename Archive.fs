namespace SAL

open System.IO
open System.IO.Compression

open SharpCompress.Common;
open SharpCompress.Readers;
open SharpCompress.Readers.Rar

open SharpCompress.Archives.SevenZip

open SharpCompress.Archives
open SharpCompress.Archives.Zip
open SharpCompress.Archives.Rar
open SharpCompress.Archives.SevenZip
open SharpCompress.Common

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

    let extractRarArchiveTo archive outputDir =
        use fileReader = File.OpenRead(archive)
        use rarReader = RarReader.Open(fileReader)
        try
            while rarReader.MoveToNextEntry() do
                    let extractOptions = ExtractionOptions()
                    extractOptions.ExtractFullPath <- true
                    rarReader.WriteEntryToDirectory(outputDir, extractOptions)
        finally
            fileReader.Dispose()
            rarReader.Dispose()

    let extractSevenZipArchiveTo archive outputDir =
        use fileReader = File.OpenRead(archive)
        use sevenZipReader = SevenZipArchive.Open(fileReader).ExtractAllEntries()
        
        try
            while sevenZipReader.MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                sevenZipReader.WriteEntryToDirectory(outputDir, extractOptions)
        finally
            fileReader.Dispose()
            sevenZipReader.Dispose()