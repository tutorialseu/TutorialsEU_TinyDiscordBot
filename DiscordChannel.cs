using System.Text.Json;
namespace Discord;
public class DiscordChannel
{
    readonly DiscordBot bot;
    readonly string resourceAddress;
    public DiscordChannel(DiscordBot bot, string channelId)
    {
        resourceAddress = $"/channels/{channelId}/";
        this.bot = bot;
    }
    public async Task<IEnumerable<UserMessage>> ReadMessages()
    {
        HttpResponseMessage response = await bot.RequestAsync(HttpMethod.Get, resourceAddress + "messages");
        return await ReadUserMessagesFrom(response.Content);
    }

    public async Task<string> SendMessage(string text)
    {
        var message = new { content = text };
        HttpResponseMessage httpResponse = await bot.RequestAsync(HttpMethod.Post, resourceAddress + "messages", message);
        string response = await httpResponse.Content.ReadAsStringAsync();
        JsonDocument document = JsonDocument.Parse(response);
        JsonElement root = document.RootElement;
        string? messageId = root.GetProperty("id").GetString();
        return messageId ?? throw new Exception("No message ids returned from the server!");
    }

    private static async Task<IEnumerable<UserMessage>> ReadUserMessagesFrom(HttpContent response)
    {
        string responseJson = await response.ReadAsStringAsync();
        JsonDocument document = JsonDocument.Parse(responseJson);
        JsonElement root = document.RootElement;
        IEnumerable<UserMessage> userMessages = root.EnumerateArray().Select(message =>
        new UserMessage
        (
            MessageId: message.GetProperty("id").GetString(),
            UserId: message.GetProperty("author").GetProperty("id").GetString(),
            Content: message.GetProperty("content").GetString()
        ));
        return userMessages;
    }

    public async Task<float> DeleteMessage(string messageId)
    {
        HttpResponseMessage response = await bot.RequestAsync(HttpMethod.Delete, resourceAddress + $"messages/{messageId}");
        string json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(json))
        {
            return 0;
        }
        JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;
        JsonElement retryAfterElement = root.GetProperty("retry_after");
        string? retryAfter = retryAfterElement.ValueKind == JsonValueKind.Null ?
            null : retryAfterElement.GetString();
        return float.Parse(retryAfter ?? "0");
    }
}