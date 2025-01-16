namespace sekur_client_windows_dotnet;

public class Program
{
    public static async Task Main(string[] args)
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        ConfigReader configReader = new ConfigReader(currentDir);
        string apiKey = configReader.ExtractArgument(args, "apiKey");
        
        List<FileInfo> files = configReader.ReadFiles();
        List<DirectoryInfo> folders = configReader.ReadDirectories();

        Zipper zipper = new Zipper(currentDir);
        zipper.CopyData(files, folders);
        FileInfo zipFileName = zipper.Zip();

        string? sekurServerUrl = configReader.GetEntry("sekur-server-url", true);
        SekurServerClient sekurServerClient = new SekurServerClient(sekurServerUrl!);
        await sekurServerClient.SendContent(zipFileName, apiKey);
    }
}