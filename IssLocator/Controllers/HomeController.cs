using IssLocator.Data;
using IssLocator.Interfaces;
using IssLocator.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IssLocator.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIssLocationService _issLocationService;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext, IIssLocationService issLocationService)
        {
            _dbContext = dbContext;
            _issLocationService = issLocationService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var registeredTrackPoints = _dbContext.IssTrackPoints.AsNoTracking() ;
            var speed = 0.0;

            if (registeredTrackPoints.Count() > 2)
                speed = _issLocationService.CalculateSpeed(registeredTrackPoints.FirstOrDefault(),
                    registeredTrackPoints.Skip(1).FirstOrDefault());

            var sortedPoints = registeredTrackPoints.OrderByDescending(point => point.Timestamp);

            var viewModel = new IssLocationViewModel
            {
                TrackPoints = await PagingList.CreateAsync(sortedPoints, 20, page),
                Speed = speed
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Lokalizator ISS - szczegóły aplikacji.";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
