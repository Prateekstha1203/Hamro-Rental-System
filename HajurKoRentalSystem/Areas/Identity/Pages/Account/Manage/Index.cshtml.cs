// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HajurKoRentalSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HajurKoRentalSystem.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public string Username { get; set; }

        
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string Name { get; set; }

            public string LicenseNumber { get; set; }

            public string CitizenshipNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var customer = _unitOfWork.Customer.GetAll().Where(x => x.UserId == user.Id).FirstOrDefault();    
            var appUser = _unitOfWork.User.GetAll().Where(x => x.Id == user.Id).FirstOrDefault();

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = appUser.Name,
                LicenseNumber = customer?.LicenseNumber,
                CitizenshipNumber = customer?.CitizenshipNumber,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile license, IFormFile citizenship)
        {
            var user = await _userManager.GetUserAsync(User);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var customer = _unitOfWork.Customer.GetAll().Where(x => x.UserId == userId).FirstOrDefault();

            if(customer != null)
            {
                if (license != null)
                {
                    using (var dataStream = new MemoryStream())
                    {
                        license.CopyToAsync(dataStream);

                        customer.License = dataStream.ToArray();
                    }

                    customer.LicenseNumber = Input.LicenseNumber;
                    
                }

                if (citizenship != null)
                {
                    using (var dataStream = new MemoryStream())
                    {
                        citizenship.CopyToAsync(dataStream);

                        customer.Citizenship = dataStream.ToArray();
                    }

                    customer.CitizenshipNumber = Input.CitizenshipNumber;
                }
            }

            _unitOfWork.Save();

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
