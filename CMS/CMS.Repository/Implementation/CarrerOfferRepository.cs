using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<int> Delete(int id)
        {
            try
            {
                var careerOffer = await _context.CarrerOffers.FindAsync(id);
                _context.CarrerOffers.Remove(careerOffer);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CarrerOffer>> GetAll()
        {
            try
            {

                return await _context.CarrerOffers.Include(c => c.Positions).AsNoTracking().ToListAsync();



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CarrerOffer> GetById(int id)
        {
            try
            {
                var carrerOffer = await _context.CarrerOffers.Include(c => c.Positions)
                    .AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                return carrerOffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(CarrerOffer entity)
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

        public async Task<int> Update(CarrerOffer entity)
        {
            try
            {

                // _context.Entry<Company>(entity).State = EntityState.Detached;
                //  _context.Set<Company>().Update(entity);
                _context.Update(entity);

                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Position> GetPositionByIdAsync(int positionId)
        {
            try
            {
                var position = await _context.Positions
                    .FirstOrDefaultAsync(p => p.PositionId == positionId);

                return position;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                throw;
            }
        }

        //public async Task<IEnumerable<CarrerOffer>> GetAllCarrerOffersAsync()
        //{
        //    return await _dbContext.CarrerOffers.ToListAsync();
        //}
        //public async Task<CarrerOffer> GetCarrerOfferByIdAsync(int carrerOfferId)
        //{
        //    return await _dbContext.CarrerOffers.FindAsync(carrerOfferId);
        //}

        //public async Task CreateCarrerOfferAsync(CarrerOffer carrerOffer)
        //{
        //    carrerOffer.IsActive = true;
        //    carrerOffer.ModifiedBy = carrerOffer.ModifiedBy;
        //    carrerOffer.ModifiedOn = DateTime.Now;


        //    _dbContext.CarrerOffers.Add(carrerOffer);
        //    await _dbContext.SaveChangesAsync();
        //}

        //public async Task UpdateCarrerOfferAsync(CarrerOffer carrerOffer)
        //{
        //    carrerOffer.IsActive = true;
        //    carrerOffer.ModifiedBy = carrerOffer.ModifiedBy;
        //    carrerOffer.ModifiedOn = DateTime.Now;


        //    _dbContext.Entry(carrerOffer).State = EntityState.Modified;
        //    await _dbContext.SaveChangesAsync();
        //}

        //public async Task DeleteCarrerOfferAsync(CarrerOffer carrerOffer)
        //{
        //    carrerOffer.IsActive = true;
        //    carrerOffer.ModifiedBy = carrerOffer.ModifiedBy;
        //    carrerOffer.ModifiedOn = DateTime.Now;


        //    _dbContext.CarrerOffers.Remove(carrerOffer);
        //    await _dbContext.SaveChangesAsync();
        //}

    }
}
