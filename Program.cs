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

        static string PromptForRemotePath()
        {
            Console.Write("Enter your Rock Band 3 installation path: ");
            string path = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(path) ? PromptForRemotePath() : path;
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
                try
                {
                    Console.WriteLine($"Downloading {remoteFilePath} to {localFilePath}...");
                    byte[] fileBytes = xbox.DownloadFile(remoteFilePath + "discordrp.json");
                    File.WriteAllBytes(localFilePath, fileBytes);
                    Console.WriteLine("Download completed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during download: {ex.Message}");
                }

                await Task.Delay(5000); // Wait for 5 seconds before the next download
            }
        }

        static async Task Main(string[] args)
        {
            string ipAddress = PromptForIp();

            IXbox xbox = new Xbox360(ipAddress);

            string remoteFilePath = PromptForRemotePath();

            await AutomaticDownload(xbox, remoteFilePath); 
            Console.WriteLine("Neighborhood3DXPresence is running... press Enter to end...");
            Console.ReadLine(); 
        }

    }
}
