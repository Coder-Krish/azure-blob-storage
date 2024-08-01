using System.IO.Compression;

namespace azure_blob_storage;

internal class ZipService
{
    public string CreateZipFile(List<string> filePaths)
    {
        string zipFilePath = Path.Combine(Path.GetTempPath(), $"upload_{Guid.NewGuid()}.zip");

        using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
        {
            foreach (var filePath in filePaths)
            {
                zipArchive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
            }
        }

        return zipFilePath;
    }
}
