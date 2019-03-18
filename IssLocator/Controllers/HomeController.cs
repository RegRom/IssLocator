using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using IssLocator.Data;
using Microsoft.AspNetCore.Mvc;
using IssLocator.Models;
using IssLocator.ViewModels;

namespace IssLocator.Controllers
{
    public class HomeController : Controller
    {
        private readonly IssLocationController _issLocationController;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(IssLocationController issLocationController, ApplicationDbContext dbContext)
        {
            _issLocationController = issLocationController;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            _issLocationController.StartTracking();

            var registeredTrackPoints = _dbContext.IssTrackPoints;
            var speed = 0.0;

            if (registeredTrackPoints.Count() > 2)
                speed = IssLocationController.CalculateSpeed(registeredTrackPoints.FirstOrDefault(),
                    registeredTrackPoints.Skip(1).FirstOrDefault());

            var viewModel = new IssLocationViewModel
            {
                TrackPoints = registeredTrackPoints.ToList(),
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
