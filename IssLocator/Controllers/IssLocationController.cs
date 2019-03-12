using GeoCoordinatePortable;
using IssLocator.Data;
using IssLocator.Models;
using IssLocator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<ActionResult<IssLocation>> GetIssLocation()
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(IssLocationUrl);

                var deserializedJson = JsonConvert.DeserializeObject<IssLocation>(json);

                return deserializedJson;
            }
        }

        private double CalculateDistance(IssLocation beginningLocation, IssLocation endingLocation)
        {
            var start = new GeoCoordinate(beginningLocation.Latitude, beginningLocation.Longitude);
            var end = new GeoCoordinate(endingLocation.Latitude, endingLocation.Longitude);

            return start.GetDistanceTo(end);
        }

        private double CalculateSpeed(IssLocation beginningLocation, IssLocation endLocation)
        {
            var distance = CalculateDistance(beginningLocation, endLocation) / Kilometers;
            var timeDelta = endLocation.Timestamp - beginningLocation.Timestamp / Hours;

            return distance / timeDelta;
        }

        public async Task<IActionResult> TrackIss()
        {
            var startTrackPoint = await GetIssLocation();
            await _dbContext.IssLocations.AddAsync(startTrackPoint.Value);

            await Task.Delay(IssPollTime);

            var endTrackPoint = await GetIssLocation();
            await _dbContext.IssLocations.AddAsync(endTrackPoint.Value);

            var issSpeed = CalculateSpeed(startTrackPoint.Value, endTrackPoint.Value);

            var viewModel = new IssLocationViewModel
            {
                LocationsPoints = new List<IssLocation>() {startTrackPoint.Value, endTrackPoint.Value},
                Speed = issSpeed
            };

            return View(viewModel);
        }
    }
}
