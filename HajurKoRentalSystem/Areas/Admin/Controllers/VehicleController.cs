using HajurKoRentalSystem.Areas.Admin.ViewModel;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Xml.Linq;

namespace HajurKoRentalSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehicleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VehicleController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region 
        [HttpGet]
        public IActionResult Index()
        {
            var vehicles = _unitOfWork.Vehicle.GetAll();

            return View(vehicles);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var vehicles = _unitOfWork.Vehicle.Get(id);
            var rentals = _unitOfWork.Rental.GetAll().Where(x => x.RentalStatus == Constants.Approved && x.VehicleId == id);
            var users = _unitOfWork.User.GetAll();
            var rents = (from rent in rentals
                         join user in users
                            on rent.CustomerId equals user.Id
                         join staff in users
                            on rent.ApprovedBy equals staff.Id
                         select new VehicleDetailRentViewModel()
                         {
                             CustomerName = user.Name,
                             RentedDays = (rent.EndDate - rent.StartDate).TotalDays,
                             ReturnedDate = rent.ReturnedDate != null ? rent.ReturnedDate?.ToString("dd/MM/yyyy") : "Not returned yet",
                             ApprovedStaff = staff.Name,
                             TotalAmount = $"Rs {_unitOfWork.Rental.Get(rent.Id).TotalAmount}"
                         }).ToList();

            var detail = new VehicleDetailViewModel()
            {
                Vehicle = vehicles,
                RentailDetails = rents
            };

            return View(detail);
        }

        [HttpGet]
        public IActionResult Upsert(int id)
        {
            var vehicle = new Vehicle();

            if(id == 0)
            {
                return View(vehicle);
            }

            vehicle = _unitOfWork.Vehicle.Get(id);

            if(vehicle == null)
            {
                return NotFound();  
            }

            return View(vehicle);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var vehicle = _unitOfWork.Vehicle.Get(id);

            return View(vehicle);
        }
        #endregion

        #region
        [HttpPost, ActionName("Upsert")]
        public IActionResult UpsertVehicle(Vehicle vehicle)
        {
            var file = Request.Form.Files.FirstOrDefault();

            byte[] image;

            var wwwRootPath = _webHostEnvironment.WebRootPath;

            var fileName = $"[Vehicle] {vehicle.Brand} {vehicle.Model} - Image";

            var path = $@"images\vehicles\";

            var uploads = Path.Combine(wwwRootPath, path);

            var extension = Path.GetExtension(file.FileName);

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                file.CopyTo(fileStreams);
            }

            using (var dataStream = new MemoryStream())
            {
                file.CopyToAsync(dataStream);

                image = dataStream.ToArray();
            }

            if(vehicle.Id == 0)
            {
                var result = new Vehicle()
                {
                    Brand = vehicle.Brand,
                    Color = vehicle.Color,
                    Description = vehicle.Description,
                    Image = image,
                    Model = vehicle.Model,
                    PricePerDay = vehicle.PricePerDay,
                    ImageURL = @$"\images\vehicles\" + fileName + extension,
                };

                _unitOfWork.Vehicle.Add(result);
                _unitOfWork.Save();
            }

            else
            {
                var result = new Vehicle()
                {
                    Id = vehicle.Id,
                    Brand = vehicle.Brand,
                    Color = vehicle.Color,
                    Description = vehicle.Description,
                    Image = image,
                    Model = vehicle.Model,
                    PricePerDay = vehicle.PricePerDay,
                    ImageURL = @$"\images\vehicles\" + fileName + extension,
                };

                _unitOfWork.Vehicle.Update(result);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteVehicle(int id)
        {
            var vehicle = _unitOfWork.Vehicle.Get(id);

            _unitOfWork.Vehicle.Remove(vehicle); 
            
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
        #endregion
    }
}
