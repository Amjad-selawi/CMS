using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Implementation
{
    public class CarrerOfferRepository : ICarrerOfferRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CarrerOfferRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CarrerOffer>> GetAllCarrerOffersAsync()
        {
            return await _dbContext.CarrerOffers.ToListAsync();
        }
        public async Task<CarrerOffer> GetCarrerOfferByIdAsync(int carrerOfferId)
        {
            return await _dbContext.CarrerOffers.FindAsync(carrerOfferId);
        }

        public async Task CreateCarrerOfferAsync(CarrerOffer carrerOffer)
        {
            _dbContext.CarrerOffers.Add(carrerOffer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCarrerOfferAsync(CarrerOffer carrerOffer)
        {
            _dbContext.Entry(carrerOffer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCarrerOfferAsync(CarrerOffer carrerOffer)
        {
            _dbContext.CarrerOffers.Remove(carrerOffer);
            await _dbContext.SaveChangesAsync();
        }

    }
}
