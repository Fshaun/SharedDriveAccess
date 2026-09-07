using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string networkDrivePath = @"/app/network";
        string localDrivePath = @"/app/data";

        try
        {
            Console.WriteLine($"Checking network drive at: {networkDrivePath}");
            if (!Directory.Exists(networkDrivePath))
            {
                Console.WriteLine("Network drive not found.");
                return;
            }

            string[] files = Directory.GetFiles(networkDrivePath);

            if (files.Length == 0)
            {
                Console.WriteLine("No files found in the network drive.");
                return;
            }

            Console.WriteLine("Found files:");
            foreach (var file in files)
            {
                Console.WriteLine(Path.GetFileName(file));
            }

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(localDrivePath, fileName);

                File.Copy(file, destinationPath, overwrite: true);
                Console.WriteLine($"Copied: {fileName} to {localDrivePath}");
            }

            Console.WriteLine("All files copied successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
