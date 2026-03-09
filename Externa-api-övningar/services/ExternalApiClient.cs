using Externa_api_övningar.Models; // för att kunna använda Post modellen
using System.Text.Json; // för att konvertera json -> objekt

// bra för del 1
// hämta 10 post + felhantering 

namespace Externa_api_övningar.Services; // ändrat till stort S för att följa C# standard

public class ExternalApiClient
{
    // den här klassen ska vara som en "liten robot som bara kan hämta posts från api"

    // för att prata med api behöver man HttpClient
    private readonly HttpClient _httpClient;

    // constructor som tar emot HttpClient från Dependency Injection
    public ExternalApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // metoden gör så att jag hämtar count antal posts och ger mig List<Post>
    public async Task<List<Post>> GetPostsAsync(int count)
    {
        // bygger url
        var url = $"posts?_limit={count}";

        // skickar GET-request till externa api:t
        using var response = await _httpClient.GetAsync(url);

        // felhantering / statuskod
        if (!response.IsSuccessStatusCode)
        {
            // om det inte är statuskod 200-299 så kastar jag ett undantag
            throw new Exception($"Failed to fetch posts. Status code: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        // 4. läs body (json) från response
        var json = await response.Content.ReadAsStringAsync();

        // konvertera JSON -> List<Post>
        var posts = JsonSerializer.Deserialize<List<Post>>(json, new JsonSerializerOptions
        {
            // gör så att det inte spelar roll om JSON har små eller stora bokstäver
            // t.ex userId -> UserId
            PropertyNameCaseInsensitive = true
        });

        // om deserialiseringen misslyckas returnerar jag en tom lista istället för null
        return posts ?? new List<Post>();
    }
}