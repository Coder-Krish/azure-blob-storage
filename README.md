# File Compression and Azure Blob Storage Console Application

## Overview

This console application allows users to compress multiple files into a single zip file and upload it to Azure Blob Storage. Additionally, it supports downloading files from Azure Blob Storage using a GUID stored in a CSV file. The application handles basic file operations and provides necessary feedback and instructions for users.

## Features

- **Upload Files**: Compress multiple files into a zip file and upload it to Azure Blob Storage.
- **Download Files**: Download files from Azure Blob Storage using a GUID found in a CSV file.
- **Automatic Directory Management**: Creates necessary directories in the assembly directory for downloading files.

## Prerequisites

- .NET 6.0 or later
- Azure Blob Storage account
- Valid connection string and storage account details
- CSV file for GUID-to-SAS URI mapping

## Configuration

The application uses `appsettings.json` for configuration. Update this file with your Azure Blob Storage details:

```json
{
  "AzureBlobStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=yourkey;TableEndpoint=yourendpoint;",
    "ContainerName": "my-container",
    "CsvFilePath": "D:/AzureBlobStorage/uploaded-blobs/file.csv",
    "DownloadURIValidUptoInHours": 1
  }
}
```

## Usage
### Upload Files
1. Run the application.
2. When prompted, enter the paths to the files you want to include in the zip. Type 'done' when finished.
3. The application will create a zip file, upload it to Azure Blob Storage, and clean up the local zip file.

### Download Files
1. Run the application.
2. Enter 'download' when prompted.
3. Provide the GUID associated with the file you want to download.
4. Provide the path where you want it to download.
5. The application will download the file to the specified path.

## Running the Application
1. Build the Application: Use Visual Studio or the .NET CLI to build the application.
2. Run with Elevated Permissions: Ensure the application has the necessary permissions to create directories and write files. Run the application as an administrator if required.

```bash
dotnet run
```

## Troubleshooting
1. Access Denied Errors: Ensure you have the necessary permissions to access the directory and files. Run the application with administrative privileges if needed.
2. File Not Found: Verify the file paths and ensure they are correct.
3. Invalid GUID: Check that the GUID provided is present in the CSV file and correctly maps to a valid SAS URI.