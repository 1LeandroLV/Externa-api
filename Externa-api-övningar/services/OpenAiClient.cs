using System.Net.Http.Headers; //behövs för Authorization;
using System.Text; //behövs för att bygga text
using System.Text.Json; //behövs för att skapa och läsa json
using Externa_api_övningar.Models; //behövs för att använda Post



//bra för del 2
// skicka text till open al + får json tillbacka
namespace Externa_api_övningar.services;

public class OpenAiClient //prata med openAi
{
    private readonly HttpClient _httpClient;
    public OpenAiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    //skapa metoden
    public async Task<string> AnalyzePostAsync(List<Post> Post)
    {
        //hämta api nyckel från datorn 
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey)) //kollar om myckeln är tom 
        {

            throw new Exception("OPENAI API saknas");
        }
        //detta skicka denna header till openal
        _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", apiKey);

        // 1 ta emot posts
        // 2läsa api key
        // 3 förnerda request 
    }

}