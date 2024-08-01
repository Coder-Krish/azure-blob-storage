namespace azure_blob_storage;

internal class AzureBlobSettings
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
    public string CsvFilePath { get; set; }
    public int DownloadURIValidUptoInHours { get; set; }
}
