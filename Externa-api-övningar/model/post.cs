//måste, för desrialisering

//tumregel när man jobbar med api 
//1, titta på json 
//2skapa model som matchar json
//3deserialize json model

namespace Externa_api_övningar.model
{
    public class post
    {
        public int UserId { get; set; }
        public int Id { get; set; }
       public string Title { get; set; }

        public string body { get; set; }

    }
}
