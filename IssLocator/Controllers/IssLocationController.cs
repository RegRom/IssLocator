using System;
using GeoCoordinatePortable;
using IssLocator.Data;
using IssLocator.Models;
using IssLocator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using IssLocator.Dtos;
using Microsoft.AspNetCore.Routing.Constraints;
using static IssLocator.Constants.ApiSources;
using static IssLocator.Constants.UnitsConstants;

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
            else
            {
                Console.WriteLine("JSON File is invalid. Failed to map Track Point");
                return null;
            }
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

        //public async Task<IActionResult> AddIssTrackPoint()
        //{
        //    var startbeginningTrackPoint = await GetIssLocation();
        //    await _dbContext.IssTrackPoints.AddAsync(startbeginningTrackPoint.Value);

        //    await Task.Delay(IssPollTime);

        //    var endbeginningTrackPoint = await GetIssLocation();
        //    await _dbContext.IssTrackPoints.AddAsync(endbeginningTrackPoint.Value);

        //    var issSpeed = CalculateSpeed(startbeginningTrackPoint.Value, endbeginningTrackPoint.Value);

        //    var viewModel = new IssLocationViewModel
        //    {
        //        LocationsPoints = new List<IssLocationDto>() {startbeginningTrackPoint.Value, endbeginningTrackPoint.Value},
        //        Speed = issSpeed
        //    };

        //    return View(viewModel);
        //}
    }
}
