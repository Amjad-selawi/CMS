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
    public class StatusRepository : IStatusRepository
    {
        readonly ApplicationDbContext _context;
        public StatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Status>> GetAll()
        {

            try
            {
                return await _context.Statuses.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(Status entity)
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
    }
}
