using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CMS.Repository.Implementation
{
    public class CountryRepository : ICountryRepository
    {
        readonly ApplicationDbContext _context; 
        public CountryRepository(ApplicationDbContext context)
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


        public int Delete(int id,string deletedByUserId)
        {
            try
            {
                var country = _context.Countries
                    .Include(c => c.Companies)
                    .FirstOrDefault(c => c.Id == id);

                if (country != null)
                {
                    foreach (var com in country.Companies)
                    {
                        _context.Companies.Remove(com);
                    }
                    _context.Countries.Remove(country);
                    return _context.SaveChanges();
                }

                return 0; // Return 0 if the country with the given id is not found.
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, $"Deleted by : {deletedByUserId}", $"Country deleted with ID: {id}");
                throw;
            }
        }


        public async Task<List<Country>> GetAll()
        {
            try
            {
               
              return await _context.Countries.Include(c=>c.Companies).AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                LogException(nameof(GetAll), ex,null,null);
                throw;
            }
        }

        public async Task<Country> GetById(int id,string createdByUserId)
        {
            try
            {
                var country = await _context.Countries
                    .Include(c=>c.Companies)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c=>c.Id==id);
                return country;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetById), ex, $"Done by : {createdByUserId}", $"Error retrieving country with ID: {id}");
                throw;
            }

        }

        public async Task<int> Insert(Country entity,string createdByUserId)
        {
            try
            {
                _context.Add(entity);
               return await _context.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                LogException(nameof(Insert), ex, $"Created by : {createdByUserId}", $"country inserted with ID: {entity.Id}");
                throw;
            }
        }

        public async Task<int> Update(Country entity,string modifiedByUserId)
        {
            try
            {

                _context.Update(entity);
              return  await _context.SaveChangesAsync();
               

            }
            catch (Exception ex)
            {
                LogException(nameof(Update), ex, $"Modified by : {modifiedByUserId}", $"Error updating country with ID: {entity.Id}");
                throw;
            }
        }


        public bool DoesCountryNameExist(string name)
        {
            try
            {
                return _context.Countries.Any(x => x.Name == name);
            }
            catch (Exception ex)
            {
                LogException(nameof(DoesCountryNameExist), ex,null,null);
                throw;
            }

        }


        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            try
            {
                return await _context.Countries.ToListAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllCountriesAsync), ex, null, null);
                throw;
            }
        }

        
    }
}
