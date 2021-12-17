namespace SAL

open SharpCompress.Archives
open SharpCompress.Archives.Rar
open SharpCompress.Common
open SharpCompress.Readers
module Archive =
    let extractRarArchiveTo archive outputDir =
        using (RarArchive.Open(System.IO.File.OpenRead(archive))) (fun reader ->
            while reader.ExtractAllEntries().MoveToNextEntry() do
                let extractOptions = ExtractionOptions()
                reader.WriteToDirectory(outputDir, extractOptions)
                
        )