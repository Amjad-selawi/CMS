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
        private readonly ApplicationDbContext _context;

        public CarrerOfferRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task DeleteCarrerOfferAsync(int id)
        {
            try
            {
                var careerOffer = await _context.CarrerOffers.FindAsync(id);
                _context.CarrerOffers.Remove(careerOffer);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CarrerOffer>> GetAllCarrerOffersAsync()
        {
            try
            {

                return await _context.CarrerOffers.ToListAsync();



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CarrerOffer> GetCarrerOfferByIdAsync(int id)
        {
            try
            {
                var carrerOffer = await _context.CarrerOffers.FirstOrDefaultAsync(c => c.Id == id);
                return carrerOffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateCarrerOfferAsync(CarrerOffer entity)
        {
            try
            {

                _context.Add(entity);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> CountAllAsync()
        {
            return await _context.CarrerOffers.CountAsync();
        }

        public async Task UpdateCarrerOfferAsync(CarrerOffer entity)
        {
            try
            {
                _context.Update(entity);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
