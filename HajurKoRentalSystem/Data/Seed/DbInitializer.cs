﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HajurKoRentalSystem.Models.Constants;
using HajurKoRentalSystem.Models;

namespace HajurKoRentalSystem.Data.Seed;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(ApplicationDbContext dbContext, 
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Initialize()
    {
        try
        {
            if (_dbContext.Database.GetPendingMigrations().Count() > 0)
            {
                _dbContext.Database.Migrate();
            }
        }
        catch (Exception)
        {
            throw;
        }

        if (!_roleManager.RoleExistsAsync(Constants.Admin).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(Constants.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constants.Staff)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constants.Customer)).GetAwaiter().GetResult();
        }

        var user = new User
        {
            Name = "Admin",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            Address = "Downtown",
            PhoneNumber = "9800000000",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        var userManager = _userManager.CreateAsync(user, "Admin@123").GetAwaiter().GetResult();

        var result = _dbContext.Users.FirstOrDefault(u => u.Email == "admin@admin.com");

        _userManager.AddToRoleAsync(user, Constants.Admin).GetAwaiter().GetResult();

        await _dbContext.SaveChangesAsync();
    }
}
