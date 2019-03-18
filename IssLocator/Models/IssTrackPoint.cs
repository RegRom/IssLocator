using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IssLocator.Dtos;
using Newtonsoft.Json;

namespace IssLocator.Models
{
    public class IssTrackPoint
    {
        [Key]
        public int TrackPointId { get; set; }
        public long Timestamp { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public DateTime ToDateTime()
        {
            var dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(Timestamp);
        }
    }
}
