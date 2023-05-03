using HajurKoRentalSystem.Areas.Admin.ViewModel;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HajurKoRentalSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Customer()
        {
            var users = _unitOfWork.User.GetAll();
            var customers = _unitOfWork.Customer.GetAll();
            var rentals = _unitOfWork.Rental.GetAll();

            var result = (from customer in customers
                          join user in users
                            on customer.UserId equals user.Id
                          join rental in rentals
                            on user.Id equals rental.CustomerId into userRentals
                          from userRental in userRentals.DefaultIfEmpty()
                          group userRental by user into rentalGroup
                          select new UserViewModel
                          {
                              UserId = rentalGroup.Key.Id,
                              Name = rentalGroup.Key.Name,
                              Email = rentalGroup.Key.Email,
                              Address = rentalGroup.Key.Address,
                              PhoneNumber = rentalGroup.Key.PhoneNumber,
                              TotalRents = rentalGroup.Count(x => x != null),
                              LastRentedDate = rentalGroup.Max(x => x != null ? x.RequestedDate.ToString("dd/MM/yyyy") : "Not rented yet")
                          }).DistinctBy(x => x.UserId).ToList();

            return View(result);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = _unitOfWork.User.Retrieve(id);
            var users = _unitOfWork.User.GetAll();
            var rentals = _unitOfWork.Rental.GetAll().Where(x => x.RentalStatus == Constants.Approved && x.CustomerId == id);
            var vehicles = _unitOfWork.Vehicle.GetAll();
            
            var rents = (from rent in rentals
                         join vehicle in vehicles
                            on rent.VehicleId equals vehicle.Id
                         join staff in users
                            on rent.ApprovedBy equals staff.Id
                         select new CustomerDetailRentViewModel()
                         {
                             VehicleName = $"{vehicles.FirstOrDefault(x => x.Id == vehicle.Id).Model} - {vehicles.FirstOrDefault(x => x.Id == vehicle.Id).Brand}",
                             RentedDays = (rent.EndDate - rent.StartDate).TotalDays,
                             ReturnedDate = rent.ReturnedDate != null ? rent.ReturnedDate?.ToString("dd/MM/yyyy") : "Not returned yet",
                             ApprovedStaff = staff.Name,
                             TotalAmount = $"Rs {_unitOfWork.Rental.Get(rent.Id).TotalAmount}"
                         }).ToList();

            var result = new CustomerRentDetails()
            {
                Customer = user,
                CustomerRent = rents
            };

            return View(result);
        }

        [HttpGet]
        public IActionResult Staff()
        {
            var users = _unitOfWork.User.GetAll();
            var staffs = _unitOfWork.Staff.GetAll();

            var result = (from user in users
                          join staff in staffs
                            on user.Id equals staff.UserId
                          select new UserViewModel
                          {
                              UserId = user.Id,
                              Name = user.Name,
                              Email = user.Email,
                              Address = user.Address,
                              PhoneNumber = user.PhoneNumber,
                          }).ToList();

            return View(result);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var staff = new UserViewModel();

            return View(staff);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel staff)
        {
            var image = Request.Form.Files.FirstOrDefault();

            var password = Constants.Password;

            var role = staff.Role;

            var user = new Models.User()
            {
                Name = staff.Name,
                PhoneNumber = staff.PhoneNumber,
                Email = staff.Email,
                Address = staff.Address,
                UserName = staff.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);

                var model = new Staff()
                {
                    UserId = user.Id
                };

                _unitOfWork.Staff.Add(model);
                _unitOfWork.Save();
            }

            await _emailSender.SendEmailAsync(staff.Email, "Email Confirmation",
                        $"Dear {user.Name},<br><br>You have been registered to our system as a {role}. " +
                        $"<br>Please use your registered email and password as <b>\"{password}\"</b> as the login credential for the system." +
                        $"<br><br>Regards,<br>Hajur ko Car Rental");

            TempData["Success"] = "Staff successfully registered";

            return RedirectToAction("Staff");

        }
    }
}
