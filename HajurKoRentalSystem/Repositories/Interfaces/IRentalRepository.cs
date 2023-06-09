﻿using HajurKoRentalSystem.Models;
using HajurKoRentalSystem.Repositories.Interfaces;

namespace HajurKoRentalSystem.Repositories.Interfaces; 

public interface IRentalRepository : IRepository<Rental> 
{
    void CancelRent(int Id);
}