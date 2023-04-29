namespace HajurKoRentalSystem.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customer { get; set; }

        IDamageRequestRepository DamageRequest { get; set; }

        IOfferRepository Offer { get; set; }

        IRentalRepository Rental { get; set; }

        IStaffRepository Staff { get; set; }

        IUserRepository User { get; set; }

        IVehicleRepository Vehicle { get; set; }

        void Save();
    }
}
