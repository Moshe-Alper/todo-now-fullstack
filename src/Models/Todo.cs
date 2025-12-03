using Newtonsoft.Json;

namespace TodoApi.Models
{
    public class Todo
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;
        
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }
        
        [JsonProperty("createdAt")]
        public long CreatedAt { get; set; }
    }
}