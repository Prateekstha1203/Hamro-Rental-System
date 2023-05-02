using HajurKoRentalSystem.Repositories.Interfaces;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Data;
using HajurKoRentalSystem.Repositories;

namespace HajurKoRentalSystem.Infrastructure.Implementation.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RentalRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void CancelRent(int Id)
    {
        var rental = _dbContext.Rentals.Find(Id);

        var vehicle = _dbContext.Vehicles.Find(rental.VehicleId);

        if (rental != null)
        {
            vehicle.IsAvailable = true;

            _dbContext.Rentals.Remove(rental);

            _dbContext.SaveChanges();
        }
    }

}
