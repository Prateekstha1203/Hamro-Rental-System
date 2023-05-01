using HajurKoRentalSystem.Areas.Admin.ViewModel;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HajurKoRentalSystem.Areas.Admin.Controllers
{
	[Authorize(Roles = Constants.Admin)]
	[Area("Admin")]
	public class RentalController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public RentalController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Requests()
		{
			var rentals = _unitOfWork.Rental.GetAll().Where(x => x.RentalStatus == Constants.Pending).ToList();

			var result = rentals.Select(x => new RentalDetailsViewModel
			{
				Id = x.Id,
				UserId = x.CustomerId,
				UserName = _unitOfWork.User.Retrieve(x.CustomerId).Name,
				VehicleId = x.VehicleId,
				VehicleName = $"{_unitOfWork.Vehicle.Get(x.VehicleId).Model} {_unitOfWork.Vehicle.Get(x.VehicleId).Brand}",
				StartDate = x.StartDate.ToString("dd/MM/yyyy"),
				EndDate = x.EndDate.ToString("dd/MM/yyyy"),
				TotalAmount = $"Rs {x.TotalAmount}",
				VehicleImages = _unitOfWork.Vehicle.Get(x.VehicleId).Image,
			}).ToList();

			return View(result);
		}

		[HttpPost]
		public IActionResult AcceptRent(int rentalId)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var rent = _unitOfWork.Rental.Get(rentalId);

			var user = _unitOfWork.User.Retrieve(rent.CustomerId);

			rent.IsApproved = true;
			rent.RentalStatus = Constants.Approved;

			_unitOfWork.Save();

			TempData["Success"] = "Rent accepted successfully";

			return RedirectToAction("Requests");
		}
	}
}
