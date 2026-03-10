namespace Externa_api_övningar.model
{
    public class AipostResult
    { 
        public int Id { get; set; }
        public string summary { get; set; }
        public List<string> Tags { get; set; } = new();
         public string Risk_Level { get; set; }
    }
}
