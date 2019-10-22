using IssLocator.Models;
using System.Collections.Generic;

namespace IssLocator.ViewModels
{
    public class IssLocationViewModel
    {
        public double Speed { get; set; }
        public IEnumerable<IssTrackPoint> TrackPoints { get; set; }
    }
}
