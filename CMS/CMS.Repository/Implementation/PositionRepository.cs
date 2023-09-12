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
        public async  Task<int> Delete(int id)
        {
            try
            {
                //var position=await _context.Positions.FindAsync(id);

                var position =await _context.Positions.Include(c => c.Candidates)
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (position.Candidates != null && position.Candidates.Any()) {
                    foreach (var c in position.Candidates.ToList())
                    {
                        _context.Candidates.Remove(c);
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
                return await _context.Positions.AsNoTracking().ToListAsync();
            }
            catch (Exception ex){
                throw ex;
            }
        }

        public async Task<Position> GetById(int id)
        {
            try
            {
                //var pos = await _context.FindAsync<Position>(id);
                var pos=await _context.Positions.AsNoTracking().FirstOrDefaultAsync(p=>p.Id==id);
                return pos;
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
    }
}
