using azure_blob_storage;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;

class Program
{
    static bool IsRunningAsAdministrator()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
    static async Task Main(string[] args)
    {
        //if (!IsRunningAsAdministrator())
        //{
        //    Console.WriteLine("This application requires administrative privileges to run properly.");
        //    return;
        //}
        // Build configuration
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var azureBlobSettings = config.GetSection("AzureBlobStorage").Get<AzureBlobSettings>();


        // Prompt the user to choose an action
        Console.WriteLine("1. Upload files");
        Console.WriteLine("2. Download a file");
        Console.WriteLine("Choose an action:");

        var choice = Console.ReadLine();
        HelperService helper = new HelperService(); 

        switch (choice)
        {
            case "1":
                await helper.UploadFilesAsync(azureBlobSettings);
                break;

            case "2":
                await helper.DownloadFileAsync(azureBlobSettings);
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }

        Console.ReadLine();
    }

    
}