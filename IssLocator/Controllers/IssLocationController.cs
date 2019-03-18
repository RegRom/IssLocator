using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Hangfire;
using IssLocator.Data;
using IssLocator.Dtos;
using IssLocator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static IssLocator.Constants.ApiSources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IssLocator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public IssLocationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IssLocationDto> GetIssLocation()
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(IssLocationUrl);

                var deserializedJson = JsonConvert.DeserializeObject<IssLocationDto>(json);

                return deserializedJson;
            }
        }

        private static double CalculateDistance(IssTrackPoint beginningTrackPoint, IssTrackPoint endingTrackPoint)
        {
            var start = new GeoCoordinate(beginningTrackPoint.Latitude, beginningTrackPoint.Longitude);
            var end = new GeoCoordinate(endingTrackPoint.Latitude, endingTrackPoint.Longitude);

            return start.GetDistanceTo(end);
        }

        public static double CalculateSpeed(IssTrackPoint beginningTrackPoint, IssTrackPoint endingTrackPoint)
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

        public async Task AddIssTrackPoint()
        {
            var location = await GetIssLocation();
            var beginningTrackPoint = MapIssTrackPoint(location);

            await _dbContext.IssTrackPoints.AddAsync(beginningTrackPoint);

            await _dbContext.SaveChangesAsync();
        }

        public void StartTracking()
        {
            RecurringJob.AddOrUpdate(() => AddIssTrackPoint(), Cron.MinuteInterval(1));
        }
    }
}
