using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountryRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public void LogException(string methodName, Exception ex, string additionalInfo)
        {
            var currentUser = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            string userId = currentUser?.Id.ToString();
            _context.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,CreatedByUserId = userId,
                LogTime = DateTime.Now,
                
                AdditionalInfo = additionalInfo
            });
            _context.SaveChanges();
        }


        public int Delete(int id)
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
                LogException(nameof(Delete), ex, $"Country deleted with ID: {id}");
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
                LogException(nameof(GetAll), ex,null);
                throw;
            }
        }

        public async Task<Country> GetById(int id)
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
                LogException(nameof(GetById), ex, $"Error retrieving country with ID: {id}");
                throw;
            }

        }

        public async Task<int> Insert(Country entity)
        {
            try
            {
                _context.Add(entity);
               return await _context.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                LogException(nameof(Insert), ex, $"country inserted with ID: {entity.Id}");
                throw;
            }
        }

        public async Task<int> Update(Country entity)
        {
            try
            {

                _context.Update(entity);
              return  await _context.SaveChangesAsync();
               

            }
            catch (Exception ex)
            {
                LogException(nameof(Update), ex, $"Error updating country with ID: {entity.Id}");
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
                LogException(nameof(DoesCountryNameExist), ex,null);
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
                LogException(nameof(GetAllCountriesAsync), ex, "GetAllCountriesAsync not working");
                throw;
            }
        }

        
    }
}
