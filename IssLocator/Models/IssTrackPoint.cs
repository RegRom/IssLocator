using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IssLocator.Models
{
    public class IssTrackPoint
    {
        [Key]
        [DisplayName("Id")]
        public int TrackPointId { get; set; }

        [DisplayName("Czas pomiaru")]
        public long Timestamp { get; set; }

        [DisplayName("Długość geograficzna")]
        public double Longitude { get; set; }

        [DisplayName("Szerokość geograficzna")]
        public double Latitude { get; set; }

        public DateTime ToDateTime()
        {
            var dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(Timestamp);
        }
    }
}
