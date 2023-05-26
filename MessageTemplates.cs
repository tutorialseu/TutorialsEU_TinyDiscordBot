static class MessageTemplates
{
    public static string ProductList(Dictionary<string, string> products)
    {
        IEnumerable<string> productNames = products.Select((product, i) => $"{i + 1}. {product.Key}");
        string strProductNames = string.Join("\n", productNames);
        string template = $"Select a product by it's id from the following list:\n{strProductNames}";
        return template;
    }

    public static string PriceReport(string messageContent, string price)
    => $"{messageContent} costs {price}";
}