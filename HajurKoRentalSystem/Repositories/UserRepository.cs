using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Data;
using HajurKoRentalSystem.Repositories.Interfaces;
using HajurKoRentalSystem.Repositories;

namespace HajurKoRentalSystem.Infrastructure.Implementation.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

	public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
	{
        _dbContext = dbContext;
    }
}
