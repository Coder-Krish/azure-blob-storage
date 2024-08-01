using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace azure_blob_storage;

internal class BlobService
{
    private readonly AzureBlobSettings _settings;

    public BlobService(AzureBlobSettings settings)
    {
        _settings = settings;
    }

    public async Task UploadFileToBlobAsync(string filePath)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

        // Create the container if it does not exist
        await containerClient.CreateIfNotExistsAsync();

        // Get a reference to a blob
        string blobName = Path.GetFileName(filePath);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Console.WriteLine($"Uploading to Blob storage as blob:\n\t {blobClient.Uri}\n");

        try
        {
            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();

            Console.WriteLine($"Upload of {blobName} complete.");

            // Generate SAS URI for the uploaded blob
            var sasBuilder = new Azure.Storage.Sas.BlobSasBuilder
            {
                BlobContainerName = _settings.ContainerName,
                BlobName = blobName,
                Resource = "b", // "b" for blob
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(_settings.DownloadURIValidUptoInHours)
            };
            //sets permission to read.
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate the SAS token
            var credentials = new Azure.Storage.StorageSharedKeyCredential(
                GetStorageAccountName(_settings.ConnectionString),
                GetStorageAccountKey(_settings.ConnectionString)
            );

            string sasToken = sasBuilder.ToSasQueryParameters(credentials).ToString();
            string sasUri = $"{blobClient.Uri}?{sasToken}";

            // Generate a unique identifier
            //string uniqueId = Guid.NewGuid().ToString();

            // Write the metadata to a CSV file
            string csvFilePath = _settings.CsvFilePath;
            //using (StreamWriter sw = new StreamWriter(csvFilePath, true))
            //{
            //    sw.WriteLine($"{uniqueId},{blobName},{sasUri}");
            //}

            // Check if the file exists and write headers if it does not
            bool fileExists = File.Exists(csvFilePath);
            using (StreamWriter sw = new StreamWriter(csvFilePath, true))
            {
                // Write header if the file does not exist
                if (!fileExists)
                {
                    sw.WriteLine("GUID,FileName,SASUri");
                }

                // Write new data
                sw.WriteLine($"{Guid.NewGuid()},{blobName},{sasUri}");
            }

            Console.WriteLine("Metadata stored in CSV file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }

    private string GetStorageAccountName(string connectionString)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var uri = blobServiceClient.Uri;
        var accountName = new Uri(uri.AbsoluteUri).Host.Split('.')[0];
        return accountName;
    }

    private string GetStorageAccountKey(string connectionString)
    {
        var connStringParts = connectionString.Split(';');
        foreach (var part in connStringParts)
        {
            if (part.StartsWith("AccountKey="))
            {
                return part.Substring("AccountKey=".Length);
            }
        }
        throw new InvalidOperationException("AccountKey not found in connection string.");
    }
}
