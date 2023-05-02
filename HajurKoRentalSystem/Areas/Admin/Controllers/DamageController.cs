using HajurKoRentalSystem.Areas.Admin.ViewModel;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HajurKoRentalSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.Admin)]
    public class DamageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public DamageController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var rentals = _unitOfWork.Rental.GetAll();
            var requests = _unitOfWork.DamageRequest.GetAll();
            var vehicles = _unitOfWork.Vehicle.GetAll();
            var users = _unitOfWork.User.GetAll();

            var output = (from rental in rentals
                          join request in requests
                             on rental.Id equals request.RentalId
                          join vehicle in vehicles
                             on rental.VehicleId equals vehicle.Id
                          join user in users
                             on rental.CustomerId equals user.Id
                          join staff in users
                             on rental.ApprovedBy equals staff.Id
                          select new DamageViewModel()
                          {
                              DamageId = request.Id,
                              Description = request.DamageDescription,
                              RequestDate = request.RequestDate.ToString("dd/MM/yyyy"),
                              ApprovedDate = rental.ProcessDate?.ToString("dd/MM/yyyy"),
                              RentalId = rental.Id,
                              VehicleId = rental.VehicleId,
                              VehicleName = $"{vehicle.Model} {vehicle.Brand}",
                              CustomerId = user.Id,
                              CustomerName = user.Name,
                              CustomerPhone = user.PhoneNumber,
                              ApproverId = staff.Id,
                              ApproverName = staff.Name,
                              PaymentStatus = request.IsPaid ? "Paid" : "Unpaid",
                              Cost = request.RepairCost == null ? "Not charged yet" : $"Rs {request.RepairCost}"
                          }).ToList();

            return View(output);
        }

        public IActionResult Details(int id)
        {
            var requests = _unitOfWork.DamageRequest.GetAll().Where(x => x.Id == id);
            var rentals = _unitOfWork.Rental.GetAll();
            var vehicles = _unitOfWork.Vehicle.GetAll();
            var users = _unitOfWork.User.GetAll();

            var output = (from rental in rentals
                          join request in requests
                             on rental.Id equals request.RentalId
                          join vehicle in vehicles
                             on rental.VehicleId equals vehicle.Id
                          join user in users
                             on rental.CustomerId equals user.Id
                          join staff in users
                             on rental.ApprovedBy equals staff.Id
                          select new DamageViewModel()
                          {
                              DamageId = request.Id,
                              Description = request.DamageDescription,
                              RequestDate = request.RequestDate.ToString("dd/MM/yyyy"),
                              ApprovedDate = rental.ProcessDate?.ToString("dd/MM/yyyy"),
                              RentalId = rental.Id,
                              VehicleId = rental.VehicleId,
                              VehicleName = $"{vehicle.Model} {vehicle.Brand}",
                              CustomerId = user.Id,
                              CustomerName = user.Name,
                              CustomerPhone = user.PhoneNumber,
                              ApproverId = staff.Id,
                              ApproverName = staff.Name,
                              PaymentStatus = request.IsPaid ? "Paid" : "Unpaid",
                              Cost = request.RepairCost == null ? "Not charged yet" : $"Rs {request.RepairCost}"
                          }).FirstOrDefault();

            return View(output);
        }

        [HttpPost]
        public IActionResult Details(DamageViewModel damage)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var request = _unitOfWork.DamageRequest.GetFirstOrDefault(x => x.Id == damage.DamageId);
            var rental = _unitOfWork.Rental.GetFirstOrDefault(x => x.Id == request.RentalId);
            var vehicle = _unitOfWork.Vehicle.GetFirstOrDefault(x => x.Id == rental.VehicleId);
            var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == rental.CustomerId);

            request.RepairCost = damage.RepairCost;
            request.IsPaid = damage.PaymentStatus == "true" ? true : false;
            request.IsApproved = true;
            request.ApprovedDate = DateTime.Now;
            request.ApprovedBy = claim.Value;

            _unitOfWork.Save();

            if (request.IsPaid == true)
            {
                rental.IsReturned = true;

                rental.ReturnedDate = DateTime.Now;

                var customer = _unitOfWork.Customer.GetAll().Where(x => x.UserId == rental.CustomerId).FirstOrDefault();

                if (customer != null)
                {
                    customer.IsActive = true;
                }

                vehicle.IsAvailable = true;
                
                _unitOfWork.Save();

                _emailSender.SendEmailAsync(user.UserName, "Successful Payment",
                    $"Dear {user.Name},<br><br>Your request for the damage on vehicle {vehicle.Model} - {vehicle.Brand} has been accepted. " +
                        $"<br>You can now surf the store and rent other vehicles." +
                        $"<br><br>Regards,<br>Hajur ko Car Rental");

                TempData["Success"] = "Payment verified successfully ";

                return RedirectToAction("Index");
            }

            _emailSender.SendEmailAsync(user.UserName, "Damage Request Update",
                    $"Dear {user.Name},<br><br>Your request for the damage on vehicle {vehicle.Model} - {vehicle.Brand} has been validated. " +
                        $"<br>Please check the system to examine updated details." +
                        $"<br><br>Regards,<br>Hajur ko Car Rental");

            TempData["Success"] = "Request successfully updated";

            return RedirectToAction("Index");

        }
    }
}
