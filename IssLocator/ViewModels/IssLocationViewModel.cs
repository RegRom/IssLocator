using IssLocator.Models;
using System.Collections.Generic;

namespace IssLocator.ViewModels
{
    public class IssLocationViewModel
    {
        public double Speed { get; set; }
        public List<IssLocation> LocationsPoints { get; set; }
    }
}
