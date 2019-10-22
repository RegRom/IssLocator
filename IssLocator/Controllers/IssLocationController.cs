using IssLocator.Data;
using IssLocator.Dtos;
using IssLocator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using static IssLocator.Constants.ApiSources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IssLocator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssLocationController : ControllerBase
    {
        private readonly IIssLocationService _issLocationService;

        public IssLocationController(ApplicationDbContext dbContext, IIssLocationService issLocationService)
        {
            _issLocationService = issLocationService;
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

        public async Task AddLocationToDb()
        {
            var location = await GetIssLocation();

            await _issLocationService.AddIssTrackPoint(location);
        }
    }
}
