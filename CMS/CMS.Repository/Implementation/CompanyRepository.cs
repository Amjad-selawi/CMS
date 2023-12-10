using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CMS.Repository.Implementation
{
    public class CompanyRepository : ICompanyRepository
    {
       private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
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
                // var company=await _context.Companies.FindAsync(id);
                var company = await _context.Companies.Include(c => c.Candidates)
                     .FirstOrDefaultAsync(c=>c.Id==id);

                if (company.Candidates != null && company.Candidates.Any())
                {
                    foreach (var c in company.Candidates.ToList())
                    {
                        _context.Candidates.Remove(c);
                    }
                }
                _context.Companies.Remove(company);
              return  await _context.SaveChangesAsync();
                
            }
            catch(Exception ex)
            {
                LogException(nameof(Delete), ex, $"Deleted by : {deletedByUserId}", $"Company deleted with ID: {id}");
                throw ex;
            }
        }

        public async Task<List<Company>> GetAll()
        {
            try
            {

              return await _context.Companies.Include(c=>c.Country)
                    .Include(c=>c.Candidates)
                    .AsNoTracking().ToListAsync();
            
               

            }
            catch(Exception ex)
            {
                LogException(nameof(GetAll), ex,null,null);
                throw ex;
            }
        }

        public async Task<Company> GetById(int id, string createdByUserId)
        {
            try
            {
                var company=await _context.Companies
                    .Include(c=>c.Country)
                    .Include(c=>c.Candidates)
                    .AsNoTracking().FirstOrDefaultAsync(c=>c.Id==id);
               // var company=await _context.FindAsync<Company>(id);
                return company;
            }
            catch (Exception ex) {
                LogException(nameof(GetById), ex, $"Done by : {createdByUserId}", $"Error retrieving company with ID: {id}");
                throw ex;
            }
        }

        public async Task<int> Insert(Company entity, string createdByUserId)
        {
            try
            {

                _context.Add(entity);
               return await _context.SaveChangesAsync();
              
            }
            catch (Exception ex)
            {
                LogException(nameof(Insert), ex, $"Created by : {createdByUserId}", $"Company inserted with ID: {entity.Id}");
                throw ex;
            }
        }

        public async Task<int> Update(Company entity,string modifiedByUserId)
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
                LogException(nameof(Update), ex, $"Modified by : {modifiedByUserId}", $"Error updating company with ID: {entity.Id}");
                throw ex;
            }
        }


        public bool DoesCompanyNameExist(string name, int countryId)
        {
            try
            {
                return _context.Companies.Any(x => x.Name == name && x.CountryId == countryId);
            }

            catch (Exception ex)
            {
                LogException(nameof(DoesCompanyNameExist), ex, null, null);
                throw ex;
            }

        }


    }
}
