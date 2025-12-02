using Newtonsoft.Json;

namespace TodoApi.Models
{
    public class Todo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("userId")]
        public string UserId { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }
    }
}