using IssLocator.Models;
using Newtonsoft.Json;

namespace IssLocator.Dtos
{
    public class IssLocationDto
    {
        [JsonProperty(PropertyName = "iss_position")]
        public CoordinatesDto IssCoordinatesDto { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string HasSucceeded { get; set; }
    }
}
