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
        public void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo)
        {
            _context.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                LogTime = DateTime.Now,
                CreatedByUserId = createdByUserId,
                AdditionalInfo = additionalInfo
            });
            _context.SaveChanges();
        }


        public async Task<int> Delete(int id, string deletedByUserId)
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
                LogException(nameof(Delete), ex,$"Deleted by : {deletedByUserId}", $"Position deleted with ID: {id}");
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
                LogException(nameof(GetAll), ex, null, null);
                throw ex;
            }
        }

        public async Task<Position> GetById(int id, string createdByUserId)
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
                LogException(nameof(GetById), ex,$"Done by : {createdByUserId}", $"Error retrieving position with ID: {id}");
                throw ex;
            }

        }

        public async Task<int> Insert(Position entity, string createdByUserId)
        {
            try
            {
                _context.Add(entity);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                LogException(nameof(Insert), ex, $"Created by : {createdByUserId}", $"Position inserted with ID: {entity.Id}");
                throw ex;
            }
        }

        public async Task<int> Update(Position entity, string modifiedByUserId)
        {
            try
            {

                _context.Update(entity);
                return await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                LogException(nameof(Update), ex, $"Modified by : {modifiedByUserId}", $"Error updating position with ID: {entity.Id}");
                throw ex;
            }
        }


        public bool DoesPositionNameExist(string name)
        {
            try
            {
                return _context.Positions.Any(x => x.Name == name);
            }
            catch(Exception ex)
            {
                LogException(nameof(DoesPositionNameExist), ex, null, null);
                throw ex;
            }
        }




        
    }
}

