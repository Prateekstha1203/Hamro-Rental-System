using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Data;
using HajurKoRentalSystem.Repositories.Interfaces;
using HajurKoRentalSystem.Repositories;

namespace HajurKoRentalSystem.Infrastructure.Implementation.Repositories;

public class StaffRepository : Repository<Staff>, IStaffRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StaffRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
