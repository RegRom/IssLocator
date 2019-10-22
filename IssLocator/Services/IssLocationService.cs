using GeoCoordinatePortable;
using IssLocator.Data;
using IssLocator.Dtos;
using IssLocator.Interfaces;
using IssLocator.Models;
using System;
using System.Threading.Tasks;

namespace IssLocator.Services
{
    public class IssLocationService : IIssLocationService
    {
        private readonly ApplicationDbContext _dbContext;

        public IssLocationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private double CalculateDistance(IssTrackPoint beginningTrackPoint, IssTrackPoint endingTrackPoint)
        {
            var start = new GeoCoordinate(beginningTrackPoint.Latitude, beginningTrackPoint.Longitude);
            var end = new GeoCoordinate(endingTrackPoint.Latitude, endingTrackPoint.Longitude);

            return start.GetDistanceTo(end);
        }

        public double CalculateSpeed(IssTrackPoint beginningTrackPoint, IssTrackPoint endingTrackPoint)
        {
            var distance = CalculateDistance(beginningTrackPoint, endingTrackPoint);
            var timeDelta = (endingTrackPoint.Timestamp - beginningTrackPoint.Timestamp);
            var speed = distance / timeDelta;

            return Math.Round(speed, 2);
        }

        private static IssTrackPoint MapIssTrackPoint(IssLocationDto locationDto)
        {
            if (locationDto.HasSucceeded.Equals("success"))
            {
                var beginningTrackPoint = new IssTrackPoint
                {
                    Timestamp = locationDto.Timestamp,
                    Latitude = locationDto.IssCoordinatesDto.Latitude,
                    Longitude = locationDto.IssCoordinatesDto.Longitude
                };
                return beginningTrackPoint;
            }

            Console.WriteLine("JSON File is invalid. Failed to map Track Point");
            return null;
        }

        public async Task AddIssTrackPoint(IssLocationDto locationDto)
        {
            var beginningTrackPoint = MapIssTrackPoint(locationDto);

            await _dbContext.IssTrackPoints.AddAsync(beginningTrackPoint);

            await _dbContext.SaveChangesAsync();
        }

        //public void StartTracking()
        //{
        //    RecurringJob.AddOrUpdate(() => AddIssTrackPoint(), Cron.MinuteInterval(1));
        //}
    }
}
