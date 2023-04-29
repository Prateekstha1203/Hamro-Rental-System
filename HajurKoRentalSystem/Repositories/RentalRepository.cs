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

}
