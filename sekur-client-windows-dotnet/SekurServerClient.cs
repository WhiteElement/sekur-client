using System.Net.Http.Headers;

namespace sekur_client_windows_dotnet;

public class SekurServerClient
{
    private readonly HttpClient _client;

    public SekurServerClient(string sekurServerUrl)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(sekurServerUrl!);
    }

    public async Task SendContent(FileInfo zipFileName)
    {
        StreamContent content = PrepareContent(zipFileName.FullName);
        await _client.PostAsync("archive?device=laptop", content);
    }

    private StreamContent PrepareContent(string fileName)
    {
        var multipart = new MultipartFormDataContent();
        var streamContent = new StreamContent(new FileStream(fileName, FileMode.Open, FileAccess.Read));
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        multipart.Add(streamContent, "file");
    
        return streamContent;
    }
}