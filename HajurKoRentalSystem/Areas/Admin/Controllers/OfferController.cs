using HajurKoRentalSystem.Areas.Admin.ViewModel;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HajurKoRentalSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.Admin)]
    public class OfferController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OfferController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var offers = _unitOfWork.Offer.GetAll();
            return View(offers);
        }

        public IActionResult Detail(int id)
        {
            var offer = _unitOfWork.Offer.Get(id);

            var result = new OfferDetailViewModel()
            {
                Name = offer.Name,
                StartDate = offer.StartDate.ToString("dd/MM/yyyy"),
                EndDate = offer.EndDate.ToString("dd/MM/yyyy"),
                Discount = $"{offer.Discount}%",
                Description = offer.Description,
                Vehicles = _unitOfWork.Vehicle.GetAll().Where(x => x.OfferId == offer.Id).ToList()
            };

            return View(result);
        }

        public IActionResult Insert()
        {
            var offer = new OfferViewModel();

            var vehicles = _unitOfWork.Vehicle.GetAll()
                .Select(x => new SelectListItem()
                {
                    Text = $"{x.Model} {x.Brand}",
                    Value = x.Id.ToString()
                }).ToList();

            offer.Vehicles = vehicles;

            return View(offer);
        }

        [HttpPost]
        public IActionResult Insert(OfferViewModel offerViewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var id = _unitOfWork.Offer.GetAll().Select(x => x.Id).DefaultIfEmpty(0).Max();

            var offerId = id + 1;

            var item = new Offer()
            {
                Name = offerViewModel.Offer.Name,
                StartDate = offerViewModel.Offer.StartDate,
                EndDate = offerViewModel.Offer.EndDate,
                Discount = offerViewModel.Offer.Discount,
                Description = offerViewModel.Offer.Description.Replace("<p>", "").Replace("</p>", ""),
            };

            _unitOfWork.Save();

            _unitOfWork.Offer.Add(item);

            foreach (var vehicle in offerViewModel.VehicleList)
            {
                var result = _unitOfWork.Vehicle.Get(vehicle);

                result.OfferId = offerId;
            }

            _unitOfWork.Save();

            TempData["Success"] = "Offer successfully added";

            return RedirectToAction("Index");
        }
    }
}
