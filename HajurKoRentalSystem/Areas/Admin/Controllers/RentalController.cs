using HajurKoRentalSystem.Areas.Admin.ViewModel;
using HajurKoRentalSystem.Models;
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
		private readonly IEmailSender _emailSender;
		public RentalController(IUnitOfWork unitOfWork, IEmailSender emailSender)
		{
			_unitOfWork = unitOfWork;
			_emailSender = emailSender;
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

			var vehicle = _unitOfWork.Vehicle.Get(rent.VehicleId);

			rent.IsApproved = true;
			rent.ProcessDate = DateTime.Now;
			rent.ApprovedBy = claim.Value;
			rent.RentalStatus = Constants.Approved;

			_unitOfWork.Save();

			TempData["Success"] = "Rent accepted successfully";

            _emailSender.SendEmailAsync(user.UserName, "Successful Rent",
                    $"Dear {user.Name},<br><br>Your rent for the vehicle {vehicle.Brand} {vehicle.Model} has been approved. " +
                    $"<br>Kindly check your system for the details and visit the store to recieve the vehicle." +
                    $"<br><br>Regards,<br>Hajur ko Car Rental");

            return RedirectToAction("Requests");
		}

        [HttpPost]
        public IActionResult RejectRent(int rentalId)
        {
            var rent = _unitOfWork.Rental.Get(rentalId);

            var user = _unitOfWork.User.Retrieve(rent.CustomerId);

            var vehicle = _unitOfWork.Vehicle.Get(rent.VehicleId);

			_unitOfWork.Rental.CancelRent(rentalId);

            TempData["Success"] = "Rent rejected successfully";

            _emailSender.SendEmailAsync(user.UserName, "Unsuccessful Rent",
                        $"Dear {user.Name},<br><br>Your rent for the vehicle {vehicle.Brand} {vehicle.Model} has been rejected. " +
                        $"<br>Kindly visit the store to know about the rejection." +
                        $"<br><br>Regards,<br>Hajur ko Car Rental");

            return RedirectToAction("Requests");

        }
    }
}
