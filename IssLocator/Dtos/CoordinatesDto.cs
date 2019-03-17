using Newtonsoft.Json;

namespace IssLocator.Dtos
{
    public class CoordinatesDto
    {
        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }
    }
}
