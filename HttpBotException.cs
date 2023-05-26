public class HttpBotException : Exception
{
    public HttpBotException(HttpResponseMessage response)
    => Response = response.Content.ReadAsStringAsync().Result;

    public string Response { get; }
}