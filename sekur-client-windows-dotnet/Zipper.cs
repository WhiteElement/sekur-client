using System.IO.Compression;
using NLog;

namespace sekur_client_windows_dotnet;

public class Zipper
{
    private readonly DirectoryInfo _tempDir;
    private readonly string _currentDir;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public Zipper(string currentDir)
    {
        _tempDir = Directory.CreateDirectory(Path.Combine(currentDir, "temp"));
        _currentDir = currentDir;
    }

    public void CopyData(List<FileInfo> files, List<DirectoryInfo> folders)
    {
        files.ForEach(file =>
        {
            if (!File.Exists(file.FullName))
                _logger.Log(LogLevel.Warn, $"Could not copy '{file.FullName} because it doesn't exist'");
            else
                File.Copy(file.FullName, Path.Combine(_tempDir.FullName, file.Name), true);
        });
        folders.ForEach(folder =>
        {
            if (!folder.Exists)
                _logger.Log(LogLevel.Warn, $"Could not copy '{folder.FullName} because it doesn't exist'");
            else
                CopyDirectory(folder.FullName, Path.Combine(_tempDir.FullName, folder.Name), true);
        });
        _logger.Log(LogLevel.Info, "Copied Files and Folders");
    }
    
    private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    public FileInfo Zip()
    {
        var zipFileName = new FileInfo(Path.Combine(_currentDir, $"{DateTime.Now.ToString("d_M_yyyy")}.zip"));
        if (zipFileName.Exists) File.Delete(zipFileName.FullName);

        _logger.Log(LogLevel.Info, $"Zipping to Archive -> {zipFileName}");
        ZipFile.CreateFromDirectory(_tempDir.FullName, zipFileName.FullName, CompressionLevel.SmallestSize, false);

        _logger.Log(LogLevel.Info, "Cleaning up");
        Directory.Delete(_tempDir.FullName, true);
        _logger.Log(LogLevel.Info, "Finished");

        return zipFileName;
    }
}