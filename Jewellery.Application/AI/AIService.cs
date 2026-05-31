using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class AIService
{
    private readonly IConfiguration _config;

    public AIService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> GetChatResponse(string message)
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);

        var body = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "You are a jewellery shop assistant. Help users choose jewellery." },
                new { role = "user", content = message }
            }
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            content
        );

        var json = await response.Content.ReadAsStringAsync();

        dynamic result = JsonConvert.DeserializeObject(json);

        return result.choices[0].message.content;
    }
    public class ChatRequest
    {
        public string Message { get; set; }
    }
}