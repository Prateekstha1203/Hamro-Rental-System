using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HajurKoRentalSystem.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var vehicles = _unitOfWork.Vehicle.GetAll().Where(x => x.IsAvailable).ToList();
            return View(vehicles);
        }

		public IActionResult Cars()
		{
			var vehicles = _unitOfWork.Vehicle.GetAll().Where(x => x.IsAvailable).ToList();
			return View(vehicles);
		}

		public IActionResult Details(int vehicleId) 
        {
            var vehicle = _unitOfWork.Vehicle.Get(vehicleId);
            return View(vehicle);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}