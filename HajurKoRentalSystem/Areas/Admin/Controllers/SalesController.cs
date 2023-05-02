using HajurKoRentalSystem.Areas.Admin.ViewModel;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HajurKoRentalSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.Admin)]
    public class SalesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var vehicles = _unitOfWork.Vehicle.GetAll();
            var rentals = _unitOfWork.Rental.GetAll();
            var users = _unitOfWork.User.GetAll();
            var customers = _unitOfWork.Customer.GetAll();

            var vehicleRents = (from vehicle in vehicles
                                join rental in rentals
                                    on vehicle.Id equals rental.VehicleId
                                group vehicle by new { vehicle.Id, vehicle.Model, vehicle.Brand}
                                into g
                                select new VehicleRentViewModel()
                                {
                                    Count = g.Count(),
                                    Vehicle = $"{g.Key.Model} - {g.Key.Brand}"
                                }).ToList();

            var userRents = (from user in users
                             join customer in customers
                                on user.Id equals customer.UserId
                             join rental in rentals
                                on user.Id equals rental.CustomerId
                             group user by new { user.Id, user.Name, user.Address, user.PhoneNumber } into g
                             select new UserRentViewModel()
                             {
                                 Count = g.Count(),
                                 User = g.Key.Name,
                                 PhoneNumber = g.Key.PhoneNumber,
                                  Address = g.Key.Address
                             }).ToList();

            var inactiveCustomers = (from customer in customers
                                     join user in users
                                         on customer.UserId equals user.Id
                                     join rental in rentals
                                         on user.Id equals rental.CustomerId into rentalGroup
                                     where !rentalGroup.Any(x => x.RequestedDate >= DateTime.Now.AddMonths(-3))
                                     select new InActiveCustomerViewModel()
                                     {
                                         CustomerId = customer.Id,
                                         CustomerName = user.Name,
                                         LastRentedDate = rentalGroup.OrderByDescending(x => x.RequestedDate).Select(x => x.RequestedDate).FirstOrDefault().ToString("dd/MM/yyyy")
                                     }).ToList();


            var result = new SalesViewModel()
            {
                InactiveUserCount = inactiveCustomers,
                UserRentCount = userRents,
                VehicleRentCount = vehicleRents
            };

            return View(result);
        }
    }
}
