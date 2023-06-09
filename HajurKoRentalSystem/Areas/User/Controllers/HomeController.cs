﻿using HajurKoRentalSystem.Areas.User.ViewModels;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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
            var availableVehicles = _unitOfWork.Vehicle.GetAll().Where(x => x.IsAvailable).ToList();

            var vehicles = (from vehicle in availableVehicles
                            select new VehiclesViewModel
                            {
                                Id = vehicle.Id,
                                Name = $"{vehicle.Model} {vehicle.Brand}",
                                ImageURL = vehicle.ImageURL,
                                Offer = vehicle.OfferId != null ?
                                ((_unitOfWork.Offer.Retrieve(vehicle.OfferId).EndDate > DateTime.Now && _unitOfWork.Offer.Retrieve(vehicle.OfferId).StartDate < DateTime.Now)
                                ? $"{_unitOfWork.Offer.Retrieve(vehicle.OfferId).Discount}% Offer" : "No offer") : "No offer",
                                PricePerDay = $"Rs. {vehicle.PricePerDay}/day"
                            }).DistinctBy(x => x.Id).ToList();

            return View(vehicles);
        }

		public IActionResult Cars()
		{
			var vehicles = _unitOfWork.Vehicle.GetAll().Where(x => x.IsAvailable).ToList()
                .Select(x => new VehicleViewModel()
                {
					Id = x.Id,
					Image = x.ImageURL,
					Name = $"{x.Brand} - {x.Model}",
					Price = x.PricePerDay,
					Offer = x.OfferId != null ? $"{_unitOfWork.Offer.GetFirstOrDefault(y => y.Id == x.OfferId)?.Discount}%" : ""
				}).ToList();

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