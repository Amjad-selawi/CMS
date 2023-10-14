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
    public class CountryRepository : ICountryRepository
    {
        readonly ApplicationDbContext _context; 
        public CountryRepository(ApplicationDbContext context)
        {
            _context = context; 
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
                throw ex;
            }
        }


        //public async  Task<int> Delete(int id)
        //{
        //    try
        //    {
        //       // var country=await _context.Countries.FindAsync(id);
        //       var country=await _context.Countries.Include(c=>c.Companies)
        //            .FirstOrDefaultAsync(c=>c.Id==id);

        //        foreach(var com in country.Companies) {
        //        _context.Companies.Remove(com);
        //        }
        //        _context.Countries.Remove(country);
        //       return  await _context.SaveChangesAsync();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<List<Country>> GetAll()
        {
            try
            {
               
              return await _context.Countries.Include(c=>c.Companies).AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
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
                throw ex;
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
                throw ex;
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
                throw ex;
            }
        }
        public async Task<Country> GetCountryByNameAsync(string countryName)
        {
            return await _context.Countries
                .FirstOrDefaultAsync(c => c.Name == countryName);
        }
        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.ToListAsync();
        }

        public bool DoesCountryNameExist(string name)
        {
            return _context.Countries.Any(x => x.Name == name);
        }

    }
}
