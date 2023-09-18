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

        public async Task<int> Delete(int id)
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
                throw ex;
            }
        }

        public async Task<Company> GetById(int id)
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
                throw ex;
            }
        }

        public async Task<int> Insert(Company entity)
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

        public async Task<int> Update(Company entity)
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
    }
}
