using HajurKoRentalSystem.Repositories.Interfaces;
using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Data;
using HajurKoRentalSystem.Repositories;

namespace HajurKoRentalSystem.Infrastructure.Implementation.Repositories;

public class DamageRequestRepository : Repository<DamageRequest>, IDamageRequestRepository
{
    private readonly ApplicationDbContext _dbContext;

	public DamageRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
	{
        _dbContext = dbContext;
    }
}
