using CMS.Domain.Entities;
using CMS.Domain;
using CMS.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMS.Repository.Implementation
{
    public class TemplatesRepository : ITemplatesRepository
    {
        private readonly ApplicationDbContext Db;

        public TemplatesRepository(ApplicationDbContext _db)
        {
            Db = _db;
        }

        public async Task<IEnumerable<Templates>> GetAllTemplates()
        {
            return await Db.Templates.ToListAsync();
        }

        public async Task<Templates> GetTemplateById(int interviewId)
        {
            return await Db.Templates.FindAsync(interviewId);
        }

        public async Task Create(Templates entity)
        {
            entity.IsActive = true;
            entity.ModifiedBy = entity.ModifiedBy;
            entity.ModifiedOn = DateTime.Now;

            Db.Templates.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task Update(Templates entity)
        {
            entity.IsActive = true;
            entity.ModifiedBy = entity.ModifiedBy;
            entity.ModifiedOn = DateTime.Now;

            Db.Templates.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task Delete(Templates entity)
        {
            entity.IsDelete = true;
            entity.ModifiedBy = entity.ModifiedBy;
            entity.ModifiedOn = DateTime.Now;

            Db.Templates.Remove(entity);
            await Db.SaveChangesAsync();
        }
    }
}
