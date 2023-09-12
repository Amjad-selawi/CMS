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
    public class PositionRepository : IPositionRepository
    {
        readonly ApplicationDbContext _context;
        public PositionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Delete(int id)
        {
            try
            {
                // var country=await _context.Countries.FindAsync(id);
                var country = await _context.Countries.Include(c => c.Companies)
                     .FirstOrDefaultAsync(c => c.Id == id);

                foreach (var com in country.Companies)
                {
                    _context.Companies.Remove(com);
                }
                _context.Countries.Remove(country);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Position>> GetAll()
        {
            try
            {

                return await _context.Positions.Include(c => c.CareerOffer).Include(c=>c.Interviews).AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Position> GetById(int id)
        {
            try
            {
                var postion = await _context.Positions
                    .Include(c => c.CareerOffer).Include(c => c.Interviews)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.PositionId == id);
                return postion;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<int> Insert(Position entity)
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

        public async Task<int> Update(Position entity)
        {
            try
            {

                _context.Update(entity);
                return await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public async  Task Delete(Position entity)
        //{
        //    entity.IsDelete = true;
        //    entity.ModifiedBy = entity.ModifiedBy;
        //    entity.ModifiedOn = DateTime.Now;

        //    _context.Positions.Remove(entity);
        //    await _context.SaveChangesAsync();


        //    //try
        //    //{
        //    //    var country=await _context.Positions.FindAsync(id);
        //    //    _context.Positions.Remove(country);
        //    //   return await _context.SaveChangesAsync();

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
        //}

        //public async Task<IEnumerable<Position>> GetAll()
        //{
        //    return await _context.Positions.ToListAsync();


        //    //try
        //    //{
        //    //    return await _context.Positions.AsNoTracking().ToListAsync();
        //    //}
        //    //catch (Exception ex){
        //    //    throw ex;
        //    //}
        //}

        //public async Task<Position> GetById(int id)
        //{
        //    return await _context.Positions.FindAsync(id);

        //    //try
        //    //{
        //    //    //var pos = await _context.FindAsync<Position>(id);
        //    //    var pos=await _context.Positions.AsNoTracking().FirstOrDefaultAsync(p=>p.Id==id);
        //    //    return pos;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
        //}

        //public async Task Insert(Position entity)
        //{

        //    entity.IsActive = true;
        //    entity.ModifiedBy = entity.ModifiedBy;
        //    entity.ModifiedOn = DateTime.Now;

        //    _context.Positions.Add(entity);
        //    await _context.SaveChangesAsync();

        //    //try
        //    //{
        //    //    _context.Add(entity);
        //    //   return await _context.SaveChangesAsync();

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
        //}

        //public async Task Update(Position entity)
        //{
        //    entity.IsActive = true;
        //    entity.ModifiedBy = entity.ModifiedBy;
        //    entity.ModifiedOn = DateTime.Now;

        //    _context.Positions.Update(entity);
        //    await _context.SaveChangesAsync();

        //    //try
        //    //{

        //    //    _context.Update(entity);
        //    //   return await _context.SaveChangesAsync();


        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
    }
    }

