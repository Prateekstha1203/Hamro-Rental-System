using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HajurKoRentalSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehicleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Razor Pages
        [HttpGet]
        public IActionResult Index()
        {
            var vehicles = _unitOfWork.Vehicle.GetAll();

            return View(vehicles);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var vehicle = _unitOfWork.Vehicle.Get(id);

            return View(vehicle);
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
