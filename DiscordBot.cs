using System.Text;
using System.Text.Json;
namespace Discord;
public class DiscordBot
{
    public HttpRequestMessage CreateRequest(HttpMethod method, string path)
    {
        HttpRequestMessage request = new(method, baseUrl + path);
        request.Headers.Add("Authorization", "Bot " + botToken);
        return request;
    }
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        HttpResponseMessage httpResponse = await httpClient.SendAsync(request);
        if (!httpResponse.IsSuccessStatusCode)
            throw new HttpBotException(httpResponse);
        return httpResponse;
    }
    public Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path)
    {
        HttpRequestMessage request = CreateRequest(method, path);
        return SendAsync(request);
    }

    public Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path, object content)
    {
        HttpRequestMessage request = CreateRequest(method, path);
        string jsonPayload = JsonSerializer.Serialize(content);
        StringContent stringContent = new(jsonPayload, Encoding.UTF8, "application/json");
        request.Content = stringContent;
        return SendAsync(request);
    }
    public DiscordBot(string botToken)
    => this.botToken = botToken;

    readonly HttpClient httpClient = new();
    readonly string botToken;
    readonly string baseUrl = $"https://discordapp.com/api";
}
