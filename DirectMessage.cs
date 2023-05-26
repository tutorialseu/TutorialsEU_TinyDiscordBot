using System.Text.Json;
namespace Discord;
public class DirectMessage
{
    readonly DiscordBot bot;
    public DirectMessage(DiscordBot bot)
    => this.bot = bot;

    public async Task Send(string recipientId, string message)
    {
        string dmId = await InitDm(recipientId);
        await SendDm(dmId, message);
    }

    private Task SendDm(string dmId, string message)
    => bot.RequestAsync(HttpMethod.Post, $"/channels/{dmId}/messages"
                        , new
                        {
                            content = message
                        });

    private async Task<string> InitDm(string recipientId)
    {
        var httpResponse = await bot.RequestAsync(HttpMethod.Post, "/users/@me/channels"
                            , new { recipient_id = recipientId });
        string response = await httpResponse.Content.ReadAsStringAsync();
        JsonDocument document = JsonDocument.Parse(response);
        return document.RootElement.GetProperty("id").GetString() ?? "";
    }
}