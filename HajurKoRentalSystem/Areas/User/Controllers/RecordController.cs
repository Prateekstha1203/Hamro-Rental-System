using HajurKoRentalSystem.Areas.User.ViewModels;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HajurKoRentalSystem.Areas.User.Controllers
{
    [Authorize]
    [Area("User")]
    public class RecordController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecordController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Request()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var vehicles = _unitOfWork.Vehicle.GetAll();

            var rentals = _unitOfWork.Rental.GetAll().Where(x => x.CustomerId == userId && x.RentalStatus == Constants.Pending).ToList();

            var result = rentals.Select(x => new RecordViewModel
            {
                Id = x.Id,
                UserId = x.CustomerId,
                VehicleId = x.VehicleId,
                VehicleName = $"{_unitOfWork.Vehicle.Get(x.VehicleId).Model} - {_unitOfWork.Vehicle.Get(x.VehicleId).Brand}",
                RequestedDate = x.RequestedDate.ToString("dd/MM/yyyy"),
                StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                TotalAmount = $"Rs {x.TotalAmount}",
                ActionBy = _unitOfWork.User.Retrieve(x.ApprovedBy).Name,
                ActionDate = x.ProcessDate?.ToString("dd/MM/yyyy"),
                Image = _unitOfWork.Vehicle.Get(x.VehicleId).Image,
            }).ToList();

            return View(result);
        }

        [HttpGet]
        public IActionResult Accepted()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var vehicles = _unitOfWork.Vehicle.GetAll();

            var rentals = _unitOfWork.Rental.GetAll().Where(x => x.CustomerId == userId && x.RentalStatus == Constants.Approved && x.ReturnedDate == null).ToList();

            var result = rentals.Select(x => new RecordViewModel
            {
                Id = x.Id,
                UserId = x.CustomerId,
                VehicleId = x.VehicleId,
                VehicleName = $"{_unitOfWork.Vehicle.Get(x.VehicleId).Model} - {_unitOfWork.Vehicle.Get(x.VehicleId).Brand}",
                RequestedDate = x.RequestedDate.ToString("dd/MM/yyyy"),
                StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                TotalAmount = $"Rs {x.TotalAmount}",
                ActionBy = _unitOfWork.User.Retrieve(x.ApprovedBy).Name,
                ActionDate = x.ProcessDate?.ToString("dd/MM/yyyy"),
                Image = _unitOfWork.Vehicle.Get(x.VehicleId).Image,
                IsDamaged = x.IsDamaged,
            }).ToList();

            return View(result);
        }

        [HttpGet]
        public IActionResult Returns()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var vehicles = _unitOfWork.Vehicle.GetAll();

            var rentals = _unitOfWork.Rental.GetAll().Where(x => x.CustomerId == userId && x.RentalStatus == Constants.Approved && x.ReturnedDate != null).ToList();

            var result = rentals.Select(x => new RecordViewModel
            {
                Id = x.Id,
                UserId = x.CustomerId,
                VehicleId = x.VehicleId,
                VehicleName = $"{_unitOfWork.Vehicle.Get(x.VehicleId).Model} - {_unitOfWork.Vehicle.Get(x.VehicleId).Brand}",
                RequestedDate = x.RequestedDate.ToString("dd/MM/yyyy"),
                StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                ReturnedDate = x.ReturnedDate?.ToString("dd/MM/yyyy"),
                TotalAmount = $"Rs {x.TotalAmount}",
                ActionBy = _unitOfWork.User.Retrieve(x.ApprovedBy).Name,
                ActionDate = x.ProcessDate?.ToString("dd/MM/yyyy"),
                Image = _unitOfWork.Vehicle.Get(x.VehicleId).Image,
            }).ToList();

            return View(result);
        }

        [HttpGet]
        public IActionResult DamageDetails(int id)
        {
            var rental = _unitOfWork.Rental.Get(id);

            var vehicle = _unitOfWork.Vehicle.Get(rental.VehicleId);

            var damage = _unitOfWork.DamageRequest.GetAll().Where(x => x.RentalId == rental.Id).FirstOrDefault();

            var request = new DamageRequestViewModel()
            {
                VehicleName = $"{vehicle.Model} - {vehicle.Brand}",
                DamageRequest = damage,
            };

            return View(request);
        }

        [HttpGet]
        public IActionResult DamageRequest(int id)
        {
            var rental = _unitOfWork.Rental.Get(id);
            
            var vehicle = _unitOfWork.Vehicle.Get(rental.VehicleId);

            var damageRequest = new DamageRequestViewModel()
            {
                DamageRequest = new DamageRequest()
                {
                    RentalId = id,
                },
                VehicleName = $"{vehicle.Model} - {vehicle.Brand}"
            };

            return View(damageRequest);
        }

        [HttpPost]
        public IActionResult Requested(int id)
        {
            _unitOfWork.Rental.CancelRent(id);
            TempData["Success"] = "Rent successfully canceled";
            return RedirectToAction("Request");
        }

        [HttpPost]
        public IActionResult Accepted(int id)
        {
            _unitOfWork.Rental.CancelRent(id);
            TempData["Success"] = "Rent successfully canceled";
            return RedirectToAction("Accepted");
        }

        [HttpPost]
        public IActionResult DamageRequest(DamageRequestViewModel request)
        {
            var rental = _unitOfWork.Rental.Get(request.DamageRequest.RentalId);

            rental.IsDamaged = true;

            var vehicle = _unitOfWork.Vehicle.Get(rental.VehicleId);

            var user = _unitOfWork.User.Retrieve(rental.CustomerId);

            var customer = _unitOfWork.Customer.GetFirstOrDefault(x => x.UserId == user.Id);
            
            if (customer != null)
            {
                customer.IsActive = false;
            }

            var damage = new DamageRequest()
            {
                RentalId = request.DamageRequest.RentalId,
                DamageDescription = request.DamageRequest.DamageDescription,
            };

            _unitOfWork.DamageRequest.Add(damage);

            _unitOfWork.Save();

            TempData["Success"] = "Damage Request successfully notified";

            return RedirectToAction("Accepted");
        }
    }
}
