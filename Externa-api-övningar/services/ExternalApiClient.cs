using Externa_api_övningar.Models; //för att kunna använda Post modellen
using System.Text.Json; //för at kovertera json ->  object

//bra för del 1
//hämta 10 post + felhantering 

namespace Externa_api_övningar.services
{ 
    public class ExternalApiClient
    {
    //den här klassen ska vara som  en "liten robot som bara kan hämta posts från api

    //för att prata med api behöver man httpclient
    private readonly HttpClient _httpClient;
    public ExternalApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
        // metoden gör så att jag hämta count antal pots och ge mig List<Post> 
        public async Task<List<Post>> GetPostsAsync(int count)
        {
            //byggger url 
            var url = $"posts?_limit={count}";

            //skicka get-request
            using var response = await _httpClient.GetAsync(url);

            //felhantering /statuskod)
            if (!response.IsSuccessStatusCode)
            {
                //om det inte är 200-299 så kastar jag ett undantag
                throw new Exception($"Failed to fetch posts. Status code: {response.StatusCode}");
            }
            // 4 läs body (json) 
            var json = await response.Content.ReadAsStringAsync();

            var posts = JsonSerializer.Deserialize<List<Post>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // gör så att det inte spelar roll om det är stora eller små bokstäver i json
            });
            return posts ?? new List<Post>(); //om deserialiseringen misslyckas så returnerar jag en tom lista istället för null
        }
    }
}
