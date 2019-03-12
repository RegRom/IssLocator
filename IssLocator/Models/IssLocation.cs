namespace IssLocator.Models
{
    public class IssLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long Timestamp { get; set; }
        public bool HasSucceded { get; set; }
    }
}
