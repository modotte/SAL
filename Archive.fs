namespace SAL

open System.IO
open System.IO.Compression

open SharpCompress.Common;
open SharpCompress.Readers;
open SharpCompress.Readers.Rar

module Archive =
    let extractZipArchiveTo archive outputDir =
        ZipFile.ExtractToDirectory(archive, outputDir)
       
 
    // BUG: Doesn't work yet due unable to dispose opened
    // archive file
    let extractRarArchiveTo archive outputDir =
        use reader = RarReader.Open(File.OpenRead(archive))
        while reader.MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                extractOptions.ExtractFullPath <- true
                reader.WriteEntryToDirectory(outputDir, extractOptions)