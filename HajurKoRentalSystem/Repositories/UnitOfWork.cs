using HajurKoRentalSystem.Data;
using HajurKoRentalSystem.Infrastructure.Implementation.Repositories;
using HajurKoRentalSystem.Repositories.Interfaces;

namespace HajurKoRentalSystem.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            User = new UserRepository(_dbContext);
            Customer = new CustomerRepository(_dbContext);
            DamageRequest = new DamageRequestRepository(_dbContext);
            Offer = new OfferRepository(_dbContext);
            Rental = new RentalRepository(_dbContext);
            Staff = new StaffRepository(_dbContext);
            Vehicle = new VehicleRepository(_dbContext);
        }

        public ICustomerRepository Customer { get; set; }

        public IDamageRequestRepository DamageRequest { get; set; }

        public IOfferRepository Offer { get; set; }

        public IRentalRepository Rental { get; set; }

        public IStaffRepository Staff { get; set; }

        public IUserRepository User { get; set; }

        public IVehicleRepository Vehicle { get; set; }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
