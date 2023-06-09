﻿using HajurKoRentalSystem.Areas.User.ViewModels;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Security.Claims;

namespace HajurKoRentalSystem.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class RentalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public RentalController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Rental(int vehicleId)
        {
			var vehicle = _unitOfWork.Vehicle.Get(vehicleId);

			if (vehicle.OfferId != null)
			{
				var offer = _unitOfWork.Offer.GetAll().FirstOrDefault(x => x.Id == vehicle.OfferId);
			}

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var userId = claim.Value;
			var user = _unitOfWork.User.Retrieve(userId);
			var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

			var rent = new RentalViewModel()
			{
				VehicleId = vehicleId,
				VehicleName = $"{vehicle.Model} - {vehicle.Brand}",
				VehicleDescription = vehicle.Description,
				CustomerId = userId,
				CustomerName = user.Name,
				CustomerAddress = user.Address,
				StartingPrice = vehicle.PricePerDay,
				HasOffer = vehicle.OfferId != null ?
							((_unitOfWork.Offer.Retrieve(vehicle.OfferId).EndDate > DateTime.Now &&
							_unitOfWork.Offer.Retrieve(vehicle.OfferId).StartDate < DateTime.Now) ? "Yes" : "No") : "No",
				ActualPrice = vehicle.OfferId != null ?
							((_unitOfWork.Offer.Retrieve(vehicle.OfferId).EndDate > DateTime.Now &&
							_unitOfWork.Offer.Retrieve(vehicle.OfferId).StartDate < DateTime.Now) ?
							(vehicle.PricePerDay - _unitOfWork.Offer.GetAll().FirstOrDefault(x => x.Id == vehicle.OfferId).Discount / 100 * vehicle.PricePerDay) : vehicle.PricePerDay) : vehicle.PricePerDay,
			};

			if (role == Constants.Admin || role == Constants.Staff)
			{
				rent.PriceForRegularAndStaffs = rent.ActualPrice - (0.25 * rent.ActualPrice);
			}
			else if (role == Constants.Customer)
			{
				var customer = _unitOfWork.Customer.GetAll().Where(x => x.UserId == userId).FirstOrDefault();

				if (!customer.IsActive)
				{
					TempData["Delete"] = "You haven't paid the due amount of your damage request. Can't process further on.";

					return RedirectToAction("Index", "Home");
				}

				if (customer.IsRegular)
				{
					rent.PriceForRegularAndStaffs = vehicle.PricePerDay - (0.10 * vehicle.PricePerDay);
				}

				rent.CustomerCitizenshipNumber = customer.CitizenshipNumber == null ? "No citizenship found" : customer.CitizenshipNumber;
				rent.CustomerLicenseNumber = customer.LicenseNumber == null ? "No license found" : customer.LicenseNumber;
			}

            return View(rent);
        }

        [HttpPost]
        public IActionResult Rental(RentalViewModel model)
        {
            var vehicle = _unitOfWork.Vehicle.Get(model.VehicleId);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var user = _unitOfWork.User.Retrieve(userId);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var price = 0.0;
            var days = ((model.EndDate - model.StartDate).TotalDays);

            if (role == Constants.Admin || role == Constants.Staff)
            {
                price = vehicle.PricePerDay - (0.25 * vehicle.PricePerDay);
            }
            else if (role == Constants.Customer)
            {
                var customer = _unitOfWork.Customer.GetFirstOrDefault(x => x.UserId == user.Id);

                price = vehicle.PricePerDay;

                if (customer.IsRegular)
                {
                    price = vehicle.PricePerDay - (0.10 * vehicle.PricePerDay);
                }

                if (customer.LicenseNumber == null || customer.CitizenshipNumber == null)
                {
                    TempData["Delete"] = "Please add your citizenship and license before renting a car";

                    return RedirectToAction("Documents", "Profile", new { area = "Account" });
                }
            }

            var result = new Rental()
            {
                CustomerId = model.CustomerId,
                VehicleId = model.VehicleId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TotalAmount = (float)(days * price),
            };

            vehicle.IsAvailable = false;
            _unitOfWork.Rental.Add(result);
            _unitOfWork.Save();
            TempData["Success"] = "Your rental request has been notified.";
            return RedirectToAction("Index", "Home");
        }
    }
}
