using IssLocator.Dtos;
using IssLocator.Models;
using System.Threading.Tasks;

namespace IssLocator.Interfaces
{
    public interface IIssLocationService
    {
        Task AddIssTrackPoint(IssLocationDto locationDto);
        double CalculateSpeed(IssTrackPoint beginningTrackPoint, IssTrackPoint endingTrackPoint);
    }
}
