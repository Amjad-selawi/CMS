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
                //var position=await _context.Positions.FindAsync(id);

                var position =await _context.Positions.Include(c => c.Interviews)
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (position.Interviews != null && position.Interviews.Any()) {
                    foreach (var c in position.Interviews.ToList())
                    {
                        _context.Interviews.Remove(c);
                    }
                }
           
                _context.Positions.Remove(position);
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

                return await _context.Positions.Include(c => c.CarrerOffer).Include(c=>c.Interviews).Include(c=>c.Candidates).AsNoTracking().ToListAsync();

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
                    .Include(c => c.CarrerOffer).Include(c => c.Interviews).Include(c=>c.Candidates)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
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


        public bool DoesPositionNameExist(string name)
        {
            return _context.Positions.Any(x => x.Name == name);
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

