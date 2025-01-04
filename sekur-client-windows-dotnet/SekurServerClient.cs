using System.Net.Http.Headers;
using NLog;

namespace sekur_client_windows_dotnet;

public class SekurServerClient
{
    private readonly HttpClient _client;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public SekurServerClient(string sekurServerUrl)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(sekurServerUrl!);
    }

    public async Task SendContent(FileInfo zipFileName, string apiKey)
    {
        MultipartFormDataContent content = PrepareContent(zipFileName.FullName);
        _client.DefaultRequestHeaders.Add("apiKey", apiKey);
        const string path = "archive?device=laptop";
        
        HttpResponseMessage response = await _client.PostAsync(path, content);
        if (!response.IsSuccessStatusCode)
        {
            string ans = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Error, $"Could not send {zipFileName} successfully to sekur-server at '{_client.BaseAddress}/{path}\n{ans}");
            return;
        }

        _logger.Log(LogLevel.Info, $"Successfully sent {zipFileName} to sekur-server");
    }

    private MultipartFormDataContent PrepareContent(string fileName)
    {
        var multipart = new MultipartFormDataContent();
        var streamContent = new StreamContent(new FileStream(fileName, FileMode.Open, FileAccess.Read));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        multipart.Add(streamContent, "file", Path.GetFileName(fileName));

        return multipart;
    }
}