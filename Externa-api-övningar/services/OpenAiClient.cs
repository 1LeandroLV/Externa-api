using System.Net.Http.Headers; // behövs för Authorization
using System.Text; // behövs för att bygga text
using System.Text.Json; // behövs för att skapa och läsa json
using Externa_api_övningar.Models; // behövs för att använda Post modellen

// bra för del 2
// skicka text till OpenAI + få json tillbaka
namespace Externa_api_övningar.Services;

public class OpenAiClient // pratar med OpenAI
{
    private readonly HttpClient _httpClient;

    public OpenAiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // metoden skickar posts till OpenAI och får tillbaka analys
    public async Task<string> AnalyzePostsAsync(List<Post> posts)
    {
        // hämta api-nyckel från datorn
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        // kollar om nyckeln är tom
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new Exception("OPENAI API saknas");
        }

        // detta skickar Authorization header till OpenAI
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        // ta min lista med posts och gör om den till vanlig text
        var inputText = BuildInputText(posts);

        // skapa ett C# objekt som senare blir JSON
        var requestBody = new
        {
            model = "gpt-5-mini",
            input = inputText,

            // här bestämmer vi hur svaret från AI ska se ut
            text = new
            {
                format = new
                {
                    type = "json_schema", // svaret ska följa ett JSON schema
                    name = "post_analysis",
                    strict = true, // AI ska försöka följa schemat exakt

                    // här beskriver vi hur JSON svaret ska se ut
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,

                        properties = new
                        {
                            items = new
                            {
                                type = "array",

                                items = new
                                {
                                    type = "object",
                                    additionalProperties = false,

                                    properties = new
                                    {
                                        id = new { type = "integer" },

                                        summary = new { type = "string" },

                                        tags = new
                                        {
                                            type = "array",
                                            items = new { type = "string" }
                                        },

                                        risk_level = new
                                        {
                                            type = "string",
                                            @enum = new[] { "low", "medium", "high" }
                                        }
                                    },

                                    required = new[] { "id", "summary", "tags", "risk_level" }
                                }
                            }
                        },

                        required = new[] { "items" }
                    }
                }
            }
        };

        // gör om requestBody objektet till JSON
        var json = JsonSerializer.Serialize(requestBody);

        // skapa http content
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // skicka POST request till OpenAI
        var response = await _httpClient.PostAsync("responses", content);

        // om OpenAI returnerar fel
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI API error: {response.StatusCode} {error}");
        }

        // läs hela svaret från OpenAI
        var responseJson = await response.Content.ReadAsStringAsync();

        // navigera i JSON svaret
        using var doc = JsonDocument.Parse(responseJson);

        var outputText = doc.RootElement
            .GetProperty("output")[0]
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();

        // returnera AI svaret
        return outputText ?? "{}";
    }

    // hjälpmetod som gör om posts till text som AI kan förstå
    private string BuildInputText(List<Post> posts)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Analysera följande inlägg:");
        sb.AppendLine("Returnera för varje post: id, summary, tags och risk_level.");
        sb.AppendLine("Risk_level ska vara low, medium eller high.");
        sb.AppendLine();

        // här går vi igenom varje post
        foreach (var post in posts)
        {
            sb.AppendLine($"ID: {post.Id}");
            sb.AppendLine($"Title: {post.Title}");
            sb.AppendLine($"Body: {post.Body}");
            sb.AppendLine();
        }

        // returnera texten
        return sb.ToString();
    }
}