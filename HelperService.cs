using Azure.Storage.Blobs;

namespace azure_blob_storage;

internal class HelperService
{
    public async Task UploadFilesAsync(AzureBlobSettings? azureBlobSettings)
    {
        // Prompt user to enter the paths to the files
        List<string> filePaths = new List<string>();
        while (true)
        {
            Console.WriteLine("Enter the path to a file you want to include in the zip (or type 'done' to finish):");
            string input = Console.ReadLine();

            if (input.Equals("done", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (!File.Exists(input))
            {
                Console.WriteLine("File not found. Please make sure the path is correct.");
                continue;
            }

            filePaths.Add(input);
        }

        if (filePaths.Count == 0)
        {
            Console.WriteLine("No files provided. Exiting.");
            return;
        }

        // Create the zip service and generate a zip file
        var zipService = new ZipService();
        string zipFilePath = zipService.CreateZipFile(filePaths);

        // Create the blob upload service
        var blobService = new BlobService(azureBlobSettings);

        // Upload the zip file
        await blobService.UploadFileToBlobAsync(zipFilePath);

        // Clean up
        File.Delete(zipFilePath);
    }

    public async Task DownloadFileAsync(AzureBlobSettings? azureBlobSettings)
    {
        // Prompt for GUID and save path
        Console.WriteLine("Enter the GUID of the file to download:");
        string guid = Console.ReadLine();
        Console.WriteLine("Enter the path where the file should be saved:");

        string savePath = Console.ReadLine();

        EnsureDirectoryExists(savePath);

        if (EnsureFileAccess(savePath))
        {
            if (await DownloadFileFromBlobAsync(guid, savePath, azureBlobSettings))
            {
                Console.WriteLine("File downloaded successfully.");
            }
            else
            {
                Console.WriteLine("Failed to download the file or GUID not found.");
            }
        }
        else
        {
            Console.WriteLine("Access Failed.");
        }
    }

    private async Task<bool> DownloadFileFromBlobAsync(string guid, string savePath, AzureBlobSettings azureBlobSettings)
    {
        // Check if the GUID exists in the CSV file
        var line = File.ReadLines(azureBlobSettings.CsvFilePath)
                       .FirstOrDefault(l => l.StartsWith(guid + ","));

        if (line == null)
        {
            Console.WriteLine("GUID not found in the CSV file.");
            return false;
        }

        // Extract the SAS URI from the CSV line
        var parts = line.Split(',');
        if (parts.Length < 3)
        {
            Console.WriteLine("Invalid CSV format.");
            return false;
        }

        var sasUri = parts[2];

        try
        {
            var blobClient = new BlobClient(new Uri(sasUri));

            Console.WriteLine($"Downloading from Blob storage as blob:\n\t {blobClient.Uri}\n");

            // Download the blob
            var downloadResponse = await blobClient.DownloadAsync();
            using (FileStream fs = File.OpenWrite(savePath))
            {
                await downloadResponse.Value.Content.CopyToAsync(fs);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
            return false;
        }
    }

    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"Directory created: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory: {ex.Message}");
            }
        }
    }

    private bool EnsureFileAccess(string filePath)
    {
        try
        {
            using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                // File access is successful
                Console.WriteLine($"File access successful: {filePath}");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing file: {ex.Message}");
            return false;
        }
    }
}
