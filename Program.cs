using NeighborSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Neighborhood3DXPresence
{
    internal class Program
    {
        static string PromptForIp()
        {
            Console.Write("Enter the IP Address of your Xbox 360: ");
            string ip = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(ip) ? PromptForIp() : ip;
        }

static async Task AutomaticDownload(IXbox xbox, string remoteFilePath)
{
    string localDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dx360_shim", "dev_hdd0", "game", "BLUS30463", "USRDIR");
    string localFilePath = Path.Combine(localDirectory, "discordrp.json");

    if (!Directory.Exists(localDirectory))
    {
        Directory.CreateDirectory(localDirectory);
        Console.WriteLine($"Created directory: {localDirectory}");
    }

    while (true)
    {
        // Start a new download task
        _ = DownloadFileAsync(xbox, remoteFilePath, localFilePath);

        // Wait before starting the next download attempt
        await Task.Delay(5000);
    }
}

static async Task DownloadFileAsync(IXbox xbox, string remoteFilePath, string localFilePath)
{
    using (var cts = new CancellationTokenSource())
    {
        cts.CancelAfter(TimeSpan.FromSeconds(5.5)); // Set the timeout

        try
        {
            Console.WriteLine($"Downloading {remoteFilePath} to {localFilePath}...");

            var downloadTask = Task.Run(() => xbox.DownloadFile(remoteFilePath), cts.Token);
            byte[] fileBytes = await downloadTask;

            // Write the downloaded bytes to a file
            await File.WriteAllBytesAsync(localFilePath, fileBytes);
            Console.WriteLine("Download completed successfully.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Download timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during download: {ex.Message}\n{ex.StackTrace}");
        }
    }
}

        static async Task Main(string[] args)
        {
            string ipAddress = PromptForIp();

            IXbox xbox = new Xbox360(ipAddress);

            string remoteFilePath = "GAME:\\discordrp.json";

            await AutomaticDownload(xbox, remoteFilePath);
            Console.WriteLine("Neighborhood3DXPresence is running... press Enter to end...");
            Console.ReadLine();
        }

    }
}
