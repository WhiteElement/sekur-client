using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using NLog;
using NLog.Config;

namespace sekur_client_windows_dotnet;

public class ConfigReader
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _currentDir;

    public ConfigReader(string currentDir)
    {
        _currentDir = currentDir;
        LoadNLogConfiguration();
    }

    public List<FileInfo> ReadFiles()
    {
        return File.ReadAllLines("./files.txt")
            .Where(line => !line.StartsWith("#"))
            .Select(line =>
            {
                _logger.Log(LogLevel.Info, "|> {}", line);
                return new FileInfo(line);
            })
            .ToList();
    }

    public List<DirectoryInfo> ReadDirectories()
    {
        return File.ReadAllLines("./folders.txt")
            .Where(line => !line.StartsWith("#"))
            .Select(line =>
            {
                _logger.Log(LogLevel.Info, "|> {}", line);
                return new DirectoryInfo(line);
            })
            .ToList();
    }

    public string? GetEntry(string entry, bool throwIfMissing)
    {
        XElement configFile = XElement.Load(Path.Combine(_currentDir, "config.xml"));
        string res = configFile.Element("sekur-server-url")?.Value;
        
        if (throwIfMissing && res == null)
            throw new ArgumentException($"config.xml > '{entry}' cannot be null");

        return res;
    }

    private static void LoadNLogConfiguration()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "sekur_client_windows_dotnet.NLog.config"; // Adjust to match your namespace and file name

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
                throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

            using (var reader = XmlReader.Create(stream))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(reader, null);
            }
        }
    }

    public string ExtractArgument(string[] args, string wanted)
    {
        IEnumerable<string> filtered = args
            .Where(x => x.Contains(wanted))
            .ToArray();
            
        if (!filtered.Any())
            throw new ArgumentException("No 'apiKey' provided");
            
        return filtered
            .Single()
            .Replace($"--{wanted}=", String.Empty);
    }
}